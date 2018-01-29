using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock.AutoMock;

namespace LexisNexis.Red.CommonTests.Business
{
    public partial class PublicationContentUtilTest
    {
        AutoMockSettings autoMockSettings;
        MockingContainer<PublicationContentUtil> container;
        DlBook dlBook;
        int bookId = 41;
        public PublicationContentUtilTest()
        {
            LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Wait();
            dlBook = new DlBook
            {
                BookId = bookId,
                InitVector = "QDFCMmMzRDRlNUY2ZzdIOA==",
                K2Key = "MXdi9JQDDUY1hf5TZCEHjeBtjKL605dInVG3xYTF76U=",
                HmacKey = "9jU0MAYPOPufUZoZJCW7qhsQe20=",
                LastDownloadedVersion = 1,
                ServiceCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode,
                Email = GlobalAccess.Instance.CurrentUserInfo.Email
            };
            //var contentKey = dlBook.GetContentKey(GlobalAccess.Instance.CurrentUserInfo.SymmetricKey).Result;
            //GlobalAccess.Instance.CurrentPublication = new PublicationContent(dlBook, contentKey);
            autoMockSettings = new AutoMockSettings
            {
                ConstructorArgTypes = new Type[] { typeof(IPublicationAccess), typeof(IPackageAccess), typeof(IRecentHistoryAccess) }
            };
            container = new MockingContainer<PublicationContentUtil>(autoMockSettings);

        }
        [SetUp]
        public void SetUp()
        {

        }

        [TearDown]
        public void Cleanup()
        {

        }
    }
}
