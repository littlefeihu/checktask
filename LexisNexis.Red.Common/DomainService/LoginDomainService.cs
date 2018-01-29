using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DominService
{
    public class LoginDomainService : ILoginDomainService
    {
        IUserAccess userAccess;
        IConnectionMonitor connectionMonitor;
        IAuthenticationService authenticationService;
        public LoginDomainService(IUserAccess userAccess, IConnectionMonitor connectionMonitor, IAuthenticationService authenticationService)
        {
            this.userAccess = userAccess;
            this.connectionMonitor = connectionMonitor;
            this.authenticationService = authenticationService;
        }

        public User LoginOffline(string email, string password, string countryCode)
        {
            return userAccess.LoginOffline(email, password, countryCode);
        }
        public async Task<Tuple<User, LoginStatusEnum>> LoginOnline(string email, string password, string countryCode)
        {
            User user = null;
            LoginStatusEnum loginStatus = LoginStatusEnum.LoginFailure;
            LoginUser loginUser = new LoginUser
            {
                Email = email,
                Password = password,
                DeviceId = GlobalAccess.DeviceId,
                CountryCode = countryCode
            };
            var loginResult = await authenticationService.LoginUserValidation(loginUser);
            if (loginResult.IsSuccess)
            {
                var response = loginResult.DeserializeObject<LoginUserValidationResponse>();

                if (response.state=="success")
                {
                    loginStatus = LoginStatusEnum.LoginSuccess;
                    user = new User { Email = email, Password = password, CountryCode = countryCode };
                }
                user = UpdateUserToDb(response, loginUser);


                //if (response.ValidUser)
                //{
                //    if (response.Locked)
                //    {
                //        loginStatus = LoginStatusEnum.AccountLocked;
                //    }
                //    else
                //    {
                //        user = UpdateUserToDb(response, loginUser);
                //        bool needRegisterDevice = (response.FirstLogin || string.IsNullOrEmpty(user.SymmetricKey));
                //        if (needRegisterDevice)
                //        {
                //            user.SymmetricKey = await RegisterDevice(loginUser.Email, loginUser.Password, loginUser.CountryCode);

                //            loginStatus = (!string.IsNullOrEmpty(user.SymmetricKey) ? LoginStatusEnum.LoginSuccess : LoginStatusEnum.LoginFailure);
                //        }
                //        else
                //        {
                //            loginStatus = LoginStatusEnum.LoginSuccess;
                //        }
                //    }
                //}
                //else
                //{
                //    loginStatus = response.EmailAccountIsExists ? LoginStatusEnum.EmailOrPwdError : LoginStatusEnum.AccountNotExist;
                //}
            }
            return new Tuple<User, LoginStatusEnum>(user, loginStatus);
        }

        private User UpdateUserToDb(LoginUserValidationResponse response, LoginUser loginUser)
        {
            var user = userAccess.GetUserByEmail(loginUser.Email, loginUser.CountryCode);
            if (user == null)
            {
                user = new User
                {
                    CountryCode = loginUser.CountryCode,
                    Email = loginUser.Email,
                    Password = loginUser.Password,
                    FirstName = response.FirstName,
                    LastName = response.LastName,
                    LastSuccessfulLogin = DateTime.Now,
                    LockedStatus = response.Locked,
                    NeedChangePassword = response.MachineGeneratedPassword,
                    SyncAnnotations = response.SaveAnnotationOnSync,
                    IsAlive = true,
                    LastSyncDate = DateTime.Now.ToString()
                };
                userAccess.InsertUser(user);
            }
            else
            {
                user.IsAlive = true;
                user.LockedStatus = response.Locked;
                user.NeedChangePassword = response.MachineGeneratedPassword;
                user.Password = loginUser.Password;
                user.SyncAnnotations = response.SaveAnnotationOnSync;
                userAccess.UpdateUserStatus(user);
            }
            return user;
        }
        private async Task<string> RegisterDevice(string email, string password, string countryCode)
        {
            RegisterDevice registerUserDevice = new RegisterDevice
            {
                Email = email,
                Password = password,
                DeviceId = GlobalAccess.DeviceId,
                DeviceOS = GlobalAccess.DeviceOS,
                DeviceTypeId = (int)GlobalAccess.DeviceService.GetDeviceType(),
                EreaderVersion = GlobalAccess.DeviceService.GetEreaderVersion(),
                CountryCode = countryCode
            };
            var registerResult = await authenticationService.RegisterDevice(registerUserDevice);
            if (registerResult.IsSuccess)
            {
                var response = registerResult.DeserializeObject<RegisterDeviceResponse>();
                bool updateSymmetricKeySuccess = (userAccess.UpdateSymmetricKey(response.Key, email, countryCode) > 0);
                if (updateSymmetricKeySuccess)
                {
                    return response.Key;
                }
            }
            else
            {
                switch (registerResult.Content)
                {
                    case Constants.MAX_REG_EXCEEDED:
                        throw new MaxDeviceExceededException(registerResult.Content);
                    case Constants.NET_ERROR:
                        throw new CusNetDisConnectedException(registerResult.Content);
                }
            }
            return null;
        }

        public async Task<PasswordResetEnum> ResetPassword(string email, string countryCode)
        {
            PasswordReset resetUserPassword = new PasswordReset
            {
                Email = email,
                CountryCode = countryCode,
                DeviceId = GlobalAccess.DeviceId
            };
            var result = await authenticationService.ResetPassword(resetUserPassword);
            if (!result.IsSuccess)
            {
                switch (result.Content)
                {
                    case Constants.DEVICE_NOT_MATCHED:
                        return PasswordResetEnum.DeviceIdNotMatched;
                    case Constants.EMAIL_NOT_EXIST:
                        return PasswordResetEnum.EmailNotExist;
                }
            }
            else
            {
                PasswordResetResponse res = result.DeserializeObject<PasswordResetResponse>();
                if (res.Status)
                    return PasswordResetEnum.ResetSuccess;
            }
            return PasswordResetEnum.ResetFailure;
        }

        public async Task<PasswordChangeEnum> PasswordChange(string email, string countryCode, string password1, string password2)
        {
            PasswordChange userPasswordChange = new PasswordChange
            {
                Email = email,
                Password = password1,
                Password2 = password2,
                DeviceId = GlobalAccess.DeviceId,
                CountryCode = countryCode
            };
            var changeResult = await authenticationService.PasswordChange(userPasswordChange);
            if (changeResult.IsSuccess)
            {
                var response = changeResult.DeserializeObject<PasswordChangeResponse>();
                if (response.Status)
                {
                    bool updateLocalPWD = (userAccess.UpdatePassword(email, countryCode, password1) > 0);
                    if (updateLocalPWD)
                        return PasswordChangeEnum.ChangeSuccess;
                }
            }
            return PasswordChangeEnum.ChangeFailure;
        }
        public LoginUserDetails GetLastUserLoginInfo()
        {//when first login ,user is null,return value is null
            LoginUserDetails loginUserDetails = null;
            var user = userAccess.GetLastUserLoginInfo();
            if (user != null)
            {
                var country = ConfigurationService.GetAllCountryMap().FirstOrDefault(o => o.CountryCode == user.CountryCode);
                loginUserDetails = new LoginUserDetails(user.Email,
                                                        user.LockedStatus,
                                                        user.Password,
                                                        country,
                                                        user.NeedChangePassword,
                                                        user.SyncAnnotations,
                                                        user.SymmetricKey,
                                                        user.FirstName,
                                                        user.LastName,
                                                        user.LastSyncDate);
            }
            return loginUserDetails;
        }

        public User GetLastIsAliveLoginUser()
        {
            return userAccess.GetLastIsAliveLoginUser();
        }
        public void Logout(string email, string countryCode)
        {
            userAccess.Logout(email, countryCode);
        }

        public LoginUserDetails UpdateCurrentUser(User user)
        {
            if (user == null)
                return null;
            userAccess.UpdateLastLoginDate(user.Email, user.CountryCode);
            var country = ConfigurationService.GetAllCountryMap().FirstOrDefault(o => o.CountryCode == user.CountryCode);

            var loginUserDetails = new LoginUserDetails(user.Email,
                                        user.LockedStatus,
                                        user.Password,
                                        country,
                                        user.NeedChangePassword,
                                        user.SyncAnnotations,
                                        user.SymmetricKey,
                                        user.FirstName,
                                        user.LastName,
                                        user.LastSyncDate);

            return loginUserDetails;
        }
    }
}
