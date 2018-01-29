using LexisNexis.Red.Common.Business;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Business
{
    [SetUpFixture]
    public class Custom_SetUpFixture
    {

        [SetUp]
        public void RunBeforeAnyTests()
        {
            TestInit.Init().Wait();
        }

        [TearDown]
        public void RunAfterAnyTests()
        {
            LoginUtil.Instance.Logout();
        }
    }
}
