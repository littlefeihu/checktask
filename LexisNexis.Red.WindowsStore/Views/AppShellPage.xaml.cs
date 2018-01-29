using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.WindowsStore.ViewModels;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.StoreApps;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LexisNexis.Red.WindowsStore.Views
{


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppShellPage : VisualStateAwarePage
    {
        private int seletedIndex=-1;

        private bool isNavigationPanelOpen;

        private readonly List<NavMenuItem> navlist = new List<NavMenuItem>(
            new[]
            {
                new NavMenuItem
                {
                    Symbol = "\uE82D",
                    LabelId = "NavMenu-MyPublications",
                    DestPage = "Publications"
                },
                new NavMenuItem
                {
                    Symbol = "\uE70F",
                    LabelId = "NavMenu-Annotations",
                    DestPage = "Annotations"
                }
            });

        public Frame ContentFrame
        {
            get { return MainContentFrame; }
        }

        public AppShellPage()
        {
            InitializeComponent();
            TopNavMenuList.ItemsSource = navlist;
            TopNavMenuList.Loaded += (sender, e) =>
            {
                TopNavMenuList.SelectedIndex = seletedIndex;
            }
            ;
          //  ResourceLoader.
        }

        private void OnNavigatingToPage(object sender, NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType.Name.EndsWith("Page", StringComparison.OrdinalIgnoreCase))
            {

                var pageName = e.SourcePageType.Name.Substring(0, e.SourcePageType.Name.Length - 4);
                seletedIndex = -1;
                for (int i = 0; i < navlist.Count; i++)
                {
                    if (navlist[i].DestPage==pageName)
                    {
                        seletedIndex = i;
                        break;
                    }
                    
                }
               
            }
            TopNavMenuList.SelectedIndex = seletedIndex;
        }

        private void CloseNavigation()
        {
            if (isNavigationPanelOpen==true)
            {
                NavigationGrid.Width = 48;
                isNavigationPanelOpen = false;
            }          
        }

        private void OpenNavigation()
        {
            if (isNavigationPanelOpen == false)
            {
                NavigationGrid.Width = 320;
                isNavigationPanelOpen = true;
            }
        }

        private void OnNavigatedToPage(object sender, NavigationEventArgs e)
        {
            var page = e.Content as Page;
            if (page != null)
            {
                var control = page;
                control.Loaded += ContentPageLoaded;
            }
        }

        private void ContentPageLoaded(object sender, RoutedEventArgs e)
        {
            var page = sender as Page;
            if (page != null)
            {
                page.Focus(FocusState.Programmatic);
                page.Loaded -= ContentPageLoaded;
            }
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            if (pageState == null) return;

            base.SaveState(pageState);

            pageState["CurrentContentPageIndex"] = seletedIndex;
        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            if (pageState == null) return;

            base.LoadState(navigationParameter, pageState);

            if (pageState.ContainsKey("CurrentContentPageIndex"))
            {
                seletedIndex = (int)pageState["CurrentContentPageIndex"] ;
            }
        }

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            if (isNavigationPanelOpen)
            {
                CloseNavigation();
            }
            else
            {
                OpenNavigation();
            }
        }

        private void MainContentFrameOnGotFocus(object sender, RoutedEventArgs e)
        {
            CloseNavigation();
        }

        private void TopNavMenuOnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as NavMenuItem;
            if (item!=null)
            {
                if (!string.IsNullOrEmpty(item.DestPage))
                {
                    var navigationService = IoCContainer.Instance.ResolveInterface<INavigationService>();
                    navigationService.Navigate(item.DestPage, item.Arguments);
                }
            }
            

            if (isNavigationPanelOpen)
            {
                CloseNavigation();
            }

        }
    }
}
