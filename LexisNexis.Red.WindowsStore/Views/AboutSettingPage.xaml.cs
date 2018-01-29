using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LexisNexis.Red.Common.Business;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LexisNexis.Red.WindowsStore.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutSettingPage : Page
    {
        public string ApplicationVersion
        {
            get
            {
                return String.Format("{0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);
            }
        }

        public string LastSyncTime
        {
            get
            {
                var timeFormat = new ResourceLoader().GetString("DateTimeFormat");
                return SettingsUtil.Instance.GetLastSyncedTime().ToString(timeFormat);
            }
        }

        public AboutSettingPage()
        {
            InitializeComponent();
        }

        private void AboutLexisNexisRedClick(object sender, RoutedEventArgs e)
        {
            var content = SettingsUtil.Instance.GetLexisNexisRedInfo();
            HeadPanel.Visibility= Visibility.Collapsed;
            ContentWebView.Visibility = Visibility.Visible;
            ContentWebView.NavigateToString(content);
        }

        private void AboutLexisNexisClick(object sender, RoutedEventArgs e)
        {
            var content = SettingsUtil.Instance.GetLexisNexisInfo();
            HeadPanel.Visibility = Visibility.Collapsed;
            ContentWebView.Visibility = Visibility.Visible;
            ContentWebView.NavigateToString(content);
        }

        private void TermsConditionClick(object sender, RoutedEventArgs e)
        {
            var content = SettingsUtil.Instance.GetTermsAndConditions();
            HeadPanel.Visibility = Visibility.Collapsed;
            ContentWebView.Visibility = Visibility.Visible;
            ContentWebView.NavigateToString(content);
            
        }
    }
}
