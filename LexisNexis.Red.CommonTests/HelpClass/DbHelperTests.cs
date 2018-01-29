using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexisNexis.Red.Common.HelpClass;
using NUnit.Framework;
using LexisNexis.Red.CommonTests.Interface;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.CommonTests.Impl;
namespace LexisNexis.Red.Common.HelpClass.Tests
{
    [TestFixture()]
    public class DbHelperTests
    {
        public DbHelperTests()
        {
            IoCContainer.Instance.RegisterInterface<IDevice, Device>();
            IoCContainer.Instance.RegisterInterface<IDirectory,MyDirectory>();
            GlobalAccess.Instance.Init();
        }

    }
}
