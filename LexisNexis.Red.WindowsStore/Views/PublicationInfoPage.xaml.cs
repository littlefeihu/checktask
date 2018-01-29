using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.WindowsStore.Common;
using LexisNexis.Red.WindowsStore.ViewModels;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.StoreApps;

namespace LexisNexis.Red.WindowsStore.Views
{
    public sealed partial class PublicationInfoPage : VisualStateAwarePage, IContentPage
    {
        public PublicationInfoPage()
        {
            InitializeComponent();
        }

   
        protected override void GoBack(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var info=DataContext as PublicationInfoPageViewModel;
            //var navigationService = IoCContainer.Instance.ResolveInterface<INavigationService>();
            if(info!=null && info.Publication.IsDownloading)
            {
                info.NategateTo("Publications", "");
                //navigationService.Navigate("Publications", "");
            }
            else
            {
                info.GoBack();
            }
        }

    }
}
