using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.DominService;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using LexisNexis.Red.Common.HelpClass.Tools;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Business
{
    public class PublicationUtil : AppServiceBase<PublicationUtil>
    {
        IConnectionMonitor connectionMonitor;
        IPublicationAccess publicationAccess;
        IDeliveryService deliveryService;
        INetwork networkService;
        IAnnotationAccess annotationAccess;
        IPackageAccess packageAccess;
        IPublicationDomainService publicationDomainService;
        static RedLock redLock = new RedLock(2);
        public Action RecenthistoryChanged;
        public PublicationUtil(IPublicationAccess publicationAccess,
                                IDeliveryService deliveryService,
                                IConnectionMonitor connectionMonitor,
                                INetwork networkService,
                                IAnnotationAccess annotationAccess,
                                IPackageAccess packageAccess)
        {

            this.publicationAccess = publicationAccess;
            this.connectionMonitor = connectionMonitor;
            this.deliveryService = deliveryService;
            this.annotationAccess = annotationAccess;
            this.networkService = networkService;
            this.packageAccess = packageAccess;
            this.publicationDomainService = new PublicationDomainService(publicationAccess,
                                                                      connectionMonitor,
                                                                      deliveryService,
                                                                      annotationAccess,
                                                                      networkService,
                                                                      packageAccess);
        }

        /// <summary>
        ///get local publications  in offline mode 
        /// </summary>
        /// <returns>List<Publication></returns>
        public List<Publication> GetPublicationOffline()
        {
            var localDlBooks = publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential);

            if (localDlBooks.Count == 0)
                return new List<Publication>();

            var publications = from localDlBook in localDlBooks
                               orderby localDlBook.OrderBy ascending, localDlBook.RowId descending
                               let publication = localDlBook.ToPublication()
                               select publication;


            return publications.ToList<Publication>();
        }
        int GetPublicationOnlineFlag;
        /// <summary>
        ///  get remote publications in online mode
        /// </summary>
        /// <returns>Task<OnlinePublicationResult></returns>
        public async Task<OnlinePublicationResult> GetPublicationOnline()
        {
            OnlinePublicationResult onlinePublicationResult = new OnlinePublicationResult { RequestStatus = RequestStatusEnum.Failure };

            //onlinePublicationResult.Publications = GetPublicationOffline();
            //onlinePublicationResult.RequestStatus = RequestStatusEnum.Success;
            //return onlinePublicationResult;
            try
            {
                if (Interlocked.Increment(ref GetPublicationOnlineFlag) == 1)
                {
                    var pingSuccess = await connectionMonitor.PingService(null);
                    if (!pingSuccess)
                    {
                        return onlinePublicationResult;
                    }
                    // await publicationDomainService.RemoveDlBooksFromServer(GlobalAccess.Instance.UserCredential);

                    var onlineDlBooks = await publicationDomainService.GetOnlineDlBooks(GlobalAccess.Instance.Email);
                    var localDlBooks = publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential);

                    var exceptDlBooks = GetLocalDlBooksExceptOnlineDlBooks(onlineDlBooks, localDlBooks);
                    var additionalDlBooks = publicationDomainService.GetAdditionalDlBooks(exceptDlBooks);

                    onlinePublicationResult.Publications = additionalDlBooks.ToPublications();

                    var toBeDeletedDlBooks = exceptDlBooks.Except(additionalDlBooks, new DlBookEqualityComparer());

                    await publicationDomainService.DeleteDlBooksByEreader(toBeDeletedDlBooks);
                    if (toBeDeletedDlBooks != null && toBeDeletedDlBooks.Count() > 0)
                    {
                        if (RecenthistoryChanged != null)
                            RecenthistoryChanged();
                    }
                    bool noReturnFromServer = (onlineDlBooks == null || onlineDlBooks.Count == 0);
                    if (noReturnFromServer)
                    {//user did not Assign dlbook
                        onlinePublicationResult.RequestStatus = RequestStatusEnum.Success;
                        return onlinePublicationResult;
                    }
                    var updatedPublications = publicationDomainService.UpdateLocalDlBooks(onlineDlBooks, localDlBooks);
                    onlinePublicationResult.Publications.AddRange(updatedPublications);
                    onlinePublicationResult.Publications = onlinePublicationResult.Publications
                                                                                  .OrderBy(p => p.OrderBy)
                                                                                  .ThenByDescending(p => p.RowId)
                                                                                  .ToList();

                    onlinePublicationResult.RequestStatus = RequestStatusEnum.Success;
                    await DomainEvents.Publish(new GetPublicationOnlineEvent());
                }
            }
            catch (Exception ex)
            {
                Logger.Log("GetPublicationOnline" + ex.ToString());
            }
            finally
            {
                Interlocked.Decrement(ref GetPublicationOnlineFlag);
            }
            return onlinePublicationResult;
        }
        private IEnumerable<DlBook> GetLocalDlBooksExceptOnlineDlBooks(List<DlBook> onlineDlBooks, List<DlBook> localDlBooks)
        {
            IEnumerable<DlBook> exceptedDlBooks = null;
            if (onlineDlBooks == null)
                exceptedDlBooks = localDlBooks;
            else
            {
                exceptedDlBooks = localDlBooks.Except(onlineDlBooks, new DlBookEqualityComparer());
            }
            return exceptedDlBooks;
        }

        public async Task<DownloadResult> DownloadPublicationByBookId(int bookId, CancellationToken cancelToken
                                                                                , DownloadProgressEventHandler downloadHandler
                                                                                , bool checkNetLimitation = true)
        {
            DownloadResult downloadResult = new DownloadResult { DownloadStatus = DownLoadEnum.Failure };
            DlBook localDlBook = null, serverDlBook = null;
            int previousDownloadVersion = 0;
            try
            {
                var currentUser = GlobalAccess.Instance.CurrentUserInfo;
                var userCredential = GlobalAccess.Instance.UserCredential;
                await redLock.Enter().WithCancellation(cancelToken);
                var pingSuccess = await connectionMonitor.PingService(currentUser.Country.CountryCode, cancelToken);
                if (!pingSuccess)
                {
                    downloadResult.DownloadStatus = DownLoadEnum.NetDisconnected;
                    return downloadResult;
                }
                serverDlBook = await publicationDomainService.GetLatestDlBookDetail(userCredential,
                                                                                   GlobalAccess.DeviceId,
                                                                                   bookId,
                                                                                   cancelToken);
                if (checkNetLimitation)
                {
                    publicationDomainService.VerifyNetwork(serverDlBook.Size);
                }
                await publicationDomainService.DownLoadDlBook(serverDlBook, cancelToken, downloadHandler)
                                              .WithCancellation(cancelToken);
                localDlBook = publicationAccess.GetDlBookByBookId(bookId, userCredential);
                previousDownloadVersion = localDlBook.LastDownloadedVersion;
                localDlBook = await publicationDomainService.RefreshLocalDlBook(serverDlBook, localDlBook, GlobalAccess.DeviceId)
                                                             .WithCancellation<DlBook>(cancelToken);

                if (downloadHandler != null)
                {
                    downloadHandler(90, serverDlBook.Size);
                    await publicationDomainService.InstallDlBook(localDlBook, currentUser.SymmetricKey, cancelToken);
                    downloadResult.Publication = localDlBook.ToPublication();
                    await DomainEvents.Publish<DownloadCompletedEvent>(new DownloadCompletedEvent(localDlBook, RecenthistoryChanged));
                    bool updateSuccess = (publicationAccess.Update(localDlBook) > 0);
                    if (updateSuccess)
                    {
                        downloadResult.DownloadStatus = DownLoadEnum.Success;
                    }
                    await ClearDownloadedFiles(localDlBook, downloadResult.DownloadStatus, previousDownloadVersion);
                    downloadHandler(100, serverDlBook.Size);
                }
            }
            catch (NullUserException)
            {

            }
            catch (ExceedLimitationException)
            {
                downloadResult.DownloadStatus = DownLoadEnum.OverLimitation;
            }
            catch (OperationCanceledException)
            {
                if (cancelToken.IsCancellationRequested)
                    downloadResult.DownloadStatus = DownLoadEnum.Canceled;

            }
            catch (CusNetDisConnectedException)
            {
                downloadResult.DownloadStatus = DownLoadEnum.NetDisconnected;
            }
            catch (Exception ex)
            {
                Logger.Log("DownloadPublicationByBookId:" + ex.ToString());
            }
            finally
            {
                redLock.Release();
                ClearDownloadedFiles(localDlBook, downloadResult.DownloadStatus, previousDownloadVersion).Wait();
            }
            return downloadResult;
        }

        private Task ClearDownloadedFiles(DlBook dlBook, DownLoadEnum downloadStatus, int previousDownloadVersion)
        {
            return Task.Run(async () =>
             {
                 try
                 {
                     if (dlBook != null)
                     {
                         await dlBook.DeleteDlBookZipFile();
                     }
                     bool hasNewVersion = (previousDownloadVersion != 0 && dlBook.LastDownloadedVersion != previousDownloadVersion);
                     if (downloadStatus == DownLoadEnum.Success && hasNewVersion)
                     {
                         string lastVersionDirectory = Path.Combine(dlBook.ServiceCode,
                                                                    dlBook.Email,
                                                                    Constants.DLFiles,
                                                                    dlBook.BookId + "V" + previousDownloadVersion);
                         if (await GlobalAccess.DirectoryService.DirectoryExists(lastVersionDirectory))
                         {
                             await GlobalAccess.DirectoryService.DeleteDirectory(lastVersionDirectory);
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     Logger.Log("ClearDownloadedFiles error:" + ex.ToString());
                 }
             });
        }
        /// <summary>
        /// delete a book by client user
        /// </summary>
        /// <param name="bookId">bookId</param>
        public async Task DeletePublicationByUser(int bookId)
        {
            await publicationDomainService.DeleteDlbookByUser(bookId, GlobalAccess.Instance.UserCredential);
            if (RecenthistoryChanged != null)
                RecenthistoryChanged();
        }

        /// <summary>
        /// reset DlBook's order
        /// </summary>
        /// <param name="bookIds"></param>
        public async Task OrganiseDlsOrder(List<int> bookIds)
        {
            if (bookIds != null)
            {
                await publicationDomainService.ResetDlBookOrder(bookIds, GlobalAccess.Instance.UserCredential);
            }
        }
        public async Task<TOCNode> GetDlBookTOC(int bookId)
        {
            return await PublicationContentUtil.Instance.GetTOCByBookId(bookId);
        }
        public async Task<Dictionary<string, List<Index>>> GetIndexsByBookId(int bookId)
        {
            return await PublicationContentUtil.Instance.GetIndexsByBookId(bookId);
        }
    }
}
