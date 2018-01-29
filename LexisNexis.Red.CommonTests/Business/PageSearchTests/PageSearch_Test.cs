using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Business
{
    [TestFixture, Category("PageSearch_Test")]
    public class PageSearch_Test
    {
        public PageSearch_Test()
        {
            LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Wait();
            var publications = PublicationUtil.Instance.GetPublicationOnline().Result;
            PublicationUtil.Instance.DownloadPublicationByBookId(19, default(CancellationToken), (x, y) => { }, false).Wait();
        }
        [Test]
        public async void ShowPageNavigator()
        {
            var hasPage = await PageSearchUtil.Instance.IsPBO(19);
            Assert.IsTrue(hasPage);
        }
        [Test()]
        public async void SeachByPageNum()
        {
            var page = await PageSearchUtil.Instance.SeachByPageNum(19, 1);
            Assert.IsTrue(page.Count > 0);
        }
        [Test()]
        public async void GetPagesByFileName()
        {
            var page = await PageSearchUtil.Instance.GetPagesByTOCID(19, 17);
            Assert.IsTrue(page.Count > 0);
        }

        [Test()]
        public async void GetFirstPageItem()
        {
            var page = await PageSearchUtil.Instance.GetFirstPageItem(19, 70);
            Assert.IsTrue(page != null);
        }

    }
}
