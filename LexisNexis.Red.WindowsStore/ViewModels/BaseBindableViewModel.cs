using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.WindowsStore.Services;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Prism.StoreApps.Interfaces;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class BaseBindableViewModel : BindableBase
    {
        private IAlertMessageService alertMessageService;
        protected IAlertMessageService AlertMessageService
        {
            get
            {
                return alertMessageService ??
                       (alertMessageService = IoCContainer.Instance.ResolveInterface<IAlertMessageService>());
            }
        }

        private IResourceLoader resourceLoader;
        protected IResourceLoader ResourceLoader
        {
            get
            {
                return resourceLoader ?? (resourceLoader = IoCContainer.Instance.ResolveInterface<IResourceLoader>());
            }
        }

        private IEventAggregator evertAggregator;
        protected IEventAggregator EventAggregator
        {
            get
            {
                return evertAggregator ?? (evertAggregator = IoCContainer.Instance.ResolveInterface<IEventAggregator>());
            }
        }

        private INavigationService navigationService;
        protected INavigationService NavigationService
        {
            get
            {
                return navigationService ?? (navigationService = IoCContainer.Instance.ResolveInterface<INavigationService>());
            }
        }

        private ISessionStateService sessionStateService;

        protected ISessionStateService SessionStateService
        {
            get
            {
                return sessionStateService ??
                       (sessionStateService = IoCContainer.Instance.ResolveInterface<ISessionStateService>());
            }
        }
    }
}
