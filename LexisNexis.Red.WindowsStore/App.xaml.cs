using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using LexisNexis.Red.Common;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.WindowsStore.Common;
using LexisNexis.Red.WindowsStore.Implementation;
using LexisNexis.Red.WindowsStore.Services;
using LexisNexis.Red.WindowsStore.ViewModels;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using System.Collections.ObjectModel;
using LexisNexis.Red.Common.Business;
using Windows.UI.Xaml;
using LexisNexis.Red.WindowsStore.Html;
using Xamarin;


namespace LexisNexis.Red.WindowsStore
{

    sealed partial class App : AppBase
    {

        public App()
        {
            InitializeComponent();
            //RequestedTheme = ApplicationTheme.Light;

        }

        public IEventAggregator EventAggregator { get; set; }


        protected override Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
        {
#if PREVIEW
            Insights.Initialize("020c123dbc8cfe5aeb896f3bc511b5101f43aaab");
#elif RELEASE
             Insights.Initialize("020c123dbc8cfe5aeb896f3bc511b5101f43aaab");
#elif TESTING
             Insights.Initialize("ef4fd029ab6dfd947b41db93e1ec0ec0e4541669");
#else
             Insights.Initialize("ef4fd029ab6dfd947b41db93e1ec0ec0e4541669");
#endif
            var curUser = GlobalAccess.Instance.CurrentUserInfo;


          //  NavigationService = new NavigationService(NavigationService, GetPageType, SessionStateService);   

            NavigationService.Navigate(curUser == null ? "Login" : "Publications", null);
           
            return Task.FromResult<object>(null);
        }

        protected override object Resolve(Type type)
        {
            return IoCContainer.Instance.Resolve(type);

        }

        protected override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            EventAggregator = new EventAggregator();
            IoCContainer.Instance.RegisterInstance(NavigationService);
            IoCContainer.Instance.RegisterInstance(SessionStateService);
            IoCContainer.Instance.RegisterInstance(EventAggregator);
            IoCContainer.Instance.RegisterInstance<IResourceLoader>(new ResourceLoaderAdapter(new ResourceLoader()));
            IoCContainer.Instance.RegisterInstance<IDevice>(new WindowsDevice());
            IoCContainer.Instance.RegisterInstance<IDirectory>(new FileDirectory());
            IoCContainer.Instance.RegisterInstance<IAlertMessageService>(new AlertMessageService());
            IoCContainer.Instance.RegisterInstance<INetwork>(new Network());
            IoCContainer.Instance.RegisterInstance<IPackageFile>(new ZipFileManager());
            IoCContainer.Instance.RegisterInstance<ICryptogram>(new Cryptogram());
            await GlobalAccess.Instance.Init();

            Application.Current.UnhandledException += RecordUnhandledException;
            
        }

        private void RecordUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
             LogService.RecordUnhandledException(e);
        }

        protected override void OnRegisterKnownTypesForSerialization()
        {
            SessionStateService.RegisterKnownType(typeof(Country));
            SessionStateService.RegisterKnownType(typeof(List<Country>));
            SessionStateService.RegisterKnownType(typeof(Publication));
            SessionStateService.RegisterKnownType(typeof(List<Publication>));
            SessionStateService.RegisterKnownType(typeof(PublicationViewModel));
            SessionStateService.RegisterKnownType(typeof(List<PublicationViewModel>));
            SessionStateService.RegisterKnownType(typeof(TocCurrentNode));
            SessionStateService.RegisterKnownType(typeof(IndexMenuItem));
            SessionStateService.RegisterKnownType(typeof(ObservableCollection<IndexMenuItem>));
            SessionStateService.RegisterKnownType(typeof(BreadcrumbNav));
            SessionStateService.RegisterKnownType(typeof(ObservableCollection<BreadcrumbNav>));
            SessionStateService.RegisterKnownType(typeof(NodeExpandStatue));
            SessionStateService.RegisterKnownType(typeof(NavMenuItem));
            SessionStateService.RegisterKnownType(typeof(NavigationManager));
            SessionStateService.RegisterKnownType(typeof(AnnotationTag));
            SessionStateService.RegisterKnownType(typeof(TagItem));
            SessionStateService.RegisterKnownType(typeof(ObservableCollection<TagItem>));
            SessionStateService.RegisterKnownType(typeof(SearchResultModel));
            SessionStateService.RegisterKnownType(typeof(List<SearchResultModel>));
            SessionStateService.RegisterKnownType(typeof(List<string>));
            SessionStateService.RegisterKnownType(typeof(NavigationRecordManager));
            SessionStateService.RegisterKnownType(typeof(List<NavigationRecord>));
            SessionStateService.RegisterKnownType(typeof(NavigationRecord));
            SessionStateService.RegisterKnownType(typeof(NavigationType));
            SessionStateService.RegisterKnownType(typeof(RecentHistoryItem));
            SessionStateService.RegisterKnownType(typeof(ObservableCollection<RecentHistoryItem>));
        }






    }
}
