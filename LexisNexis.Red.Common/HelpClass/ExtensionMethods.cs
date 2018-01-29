using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using System.Threading;
using System.Xml.Linq;
namespace LexisNexis.Red.Common.HelpClass
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// convert dlbook to publication
        /// </summary>
        /// <param name="dlBook"></param>
        /// <returns></returns>
        public static Publication ToPublication(this DlBook dlBook)
        {
            Publication publication = new Publication
            {
                RowId = dlBook.RowId,
                Author = dlBook.Author,
                BookId = dlBook.BookId,
                ColorPrimary = dlBook.ColorPrimary,
                ColorSecondary = dlBook.ColorSecondary,
                FontColor = dlBook.FontColor,
                Description = dlBook.Description,
                DpsiCode = dlBook.DpsiCode,
                CountryCode = dlBook.CountryCode,
                FileUrl = dlBook.FileUrl,
                HmacKey = dlBook.HmacKey,
                InitVector = dlBook.InitVector,
                K2Key = dlBook.K2Key,
                IsLoan = dlBook.IsLoan,
                IsTrial = dlBook.IsTrial,
                CurrentVersion = dlBook.CurrentVersion,
                LastDownloadedVersion = dlBook.LastDownloadedVersion,
                LastUpdatedDate = dlBook.LastUpdatedDate,
                Name = dlBook.Name,
                Size = dlBook.Size,
                ValidTo = dlBook.ValidTo,
                PublicationStatus = (dlBook.DlStatus == (short)DlStatusEnum.Downloaded ? PublicationStatusEnum.Downloaded : PublicationStatusEnum.NotDownloaded),
                LocalSize = dlBook.LocalSize,
                InstalledDate = dlBook.InstalledDate,
                CurrencyDate = dlBook.CurrencyDate,
                PracticeArea = dlBook.PracticeArea,
                SubCategory = dlBook.SubCategory,
                OrderBy = dlBook.OrderBy,
                AddedGuideCard = !string.IsNullOrEmpty(dlBook.AddedGuideCard) ? JsonConvert.DeserializeObject<List<GuideCard>>(dlBook.AddedGuideCard) : null,
                DeletedGuideCard = !string.IsNullOrEmpty(dlBook.DeletedGuideCard) ? JsonConvert.DeserializeObject<List<GuideCard>>(dlBook.DeletedGuideCard) : null,
                UpdatedGuideCard = !string.IsNullOrEmpty(dlBook.UpdatedGuideCard) ? JsonConvert.DeserializeObject<List<GuideCard>>(dlBook.UpdatedGuideCard) : null

            };
            bool hasNotExpired = (publication.DaysRemaining >= 0);
            bool hasDownloaded = (publication.PublicationStatus == PublicationStatusEnum.Downloaded);
            if (hasDownloaded && hasNotExpired)
            {
                int updateCount = publication.CurrentVersion - publication.LastDownloadedVersion;
                if (updateCount > 0)
                {
                    publication.PublicationStatus = PublicationStatusEnum.RequireUpdate;
                    publication.UpdateCount = updateCount;
                }
            }

            return publication;
        }
        public static List<Publication> ToPublications(this IEnumerable<DlBook> dlBooks)
        {
            var q = from dlBook in dlBooks
                    select dlBook.ToPublication();
            return q.ToList();
        }
        /// <summary>
        /// formatter date 
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToDayMonthYearDate(this DateTime datetime)
        {
            return datetime.ToString("dd MM yyyy");
        }
        /// <summary>
        /// DaysRemaining
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static int DaysRemaining(this DateTime datetime)
        {
            return (int)Math.Ceiling(datetime.Subtract(DateTime.Today).TotalDays);
        }
        public static LoginUserDetails CheckIsNullUser(this LoginUserDetails user)
        {
            if (user == null)
            {
                Logger.Log("GetPublicationOffline:NullUserException");
                throw new NullUserException();
            }
            return user;
        }
        public static TOCNode GetLastPage(this TOCNode node)
        {
            if (node.ChildNodes != null && node.ChildNodes.Count > 0)
            {
                var pageNode = node.ChildNodes[node.ChildNodes.Count - 1].GetLastPage();
                if (pageNode != null)
                {
                    return pageNode;
                }
            }
            else
            {
                return node;
            }
            return null;
        }
        public static TOCNode GetFirstPage(this TOCNode node)
        {
            if (node.ChildNodes != null && node.ChildNodes.Count > 0)
            {
                var pageNode = node.ChildNodes[0].GetFirstPage();
                if (pageNode != null)
                {
                    return pageNode;
                }
            }
            else
            {
                return node;
            }
            return null;
        }
        public static bool IsParent(this TOCNode node)
        {
            return node.Role.ToLower() == Constants.ANCESTOR;
        }
        public static TOCNode NextSibling(this TOCNode tocNode)
        {
            if (tocNode != null && tocNode.ParentNode != null)
            {
                int index = tocNode.ParentNode.ChildNodes.IndexOf(tocNode, 0);
                bool isLastPosition = index + 1 == tocNode.ParentNode.ChildNodes.Count;
                if (isLastPosition)
                {//last node
                    return null;
                }
                else
                {
                    return tocNode.ParentNode.ChildNodes[index + 1];
                }
            }
            return null;
        }
        public static TOCNode PreviousSibling(this TOCNode tocNode)
        {
            if (tocNode != null && tocNode.ParentNode != null)
            {
                int index = tocNode.ParentNode.ChildNodes.IndexOf(tocNode, 0);
                if (index == 0)
                {//first node
                    return null;
                }
                else
                {
                    return tocNode.ParentNode.ChildNodes[index - 1];
                }
            }
            return null;
        }

        //public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        //{
        //    return Task.Run<T>(async () =>
        //    {
        //        return await task;
        //    }, cancellationToken);
        //}
        //public static Task WithCancellation(this Task task, CancellationToken cancellationToken)
        //{
        //    return Task.Run(async () =>
        //    {
        //        await task;
        //    }, cancellationToken);
        //}

        public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken, TimeSpan? timeSpan = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task, Task.Run<T>(async () =>
                {
                    if (timeSpan == null)
                        timeSpan = new TimeSpan(1, 0, 0);

                    await Task.Delay(timeSpan.Value);
                    return default(T);
                })))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
                return await task;
            }
        }
        public static async Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
            {
                if (task != await Task.WhenAny(task, tcs.Task))
                {
                    throw new OperationCanceledException(cancellationToken);
                }
                await task;
            }
        }

        public static T DeserializeObject<T>(this HttpResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        public static async Task ForEach<T>(this IEnumerable<T> source, Func<T, Task> function)
        {
            foreach (T item in source)
                await function(item);
        }

        public static void WithNoWarning(this Task task)
        {

        }


    }
}
