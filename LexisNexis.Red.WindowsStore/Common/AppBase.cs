using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LexisNexis.Red.WindowsStore.Services;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.StoreApps;

namespace LexisNexis.Red.WindowsStore.Common
{
    public abstract class AppBase : Application
    {
        private bool _isRestoringFromTermination;

        public bool IsSuspending { get; private set; }

        protected ISessionStateService SessionStateService { get; set; }

        protected INavigationService NavigationService { get; set; }

        protected Func<SplashScreen, Page> ExtendedSplashScreenFactory { get; set; }



        protected AppBase()
        {
            Suspending += OnSuspending;
        }

        protected abstract Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args);
        protected virtual Type GetPageType(string pageToken)
        {
            var assemblyQualifiedAppType = GetType().GetTypeInfo().AssemblyQualifiedName;

            var pageNameWithParameter = assemblyQualifiedAppType.Replace(GetType().FullName, GetType().Namespace + ".Views.{0}Page");

            var viewFullName = string.Format(CultureInfo.InvariantCulture, pageNameWithParameter, pageToken);
            var viewType = Type.GetType(viewFullName);

            if (viewType == null)
            {
                var resourceLoader = ResourceLoader.GetForCurrentView("/Microsoft.Practices.Prism.StoreApps/Resources/");
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, resourceLoader.GetString("DefaultPageTypeLookupErrorMessage"), pageToken, GetType().Namespace + ".Views"),
                    "pageToken");
            }

            return viewType;
        }

        protected virtual void OnRegisterKnownTypesForSerialization() { }

        protected virtual Task OnInitializeAsync(IActivatedEventArgs args)
        {
            return Task.FromResult<object>(null);
        }

        protected virtual IList<SettingsCommand> GetSettingsCommands()
        {
            return null;
        }

        protected virtual object Resolve(Type type)
        {
            return Activator.CreateInstance(type);
        }


        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var rootFrame = await InitializeFrameAsync(args);

            // If the app is launched via the app's primary tile, the args.TileId property
            // will have the same value as the AppUserModelId, which is set in the Package.appxmanifest.
            // See http://go.microsoft.com/fwlink/?LinkID=288842
            string tileId = AppManifestHelper.GetApplicationId();

            if (rootFrame != null && (!_isRestoringFromTermination || (args != null && args.TileId != tileId)))
            {
                await OnLaunchApplicationAsync(args);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        protected async Task<Frame> InitializeFrameAsync(IActivatedEventArgs args)
        {
            var rootFrame = Window.Current.Content as Frame;
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (ExtendedSplashScreenFactory != null)
                {
                    Page extendedSplashScreen = ExtendedSplashScreenFactory.Invoke(args.SplashScreen);
                    rootFrame.Content = extendedSplashScreen;
                }

                var frameFacade = new FrameFacadeAdapter(rootFrame);

                //Initialize MvvmAppBase common services
                SessionStateService = new SessionStateService();

                //Configure VisualStateAwarePage with the ability to get the session state for its frame
                VisualStateAwarePage.GetSessionStateForFrame =
                    frame => SessionStateService.GetSessionStateForFrame(frame);

                //Associate the frame with a key
                SessionStateService.RegisterFrame(frameFacade, "AppFrame");

                NavigationService = CreateNavigationService(frameFacade, SessionStateService);

                SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;



                // Set a factory for the ViewModelLocator to use the default resolution mechanism to construct view models
                ViewModelLocationProvider.SetDefaultViewModelFactory(Resolve);

                OnRegisterKnownTypesForSerialization();
                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    await SessionStateService.RestoreSessionStateAsync();
                }

                await OnInitializeAsync(args);
                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state and navigate to the last page visited
                    try
                    {
                        SessionStateService.RestoreFrameState();
                        NavigationService.RestoreSavedNavigation();
                        _isRestoringFromTermination = true;
                    }
                    catch (SessionStateServiceException)
                    {
                        // Something went wrong restoring state.
                        // Assume there is no state and continue
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            return rootFrame;
        }

        private INavigationService CreateNavigationService(IFrameFacade rootFrame, ISessionStateService sessionStateService)
        {
            var navigationService = new NavigationService(rootFrame, GetPageType, sessionStateService);
            return navigationService;
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            IsSuspending = true;
            try
            {
                var deferral = e.SuspendingOperation.GetDeferral();

                //Bootstrap inform navigation service that app is suspending.
                NavigationService.Suspending();

                // Save application state
                await SessionStateService.SaveAsync();

                deferral.Complete();
            }
            finally
            {
                IsSuspending = false;
            }
        }

        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            if (args == null || args.Request == null || args.Request.ApplicationCommands == null)
            {
                return;
            }

            var applicationCommands = args.Request.ApplicationCommands;
            var settingsCommands = GetSettingsCommands();
            if (settingsCommands != null && settingsCommands.Count>0)
            {
                foreach (var settingsCommand in settingsCommands)
                {
                    applicationCommands.Add(settingsCommand);
                }
            }
          
        }

    }
}
