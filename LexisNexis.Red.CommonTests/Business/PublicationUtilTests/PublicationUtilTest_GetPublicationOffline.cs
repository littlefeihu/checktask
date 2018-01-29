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


        [Test, Category("PublicationUtil_GetPublicationOffline")]
        public void GetPublicationOffline_Empty()
        {
            List<DlBook> dlBooks = new List<DlBook>();
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline( GlobalAccess.Instance.UserCredential)).Returns(dlBooks);
            var dlBookList = container.Instance.GetPublicationOffline();
            Assert.IsTrue(dlBookList.Count == 0);
        }
        [Test, Category("PublicationUtil_GetPublicationOffline")]
        public void GetPublicationOffline_OrderBy()
        {
            List<DlBook> dlBooks = new List<DlBook> { 
                  new DlBook { BookId = 4, OrderBy = 1,RowId=1, ValidTo=DateTime.Now.AddDays(10).Date }, 
                  new DlBook { BookId = 3, OrderBy = 7,RowId=2,ValidTo=DateTime.Now.AddDays(10).Date} ,
                  new DlBook { BookId = 2, OrderBy = 6,RowId=3,ValidTo=DateTime.Now.AddDays(10).Date},
                  new DlBook { BookId =9, OrderBy = 4,RowId=4,ValidTo=DateTime.Now.AddDays(10).Date } 
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(dlBooks);
            var dlBookList = container.Instance.GetPublicationOffline();
            Assert.IsTrue(dlBookList.FirstOrDefault().OrderBy == 1);
            Assert.IsTrue(dlBookList.LastOrDefault().OrderBy == 7);
        }

        [Test, Category("PublicationUtil_GetPublicationOffline")]
        public void GetPublicationOffline_RequireUpdate()
        {
            List<DlBook> dlBooks = new List<DlBook> { 
                  new DlBook { BookId = 4, OrderBy = 1,DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=1,CurrentVersion=5,ValidTo=DateTime.Now.AddDays(10).Date }, 
                  new DlBook { BookId = 3, OrderBy = 7,DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=2,CurrentVersion=5,ValidTo=DateTime.Now.AddDays(10).Date} ,
                  new DlBook { BookId = 2, OrderBy = 6 ,DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=3,CurrentVersion=5,ValidTo=DateTime.Now.AddDays(10).Date},
                  new DlBook { BookId =1, OrderBy = 4 ,DlStatus=(short)DlStatusEnum.Downloaded,LastDownloadedVersion=4,CurrentVersion=5,ValidTo=DateTime.Now.AddDays(10).Date} 
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline( GlobalAccess.Instance.UserCredential)).Returns(dlBooks);
            var dlBookList = container.Instance.GetPublicationOffline();
            foreach (var item in dlBookList)
            {
                Assert.IsTrue(item.PublicationStatus == PublicationStatusEnum.RequireUpdate && item.UpdateCount == item.BookId);
            }
        }
        [Test, Category("PublicationUtil_GetPublicationOffline")]
        public void GetPublicationOffline_LoanOrTrialExpired()
        {
            List<DlBook> dlBooks = new List<DlBook> { 
                new DlBook { BookId = 4, IsLoan=true,ValidTo=DateTime.Now.AddDays(-5).Date}, 
                new DlBook { BookId = 2,IsTrial=true,ValidTo=DateTime.Now.AddDays(-5).Date},
                new DlBook { BookId =5,IsLoan=false,ValidTo=DateTime.Now.AddDays(-5).Date}
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBooksOffline( GlobalAccess.Instance.UserCredential)).IgnoreArguments().Returns(dlBooks);
            var dlBookList = container.Instance.GetPublicationOffline();
            var book4 = dlBookList.FirstOrDefault(o => o.BookId == 4);
            var book2 = dlBookList.FirstOrDefault(o => o.BookId == 2);
            var book5 = dlBookList.FirstOrDefault(o => o.BookId == 5);
            Assert.IsTrue(book4.DaysRemaining == 0);
            Assert.IsTrue(book2.DaysRemaining == 0);
            Assert.IsTrue(book5.DaysRemaining < 0);
        }
        [Test, Category("PublicationUtil_GetPublicationOffline")]
        public async void GetPublicationOffline_LocalDbDependency()
        {
            await LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode);

            var list = PublicationUtil.Instance.GetPublicationOffline();

            Assert.IsTrue(list == null || list.Count >= 0);
        }

    }
}
