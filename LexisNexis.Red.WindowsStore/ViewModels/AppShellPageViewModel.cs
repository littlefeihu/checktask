using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.HelpClass;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using LexisNexis.Red.WindowsStore.Events;
using Microsoft.Practices.Prism.Mvvm.Interfaces;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class AppShellPageViewModel : BaseViewModel
    {
        private string currentUser;
        [RestorableState]
        public string CurrentUser
        {
            get { return currentUser; }
            set { SetProperty(ref currentUser, value); }
        }

        public DelegateCommand SignOutCommand { get; private set; }


        public DelegateCommand SettingsCommand { get; private set; }



        public AppShellPageViewModel()
        {
            SignOutCommand = DelegateCommand.FromAsyncHandler(SignOut);
            SettingsCommand = new DelegateCommand(GotoSettingsPage);
        }

        public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {

            var user = GlobalAccess.Instance.CurrentUserInfo;
            if (user != null)
            {
                CurrentUser = user.Email;
            }
        }

        private async Task SignOut()
        {
            NavigationService = IoCContainer.Instance.ResolveInterface<INavigationService>();
            await AlertMessageService.ShowAsync(ResourceLoader.GetString("SignOutAlertMsg"),
                ResourceLoader.GetString("SignOutAlertTitle"),

                new UICommand(ResourceLoader.GetString("CancelText")),
                new UICommand(ResourceLoader.GetString("ConfirmText"), u =>
                {
                    EventAggregator.GetEvent<LogoutEvent>().Publish(true);
                    LoginUtil.Instance.Logout();
                    SessionService.SessionState.Clear();
                    NavigationService.ClearHistory();
                    NavigationService.Navigate("Login", null);
                }));
        }

        private void GotoSettingsPage()
        {
            NavigationService = IoCContainer.Instance.ResolveInterface<INavigationService>();
            NavigationService.Navigate("Settings", null);
        }
    }
}
