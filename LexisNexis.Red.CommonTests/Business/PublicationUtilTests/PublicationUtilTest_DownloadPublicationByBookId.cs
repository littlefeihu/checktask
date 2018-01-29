using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.Tools;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.CommonTests.Interface;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telerik.JustMock;
using Telerik.JustMock.AutoMock;

namespace LexisNexis.Red.CommonTests.Business
{
    [TestFixture]
    public partial class PublicationUtilTest
    {

        [Test, Category("PublicationUtil_DownloadPublication")]
        public async void DownloadPublicationByBookId_NetDisconnected()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, source.Token)).IgnoreArguments().Returns(pingTaskFailure);
            var downloadResult = await container.Instance.DownloadPublicationByBookId(1, source.Token, null);
            Assert.IsTrue(downloadResult.DownloadStatus == DownLoadEnum.NetDisconnected);
        }
        [Test, Category("PublicationUtil_DownloadPublication")]
        public async void DownloadPublicationByBookId_OverLimitation()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, source.Token)).IgnoreArguments().Returns(pingTaskSuccess);

            container.Arrange<INetwork>(networkService => networkService.GetNetworkType()).Returns(NetworkTypeEnum.Mobile);
            long flowLimitations = 20;
            container.Arrange<INetwork>(networkService => networkService.GetFlowLimitation()).Returns(flowLimitations);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(dlBook);
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileDetailByDlId(null, null, 0, default(CancellationToken))).IgnoreArguments().Returns(validateResult);

            var downloadResult = await container.Instance.DownloadPublicationByBookId(1, source.Token, null, true);
            Assert.IsTrue(downloadResult.DownloadStatus == DownLoadEnum.OverLimitation);
        }

        [Test, Category("PublicationUtil_DownloadPublication")]
        public async void DownloadPublicationByBookId_LessThanOverLimitation()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, source.Token)).IgnoreArguments().Returns(pingTaskSuccess);
            dlBook.Size = 10;
            container.Arrange<IPublicationAccess>(publicationService => publicationService.GetDlBookByBookId(Arg.AnyInt, null)).Returns(dlBook);

            container.Arrange<INetwork>(networkService => networkService.GetNetworkType()).Returns(NetworkTypeEnum.Mobile);
            long flowLimitations = 20;
            container.Arrange<INetwork>(networkService => networkService.GetFlowLimitation()).Returns(flowLimitations);

            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileDownload(dlBook, source.Token, null)).IgnoreArguments().Returns(pingTaskFailure);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(dlBook);
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileDetailByDlId(null, null, 0, default(CancellationToken))).IgnoreArguments().Returns(validateResult);

            var downloadResult = await container.Instance.DownloadPublicationByBookId(1, source.Token, null, true);
            Assert.IsTrue(downloadResult.DownloadStatus == DownLoadEnum.Failure);
        }
        [Test, Category("PublicationUtil_DownloadPublication")]
        public async void DownloadPublicationByBookId_NetDisconnected1()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, source.Token)).IgnoreArguments().Returns(pingTaskSuccess);
            dlBook.Size = 100;
            container.Arrange<IPublicationAccess>(publicationService => publicationService.GetDlBookByBookId(Arg.AnyInt, null)).Returns(dlBook);
            container.Arrange<INetwork>(networkService => networkService.GetNetworkType()).Returns(NetworkTypeEnum.None);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(dlBook);
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileDetailByDlId(null, null, 0, default(CancellationToken))).IgnoreArguments().Returns(validateResult);

            var downloadResult = await container.Instance.DownloadPublicationByBookId(1, source.Token, null, true);
            Assert.IsTrue(downloadResult.DownloadStatus == DownLoadEnum.NetDisconnected);
        }
        [Test, Category("PublicationUtil_DownloadPublication")]
        public async void DownloadPublicationByBookId_CancelDownload()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, source.Token)).IgnoreArguments().Returns(pingTaskSuccess);

            dlBook.DlStatus = (short)DlStatusEnum.Downloaded;
            container.Arrange<IPublicationAccess>(publicationService => publicationService.GetDlBookByBookId(Arg.AnyInt, null)).Returns(dlBook);
            container.Arrange<INetwork>(networkService => networkService.GetNetworkType()).Returns(NetworkTypeEnum.None);
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileDownload(dlBook, source.Token, null)).IgnoreArguments().Throws(new OperationCanceledException());

            validateResult.Result.IsSuccess = true;
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileDetailByDlId(null, null, 0, default(CancellationToken))).IgnoreArguments().Returns(validateResult);
            source.Cancel();
            var downloadResult = await container.Instance.DownloadPublicationByBookId(1, source.Token, null, false);
            Assert.IsTrue(downloadResult.DownloadStatus == DownLoadEnum.Canceled);
        }

        [Test, Ignore]
        public async void DownloadPublicationByBookId_DownloadSuccess_NoNeedInstall()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, source.Token)).IgnoreArguments().Returns(pingTaskSuccess);

            dlBook.DlStatus = (short)DlStatusEnum.NotDownloaded;
            dlBook.LastUpdatedDate = new DateTime(2015, 1, 1);
            dlBook.CurrentVersion = 3;

            container.Arrange<IPublicationAccess>(publicationService => publicationService.GetDlBookByBookId(Arg.AnyInt, null)).IgnoreArguments().Returns(dlBook);
            container.Arrange<INetwork>(networkService => networkService.GetNetworkType()).Returns(NetworkTypeEnum.None);
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileDownload(dlBook, source.Token, null)).IgnoreArguments().Returns(pingTaskSuccess);
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileStatusUpdate(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            validateResult.Result.IsSuccess = true;

            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlVersionChangeHistory(null)).IgnoreArguments().Returns(validateResult);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(dlBook);
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileDetailByDlId(null, null, 0, default(CancellationToken))).IgnoreArguments().Returns(validateResult);

            var dlMetadata = new DlMetadataDetails { SubCategory = "SubCategory", PracticeArea = "PracticeArea" };
            var metadataResult = new Task<HttpResponse>(() =>
               {
                   return new HttpResponse
                   {
                       IsSuccess = true,
                       Content = JsonConvert.SerializeObject(dlMetadata)
                   };
               });
            metadataResult.Start();
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlMetadata(null)).IgnoreArguments().Returns(metadataResult);

            var downloadResult = await container.Instance.DownloadPublicationByBookId(1, source.Token, (t, downloadSize) => { }, false);
            var returnPublication = downloadResult.Publication;
            Assert.IsTrue(returnPublication.PublicationStatus == PublicationStatusEnum.Downloaded);
            Assert.IsTrue(returnPublication.LastDownloadedVersion == dlBook.CurrentVersion);
            Assert.IsTrue(returnPublication.InstalledDate.Value.Date == DateTime.Now.Date);
            Assert.IsTrue(returnPublication.CurrencyDate == dlBook.LastUpdatedDate);
            Assert.IsTrue(returnPublication.LocalSize == dlBook.Size);
            Assert.IsTrue(returnPublication.SubCategory == dlMetadata.SubCategory);
            Assert.IsTrue(returnPublication.PracticeArea == dlMetadata.PracticeArea);
            Assert.IsTrue(downloadResult.DownloadStatus == DownLoadEnum.Success);
        }

        [Test, Category("PublicationUtil_DownloadPublication")]
        public async void DownloadPublicationByBookId_ValidateCurrencyDate_ServiceDependence()
        {
            var publicationUtil = PublicationUtil.Instance;
            int bookId = 41;
            var list = await publicationUtil.GetPublicationOnline();
            var d = list.Publications.FirstOrDefault(p => p.BookId == bookId).CurrencyDate;

            //download this book
            var downloadResult = await publicationUtil.DownloadPublicationByBookId(bookId, source.Token, new DownloadProgressEventHandler((percentage, downloadSize) =>
            {
                Debug.Write(percentage);
            }));

            var localPublications = publicationUtil.GetPublicationOffline();
            Assert.IsTrue(localPublications.FirstOrDefault(o => o.BookId == bookId).PublicationStatus == PublicationStatusEnum.Downloaded);

        }
        [Test, Category("PublicationUtil_DownloadPublication")]
        public async void DownloadPublicationByBookId_InstallDlBook_ServiceDependence()
        {
            var publicationUtil = PublicationUtil.Instance;
            var dd = await publicationUtil.GetPublicationOnline();
            int bookId = 41;
            //download this book 
            var downloadResult = await publicationUtil.DownloadPublicationByBookId(bookId, source.Token, new DownloadProgressEventHandler((bytes, downloadSize) =>
            {
                Debug.Write(bytes);
            }), false);
            var localPublications = publicationUtil.GetPublicationOffline();
            var status = localPublications.FirstOrDefault(o => o.BookId == bookId).PublicationStatus == PublicationStatusEnum.Downloaded;
            Assert.IsTrue(status);
            if (status)
            {
                // set this book as requireupdate
                var publicationAccess = new PublicationAccess();

                //reset book status to notdownloaded
                var downloadedDlBook = publicationAccess.GetDlBookByBookId(bookId, GlobalAccess.Instance.UserCredential);
                downloadedDlBook.DlStatus = (short)DlStatusEnum.NotDownloaded;
                downloadedDlBook.InstalledDate = null;
                downloadedDlBook.CurrencyDate = null;
                downloadedDlBook.LocalSize = 0;
                downloadedDlBook.SubCategory = null;
                downloadedDlBook.PracticeArea = null;
                downloadedDlBook.DeletedGuideCard = null;
                downloadedDlBook.AddedGuideCard = null;
                downloadedDlBook.UpdatedGuideCard = null;
                publicationAccess.Update(downloadedDlBook);
                Assert.IsTrue(downloadResult.DownloadStatus == DownLoadEnum.Success);
            }
        }
        [Test, Category("PublicationUtil_DownloadPublication")]
        public async void DownloadPublicationByBookIdExpire_ServiceDependence()
        {
            var publicationUtil = PublicationUtil.Instance;
            var dd = await publicationUtil.GetPublicationOnline();
            int bookId = 41;
            //download this book 
            var downloadResult = await publicationUtil.DownloadPublicationByBookId(bookId, source.Token, new DownloadProgressEventHandler((bytes, downloadSize) =>
            {
                Debug.Write(bytes);
            }));

            var localPublications = publicationUtil.GetPublicationOffline();
            var status = localPublications.FirstOrDefault(o => o.BookId == bookId).PublicationStatus == PublicationStatusEnum.Downloaded;

        }
        [Test, Category("PublicationUtil_DownloadPublication")]
        public async void DownloadPublicationByBookId_ServiceDependence()
        {
            var publicationUtil = PublicationUtil.Instance;
            var dd = await publicationUtil.GetPublicationOnline();
            int bookId = 26;
            //download this book 
            var downloadResult = await publicationUtil.DownloadPublicationByBookId(bookId, source.Token, new DownloadProgressEventHandler((bytes, downloadSize) =>
            {
                Debug.Write(bytes);
            }));
            var localPublications = publicationUtil.GetPublicationOffline();
            var status = localPublications.FirstOrDefault(o => o.BookId == bookId).PublicationStatus == PublicationStatusEnum.Downloaded;
            Assert.IsTrue(status);
            if (status)
            {
                var publicationAccess = new PublicationAccess();
                //reset book status to notdownloaded
                var downloadedDlBook = publicationAccess.GetDlBookByBookId(bookId, GlobalAccess.Instance.UserCredential);
                downloadedDlBook.DlStatus = (short)DlStatusEnum.NotDownloaded;
                downloadedDlBook.InstalledDate = null;
                downloadedDlBook.CurrencyDate = null;
                downloadedDlBook.LocalSize = 0;
                downloadedDlBook.SubCategory = null;
                downloadedDlBook.PracticeArea = null;
                downloadedDlBook.DeletedGuideCard = null;
                downloadedDlBook.AddedGuideCard = null;
                downloadedDlBook.UpdatedGuideCard = null;
                //publicationAccess.Update(downloadedDlBook);
                Assert.IsTrue(downloadResult.DownloadStatus == DownLoadEnum.Success);
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
