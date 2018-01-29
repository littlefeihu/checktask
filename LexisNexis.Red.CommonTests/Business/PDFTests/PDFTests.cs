using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexisNexis.Red.Common;
using NUnit.Framework;
using Telerik.JustMock.AutoMock;
using LexisNexis.Red.Common.Interface;
using Telerik.JustMock;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.Common.Entity;
using Newtonsoft.Json;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.CommonTests.Interface;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Business;
using System.Threading;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.CommonTests.Business;
using LexisNexis.Red.Common.Segment.Framework;
using System.IO;
using System.Diagnostics;
namespace LexisNexis.Red.CommonTests.Business
{

    [TestFixture()]
    public partial class PDFTests
    {
        [SetUp()]
        public void Test_Init()
        {
            Stopwatch time = new Stopwatch();
            time.Start();
            TestInit.Init().Wait();
            LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Wait();
            DlBook dlBook = new DlBook
            {
                BookId = 111,
                InitVector = "QDFCMmMzRDRlNUY2ZzdIOA==",
                K2Key = "MXdi9JQDDUY1hf5TZCEHjeBtjKL605dInVG3xYTF76U=",
                HmacKey = "9jU0MAYPOPufUZoZJCW7qhsQe20=",
                LastDownloadedVersion = 1,
                ServiceCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode,
                Email = GlobalAccess.Instance.CurrentUserInfo.Email
            };
            var contentKey = dlBook.GetContentKey(GlobalAccess.Instance.CurrentUserInfo.SymmetricKey).Result;
            GlobalAccess.Instance.CurrentPublication = new PublicationContent(dlBook, contentKey);
            time.Stop();
            Console.Out.WriteLine("Time : " + time.ElapsedMilliseconds);
        }
    }
}
