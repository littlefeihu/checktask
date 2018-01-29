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
namespace LexisNexis.Red.CommonTests.Business
{

    [TestFixture()]
    public partial class SegmentTests
    {
        [Test, Category("SegmentTests_Prepare")]
        public async void PrepareTest_Regular()
        {
            await SegmentUtil.Instance.Prepare();
        }

        [Test, Category("SegmentTests_Analyse")]
        public async void AnalyseTest_Regular()
        {
            await GlobalAccess.Instance.Init();
            string key = @"""=.;+-|/\':?<>[]{}!@#$%^&*()~`　，。；‘’“”／？～！＠＃￥％……＆×（—）【】｛｝｜、《》：and or near not CE_FJJ.SGM_COE.FJJ.C6";
            List<string> words =
                 SegmentUtil.Instance.PhraseSegment(key);
            Assert.AreEqual("CE NEAR/26 FJJ NEAR/26 SGM NEAR/26 COE NEAR/26 FJJ NEAR/26 C6", words[0]);
        }

        [Test, Category("SegmentTests_Analyse")]
        public async void AnalyseTest_Empty()
        {
            await GlobalAccess.Instance.Init();
            string key = @"";

            List<string> words =
                 SegmentUtil.Instance.PhraseSegment(key);
            Assert.AreEqual("", words);
        }
    }
}
