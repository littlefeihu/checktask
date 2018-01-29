using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.WindowsStore.Common;
using LexisNexis.Red.WindowsStore.ViewModels;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.StoreApps;
using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.WindowsStore.Views
{
    public sealed partial class PublicationsPage : VisualStateAwarePage,IContentPage
    {
        public PublicationsPage()
        {
            InitializeComponent();
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            if (pageState == null) return;

            base.SaveState(pageState);
        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            if (pageState == null) return;
            base.LoadState(navigationParameter, pageState);        
        }

        private void PublicationItemClick(object sender, ItemClickEventArgs e)
        {
            var pulication = e.ClickedItem as  PublicationViewModel;
            //pulication.Expired = true;
            if (pulication != null && pulication.PublicationStatus != PublicationStatusEnum.NotDownloaded && !pulication.IsDownloading)
            {
                var navigationService = IoCContainer.Instance.ResolveInterface<INavigationService>();
                navigationService.Navigate("Content", pulication.BookId);
            }
        }

        private async void RecentHistoryItemClick(object sender, ItemClickEventArgs e)
        {
            var historyItem = e.ClickedItem as RecentHistoryItem;
            if (historyItem != null)
            {
                int TOCId = await PublicationContentUtil.Instance.GetTOCIDByDocId(historyItem.BookId, historyItem.DOCID);
                string param= historyItem.BookId+"|"+TOCId;
                var navigationService = IoCContainer.Instance.ResolveInterface<INavigationService>();
                navigationService.Navigate("Content", param);
            }
        }
    }
}
