using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock.AutoMock;

namespace LexisNexis.Red.CommonTests.Business
{
    public partial class AnnCategorytagUtilTests
    {
        AutoMockSettings autoMockSettings;
        MockingContainer<AnnCategoryTagUtil> container;

        public AnnCategorytagUtilTests()
        {
          
            container = new MockingContainer<AnnCategoryTagUtil>();
            LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Wait();
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
