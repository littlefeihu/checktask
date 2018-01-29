using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DominService;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock.AutoMock;

namespace LexisNexis.Red.CommonTests.Business
{
    public partial class LoginUtilTests
    {
        string invalidEmail = "allenlexisred.com";
        string validEmail = "allen@lexisred.com";
        string validPassword = "Password1";
        string emptyField = string.Empty;
        string invalidPassword = "123";
        string validPassword1 = "1234";
        string validPassword2 = "12345";
        string countryCode = "AU";
        User u = null;
        string notExistsEmail = "allen@lexisreddsadsad.com";
        AutoMockSettings autoMockSettings;
        MockingContainer<LoginUtil> container;
        Task<bool> pingTaskFailure, pingTaskSuccess;
        Task<HttpResponse> validateResult, registerDeviceResultSuccess;
        Task<PasswordChangeEnum> passwordChangeResult;
        MockingContainer<LoginDomainService> loginDomainService = new MockingContainer<LoginDomainService>(new AutoMockSettings
        {
            ConstructorArgTypes = new Type[] { typeof(IUserAccess), typeof(IConnectionMonitor), typeof(IAuthenticationService) }
        });
        public LoginUtilTests()
        {
            autoMockSettings = new AutoMockSettings
            {
                ConstructorArgTypes = new Type[] { typeof(IConnectionMonitor), typeof(ILoginDomainService) }
            };
            LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Wait();

            container = new MockingContainer<LoginUtil>(autoMockSettings);
        }
        [SetUp()]
        public void Test_Init()
        {
            pingTaskFailure = new Task<bool>(() => { return false; });
            pingTaskSuccess = new Task<bool>(() => { return true; });
            pingTaskFailure.Start();
            pingTaskSuccess.Start();
            validateResult = new Task<HttpResponse>(() =>
            {
                return new HttpResponse
                {
                    IsSuccess = true,
                    Content = JsonConvert.SerializeObject(new LoginUserValidationResponse { ValidUser = true })
                };
            });
            validateResult.Start();

            registerDeviceResultSuccess = new Task<HttpResponse>(() =>
            {
                return new HttpResponse
                {
                    IsSuccess = true,
                    Content = JsonConvert.SerializeObject(new RegisterDeviceResponse { Key = "DyT5ib5bDQINPNechvXN/g==" })
                };
            });
            registerDeviceResultSuccess.Start();
            passwordChangeResult = new Task<PasswordChangeEnum>(() =>
            {
                return PasswordChangeEnum.ChangeFailure;
            });
            passwordChangeResult.Start();
        }
        [TearDown]
        public void Cleanup()
        {

        }
    }
}
