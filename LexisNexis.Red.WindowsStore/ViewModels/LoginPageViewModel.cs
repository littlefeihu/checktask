using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {


        private bool loadingData;

        private List<Country> countryList;
        

        [RestorableState]
        public List<Country> CountryList
        {
            get { return countryList; }
            set { SetProperty(ref countryList, value); }
        }

        public bool LoadingData
        {
            get { return loadingData; }
            set { SetProperty(ref loadingData, value); }
        }

        #region login
        private bool isLogin;
        private string password;
        private string email;
        private string selectedCountry;
        private bool invalidEmail;
        private bool invalidPassword;
        private bool noCountry;

        [RestorableState]
        public string Email
        {
            get { return email; }
            set
            {
                SetProperty(ref email, value);
                ClearError();
            }
        }

        [RestorableState]
        public bool InvalidEmail
        {
            get { return invalidEmail; }
            set { SetProperty(ref invalidEmail, value); }
        }

        [RestorableState]
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                SetProperty(ref password, value);
                ClearError();
            }
        }

        [RestorableState]
        public bool InvalidPassword
        {
            get { return invalidPassword; }
            set { SetProperty(ref invalidPassword, value); }
        }

        [RestorableState]
        public string SelectedCountry
        {
            get { return selectedCountry; }
            set
            {
                SetProperty(ref selectedCountry, value);
                ClearError();
            }
        }

        public bool NoCountry
        {
            get { return noCountry; }
            set { SetProperty(ref noCountry, value); }
        }

        public bool IsLogin
        {
            get { return isLogin; }
            set { SetProperty(ref isLogin, value); }
        }

        #endregion

        #region resetpassword
        private bool isResetPassword;

        public bool IsResetPassword
        {
            get { return isResetPassword; }
            set { SetProperty(ref isResetPassword, value); }
        }


        #endregion

        #region changepassword
        private bool showChangePasswordPopup;
        private string newPassword;
        private string passwordConfirm;
        private string errorMsgForChangePwd;     

        [RestorableState]
        public string NewPassword
        {
            get { return newPassword; }
            set
            {
                SetProperty(ref newPassword, value);
                ErrorMessageForChangePwd = null;
            }
        }

        [RestorableState]
        public string PasswordConfirm
        {
            get { return passwordConfirm; }
            set
            {
                SetProperty(ref passwordConfirm, value);
                ErrorMessageForChangePwd = null;
            }
        }

        public bool ShowChangePasswordPopup
        {
            get { return showChangePasswordPopup; }
            set
            {
                SetProperty(ref showChangePasswordPopup, value);
                ErrorMessageForChangePwd = null;
            }
        }

        public string ErrorMessageForChangePwd
        {
            get { return errorMsgForChangePwd; }
            set { SetProperty(ref errorMsgForChangePwd, value); }
        }            

        #endregion

        #region Commands
        public DelegateCommand SignInCommand { get; private set; }

        public DelegateCommand ResetPasswordCommand { get; private set; }

        public DelegateCommand CancelResetPasswordCommand { get; private set; }

        public DelegateCommand SendPassowrdReminderCommand { get; private set; }

        public DelegateCommand CancelChangePasswordCommand { get; private set; }

        public DelegateCommand SendChangePasswordCommand { get; private set; }
        #endregion

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            SelectedCountry = string.Empty;
            
            IsLogin = true;
            IsResetPassword = false;
            SignInCommand = DelegateCommand.FromAsyncHandler(SignIn);
            CountryList = ConfigurationService.GetAllCountryMap();
            ResetPasswordCommand = new DelegateCommand(GotoResetPassword);
            CancelResetPasswordCommand = new DelegateCommand(BackToLogin);
            SendPassowrdReminderCommand = DelegateCommand.FromAsyncHandler(SendPassowrdReminder);
            CancelChangePasswordCommand = new DelegateCommand(CancelChangePassword);
            SendChangePasswordCommand = DelegateCommand.FromAsyncHandler(ChangePassword);

            var lastLoginUser = LoginUtil.Instance.GetLastUserLoginInfo();
 
            if (lastLoginUser!=null)
            {
                Email = lastLoginUser.Email;
                SelectedCountry = lastLoginUser.Country.CountryCode;
            }
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);

        }


        #region private methods
        private async Task SignIn()
        {
            LoadingData = true;
            var result = await LoginUtil.Instance.ValidateUserLogin(Email,Password, SelectedCountry);
            LoadingData = false;
            //   result.LoginStatus= LoginStatusEnum.NetDisconnected;
            switch (result)
            {
                case LoginStatusEnum.LoginSuccess:
                    if (GlobalAccess.Instance.CurrentUserInfo.NeedChangePassword)
                    {

                        ShowChangePasswordPopup = true;
                        NewPassword = null;
                        PasswordConfirm = null;

                    }
                    else
                    {
                        GotoMainPage();
                    }
                    break;
                case LoginStatusEnum.NetDisconnected:
                    await
                        AlertMessageService.ShowAsync(
                            ResourceLoader.GetString("NetworkDisconnectPopupMessage"),
                            ResourceLoader.GetString("NetworkDisconnectPopupTitle"),

                            new UICommand
                            {
                                Label = "OK"
                            }
                            );
                    break;
               case LoginStatusEnum.InvalidemailAndValidPwd:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("InvalidemailAndValidPwdMsg"), "",
                    new UICommand("OK"));
                    //ErrorMessage = ResourceLoader.GetString("InvalidemailAndValidPwdMsg");
                    InvalidEmail = true;
                    InvalidPassword = true;
                    break;
                case LoginStatusEnum.ValidemailAndInvalidPwd:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("ValidemailAndInvalidPwdMsg"), "",
                    new UICommand("OK"));
                    //ErrorMessage = ResourceLoader.GetString("ValidemailAndInvalidPwdMsg");
                    InvalidEmail = true;
                    InvalidPassword = true;
                    break;
                case LoginStatusEnum.InvalidemailAndInvalidPwd:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("InvalidemailAndInvalidPwdMsg"), "",
                    new UICommand("OK"));
                    //ErrorMessage = ResourceLoader.GetString("InvalidemailAndInvalidPwdMsg");
                    InvalidEmail = true;
                    InvalidPassword = true;
                    break;
                case LoginStatusEnum.ValidemailAndEmptyPwd:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("ValidemailAndEmptyPwdMsg"), "",
                    new UICommand("OK"));
                    //ErrorMessage = ResourceLoader.GetString("ValidemailAndEmptyPwdMsg");
                    InvalidPassword = true;
                    break;
                case LoginStatusEnum.EmptyemailAndValidPwd:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("EmptyemailAndValidPwdMsg"), "",
                    new UICommand("OK"));
                    //ErrorMessage = ResourceLoader.GetString("EmptyemailAndValidPwdMsg");
                    InvalidEmail = true;
                    break;
                case LoginStatusEnum.InvalidemailAndEmptyPwd:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("InvalidemailAndEmptyPwdMsg"), "",
                    new UICommand("OK"));
                    //ErrorMessage = ResourceLoader.GetString("InvalidemailAndEmptyPwdMsg");
                    InvalidEmail = true;
                    InvalidPassword = true;
                    break;
                case LoginStatusEnum.SelectCountry:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("NotSelectCountryMsg"), "",
                     new UICommand("OK"));
                    //ErrorMessage = ResourceLoader.GetString("NotSelectCountryMsg");
                    NoCountry = true;
                    break;
                case LoginStatusEnum.AccountNotExist:
                    await
                        AlertMessageService.ShowAsync(
                            ResourceLoader.GetString("AccountNotExistErrorMsg"),
                            ResourceLoader.GetString("ErrorMsgHeader"),

                            new UICommand
                            {
                                Label = "OK"
                            }
                            );
                    break;
                case LoginStatusEnum.EmailOrPwdError:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("EmailOrPwdErrorMsg"), "",
                     new UICommand("OK"));
                    //ErrorMessage = ResourceLoader.GetString("EmailOrPwdErrorMsg");
                    InvalidEmail = true;
                    InvalidPassword = true;
                    break;
                case LoginStatusEnum.EmptyEmailAndEmptyPwd:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("EmptyEmailAndEmptyPwdMsg"), "",
                      new UICommand("OK"));
                    //ErrorMessage = ResourceLoader.GetString("EmptyEmailAndEmptyPwdMsg");
                    InvalidEmail = true;
                    InvalidPassword = true;
                    break;
                case LoginStatusEnum.DeviceLimit:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("DeviceLimitMsg"), "",
                        new UICommand("OK"));
                    break;

                default:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("UnExpectedError"), "",
                        new UICommand("OK"));
                    break;
            }

        }

        private void GotoResetPassword()
        {
            IsLogin = false;
            IsResetPassword = true;
            ClearError();
        }

        private void ClearError()
        {
            InvalidEmail = false;
            InvalidPassword = false;
            NoCountry = false;
        }

        private void BackToLogin()
        {
            IsLogin = true;
            IsResetPassword = false;
            ClearError();
        }

        private async Task SendPassowrdReminder()
        {
            LoadingData = true;
            var result = await LoginUtil.Instance.ResetPassword(Email, SelectedCountry);
            LoadingData = false;
            switch (result)
            {
                case PasswordResetEnum.NetDisconnected:
                    await
                        AlertMessageService.ShowAsync(
                            ResourceLoader.GetString("NetworkDisconnectPopupMessage"),
                            ResourceLoader.GetString("NetworkDisconnectPopupTitle"),

                            new UICommand
                            {
                                Label = "OK"
                            }
                            );
                    break;
                case PasswordResetEnum.EmailNotExist:
                    await
                        AlertMessageService.ShowAsync(
                            ResourceLoader.GetString("AccountNotExistErrorMsg"),
                            ResourceLoader.GetString("ErrorMsgHeader"),

                            new UICommand
                            {
                                Label = "OK"
                            }
                            );
                    break;

                case PasswordResetEnum.InvalidEmail:
                    await
                        AlertMessageService.ShowAsync(
                            ResourceLoader.GetString("InValidEmailMsg"),
                            "Error Message",

                            new UICommand
                            {
                                Label = "OK"
                            }
                            );
                    break;
                case PasswordResetEnum.DeviceIdNotMatched:
                    await
                        AlertMessageService.ShowAsync(
                            ResourceLoader.GetString("DeviceIdNotMatchedMsg"),
                            "Error !!",

                            new UICommand
                            {
                                Label = "OK"
                            }
                            );
                    break;
                case PasswordResetEnum.SelectCountry:
                    //ErrorMessage = ResourceLoader.GetString("NotSelectCountryMsg");
                    NoCountry = true;
                    break;
                case PasswordResetEnum.ResetSuccess:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("ResetPwdSentMsg"), "",
                        new UICommand
                        {
                            Label = "OK",
                            Invoked = x => BackToLogin()
                        });
                    break;
            }
        }

        private async Task ChangePassword()
        {

            var result = await LoginUtil.Instance.ChangePassword(NewPassword, PasswordConfirm);
            switch (result)
            {
                case PasswordChangeEnum.LengthInvalid:
                    ErrorMessageForChangePwd =
                        ResourceLoader.GetString("PasswordToShortErrorMsg");
                    break;
                case PasswordChangeEnum.NotMatch:
                    ErrorMessageForChangePwd =
                        ResourceLoader.GetString("PasswordNotMatchErrorMsg");
                    break;
                case PasswordChangeEnum.ChangeFailure:
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("UnExpectedError"), "",
                        new UICommand
                        {
                            Label = "OK"
                        });
                    break;
                case PasswordChangeEnum.ChangeSuccess:
                    ShowChangePasswordPopup = false;
                    await AlertMessageService.ShowAsync(ResourceLoader.GetString("PasswordChangedMsg"), "",
                        new UICommand
                        {
                            Label = "OK",
                            Invoked = c =>
                            {
                                GotoMainPage();
                            }
                        });
                    break;
            }

        }

        private void CancelChangePassword()
        {
            // alertMessageService.ShowAsync()
            ShowChangePasswordPopup = false;
        }

        private void GotoMainPage()
        {
            NavigationService.Navigate("Publications", null);
        }
        #endregion
    }
}
