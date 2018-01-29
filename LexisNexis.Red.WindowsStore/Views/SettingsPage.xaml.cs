using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LexisNexis.Red.WindowsStore.Common;
using LexisNexis.Red.WindowsStore.ViewModels;
using Microsoft.Practices.Prism.StoreApps;
using LexisNexis.Red.Common.HelpClass;
using Microsoft.Practices.Prism.Mvvm.Interfaces;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace LexisNexis.Red.WindowsStore.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : VisualStateAwarePage, IContentPage
    {
        private static readonly List<SettingsMenuItem> SETTINGS_MENU_LIST = new List<SettingsMenuItem>
        {
            new SettingsMenuItem
            {
                Name = "Publication",
                LabelId = "SettingsLabel-Publication",
                Content = typeof(PublicationSettingPage)
            },
            new SettingsMenuItem
            {
                Name = "Help",
                LabelId = "SettingsLabel-Help",
                Content = typeof(HelpSettingPage)
            },
            new SettingsMenuItem
            {
                Name = "About", 
                LabelId = "SettingsLabel-About",
                Content = typeof(AboutSettingPage)
            }
        };

        public List<SettingsMenuItem> MenuList
        {
            get { return SETTINGS_MENU_LIST; }
        }

        private string selectedMenu = "Publication";

        public string SelectedMenu
        {
            get { return selectedMenu; }
            set { selectedMenu = value; }
        }

        public SettingsPage()
        {
            InitializeComponent();
            Loaded += SettingsPageLoaded;
        }

        void SettingsPageLoaded(object sender, RoutedEventArgs e)
        {

            LoadContent(selectedMenu);
        }

        void LoadContent(string menuName)
        {
            MenuListView.SelectedValue = selectedMenu;
            var menuItem = MenuList.FirstOrDefault(x => x.Name == menuName);
            if (menuItem != null && menuItem.Content != null)
            {
                SettingContentFrame.Navigate(menuItem.Content);
            }
        }

        void LoadContent(SettingsMenuItem menuItem)
        {
            MenuListView.SelectedValue = menuItem.Name;
            if (menuItem.Content != null)
            {
                SettingContentFrame.Navigate(menuItem.Content);
            }
        }


        private void SettingsMenuItemClick(object sender, ItemClickEventArgs e)
        {
            var menu = e.ClickedItem as SettingsMenuItem;
            if (menu != null)
            {
                selectedMenu = menu.Name;
                LoadContent(menu);
            }
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            if (pageState == null) return;

            base.SaveState(pageState);

            pageState["CurrentMenu"] = selectedMenu;

        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            if (pageState == null) return;

            base.LoadState(navigationParameter, pageState);

            if (pageState.ContainsKey("CurrentMenu"))
            {
                selectedMenu = pageState["CurrentMenu"].ToString();
            }
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            var navigationService = IoCContainer.Instance.ResolveInterface<INavigationService>();
            navigationService.Navigate("Publications", "");
        }
    }
}
