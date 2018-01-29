using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using LexisNexis.Red.WindowsStore.Common;
using LexisNexis.Red.WindowsStore.Views;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.StoreApps.Interfaces;

namespace LexisNexis.Red.WindowsStore.Services
{
    public class NavigationService : INavigationService
    {
        private readonly INavigationService _frameNavigationService;

        private const string LAST_CONTENT_NAVIGATION_PARAMETER_KEY = "LastContentNavigationParameter";
        private const string LAST_CONTENT_NAVIGATION_PAGE_KEY = "LastContentNavigationPageKey";
        private IFrameFacade _contentFrameFacade;
        private IFrameFacade _rootFrame;
        private readonly Func<string, Type> _navigationResolver;
        private readonly ISessionStateService _sessionStateService;


        public NavigationService(IFrameFacade rootFrame, Func<string, Type> navigationResolver, ISessionStateService sessionStateService)
        {
            _rootFrame = rootFrame;
            _frameNavigationService = new FrameNavigationService(rootFrame, navigationResolver, sessionStateService);
            _navigationResolver = navigationResolver;
            _sessionStateService = sessionStateService;
        }

        private bool IsInShellPage()
        {

            if (_rootFrame != null)
            {
                return _rootFrame.Content is AppShellPage;
            }
            return false;
        }

        private void InitContentFrame()
        {
            if (_contentFrameFacade == null)
            {

                if (_rootFrame != null)
                {
                    var shell = _rootFrame.Content as AppShellPage;
                    if (shell != null)
                    {
                        _contentFrameFacade = new FrameFacadeAdapter(shell.ContentFrame);
                        _sessionStateService.RegisterFrame(_contentFrameFacade, "ContentFrame");
                        _contentFrameFacade.Navigating += ContentFrameNavigating;
                        _contentFrameFacade.Navigated += ContentFrameNavigated;
                    }

                }
            }

        }

        public bool Navigate(string pageToken, object parameter)
        {

            Type pageType = _navigationResolver(pageToken);

            if (pageType != null)
            {
                if (typeof(IContentPage).GetTypeInfo().IsAssignableFrom(pageType.GetTypeInfo()))
                {
                    if (!IsInShellPage())
                    {
                        _frameNavigationService.Navigate("AppShell", null);
                        InitContentFrame();
                    }
                    var lastNavigationParameter =
                        _sessionStateService.SessionState.ContainsKey(LAST_CONTENT_NAVIGATION_PARAMETER_KEY)
                            ? _sessionStateService.SessionState[LAST_CONTENT_NAVIGATION_PARAMETER_KEY]
                            : null;
                    var lastPageTypeFullName =
                        _sessionStateService.SessionState.ContainsKey(LAST_CONTENT_NAVIGATION_PAGE_KEY)
                            ? _sessionStateService.SessionState[LAST_CONTENT_NAVIGATION_PAGE_KEY] as string
                            : string.Empty;
                    if (lastPageTypeFullName != pageType.FullName || lastNavigationParameter != parameter)
                    {
                        return _contentFrameFacade.Navigate(pageType, parameter);
                    }
                }
                else
                {
                    if (_contentFrameFacade != null)
                    {
                       
                        _sessionStateService.UnregisterFrame(_contentFrameFacade);
                        _contentFrameFacade = null;

                    }
                    return _frameNavigationService.Navigate(pageToken, parameter);
                }
            }
            return false;
        }



        public void GoBack()
        {
            if (_contentFrameFacade != null)
            {
                _contentFrameFacade.GoBack();
            }
            else
            {
                _frameNavigationService.GoBack();
            }
        }

        public bool CanGoBack()
        {
            if (_contentFrameFacade != null)
            {
                return _contentFrameFacade.CanGoBack;
            }
            return _frameNavigationService.CanGoBack();
        }

        public void ClearHistory()
        {
            _contentFrameFacade = null;
            _frameNavigationService.ClearHistory();

        }

        public void RestoreSavedNavigation()
        {
            _frameNavigationService.RestoreSavedNavigation();
            InitContentFrame();
            if (_contentFrameFacade != null)
            {
                var navigationParameter = _sessionStateService.SessionState[LAST_CONTENT_NAVIGATION_PARAMETER_KEY];
                NavigateToCurrentViewModel(NavigationMode.Refresh, navigationParameter);
            }

        }


        private void NavigateToCurrentViewModel(NavigationMode navigationMode, object parameter)
        {
            var frameState = _sessionStateService.GetSessionStateForFrame(_contentFrameFacade);
            var viewModelKey = "ViewModel-" + _contentFrameFacade.BackStackDepth;

            if (navigationMode == NavigationMode.New)
            {
                // Clear existing state for forward navigation when adding a new page/view model to the
                // navigation stack
                var nextViewModelKey = viewModelKey;
                int nextViewModelIndex = _contentFrameFacade.BackStackDepth;
                while (frameState.Remove(nextViewModelKey))
                {
                    nextViewModelIndex++;
                    nextViewModelKey = "ViewModel-" + nextViewModelIndex;
                }
            }

            var newView = _contentFrameFacade.Content as FrameworkElement;
            if (newView == null) return;
            var newViewModel = newView.DataContext as INavigationAware;
            if (newViewModel != null)
            {
                Dictionary<string, object> viewModelState;
                if (frameState.ContainsKey(viewModelKey))
                {
                    viewModelState = frameState[viewModelKey] as Dictionary<string, object>;
                }
                else
                {
                    viewModelState = new Dictionary<string, object>();
                }
                newViewModel.OnNavigatedTo(parameter, navigationMode, viewModelState);
                frameState[viewModelKey] = viewModelState;
            }
        }

        private void NavigateFromCurrentViewModel(bool suspending)
        {
            if (_contentFrameFacade != null)
            {
                var departingView = _contentFrameFacade.Content as FrameworkElement;
                if (departingView == null) return;
                var frameState = _sessionStateService.GetSessionStateForFrame(_contentFrameFacade);
                var departingViewModel = departingView.DataContext as INavigationAware;

                var viewModelKey = "ViewModel-" + _contentFrameFacade.BackStackDepth;
                if (departingViewModel != null)
                {
                    var viewModelState = frameState.ContainsKey(viewModelKey)
                        ? frameState[viewModelKey] as Dictionary<string, object>
                        : null;

                    departingViewModel.OnNavigatedFrom(viewModelState, suspending);
                }
            }
        }


        public void Suspending()
        {
            NavigateFromCurrentViewModel(true);
            _frameNavigationService.Suspending();
        }


        private void ContentFrameNavigating(object sender, EventArgs e)
        {
            NavigateFromCurrentViewModel(false);
        }

        /// <summary>
        /// Handles the Navigated event of the Frame control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MvvmNavigatedEventArgs"/> instance containing the event data.</param>
        private void ContentFrameNavigated(object sender, MvvmNavigatedEventArgs e)
        {
            // Update the page type and parameter of the last navigation
            _sessionStateService.SessionState[LAST_CONTENT_NAVIGATION_PAGE_KEY] = _contentFrameFacade.Content.GetType().FullName;
            _sessionStateService.SessionState[LAST_CONTENT_NAVIGATION_PARAMETER_KEY] = e.Parameter;

            NavigateToCurrentViewModel(e.NavigationMode, e.Parameter);
        }
    }
}
