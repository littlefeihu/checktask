using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Mvvm;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class PublicationsPageViewModel : BaseViewModel
    {
        private const string FILTER_VALUE_ALL = "All";
        private const string FILTER_VALUE_LOAN = "Loan";
        private const string FILTER_VALUE_SUBSCRIPTION = "Subscription";

        private bool loadingData;
        public bool LoadingData
        {
            get { return loadingData; }
            set { SetProperty(ref loadingData, value); }
        }


        #region Publications

        private bool readyForReorder;

        private string selectedFilter = FILTER_VALUE_ALL;

        [RestorableState]
        public string SelectedFilter
        {
            get { return selectedFilter; }
            set
            {
                SetProperty(ref selectedFilter, value);
                FilterData();
            }
        }

        private List<PublicationViewModel> publications;
        [RestorableState]
        public List<PublicationViewModel> Publications
        {
            get { return publications; }
            set
            {
                SetProperty(ref publications, value);
                HasPublications = value != null && value.Count > 0;
                //FilterData();
            }
        }

        private bool hasPublications = true;
        public bool HasPublications
        {
            get { return hasPublications; }
            set { SetProperty(ref hasPublications, value); }
        }

        private int publicationCount;
        [RestorableState]
        public int PublicationCount
        {
            get { return publicationCount; }
            set
            {
                SetProperty(ref publicationCount, value);

            }
        }

        private ObservableCollection<PublicationViewModel> filtedPublications;
        public ObservableCollection<PublicationViewModel> FiltedPublications
        {
            get { return filtedPublications; }
            set
            {
                SetProperty(ref filtedPublications, value);

            }
        }

        #endregion

        #region History

        private bool hasHistory;

        public bool HasHistory
        {
            get { return hasHistory; }
            set { SetProperty(ref hasHistory, value); }
        }

        private ObservableCollection<RecentHistoryItem> historyPublications;
        [RestorableState]
        public ObservableCollection<RecentHistoryItem> HistoryPublications
        {
            get { return historyPublications; }
            set { SetProperty(ref historyPublications, value); }
        }
        #endregion


        public PublicationsPageViewModel()
        {
            FiltedPublications = new ObservableCollection<PublicationViewModel>();
            FiltedPublications.CollectionChanged += ItemsCollectionChanged;

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

        public override async void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);

            if (SessionService.SessionState.ContainsKey(ALL_PUBLICATIONS_SESSION_KEY))
            {
                Publications =
                    SessionService.SessionState[ALL_PUBLICATIONS_SESSION_KEY] as List<PublicationViewModel>;
            }
            #region recently history
            var historys = PublicationContentUtil.Instance.GetRecentHistory();
            if (historys == null || historys.Count == 0)
            {
                HasHistory = false;
            }
            else
            {
                HasHistory = true;
                HistoryPublications = new ObservableCollection<RecentHistoryItem>(historys);
            }
            #endregion

            #region publications
            var offlineResult = PublicationUtil.Instance.GetPublicationOffline();
            UpdatePublications(offlineResult,true);
            var onlineResult = await PublicationUtil.Instance.GetPublicationOnline();

            if (onlineResult.RequestStatus == RequestStatusEnum.Success)
            {
                UpdatePublications(onlineResult.Publications);
            }
            #endregion

        }


        private void UpdatePublications(List<Publication> newList,bool isOffline=false)
        {
            bool isDataChanged = false;
            if (Publications != null)
            {
                if (newList == null)
                {
                    Publications = null;
                    isDataChanged = true;
                }
                else
                {
                    var newCollection = new List<PublicationViewModel>();
                    foreach (var publication in newList)
                    {
                        var currrentPublicaton = Publications.FirstOrDefault(x => x.BookId == publication.BookId);
                        if (currrentPublicaton != null)
                        {
                            if (currrentPublicaton.Version != publication.CurrentVersion.ToString() || isOffline)
                            {
                                currrentPublicaton.UpdateData(publication);
                                isDataChanged = true;
                            }
                            newCollection.Add(currrentPublicaton);
                        }
                        else
                        {
                            newCollection.Add(new PublicationViewModel(publication));
                            isDataChanged = true;
                        }
                    }
                    Publications = newCollection;
                }
            }
            else
            {
                Publications = newList == null ? null : newList.Select(x => new PublicationViewModel(x)).ToList();
                isDataChanged = true;
            }
            if (isDataChanged)
            {
                FilterData();
                SessionService.SessionState[ALL_PUBLICATIONS_SESSION_KEY] = Publications;
            }

        }


        private void FilterData()
        {
            readyForReorder = false;
            FiltedPublications.Clear();
            if (Publications != null)
            {
                switch (SelectedFilter)
                {
                    case FILTER_VALUE_ALL:
                        Publications.ForEach(x => FiltedPublications.Add(x));
                        break;
                    case FILTER_VALUE_LOAN:
                        Publications.ForEach(x =>
                        {
                            if (x.IsLoan)
                            {
                                FiltedPublications.Add(x);
                            }

                        });

                        break;
                    case FILTER_VALUE_SUBSCRIPTION:
                        Publications.ForEach(x =>
                        {
                            if (!x.IsLoan)
                            {
                                FiltedPublications.Add(x);
                            }

                        });

                        break;
                }
            }
            PublicationCount = FiltedPublications.Count();
            readyForReorder = true;
        }

        private async Task ReorderPublications()
        {
            if (SelectedFilter == FILTER_VALUE_ALL)
            {
                for (int i = 0; i < FiltedPublications.Count; i++)
                {
                    Publications[i] = FiltedPublications[i];
                }
            }
            else
            {
                int j = 0;
                foreach (var p in FiltedPublications)
                {

                    while (Publications[j].IsLoan != p.IsLoan)
                    {
                        j++;
                    }
                    Publications[j] = p;
                    j++;
                }
            }
            await PublicationUtil.Instance.OrganiseDlsOrder(Publications.Select(x => x.BookId).ToList());

        }
    }
}
