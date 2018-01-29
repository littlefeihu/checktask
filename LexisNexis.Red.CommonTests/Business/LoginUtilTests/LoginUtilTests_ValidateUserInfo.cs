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
    public partial class LoginUtilTests
    {
        [Test, Category("LoginUtil_ValidateUserInfo")]
        public async void ValidateUserInfo_SelectCountry()
        {
            var loginReresult = await container.Instance.ValidateUserLogin(invalidEmail, invalidEmail, emptyField);
            Assert.IsTrue(loginReresult == LoginStatusEnum.SelectCountry);
        }
         [Test, Category("LoginUtil_ValidateUserInfo")]
        public async void ValidateUserInfo_EmptyEmailAndEmptyPwd()
        {
            var loginReresult = await container.Instance.ValidateUserLogin(emptyField, emptyField, emptyField);
            Assert.IsTrue(loginReresult == LoginStatusEnum.EmptyEmailAndEmptyPwd);
        }
        [Test, Category("LoginUtil_ValidateUserInfo")]
        public async void ValidateUserInfo_InvalidemailAndValidPwd()
        {
            var loginReresult = await container.Instance.ValidateUserLogin(invalidEmail, validPassword, countryCode);
            Assert.IsTrue(loginReresult == LoginStatusEnum.InvalidemailAndValidPwd);
        }
       [Test, Category("LoginUtil_ValidateUserInfo")]
        public async void ValidateUserInfo_ValidemailAndInvalidPwd()
        {
            var loginReresult = await container.Instance.ValidateUserLogin(validEmail, invalidPassword, countryCode);
            Assert.IsTrue(loginReresult == LoginStatusEnum.ValidemailAndInvalidPwd);
        }
       [Test, Category("LoginUtil_ValidateUserInfo")]
        public async void ValidateUserInfo_InvalidemailAndInvalidPwd()
        {
            var loginReresult = await container.Instance.ValidateUserLogin(invalidEmail, invalidPassword, countryCode);
            Assert.IsTrue(loginReresult == LoginStatusEnum.InvalidemailAndInvalidPwd);
        }
        [Test, Category("LoginUtil_ValidateUserInfo")]
        public async void ValidateUserInfo_ValidemailAndEmptyPwd()
        {
            var loginReresult = await container.Instance.ValidateUserLogin(validEmail, emptyField, countryCode);
            Assert.IsTrue(loginReresult == LoginStatusEnum.ValidemailAndEmptyPwd);
        }
        [Test, Category("LoginUtil_ValidateUserInfo")]
        public async void ValidateUserInfo_EmptyemailAndValidPwd()
        {
            var loginReresult = await container.Instance.ValidateUserLogin(emptyField, validPassword, countryCode);
            Assert.IsTrue(loginReresult == LoginStatusEnum.EmptyemailAndValidPwd);
        }
         [Test, Category("LoginUtil_ValidateUserInfo")]
        public async void ValidateUserInfo_EmptyemailAndInvalidPwd()
        {
            var loginReresult = await container.Instance.ValidateUserLogin(emptyField, invalidPassword, countryCode);
            Assert.IsTrue(loginReresult == LoginStatusEnum.EmptyemailAndInvalidPwd);
        }
       [Test, Category("LoginUtil_ValidateUserInfo")]
        public async void ValidateUserInfo_InvalidemailAndEmptyPwd()
        {
            var loginReresult = await container.Instance.ValidateUserLogin(invalidEmail, emptyField, countryCode);
            Assert.IsTrue(loginReresult == LoginStatusEnum.InvalidemailAndEmptyPwd);
        }
    }
}
