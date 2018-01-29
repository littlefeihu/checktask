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
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_NetDisconnected()
        {

            container.Arrange<ILoginDomainService>(loginDomainService => loginDomainService.LoginOffline(null, null, null)).IgnoreArguments().Returns(new Func<object>(() => { return null; }));
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskFailure);

            var actualString = await container.Instance.ValidateUserLogin(validEmail, validPassword, countryCode);
            Assert.AreEqual(true, actualString == LoginStatusEnum.NetDisconnected);
        }
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_OfflineLoginSuccess()
        {
            User user = new User { Email = validEmail, CountryCode = countryCode };
            container.Arrange<ILoginDomainService>(loginDomainService => loginDomainService.LoginOffline(null, null, null)).IgnoreArguments()
                      .Returns(user);
            container.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken)))
                     .Returns(pingTaskFailure);
            container.Arrange<ILoginDomainService>(loginDomainService => loginDomainService.UpdateCurrentUser(null)).IgnoreArguments()
                      .Returns(new LoginUserDetails("allen@lexisred.com", false, null, new Country { CountryCode = "AU",ServiceCode="AUNZ" }, false, false, null, null, null, null));


            var actualString = await container.Instance.ValidateUserLogin(validEmail, validPassword, countryCode);
            Assert.AreEqual(true, actualString == LoginStatusEnum.LoginSuccess);
        }
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_OnlineLoginSuccess()
        {
            u = new User { };

            loginDomainService.Arrange<IUserAccess>(userAccess => userAccess.GetUserByEmail(null, null)).IgnoreArguments().Returns(u);
            loginDomainService.Arrange<IUserAccess>(userAccess => userAccess.UpdateSymmetricKey(null, null, null)).IgnoreArguments().Returns(1);
            loginDomainService.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskSuccess);
            loginDomainService.Arrange<IAuthenticationService>(authenticationService => authenticationService.LoginUserValidation(null)).IgnoreArguments().Returns(validateResult);
            loginDomainService.Arrange<IAuthenticationService>(authenticationService => authenticationService.RegisterDevice(null)).IgnoreArguments().Returns(registerDeviceResultSuccess);

            var actualString = await loginDomainService.Instance.LoginOnline(validEmail, validPassword, countryCode);
            Assert.AreEqual(true, actualString.Item2 == LoginStatusEnum.LoginSuccess);
        }
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_OnlineFirstLoginSuccess()
        {
            u = null;

            loginDomainService.Arrange<IUserAccess>(userAccess => userAccess.GetUserByEmail(null, null)).IgnoreArguments().Returns(u).InSequence();
            loginDomainService.Arrange<IUserAccess>(userAccess => userAccess.UpdateSymmetricKey(null, null, null)).IgnoreArguments().Returns(1);
            loginDomainService.Arrange<IUserAccess>(userAccess => userAccess.GetUserByEmail(null, null)).IgnoreArguments().Returns(new User()).InSequence();
            loginDomainService.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskSuccess);
            loginDomainService.Arrange<IAuthenticationService>(authenticationService => authenticationService.LoginUserValidation(null)).IgnoreArguments().Returns(validateResult);
            loginDomainService.Arrange<IAuthenticationService>(authenticationService => authenticationService.RegisterDevice(null)).IgnoreArguments().Returns(registerDeviceResultSuccess);

            var actualString = await loginDomainService.Instance.LoginOnline(validEmail, validPassword, countryCode);
            Assert.AreEqual(true, actualString.Item2 == LoginStatusEnum.LoginSuccess);

        }
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_DeviceLimite()
        {
            try
            {
                u = new User { };

                loginDomainService.Arrange<IUserAccess>(userAccess => userAccess.GetUserByEmail(null, null)).IgnoreArguments().Returns(u);
                loginDomainService.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskSuccess);
                loginDomainService.Arrange<IAuthenticationService>(authenticationService => authenticationService.LoginUserValidation(null)).IgnoreArguments().Returns(validateResult);
                registerDeviceResultSuccess.Result.IsSuccess = false;
                registerDeviceResultSuccess.Result.Content = Constants.MAX_REG_EXCEEDED;

                loginDomainService.Arrange<IAuthenticationService>(authenticationService => authenticationService.RegisterDevice(null)).IgnoreArguments().Returns(registerDeviceResultSuccess);

                var actualString = await loginDomainService.Instance.LoginOnline(validEmail, validPassword, countryCode);
            }
            catch (MaxDeviceExceededException)
            {
                Assert.IsTrue(true);
            }
        }
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_EmailOrPwdError()
        {
            u = new User { };

            loginDomainService.Arrange<IUserAccess>(userAccess => userAccess.GetUserByEmail(null, null)).IgnoreArguments().Returns(u);
            loginDomainService.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskSuccess);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new LoginUserValidationResponse { ValidUser = false, EmailAccountIsExists = true });
            loginDomainService.Arrange<IAuthenticationService>(authenticationService => authenticationService.LoginUserValidation(null)).IgnoreArguments().Returns(validateResult);

            var actualString = await loginDomainService.Instance.LoginOnline(validEmail, validPassword, countryCode);
            Assert.AreEqual(true, actualString.Item2 == LoginStatusEnum.EmailOrPwdError);

        }
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_AccountNotExist()
        {
            u = new User { };

            loginDomainService.Arrange<IUserAccess>(userAccess => userAccess.GetUserByEmail(null, null)).IgnoreArguments().Returns(u);
            loginDomainService.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskSuccess);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new LoginUserValidationResponse { ValidUser = false, EmailAccountIsExists = false });
            loginDomainService.Arrange<IAuthenticationService>(authenticationService => authenticationService.LoginUserValidation(null)).IgnoreArguments().Returns(validateResult);

            var actualString = await loginDomainService.Instance.LoginOnline(validEmail, validPassword, countryCode);
            Assert.AreEqual(true, actualString.Item2 == LoginStatusEnum.AccountNotExist);
        }

        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_AccountLocked()
        {

            loginDomainService.Arrange<IUserAccess>(userAccess => userAccess.GetUserByEmail(null, null)).IgnoreArguments().Returns(new User { });
            loginDomainService.Arrange<IConnectionMonitor>(connectionMonitor => connectionMonitor.PingService(Arg.IsAny<string>(), default(CancellationToken))).Returns(pingTaskSuccess);
            validateResult.Result.IsSuccess = true;
            validateResult.Result.Content = JsonConvert.SerializeObject(new LoginUserValidationResponse { ValidUser = true, Locked = true });

            loginDomainService.Arrange<IAuthenticationService>(authenticationService => authenticationService.LoginUserValidation(null)).IgnoreArguments().Returns(validateResult);

            var actualString = await loginDomainService.Instance.LoginOnline(validEmail, validPassword, countryCode);
            Assert.AreEqual(true, actualString.Item2 == LoginStatusEnum.AccountLocked);
        }
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_EmailOrPwdError_ServiceDependency()
        {
            var loginResult = await LoginUtil.Instance.ValidateUserLogin(validEmail, Guid.NewGuid().ToString(), countryCode);
            Assert.IsTrue(loginResult == LoginStatusEnum.EmailOrPwdError);
        }
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_AccountNotExist_ServiceDependency()
        {
            var loginResult = await LoginUtil.Instance.ValidateUserLogin(notExistsEmail, "12346", countryCode);
            Assert.IsTrue(loginResult == LoginStatusEnum.AccountNotExist);
        }

        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_LoginSuccess_ServiceDependency()
        {
            var loginResult = await LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode);
            Assert.IsTrue(loginResult == LoginStatusEnum.LoginSuccess);
        }
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_GetLastUserLoginInfo_ServiceDependency()
        {
            var loginResult = await LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode);
            if (loginResult == LoginStatusEnum.LoginSuccess)
            {
                if (GlobalAccess.Instance.CurrentUserInfo.NeedChangePassword)
                {
                    await LoginUtil.Instance.ChangePassword("1234", "1234");
                }

                var r = LoginUtil.Instance.GetLastUserLoginInfo();

                Assert.IsTrue(r != null && r.Email == validEmail);
            }
            else
            {
                Assert.Fail();
            }
        }
        [Test, Category("LoginUtil_ValidateUserLogin")]
        public async void Validate_GetLastIsAliveLoginUser_ServiceDependency()
        {

            var loginResult = await LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode);
            if (loginResult == LoginStatusEnum.LoginSuccess)
            {
                var lastSyncedTime = SettingsUtil.Instance.GetLastSyncedTime();
                LoginUtil.Instance.GetLastIsAliveLoginUser();

                var currentUser = GlobalAccess.Instance.CurrentUserInfo;
                //LoginUtil.Instance.Logout();
                // GlobalAccess.Instance.CurrentUserInfo = null;
                currentUser = GlobalAccess.Instance.CurrentUserInfo;
                LoginUtil.Instance.GetLastIsAliveLoginUser();
                currentUser = GlobalAccess.Instance.CurrentUserInfo;
                loginResult = await LoginUtil.Instance.ValidateUserLogin("torres@lexisred.com", "loop", "AU");
                currentUser = GlobalAccess.Instance.CurrentUserInfo;
                LoginUtil.Instance.Logout();
                var dd = LoginUtil.Instance.GetLastUserLoginInfo();

            }
            else
            {
                Assert.Fail();
            }
        }

    }
}
