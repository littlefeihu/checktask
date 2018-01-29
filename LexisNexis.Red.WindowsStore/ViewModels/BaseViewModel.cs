using LexisNexis.Red.WindowsStore.Services;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps.Interfaces;
using Microsoft.Practices.Unity;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public abstract class BaseViewModel : ViewModel
    {
        public const string ALL_PUBLICATIONS_SESSION_KEY = "session_key_publications";

        [Dependency]
        protected INavigationService NavigationService { get; set; }

        [Dependency]
        protected IAlertMessageService AlertMessageService { get; set; }

        [Dependency]
        protected IResourceLoader ResourceLoader { get; set; }

        [Dependency]
        protected ISessionStateService SessionService { get; set; }

        [Dependency]
        protected IEventAggregator EventAggregator { get; set; }

        

    }
}
