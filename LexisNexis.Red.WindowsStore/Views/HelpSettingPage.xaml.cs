using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LexisNexis.Red.WindowsStore.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HelpSettingPage : Page
    {
        private ContactUs userContact = GlobalAccess.Instance.CurrentUserInfo.Country.ContactUs;
        public ContactUs UserContact
        {
            get { return userContact; }
            set { userContact = value; }
        }

        public HelpSettingPage()
        {
            InitializeComponent();

            this.Loaded += HelpSettingLoaded;
        }

        private void HelpSettingLoaded(object sender, RoutedEventArgs e)
        {
            ShowHead(true);
        }

        private void ShowFAQPanel(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //ShowHead(false);
        }

        private void ShowTourPanel(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //ShowHead(false);
        }

        private void ShowContactUsPanel(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ShowHead(false);
        }

        private void ShowHead(bool isShow)
        {
            HeadPanel.Visibility = isShow == true ? Visibility.Visible : Visibility.Collapsed;
            BodyPanel.Visibility = isShow == false ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
