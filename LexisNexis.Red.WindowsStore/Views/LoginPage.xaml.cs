using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LexisNexis.Red.WindowsStore.ViewModels;
using Microsoft.Practices.Prism.StoreApps;

namespace LexisNexis.Red.WindowsStore.Views
{
    public sealed partial class LoginPage : VisualStateAwarePage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ChangePasswordGrid.Height = Window.Current.Bounds.Height;
            ChangePasswordGrid.Width = Window.Current.Bounds.Width;
            Window.Current.SizeChanged += (sender, args) =>
            {
                ChangePasswordGrid.Height = Window.Current.Bounds.Height;
                ChangePasswordGrid.Width = Window.Current.Bounds.Width;
            };
            var data = DataContext as LoginPageViewModel;
            
            EmailBox.KeyUp += (sender, args) =>
            {
                if (args.Key == VirtualKey.Enter)
                {
                    if (data != null)
                    {
                        EmailBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                        if (data.IsLogin)
                        {
                            LoginBtn.Command.Execute(null);
                        }
                        else if (data.IsResetPassword)
                        {
                            ResetPwdBtn.Command.Execute(null);
                        }
                    }
                }
            };

            PwdBox.KeyUp += (sender, args) =>
            {
                if (args.Key == VirtualKey.Enter)
                {
                    if (data != null)
                    {
                        PwdBox.GetBindingExpression(PasswordBox.PasswordProperty).UpdateSource();
                        if (data.IsLogin)
                        {
                            LoginBtn.Command.Execute(null);
                        }
                        else if (data.IsResetPassword)
                        {
                            ResetPwdBtn.Command.Execute(null);
                        }
                    }
                }
            };

            NewPwdBox.KeyUp += (sender, args) =>
            {
                if (args.Key == VirtualKey.Enter)
                {
                    if (data != null && data.ShowChangePasswordPopup)
                    {
                        NewPwdBox.GetBindingExpression(PasswordBox.PasswordProperty).UpdateSource();
                        ChangePwdBtn.Command.Execute(null);
                    }
                }
            };
            PwdConfirmBox.KeyUp += (sender, args) =>
            {
                if (args.Key == VirtualKey.Enter)
                {
                    if (data != null && data.ShowChangePasswordPopup)
                    {
                        PwdConfirmBox.GetBindingExpression(PasswordBox.PasswordProperty).UpdateSource();
                        ChangePwdBtn.Command.Execute(null);
                    }
                }
            };
        }

        private void ApplicationClose(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

    }
}
