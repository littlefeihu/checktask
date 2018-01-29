using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.WindowsStore.Events;
using Microsoft.Practices.ObjectBuilder2;
using LexisNexis.Red.WindowsStore.Html;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class FontMenuItem
    {
        public int ID { get; set; }
        public double Size { get; set; }
        public string Description { get; set; }
        
    }
    public class PublicationSettingPageViewModel : BaseBindableViewModel
    {
    
        public FontMenuItem FontSize { get; set; }

        public List<FontMenuItem> FontSizeList
        {
            get { return HtmlHelper.FONTSIZE_LIST; }
        }

        private ObservableCollection<PublicationViewModel> publicationsCollection;
        public ObservableCollection<PublicationViewModel> PublicationsCollection
        {
            get { return publicationsCollection; }
            set { SetProperty(ref publicationsCollection, value); }
        }

        private bool readyForReorder;

        public List<PublicationViewModel> Publications { get; private set; }

        public PublicationSettingPageViewModel()
        {
            PublicationsCollection = new ObservableCollection<PublicationViewModel>();
            PublicationsCollection.CollectionChanged += ItemsCollectionChanged;
            if (SessionStateService.SessionState.ContainsKey(BaseViewModel.ALL_PUBLICATIONS_SESSION_KEY))
            {
                Publications =
                    SessionStateService.SessionState[BaseViewModel.ALL_PUBLICATIONS_SESSION_KEY] as List<PublicationViewModel>;
            }
            if (Publications != null)
            {
                readyForReorder = false;
                Publications.ForEach(x => PublicationsCollection.Add(x));
                readyForReorder = true;
            }

            int id=(int)SettingsUtil.Instance.GetFontSize();
            FontSize = FontSizeList[id];
           
            EventAggregator.GetEvent<PublicationDeletedEvent>().Subscribe(DeletePublication);
        }


        private async void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (readyForReorder)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        await ReorderPublications();
                        break;
                }
            }
        }

        private async Task ReorderPublications()
        {
            for (int i = 0; i < PublicationsCollection.Count; i++)
            {
                Publications[i] = PublicationsCollection[i];
            }
            await PublicationUtil.Instance.OrganiseDlsOrder(Publications.Select(x => x.BookId).ToList());
        }

        private void DeletePublication(int bookId)
        {
            if (Publications!=null)
            {
                var publication = Publications.FirstOrDefault(x => x.BookId == bookId);
                if (publication!=null)
                {
                    Publications.Remove(publication);
                    SessionStateService.SessionState[BaseViewModel.ALL_PUBLICATIONS_SESSION_KEY] = Publications;
                    PublicationsCollection.Remove(publication);
                }
            }
        }
        
    }
}
