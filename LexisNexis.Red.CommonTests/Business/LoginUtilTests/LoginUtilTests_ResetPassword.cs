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

    [TestFixture()]
    public partial class LoginUtilTests
    {
        [Test, Category("LoginUtil_ResetPassword")]
        public async void ResetPassword_InvalidEmail()
        {
            var resetResult = await container.Instance.ResetPassword(invalidEmail, countryCode);
            Assert.IsTrue(resetResult == PasswordResetEnum.InvalidEmail);
        }

        [Test, Category("LoginUtil_ResetPassword")]
        public async void ResetPassword_SelectCountry()
        {
            var resetResult = await container.Instance.ResetPassword(validEmail, null);
            Assert.IsTrue(resetResult == PasswordResetEnum.SelectCountry);
        }


        [Test, Category("LoginUtil_ResetPassword")]
        public async void ResetPassword_NetDisconnected()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskFailure);

            var resetResult = await container.Instance.ResetPassword(validEmail, countryCode);
            Assert.IsTrue(resetResult == PasswordResetEnum.NetDisconnected);
        }

        [Test, Category("LoginUtil_ResetPassword")]
        public async void ResetPassword_DeviceIdNotMatched()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskSuccess);

            container.Arrange<ILoginDomainService>(loginDomainService => loginDomainService.ResetPassword(null, null)).IgnoreArguments().Returns(Task.Run(() => { return PasswordResetEnum.DeviceIdNotMatched; }));

            var resetResult = await container.Instance.ResetPassword(validEmail, countryCode);
            Assert.IsTrue(resetResult == PasswordResetEnum.DeviceIdNotMatched);
        }


        [Test, Category("LoginUtil_ResetPassword")]
        public async void ResetPassword_EmailNotExist()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskSuccess);
            container.Arrange<ILoginDomainService>(loginDomainService => loginDomainService.ResetPassword(null, null)).IgnoreArguments().Returns(Task.Run(() => { return PasswordResetEnum.EmailNotExist; }));

            var resetResult = await container.Instance.ResetPassword(validEmail, countryCode);
            Assert.IsTrue(resetResult == PasswordResetEnum.EmailNotExist);
        }


        [Test, Category("LoginUtil_ResetPassword")]
        public async void ResetPassword_ResetSuccess()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskSuccess);
            container.Arrange<ILoginDomainService>(loginDomainService => loginDomainService.ResetPassword(null, null)).IgnoreArguments().Returns(Task.Run(() => { return PasswordResetEnum.ResetSuccess; }));

            var resetResult = await container.Instance.ResetPassword(validEmail, countryCode);
            Assert.IsTrue(resetResult == PasswordResetEnum.ResetSuccess);
        }


        [Test, Category("LoginUtil_ResetPassword")]
        public async void ResetPassword_ResetFailure()
        {
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskSuccess);
            container.Arrange<ILoginDomainService>(loginDomainService => loginDomainService.ResetPassword(null, null)).IgnoreArguments().Returns(Task.Run(() => { return PasswordResetEnum.ResetFailure; }));

            var resetResult = await container.Instance.ResetPassword(validEmail, countryCode);
            Assert.IsTrue(resetResult == PasswordResetEnum.ResetFailure);
        }
    }
}
