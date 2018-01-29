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
using LexisNexis.Red.Common.DominService;
namespace LexisNexis.Red.CommonTests.Business
{
    [TestFixture]
    public partial class LoginUtilTests
    {

        [Test, Category("LoginUtil_ChangePassword")]
        public async void ChangePassword_LengthInvalid()
        {
            var changeResult = await container.Instance.ChangePassword(invalidPassword, invalidPassword);
            Assert.IsTrue(changeResult == PasswordChangeEnum.LengthInvalid);
        }
        [Test, Category("LoginUtil_ChangePassword")]
        public async void ChangePassword_NotMatch()
        {
            var changeResult = await container.Instance.ChangePassword(validPassword1, validPassword2);
            Assert.IsTrue(changeResult == PasswordChangeEnum.NotMatch);
        }
        [Test, Category("LoginUtil_ChangePassword")]
        public async void ChangePassword_NetDisconnected()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskFailure);

            var changeResult = await container.Instance.ChangePassword(validPassword1, validPassword1);
            Assert.IsTrue(changeResult == PasswordChangeEnum.NetDisconnected);
        }

        [Test, Category("LoginUtil_ChangePassword")]
        public async void ChangePassword_ChangeSuccess()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
          
            container.Arrange<ILoginDomainService>((loginDomainService) => loginDomainService.PasswordChange(null, null, null, null)).IgnoreArguments().Returns( Task.Run(()=>{ return PasswordChangeEnum.ChangeSuccess; }));

            var changeResult = await container.Instance.ChangePassword(validPassword1, validPassword1);
            Assert.IsTrue(changeResult == PasswordChangeEnum.ChangeSuccess);
        }

        [Test, Category("LoginUtil_ChangePassword")]
        public async void ChangePassword_ChangeSuccess_ServiceDependency()
        {
            var changeResult = await LoginUtil.Instance.ChangePassword(validPassword1, validPassword1);
            Assert.IsTrue(changeResult == PasswordChangeEnum.ChangeSuccess);
        }
        [Test, Category("LoginUtil_ChangePassword")]
        public async void ChangePassword_ChangeFailure()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(null, default(CancellationToken))).IgnoreArguments().Returns(pingTaskSuccess);
            container.Arrange<ILoginDomainService>(loginDomainService => loginDomainService.PasswordChange(null, null, null, null)).IgnoreArguments().Throws(new Exception(""));

            var changeResult = await container.Instance.ChangePassword(validPassword1, validPassword1);
            Assert.IsTrue(changeResult == PasswordChangeEnum.ChangeFailure);
        }
    }
}
