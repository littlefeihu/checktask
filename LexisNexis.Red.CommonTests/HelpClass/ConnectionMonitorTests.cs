using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexisNexis.Red.Common.HelpClass;
using NUnit.Framework;
using LexisNexis.Red.CommonTests.Interface;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.Common.Interface;
namespace LexisNexis.Red.Common.HelpClass.Tests
{
    [TestFixture()]
    public class ConnectionMonitorTests
    {

        public ConnectionMonitorTests()
        {
            IoCContainer.Instance.RegisterInterface<IDevice, Device>();
            IoCContainer.Instance.RegisterInterface<IDirectory, MyDirectory>();
            IoCContainer.Instance.RegisterInterface<INetwork, Network>();

            GlobalAccess.Instance.Init();
        }
        [Test()]
        public async void PingServiceTest()
        {
            var pingResult = await new ConnectionMonitor().PingService("AU");
        }
    }
}
