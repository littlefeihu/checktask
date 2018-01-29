using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService;
using LexisNexis.Red.Common.DomainService.AnnotationSync;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telerik.JustMock.AutoMock;

namespace LexisNexis.Red.CommonTests.Business
{
    [Category("AnnotationDownloadTest")]
    [TestFixture]
    public partial class AnnotationDownloadTest
    {


        public AnnotationDownloadTest()
        {
            LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Wait();
        }
        [Test, Repeat(1)]
        public async void PerformSyncTest()
        {
            try
            {
                IoCContainer.Instance.Resolve<IAnnotationSyncService>().RequestAllDlBooksSync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Test()]
        public void GetTagsTest()
        {
            var tags = AnnCategoryTagUtil.Instance.GetTags();
            Assert.IsTrue(tags.Count > 0);
        }

    }
}
