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
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.CommonTests.Business;
namespace LexisNexis.Red.CommonTests.Business
{
    [TestFixture()]
    public partial class NavigationManagerTests
    {
        [Test, Category("BackAndForwardRecordTest")]
        public void BackAndForwardRecordTest()
        {
            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(1, 1, 1, (float)102.5));
            NavigationManager.Instance.AddRecord(new SearchBrowserRecord(1, 1, (float)102.5, 1, "","",1));
            NavigationManager.Instance.AddRecord(new AnnotationNavigatorRecord(1, 1, (float)102.5, 5, Guid.NewGuid(), new List<AnnotationTag>(), null));
            NavigationManager.Instance.AddRecord(new AnnotationNavigatorRecord(1, 1, (float)102.5, 7, Guid.NewGuid(), new List<AnnotationTag>(), AnnotationType.Highlight));

            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(1, 1, 1, (float)102.5));
            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(1, 1, 1, (float)102.5));
            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(2, 1, 1, (float)102.5));
            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(3, 1, 1, (float)102.7));
            NavigationManager.Instance.AddRecord(new AnnotationOrganiserRecord(Guid.Parse("52B4282A-B658-43F4-82AC-551E97D23BC0"), new List<AnnotationTag>(), null, "keyWords", null));
            NavigationManager.Instance.AddRecord(new AnnotationOrganiserRecord(Guid.Parse("52B4282A-B658-43F4-82AC-551E97D23BC0"), new List<AnnotationTag>(), null, "keyWords", null));

            var str = NavigationManager.Instance.SerializeRecords();

            Console.WriteLine(str);
            NavigationManager.Instance.RestoreRecords(str);
            var record = NavigationManager.Instance.CurrentRecord;
            Assert.IsTrue(record is AnnotationOrganiserRecord);

            if (NavigationManager.Instance.CanBack)
            {
                record = NavigationManager.Instance.Back();
                Assert.IsTrue(record is ContentBrowserRecord);

            }
            if (NavigationManager.Instance.CanBack)
            {
                record = NavigationManager.Instance.Back();
                Assert.IsTrue(record is ContentBrowserRecord);

            }
            if (NavigationManager.Instance.CanBack)
            {
                record = NavigationManager.Instance.Back();
                Assert.IsTrue(record is ContentBrowserRecord);

            }
            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(2, 1, 1, (float)102.5));

            Assert.IsTrue(!NavigationManager.Instance.CanForth);

        }
        [Test, Category("CalculateCurrentIndexTest")]
        public void CalculateCurrentIndexTest()
        {
            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(1, 1, 1, (float)102.5));
            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(2, 2, 1, (float)102.5));
            NavigationManager.Instance.AddRecord(new AnnotationNavigatorRecord(1, 1, (float)102.5, 5, Guid.NewGuid(), new List<AnnotationTag>(), null));
            NavigationManager.Instance.AddRecord(new AnnotationNavigatorRecord(1, 1, (float)102.5, 7, Guid.NewGuid(), new List<AnnotationTag>(), AnnotationType.Highlight));
            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(1, 1, 1, (float)102.5));
            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(2, 1, 1, (float)102.5));
            NavigationManager.Instance.AddRecord(new ContentBrowserRecord(2, 2, 1, (float)102.5));
            string recordsString = NavigationManager.Instance.SerializeRecords();
            NavigationManager.Instance.RestoreRecords(recordsString);
            NavigationManager.Instance.Back();
            NavigationManager.Instance.Back();
            NavigationManager.Instance.Back();
            NavigationManager.Instance.CalculateCurrentIndex(2, 2);

        }


    }
}
