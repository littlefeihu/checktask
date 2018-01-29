using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.DominService;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using Microsoft.Practices.Unity;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace LexisNexis.Red.Common.Business
{
    public class LoginUtil : AppServiceBase<LoginUtil>
    {
        IConnectionMonitor connectionMonitor;
        ILoginDomainService loginDomainService;
        ICryptogram cryptogram;
        public LoginUtil(IConnectionMonitor connectionMonitor, ILoginDomainService loginDomainService, ICryptogram cryptogram)
        {
            this.connectionMonitor = connectionMonitor;
            this.loginDomainService = loginDomainService;
            this.cryptogram = cryptogram;
        }

        /// <summary>
        /// login process
        /// </summary>
        /// <param name="email">your email account</param>
        /// <param name="password">password</param>
        /// <param name="countryCode">countrycode</param>
        /// <returns></returns>
        public async Task<LoginStatusEnum> ValidateUserLogin(string email, string password, string countryCode)
        {
            //Verify the legitimacy of the account information
            LoginStatusEnum loginStatus = User.ValidateUserInfo(email, password, countryCode);
            try
            {
                User user = null;
                var pingResult = await connectionMonitor.PingService(countryCode);
                if (!pingResult)
                {//offline
                    user = this.loginDomainService.LoginOffline(email, password, countryCode);
                    loginStatus = user == null ? LoginStatusEnum.NetDisconnected : loginStatus = LoginStatusEnum.LoginSuccess;
                }
                else
                {
                    var onlineResult = await loginDomainService.LoginOnline(email, password, countryCode);
                    user = onlineResult.Item1;
                    loginStatus = onlineResult.Item2;
                }
                if (loginStatus == LoginStatusEnum.LoginSuccess)
                {
                    GlobalAccess.Instance.CurrentUserInfo = loginDomainService.UpdateCurrentUser(user);
                    await DomainEvents.Publish(new LoginSuccessEvent(GlobalAccess.Instance.CurrentUserInfo));
                }
            }
            catch (CusNetDisConnectedException)
            {
                loginStatus = LoginStatusEnum.NetDisconnected;
            }
            catch (MaxDeviceExceededException)
            {
                loginStatus = LoginStatusEnum.DeviceLimit;
            }
            catch (Exception ex)
            {
                loginStatus = LoginStatusEnum.LoginFailure;
                Logger.Log("ValidateUserLogin" + ex.ToString());
            }
            return loginStatus;
        }


        public LoginUserDetails GetLastIsAliveLoginUser()
        {
            var user = loginDomainService.GetLastIsAliveLoginUser();

            return loginDomainService.UpdateCurrentUser(user);
        }

        /// <summary>
        /// reset password when you forget your old password
        /// </summary>
        /// <param name="email">your email</param>
        /// <param name="countryCode">country code</param>
        /// <returns></returns>
        public async Task<PasswordResetEnum> ResetPassword(string email, string countryCode)
        {
            PasswordResetEnum passwordResetStatus = PasswordResetEnum.ResetFailure;
            try
            {
                if (!ValidateHelper.IsEmail(email))
                    return PasswordResetEnum.InvalidEmail;
                if (string.IsNullOrEmpty(countryCode))
                    return PasswordResetEnum.SelectCountry;
                var pingResult = await connectionMonitor.PingService(countryCode);
                if (!pingResult)
                    passwordResetStatus = PasswordResetEnum.NetDisconnected;
                else
                    passwordResetStatus = await loginDomainService.ResetPassword(email, countryCode);
            }
            catch (Exception ex)
            {
                Logger.Log("ResetPassword" + ex.ToString());
            }
            return passwordResetStatus;
        }
        private bool PwdLengthInvalid(string password1, string password2)
        {
            bool lengthInvalid = (string.IsNullOrEmpty(password1) ||
                                 string.IsNullOrEmpty(password2) ||
                                            password1.Length < 4 ||
                                           password2.Length < 4);
            return lengthInvalid;
        }
        /// <summary>
        /// change your password if it is first login or after reset your password
        /// </summary>
        /// <param name="password1">password1</param>
        /// <param name="password2">password2</param>
        /// <returns>PasswordChangeEnum</returns>
        public async Task<PasswordChangeEnum> ChangePassword(string password1, string password2)
        {
            PasswordChangeEnum passwordChangeStatus = PasswordChangeEnum.ChangeFailure;
            try
            {
                if (PwdLengthInvalid(password1, password2))
                    passwordChangeStatus = PasswordChangeEnum.LengthInvalid;
                else if (password1 != password2)
                    passwordChangeStatus = PasswordChangeEnum.NotMatch;
                else
                {

                    var pingResult = await connectionMonitor.PingService(GlobalAccess.Instance.CountryCode);
                    if (!pingResult)
                        passwordChangeStatus = PasswordChangeEnum.NetDisconnected;
                    else
                        passwordChangeStatus = await loginDomainService.PasswordChange(GlobalAccess.Instance.Email,
                                                                                       GlobalAccess.Instance.CountryCode,
                                                                                       password1,
                                                                                       password2);

                }
            }
            catch (Exception ex)
            {
                Logger.Log("ChangePassword" + ex.ToString());
            }
            return passwordChangeStatus;
        }

        /// <summary>
        /// Log out
        /// </summary>
        public void Logout()
        {
            loginDomainService.Logout(GlobalAccess.Instance.Email, GlobalAccess.Instance.CountryCode);
            GlobalAccess.Instance.CurrentUserInfo = null;
            GlobalAccess.Instance.CurrentPublication = null;
            AnnCategoryTagUtil.Instance.Clear();
            NavigationManager.Instance.Clear();
        }
        /// <summary>
        /// Get the latest user login information
        /// </summary>
        /// <returns>LoginUserDetails model</returns>
        public LoginUserDetails GetLastUserLoginInfo()
        {
            return loginDomainService.GetLastUserLoginInfo();
        }

    }


}