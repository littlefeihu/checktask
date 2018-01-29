using Windows.UI.Xaml.Controls;
using LexisNexis.Red.WindowsStore.ViewModels;
using LexisNexis.Red.Common.Business;
using System.Linq;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LexisNexis.Red.WindowsStore.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PublicationSettingPage : Page
    {
        public PublicationSettingPage()
        {
            InitializeComponent();
            DataContext = new PublicationSettingPageViewModel();
        }

        private void FontSizeChaged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var font = comboBox.SelectedItem as FontMenuItem;
            SettingsUtil.Instance.SaveFontSize(font.ID);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var book = e.ClickedItem as PublicationViewModel;
            var context = DataContext as PublicationSettingPageViewModel;
            foreach (var publication in context.PublicationsCollection)
            {
                if (publication.IsTitleSelected == true)
                    publication.IsTitleSelected = false;
            }
            book.IsTitleSelected = true;
        }

        private void CollapseDeleteItem(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var context = DataContext as PublicationSettingPageViewModel;
            foreach (var publication in context.PublicationsCollection)
            {
                if (publication.IsTitleSelected == true)
                    publication.IsTitleSelected = false;
            }
        }

        private void DisableCollapseDeleteItem(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.Handled=true;
        }


    }
}
