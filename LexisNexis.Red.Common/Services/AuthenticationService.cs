using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using System;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Services
{
    public class AuthenticationService : ServiceBase, IAuthenticationService
    {
        public AuthenticationService()
            : base("")
        {

        }
        public Task<HttpResponse> LoginUserValidation(LoginUser loginUser)
        {
            return ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(loginUser.CountryCode), ServiceConfig.VALIDATE_USER, loginUser);
        }

        public Task<HttpResponse> ResetPassword(PasswordReset resetUserPassword)
        {
            return ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(resetUserPassword.CountryCode), ServiceConfig.RESET_PASSWORD, resetUserPassword);
        }

        public Task<HttpResponse> PasswordChange(PasswordChange userPasswordChange)
        {

            return ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(userPasswordChange.CountryCode), ServiceConfig.CHANGE_PASSWORD, userPasswordChange);
        }


        public Task<HttpResponse> RegisterDevice(RegisterDevice registerUserDevice)
        {
            return ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(registerUserDevice.CountryCode), ServiceConfig.REGISTER_DEVICE, registerUserDevice);
        }

    }
}
