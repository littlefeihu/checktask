using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DominService
{
    public class PublicationDomainService : IPublicationDomainService
    {
        IPublicationAccess publicationAccess;
        IConnectionMonitor connectionMonitor;
        IDeliveryService deliveryService;
        IAnnotationAccess annotationAccess;
        INetwork networkService;
        IPackageAccess packageAccess;
        public PublicationDomainService(IPublicationAccess publicationAccess,
                                        IConnectionMonitor connectionMonitor,
                                        IDeliveryService deliveryService,
                                        IAnnotationAccess annotationAccess,
                                        INetwork networkService,
                                        IPackageAccess packageAccess)
        {
            this.publicationAccess = publicationAccess;
            this.connectionMonitor = connectionMonitor;
            this.deliveryService = deliveryService;
            this.annotationAccess = annotationAccess;
            this.networkService = networkService;
            this.packageAccess = packageAccess;
        }

        public void VerifyNetwork(long localSize)
        {
            var networkType = networkService.GetNetworkType();
            if (networkType == NetworkTypeEnum.Mobile)
            {
                var flowLimitations = networkService.GetFlowLimitation();
                bool exceedLimitation = (localSize > flowLimitations);
                if (exceedLimitation)
                {
                    throw new ExceedLimitationException();
                }
            }
            else if (networkType == NetworkTypeEnum.None)
            {
                throw new CusNetDisConnectedException();
            }
        }

        public async Task ResetDlBookOrder(List<int> bookIds, UserCredential userCredential)
        {
            Dictionary<int, int> orders = new Dictionary<int, int>();
            for (int order = 1, index = 0; index < bookIds.Count; order++, index++)
            {
                int bookId = bookIds[index];
                orders.Add(bookId, order);
            }
            await publicationAccess.UpdateDlBookOrder(orders, userCredential);
        }


        private async Task DeleteDlbookByEreader(int bookId, UserCredential userCredential)
        {
            await DeletePublication(bookId, DlStatusEnum.RemovedByEreader, userCredential);
        }
        public async Task DeleteDlbookByUser(int bookId, UserCredential userCredential)
        {
            await DeletePublication(bookId, DlStatusEnum.RemovedByUser, userCredential);
        }


        private async Task<bool> RemoveDlFromServer(DlBook localDlBook, UserCredential userCredential)
        {
            // in consideration of deleting dlbook in offline mode
            int deleteVersion = localDlBook.LastDownloadedVersion == 0 ? localDlBook.CurrentVersion : localDlBook.LastDownloadedVersion;
            // Used to validate the Status Code given by eReader.
            // Status Code : 1 => DL file Successfully downloaded by eReader. 
            // Status Code : 2 => DL file deleted by the user.
            // Status Code : 3 => DL file deleted by eReader(backend).
            string dlDeletedStatus = "2";
            DlFileStatusUpdate dlFileStatusUpdate = new DlFileStatusUpdate
            {
                DeviceId = GlobalAccess.DeviceId,
                Email = userCredential.Email,
                DLId = localDlBook.BookId,
                Ver = deleteVersion.ToString(),
                StatusCode = dlDeletedStatus
            };
            var updateResult = await deliveryService.DlFileStatusUpdate(dlFileStatusUpdate);
            return updateResult;
        }
        private async Task DeletePublication(int bookId, DlStatusEnum dlStatus, UserCredential userCredential)
        {
            var localDlBook = publicationAccess.GetDlBookByBookId(bookId, userCredential);
            if (localDlBook != null)
            {
                await localDlBook.DeleteDlBookInstallFile();
                if (dlStatus == DlStatusEnum.RemovedByUser)
                {
                    var removedByUserSuccess = (publicationAccess.UpdateDlBookStatus(bookId, DlStatusEnum.RemovedByUser, userCredential) > 0);
                    if (removedByUserSuccess)
                    {
                        ClearRelevantDlBookData(bookId, userCredential);
                    }
                    var pingSuccess = await connectionMonitor.PingService(null);
                    if (pingSuccess)
                    {
                        bool removeSuccess = await RemoveDlFromServer(localDlBook, userCredential);
                        if (removeSuccess)
                            RemoveDlBookFromDb(bookId, userCredential);
                    }
                }
                else if (dlStatus == DlStatusEnum.RemovedByEreader)
                {
                    ClearRelevantDlBookData(bookId, userCredential);
                    RemoveDlBookFromDb(bookId, userCredential);
                }
            }
        }
        public async Task RemoveDlBooksFromServer(UserCredential userCredential)
        {
            var localDlBooks = publicationAccess.GetDeletedDlBooks(userCredential);
            if (localDlBooks.Count == 0)
                return;
            foreach (var deletedDlBook in localDlBooks)
            {
                var updateResult = await RemoveDlFromServer(deletedDlBook, userCredential);
                if (updateResult)
                {
                    ClearRelevantDlBookData(deletedDlBook.BookId, userCredential);
                    RemoveDlBookFromDb(deletedDlBook.BookId, userCredential);
                }
            }
        }
        private void ClearRelevantDlBookData(int bookId, UserCredential userCredential)
        {
            annotationAccess.DeleteAnnotationByBookId(bookId, userCredential);
            publicationAccess.DeleteRecentHistory(bookId, userCredential);
        }
        private void RemoveDlBookFromDb(int bookId, UserCredential userCredential)
        {
            publicationAccess.DeleteDlBookByBookId(bookId, userCredential);
        }
        public async Task<DlVersionChangeHistoryResponse> GetDlBookChangeHistory(int bookId,
                                                                                 int lastDownloadedVersion,
                                                                                 string email,
                                                                                 string deviceId)
        {
            DlVersionChangeHistoryRequest dlVersionChangeHistory = new DlVersionChangeHistoryRequest
            {
                DeviceId = deviceId,
                DlId = bookId,
                Email = email,
                Ver = lastDownloadedVersion
            };
            var changeHistoryResult = await deliveryService.DlVersionChangeHistory(dlVersionChangeHistory);
            DlVersionChangeHistoryResponse dlBookChangeHistory = null;
            if (changeHistoryResult.IsSuccess)
            {
                dlBookChangeHistory = JsonConvert.DeserializeObject<DlVersionChangeHistoryResponse>(changeHistoryResult.Content);
            }
            return dlBookChangeHistory;
        }
        public Task InstallDlBook(DlBook dlBook, string symmetricKey, CancellationToken cancelToken)
        {
            return Task.Run(async () =>
            {

                InstallResultEnum installResult = InstallResultEnum.Failure;
                PublicationContent publicationContent = null;
                try
                {
                    var contentKey = await dlBook.GetContentKey(symmetricKey).WithCancellation<byte[]>(cancelToken);
                    publicationContent = new PublicationContent(dlBook, contentKey);

                    bool hasNotInstalled = (!await publicationContent.DlBook.IsSqliteDecryped());
                    if (hasNotInstalled)
                    {
                        await publicationContent.DlBook.UnZipFile(cancelToken);
                        if (cancelToken.IsCancellationRequested)
                        {
                            cancelToken.ThrowIfCancellationRequested();
                        }
                        await publicationContent.DecryptDataBase();
                        await publicationContent.DlBook.DeleteOriginalSqlite();
                        installResult = InstallResultEnum.Success;
                    }
                    else
                    {//has been installed
                        installResult = InstallResultEnum.Success;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        installResult = InstallResultEnum.Cancel;
                    }
                    throw ex;
                }
                catch (Exception ex)
                {
                    Logger.Log("Install error:" + ex.ToString());
                    installResult = InstallResultEnum.Failure;
                    throw ex;
                }
                finally
                {
                    if (publicationContent != null && installResult != InstallResultEnum.Success)
                    {
                        publicationContent.DlBook.DeleteDlBookInstallFile().Wait();
                    }
                }
            });
        }

        private async Task<DlMetadataDetails> SetDlBookMetaData(int bookId, UserCredential userCredential, string deviceId)
        {
            DlMetadata dlMetaData = new DlMetadata()
            {
                Email = userCredential.Email,
                DeviceId = deviceId,
                DlId = bookId
            };
            DlMetadataDetails dlMetaDataDetails = null;
            //before call this function,program has ensured network available.
            var result = await deliveryService.DlMetadata(dlMetaData);
            if (result.IsSuccess)
            {
                dlMetaDataDetails = JsonConvert.DeserializeObject<DlMetadataDetails>(result.Content);
                if (dlMetaDataDetails != null)
                {
                    var metadata = publicationAccess.GetDlBookMetedata(bookId, userCredential);
                    if (metadata == null)//insert metadata
                    {
                        BookMetadata entity = new BookMetadata
                        {
                            BookId = bookId,
                            ServiceCode = userCredential.ServiceCode,
                            Email = userCredential.Email,
                            IncludedGuideCard = JsonConvert.SerializeObject(dlMetaDataDetails.GuideCard)
                        };
                        publicationAccess.InsertMetadata(entity);
                    }
                    else//update metadata
                    {
                        metadata.IncludedGuideCard = JsonConvert.SerializeObject(dlMetaDataDetails.GuideCard);
                        publicationAccess.Update(metadata);
                    }
                }
            }
            return dlMetaDataDetails;
        }

        public IEnumerable<DlBook> GetAdditionalDlBooks(IEnumerable<DlBook> exceptedDlBooks)
        {
            var holdingDlBooks = exceptedDlBooks.Where(dlBook => !dlBook.IsLoan
                                                              && !dlBook.IsTrial
                                                              && dlBook.DlStatus == (short)DlStatusEnum.Downloaded);

            foreach (var holdingDlBook in holdingDlBooks)
            {
                holdingDlBook.SetDlBookExpired();
                publicationAccess.Update(holdingDlBook);
                yield return holdingDlBook;
            }
        }
        public async Task DeleteDlBooksByEreader(IEnumerable<DlBook> toBeDeletedDlBooks)
        {
            foreach (var toBeDeletedDlBook in toBeDeletedDlBooks)
            {
                await DeleteDlbookByEreader(toBeDeletedDlBook.BookId, GlobalAccess.Instance.UserCredential);
            }
        }

        public async Task<DlBook> RefreshLocalDlBook(DlBook serverDlBook, DlBook localDlBook, string deviceid)
        {
            if (localDlBook == null)
            {
                return null;
            }
            //update SubCategory PracticeArea
            var metaData = await SetDlBookMetaData(serverDlBook.BookId,
                                                    new UserCredential(serverDlBook.Email, serverDlBook.ServiceCode),
                                                    deviceid);

            if (metaData != null)
            {
                localDlBook.SubCategory = metaData.SubCategory;
                localDlBook.PracticeArea = metaData.PracticeArea;
            }
            var changeHistory = await GetDlBookChangeHistory(serverDlBook.BookId, serverDlBook.CurrentVersion, serverDlBook.Email, deviceid);
            if (changeHistory != null)
            {
                localDlBook.AddedGuideCard = JsonConvert.SerializeObject(changeHistory.AddedGuideCard);
                localDlBook.DeletedGuideCard = JsonConvert.SerializeObject(changeHistory.DeletedGuideCard);
                localDlBook.UpdatedGuideCard = JsonConvert.SerializeObject(changeHistory.UpdatedGuideCard);
            }
            bool validDlbook = (localDlBook.DlStatus == (short)DlStatusEnum.NotDownloaded
                                 || localDlBook.DlStatus == (short)DlStatusEnum.Downloaded);
            if (validDlbook)
            {
                localDlBook.LastDownloadedVersion = serverDlBook.CurrentVersion;
                localDlBook.InstalledDate = DateTime.Now;
                localDlBook.CurrencyDate = serverDlBook.LastUpdatedDate;
                localDlBook.LocalSize = serverDlBook.Size;
                localDlBook.DlStatus = (short)DlStatusEnum.Downloaded;
                localDlBook.Size = serverDlBook.Size;
                localDlBook.UpdateFromServer(serverDlBook, true);
                return localDlBook;
            }
            else
            {
                Logger.Log("DlStatus Error");
                return null;
            }
        }
        /// <summary>
        /// Set Publication Information
        /// </summary>
        /// <param name="onlinePublicationResult"></param>
        /// <param name="onlineDlBooks"></param>
        /// <param name="localDlBooks"></param>
        public IEnumerable<Publication> UpdateLocalDlBooks(List<DlBook> onlineDlBooks, List<DlBook> localDlBooks)
        {
            //add all online dlbook to show
            foreach (var serverDlBook in onlineDlBooks)
            {
                var localDlBook = localDlBooks.FirstOrDefault(b => b.BookId == serverDlBook.BookId);
                if (localDlBook != null)
                {
                    localDlBook.UpdateFromServer(serverDlBook);
                    publicationAccess.Update(localDlBook);
                    if (localDlBook.DlStatus == (short)DlStatusEnum.Downloaded)
                    {//show localdlbook's information if which is downloaded
                        var publication = localDlBook.ToPublication();
                        yield return publication;
                    }
                    else
                    {
                        //update local orderby and rowid to serverdlbook
                        serverDlBook.OrderBy = localDlBook.OrderBy;
                        serverDlBook.RowId = localDlBook.RowId;
                        yield return serverDlBook.ToPublication();
                    }
                }
                else
                {
                    //set dlStatus as NotDownloaded
                    serverDlBook.DlStatus = (short)DlStatusEnum.NotDownloaded;
                    serverDlBook.Email = GlobalAccess.Instance.Email;
                    serverDlBook.ServiceCode = GlobalAccess.Instance.ServiceCode;

                    publicationAccess.InsertDlBook(serverDlBook);
                    yield return serverDlBook.ToPublication();
                }
            }

        }

        public async Task<DlBook> GetLatestDlBookDetail(UserCredential userCredential, string deviceId, int bookId, CancellationToken cancelToken)
        {
            var downloadValidation = await deliveryService.DlFileDetailByDlId(userCredential.Email, deviceId, bookId, cancelToken);
            if (!downloadValidation.IsSuccess)
            {
                throw new Exception("downloadValidate failure");
            }
            var serverDlBook = JsonConvert.DeserializeObject<DlBook>(downloadValidation.Content);
            if (serverDlBook != null)
            {
                serverDlBook.ServiceCode = userCredential.ServiceCode;
                serverDlBook.Email = userCredential.Email;
            }
            else
            {
                throw new NullReferenceException();
            }
            return serverDlBook;
        }

        public async Task DownLoadDlBook(DlBook serverDlBook, CancellationToken cancelToken, DownloadProgressEventHandler downloadHandler)
        {
            var downLoadSuccess = await deliveryService.DlFileDownload(serverDlBook, cancelToken, downloadHandler);
            if (!downLoadSuccess)
            {
                throw new Exception("DlFileDownload failure");
            }

            DlFileStatusUpdate dlFileStatusUpdate = new DlFileStatusUpdate
            {
                DeviceId = GlobalAccess.DeviceId,
                Email = GlobalAccess.Instance.CurrentUserInfo.Email,
                //DLId = serverDlBook.BookId,
                Ver = serverDlBook.CurrentVersion.ToString(),
                StatusCode = ((int)DlStatusEnum.Downloaded).ToString()
            };
            var updateSuccess = await deliveryService.DlFileStatusUpdate(dlFileStatusUpdate, cancelToken);
            if (!updateSuccess)
            {
                throw new Exception("DlFileStatusUpdate failure");
            }
        }

        public async Task<List<DlBook>> GetOnlineDlBooks(string email)
        {
            DomainEvents.Publish(new SyncTagsEvent()).WithNoWarning();
            UserDetails userDetails = new UserDetails
            {
                Account = email,
                DeviceId = GlobalAccess.DeviceId
            };
            var result = await deliveryService.ListFileDetails(userDetails);
            if (!result.IsSuccess)
            {
                throw new Exception("ListFileDetails  exception");
            }
            var myLooseleaf = JsonConvert.DeserializeObject<List<DlBook>>(result.Content);

            return myLooseleaf;
        }



    }
}
