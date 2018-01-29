using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
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


        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public async void GetPublicationOnline_NetDisconnected()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskFailure);

            var getResult = await container.Instance.GetPublicationOnline();
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Failure);
        }

        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public async void GetPublicationOnline_DeletedByUserEmpty()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook>();
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential, true)).Returns(localBooks);
            validateResult.Result.IsSuccess = false;
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = await container.Instance.GetPublicationOnline();
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Failure);
        }
        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_DeletedByUserNotEmpty()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> { new DlBook { BookId = 1, ServiceCode = TestUser.CountryCode, Email = TestUser.UserName } };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);
            bool isDeleted = false;
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.DeleteDlBookByBookId(1, null)).IgnoreArguments().DoInstead(() =>
            {
                isDeleted = true;
            });
            validateResult.Result.IsSuccess = false;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList { DlBooks = null });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileStatusUpdate(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(localBooks[0]);
            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(isDeleted == true);
        }
        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_DeleteAllLocalDlbooks()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook>();
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);
            List<DlBook> localBooks1 = new List<DlBook>() { new DlBook { BookId = 1, IsLoan = true, ServiceCode = TestUser.CountryCode, Email = TestUser.UserName } };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks1);
            bool isClear = false;
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.DeleteDlBookByBookId(Arg.AnyInt, null)).IgnoreArguments().DoInstead(() =>
            {
                isClear = true;
            });
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList { DlBooks = null });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(localBooks1[0]);
            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(isClear == true && getResult.RequestStatus == RequestStatusEnum.Success && (getResult.Publications == null || getResult.Publications.Count == 0));
        }
        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_DeleteSpecialDlbooks()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);

            List<DlBook> localBooks = new List<DlBook> { new DlBook { BookId = 1, IsLoan = true, ServiceCode = TestUser.CountryCode, Email = TestUser.UserName } };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(new List<DlBook>());
            bool isClear = false;
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.DeleteDlBookByBookId(Arg.AnyInt, null)).IgnoreArguments().DoInstead(() =>
            {
                isClear = true;
            });
            bool isinserted = false;
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.InsertDlBook(null)).IgnoreArguments().DoInstead(() =>
            {
                isinserted = true;
            });

            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList { DlBooks = new List<DlBook> { new DlBook { BookId = 2 }, new DlBook { BookId = 3 } } });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileStatusUpdate(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);

            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(localBooks[0]);
            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(isClear && isinserted);
        }
        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_FirstShowPublications()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook>();
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential, Arg.AnyBool)).IgnoreArguments().Returns(localBooks);
            bool isInserted = false;
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.InsertDlBook(null)).IgnoreArguments().DoInstead(() =>
            {
                isInserted = true;
            });
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList { DlBooks = new List<DlBook> { new DlBook { BookId = 1 } } });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success && getResult.Publications.Count > 0 && isInserted);
        }

        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_ExpiredAndDownLoadedDlBooksForSubscription()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook>();
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);
            var localBooks1 = new List<DlBook>();
            localBooks1.Add(new DlBook { ValidTo = DateTime.Now.AddDays(-10).Date, BookId = 2, DlStatus = (short)DlStatusEnum.Downloaded });
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks1);
            bool isInserted = false;
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.InsertDlBook(null)).IgnoreArguments().DoInstead(() =>
            {
                isInserted = true;
            });
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList { DlBooks = new List<DlBook> { new DlBook { BookId = 1 } } });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.Publications.FirstOrDefault(o => o.BookId == 2).ValidTo.Value.Date == DateTime.Now.AddDays(-10).Date);
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success && getResult.Publications.Count > 0 && isInserted);
        }

        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_RequireUpdate()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> { 
               new DlBook{  BookId=1, DlStatus=(short)DlStatusEnum.Downloaded, LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) }
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential, true)).Returns(new List<DlBook>());
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential, null)).Returns(localBooks);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList
            {
                DlBooks = new List<DlBook> { new DlBook { BookId = 1, CurrentVersion = 2, ValidTo = DateTime.Now.AddDays(10) } }
            });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success);
        }
        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_Subscription_Downloaded_DeletedByEreader()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> 
            { 
                new DlBook{BookId=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),OrderBy=1,ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),OrderBy=2,ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),OrderBy=3,ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),OrderBy=4,ServiceCode=TestUser.CountryCode,Email=TestUser.UserName},
                new DlBook{BookId=5, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),OrderBy=5,ServiceCode=TestUser.CountryCode,Email=TestUser.UserName},
                new DlBook{BookId=6, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),OrderBy=6,ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(new List<DlBook>());
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList { DlBooks = new List<DlBook>() });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(localBooks[0]);
            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.Publications.Count == 2);
            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 3));
            var p3 = getResult.Publications.FirstOrDefault(p => p.BookId == 3);

            Assert.IsTrue(p3.ValidTo.Value.DaysRemaining() < 0);

            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 4));
            var p4 = getResult.Publications.FirstOrDefault(p => p.BookId == 4);
            Assert.IsTrue(p4.ValidTo.Value.DaysRemaining() < 0);
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success);
        }

        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_Subscription_Trial()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> 
            { 
                new DlBook{BookId=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),OrderBy=3,ServiceCode=TestUser.CountryCode,Email=TestUser.UserName},
                new DlBook{BookId=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),OrderBy=4,ServiceCode=TestUser.CountryCode,Email=TestUser.UserName},
                new DlBook{BookId=5, IsLoan=false,IsTrial=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),OrderBy=5,ServiceCode=TestUser.CountryCode,Email=TestUser.UserName},
                new DlBook{BookId=6, IsLoan=false,IsTrial=true,  DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),OrderBy=6,ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(new List<DlBook>());
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList { DlBooks = new List<DlBook>() });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(localBooks[0]);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.Publications.Count == 2);
            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 3));
            var p3 = getResult.Publications.FirstOrDefault(p => p.BookId == 3);
            Assert.IsTrue(p3.ValidTo.Value.DaysRemaining() < 0);
            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 4));
            var p4 = getResult.Publications.FirstOrDefault(p => p.BookId == 4);
            Assert.IsTrue(p4.ValidTo.Value.DaysRemaining() < 0);
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success);
        }


        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_OrderByFirst()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> { };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential, true)).IgnoreArguments().Returns(localBooks);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.InsertDlBook(new DlBook())).IgnoreArguments().DoInstead<DlBook>((book) =>
            {
                book.RowId = book.BookId;
            });
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList
            {
                DlBooks = new List<DlBook> {
                    new DlBook{BookId=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),AddedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="AddedGuideCard"}}) },
                    new DlBook{BookId=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),DeletedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="DeletedGuideCard"}}) },
                    new DlBook{BookId=5, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=6, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),UpdatedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="UpdatedGuideCard"}}) }
                }
            });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success);
            Assert.IsTrue(getResult.Publications.Count == 6);
            foreach (var publication in getResult.Publications)
            {
                Assert.IsTrue(publication.RowId == publication.BookId);
            }
        }
        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_OrderBySecond()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> 
            {
                new DlBook{BookId=1,RowId=1, OrderBy=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=2,RowId=2, OrderBy=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=3,RowId=3, OrderBy=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),AddedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="AddedGuideCard"}}) },
                new DlBook{BookId=4,RowId=4, OrderBy=5, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),DeletedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="DeletedGuideCard"}}) },
                new DlBook{BookId=5,RowId=5, OrderBy=3, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=6,RowId=6, OrderBy=6, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),UpdatedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="UpdatedGuideCard"}}) }
              };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(new List<DlBook>());
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.InsertDlBook(new DlBook())).IgnoreArguments().DoInstead<DlBook>((book) =>
            {
                book.RowId = book.BookId;
            });
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList
            {
                DlBooks = new List<DlBook> {
                    new DlBook{BookId=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),AddedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="AddedGuideCard"}}) },
                    new DlBook{BookId=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),DeletedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="DeletedGuideCard"}}) },
                    new DlBook{BookId=5, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=6, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),UpdatedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="UpdatedGuideCard"}}) },
                    new DlBook{BookId=7, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=8, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) }
                }
            });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success);
            Assert.IsTrue(getResult.Publications.Count == 8);
            Assert.IsTrue(getResult.Publications.FirstOrDefault().RowId == 8);
            Assert.IsTrue(getResult.Publications.LastOrDefault().RowId == 6);

        }

        /// <summary>
        /// There is  new book  and a sorting operation
        /// </summary>
        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_OrderBySecond_SortingOperation()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> 
            {
                new DlBook{BookId=1,RowId=1,OrderBy=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=2,RowId=2,OrderBy=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=3,RowId=3,OrderBy=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),AddedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="AddedGuideCard"}}) },
                new DlBook{BookId=4,RowId=4,OrderBy=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),DeletedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="DeletedGuideCard"}}) },
                new DlBook{BookId=5,RowId=5,OrderBy=5, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=6,RowId=6,OrderBy=6, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),UpdatedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="UpdatedGuideCard"}}) }
              };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(new List<DlBook>());
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.InsertDlBook(new DlBook())).IgnoreArguments().DoInstead<DlBook>((book) =>
            {
                book.RowId = book.BookId;
            });
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList
            {
                DlBooks = new List<DlBook> {
                    new DlBook{BookId=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),AddedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="AddedGuideCard"}}) },
                    new DlBook{BookId=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),DeletedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="DeletedGuideCard"}}) },
                    new DlBook{BookId=5, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=6, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),UpdatedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="UpdatedGuideCard"}}) },
                    new DlBook{BookId=7, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),UpdatedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="UpdatedGuideCard"}}) }
                }
            });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success);
            Assert.IsTrue(getResult.Publications.Count == 7);
            Assert.IsTrue(getResult.Publications.FirstOrDefault().RowId == 7);
            Assert.IsTrue(getResult.Publications.LastOrDefault().RowId == 6);
            foreach (var publication in getResult.Publications)
            {
                Assert.IsTrue(publication.RowId == publication.BookId);
            }
        }
        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_Subscription_Downloaded_NoDeletedByEreaderOrNoExpiredHappen()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> 
            { 
                new DlBook{BookId=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),AddedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="AddedGuideCard"}}) },
                new DlBook{BookId=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),DeletedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="DeletedGuideCard"}}) },
                new DlBook{BookId=5, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=6, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),UpdatedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="UpdatedGuideCard"}}) }
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential, true)).Returns(new List<DlBook>());
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential, null)).Returns(localBooks);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList
            {
                DlBooks = new List<DlBook> {
                    new DlBook{BookId=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),AddedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="AddedGuideCard"}}) },
                    new DlBook{BookId=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),DeletedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="DeletedGuideCard"}}) },
                    new DlBook{BookId=5, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) },
                    new DlBook{BookId=6, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),UpdatedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="UpdatedGuideCard"}}) }
                }
            });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.Publications.Count == 6);
            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 3));
            Assert.IsTrue(getResult.Publications.FirstOrDefault(p => p.BookId == 3).AddedGuideCard.FirstOrDefault().Name == "AddedGuideCard");
            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 4));
            Assert.IsTrue(getResult.Publications.FirstOrDefault(p => p.BookId == 4).DeletedGuideCard.FirstOrDefault().Name == "DeletedGuideCard");
            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 6));
            Assert.IsTrue(getResult.Publications.FirstOrDefault(p => p.BookId == 6).UpdatedGuideCard.FirstOrDefault().Name == "UpdatedGuideCard");
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success);
        }

        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_GuideCard_DownloadedTest()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> 
            { 
                new DlBook{BookId=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName  },
                new DlBook{BookId=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),AddedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="AddedGuideCard"}}),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName  },
                new DlBook{BookId=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(-10),DeletedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="DeletedGuideCard"}}),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=5, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(-10),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName  },
                new DlBook{BookId=6, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10),UpdatedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="UpdatedGuideCard"}}),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName  }
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(new List<DlBook>());
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(localBooks[0]);

            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList
            {
                DlBooks = new List<DlBook> 
                {  
                    new DlBook{BookId=6, IsLoan=false,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) }  
                }
            });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.Publications.Count == 3);
            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 3));
            Assert.IsTrue(getResult.Publications.FirstOrDefault(p => p.BookId == 3).AddedGuideCard.FirstOrDefault().Name == "AddedGuideCard");

            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 4));
            Assert.IsTrue(getResult.Publications.FirstOrDefault(p => p.BookId == 4).DeletedGuideCard.FirstOrDefault().Name == "DeletedGuideCard");

            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 6));
            Assert.IsTrue(getResult.Publications.FirstOrDefault(p => p.BookId == 6).UpdatedGuideCard.FirstOrDefault().Name == "UpdatedGuideCard");

            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success);
        }

        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_GuideCard_NotDownloadedTest()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> 
            { 
                new DlBook{BookId=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, OrderBy=1, ValidTo=DateTime.Now.AddDays(10),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1, OrderBy=2,ValidTo=DateTime.Now.AddDays(10),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1,OrderBy=3, ValidTo=DateTime.Now.AddDays(10),AddedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="AddedGuideCard"}}),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1,OrderBy=4, ValidTo=DateTime.Now.AddDays(-10),DeletedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="DeletedGuideCard"}}),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=5, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1,OrderBy=5, ValidTo=DateTime.Now.AddDays(-10),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=6, IsLoan=false, DlStatus=(short)DlStatusEnum.NotDownloaded,LastDownloadedVersion=1,OrderBy=6, ValidTo=DateTime.Now.AddDays(10),UpdatedGuideCard=JsonConvert.SerializeObject(new List<GuideCard>{new GuideCard{ Name="UpdatedGuideCard"}}),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=7, IsLoan=true, DlStatus=(short)DlStatusEnum.RemovedByUser,LastDownloadedVersion=1,OrderBy=7, ValidTo=DateTime.Now.AddDays(10),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
                new DlBook{BookId=8, IsLoan=false, DlStatus=(short)DlStatusEnum.RemovedByUser,LastDownloadedVersion=1,OrderBy=8, ValidTo=DateTime.Now.AddDays(10),ServiceCode=TestUser.CountryCode,Email=TestUser.UserName },
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks.Where(dlbook => dlbook.DlStatus == (short)DlStatusEnum.RemovedByUser).ToList());
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(1, null)).IgnoreArguments().Returns(localBooks[0]);

            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList
            {
                DlBooks = new List<DlBook> 
                {  
                    new DlBook{BookId=9, IsLoan=false,LastDownloadedVersion=1, ValidTo=DateTime.Now.AddDays(10) } 
                }
            });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            Task<bool> updateStatusTask = new Task<bool>(() => { return true; });
            updateStatusTask.Start();
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.DlFileStatusUpdate(null, default(CancellationToken))).IgnoreArguments().Returns(updateStatusTask);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.Publications.Count == 3);
            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 3));
            Assert.IsTrue(getResult.Publications.FirstOrDefault(p => p.BookId == 3).AddedGuideCard.FirstOrDefault().Name == "AddedGuideCard");

            Assert.IsTrue(getResult.Publications.Exists((p) => p.BookId == 4));
            Assert.IsTrue(getResult.Publications.FirstOrDefault(p => p.BookId == 4).DeletedGuideCard.FirstOrDefault().Name == "DeletedGuideCard");


            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success);
        }

        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_ExpiredButCanUpdate()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> 
            { 
                new DlBook{BookId=1, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1,CurrentVersion=2, OrderBy=1, ValidTo=DateTime.Now.AddDays(10)},
                new DlBook{BookId=2, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1,CurrentVersion=2, OrderBy=1,ValidTo=DateTime.Now.AddDays(-10)}
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(new List<DlBook>());
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);

            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList
            {
                DlBooks = new List<DlBook> { }
            });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.Publications.Count == 2);
            var book1 = getResult.Publications.FirstOrDefault(o => o.BookId == 1);
            var book2 = getResult.Publications.FirstOrDefault(o => o.BookId == 2);
            Assert.IsTrue(book1.PublicationStatus == PublicationStatusEnum.Downloaded);
            Assert.IsTrue(book1.DaysRemaining < 0);

            Assert.IsTrue(book2.PublicationStatus == PublicationStatusEnum.Downloaded);
            Assert.IsTrue(book2.DaysRemaining < 0);

        }

        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_HasUpdateCurrencyDate()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            List<DlBook> localBooks = new List<DlBook> 
            { 
                new DlBook{BookId=1, IsLoan=false,CurrencyDate=DateTime.Now.AddDays(-10).Date, DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=2,CurrentVersion=3, OrderBy=1, ValidTo=DateTime.Now.AddDays(10)}
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDeletedDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(new List<DlBook>());
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(localBooks);

            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new DlFileList
            {
                DlBooks = new List<DlBook> {
                new DlBook{BookId=1, IsLoan=false,LastDownloadedVersion=2,CurrentVersion=3, ValidTo=DateTime.Now.AddDays(10),LastUpdatedDate=DateTime.Now.Date}
                }
            });
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(validateResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            var book1 = getResult.Publications.FirstOrDefault(o => o.BookId == 1);
            Assert.IsTrue(book1.PublicationStatus == PublicationStatusEnum.RequireUpdate);
            Assert.IsTrue(book1.CurrencyDate == DateTime.Now.AddDays(-10).Date);
        }
        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_Success()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);

            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential, Arg.AnyBool)).IgnoreArguments().Returns(new List<DlBook>()).InSequence();

            Task<HttpResponse> listDlResult = new Task<HttpResponse>(() =>
            {
                return new HttpResponse
                {
                    IsSuccess = true,
                    Content = JsonConvert.SerializeObject(new DlFileList
                    {
                        DlBooks = new List<DlBook>
                        {
                             new DlBook
                             {
                                 BookId=1,
                                 Author="",
                                 ColorPrimary="",
                                 ColorSecondary="",
                                 CurrentVersion=1,
                                 Description="",
                                 DpsiCode="",
                                 FileUrl="",
                                 FontColor="",
                                 HmacKey="",
                                 InitVector="",
                                 IsLoan=false,
                                 K2Key="",
                                 LastDownloadedVersion=1,
                                 Name="",
                                 Size=1212,
                                 ValidTo=DateTime.Now,
                                 DlStatus=(short)DlStatusEnum.Downloaded
                             }
                        }
                    })
                };
            });
            listDlResult.Start();
            container.Arrange<IDeliveryService>(deliveryService => deliveryService.ListFileDetails(null)).IgnoreArguments().Returns(listDlResult);

            var getResult = container.Instance.GetPublicationOnline().Result;
            Assert.IsTrue(getResult.RequestStatus == RequestStatusEnum.Success);
        }

        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_ServiceDependency1()
        {
            var loginresult = LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Result;
            var loginResult = loginresult == LoginStatusEnum.LoginSuccess;
            Assert.IsTrue(loginResult);
            if (loginResult)
            {
                var onlineResult1 = PublicationUtil.Instance.GetPublicationOnline().Result;

                Assert.IsTrue(onlineResult1.RequestStatus == RequestStatusEnum.Success);
                PublicationUtil.Instance.DeletePublicationByUser(19).Wait();
                onlineResult1 = PublicationUtil.Instance.GetPublicationOnline().Result;

            }
        }
        [Test, Ignore, Category("PublicationUtil_GetPublicationOnline")]
        public void PublicationUtil_DeleteByUser()
        {
            var loginresult = LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Result;
            var loginResult = loginresult == LoginStatusEnum.LoginSuccess;
            Assert.IsTrue(loginResult);
            if (loginResult)
            {
                PublicationUtil.Instance.DeletePublicationByUser(35).Wait();
                Assert.IsTrue(1 == 1);
            }
        }
        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void DaysRemainingTest()
        {
            var Dt1 = DateTime.Parse("2015-05-20 00:00:00.000");
            var Dt3 = DateTime.Parse("2015-05-20 00:00:00.000");
            var Dt5 = DateTime.Parse("2015-05-20 00:00:00.000");
            var Dt4 = DateTime.Parse("2015-05-21 00:00:00.000");
            var Dt6 = DateTime.Parse("2015-05-21 00:00:00.000");
            var Dt7 = DateTime.Parse("2015-05-22 00:00:00.000");
            var Dt2 = DateTime.Parse("2015-05-21 00:00:00.000");
            var r2 = Dt1.Subtract(Dt2).TotalDays;
            var r3 = Dt3.Subtract(Dt2).TotalDays;
            var r5 = Dt5.Subtract(Dt2).TotalDays;
            var r4 = Dt4.Subtract(Dt2).TotalDays;
            var r6 = Dt6.Subtract(Dt2).TotalDays;
            var r7 = Dt7.Subtract(Dt2).TotalDays;
            var d = Dt7.DaysRemaining();
        }


        [Test, Category("PublicationUtil_GetPublicationOnline")]
        public void GetPublicationOnline_ServiceDependencyforDictionary()
        {
            System.IO.File.Delete( @"C:\AppRootPath\dictionaryAU.sqlite");
            var loginresult = LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Result;
            var loginResult = loginresult == LoginStatusEnum.LoginSuccess;
            Assert.IsTrue(loginResult);
            if (loginResult)
            {
                Task t1 = new Task( () => {  PublicationUtil.Instance.GetPublicationOnline(); });
                Task t2 = new Task( () => {  PublicationUtil.Instance.GetPublicationOnline(); });
                Task t3 = new Task( () => {  PublicationUtil.Instance.GetPublicationOnline(); });
                Task t4 = new Task( () => {  PublicationUtil.Instance.GetPublicationOnline(); });
                Task[] tList = new Task[4];
                tList[0] = t1;
                tList[1] = t2;
                tList[2] = t3;
                tList[3] = t4;
                t1.Start();
                t2.Start();
                t3.Start();
                t4.Start();

                Task.WaitAll(tList);
                Thread.Sleep(15000);
            }
            
        }

    }
}
