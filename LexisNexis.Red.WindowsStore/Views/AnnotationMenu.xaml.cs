using LexisNexis.Red.WindowsStore.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LexisNexis.Red.WindowsStore.Views
{
    public sealed partial class AnnotationMenu : UserControl
    {
        private static readonly List<ContentMenuItem> MENU_LIST = new List<ContentMenuItem>
        {
            new ContentMenuItem{ Name = "All", LabelId = "AnnotationFilterAllText"},
            new ContentMenuItem{ Name = "Notes", LabelId = "AnnotationFilterNotesText"},
            new ContentMenuItem{ Name = "Highlights", LabelId = "AnnotationFilterHighlightsText"}
        };

        public List<ContentMenuItem> MenuList
        {
            get { return MENU_LIST; }
        }


        private string selectedMenu = "All";

        public string SelectedMenu
        {
            get { return selectedMenu; }
            set { selectedMenu = value; }
        }

        public AnnotationMenu()
        {
            this.InitializeComponent();
            Loaded += ContentPageLoaded;
            AnnotationMenuListView.ItemsSource = MenuList;
        }

        private void ContentPageLoaded(object sender, RoutedEventArgs e)
        {
            LoadLeftSideContent(selectedMenu);
        }

        private void MenuListViewOnItemClick(object sender, ItemClickEventArgs e)
        {
            var menu = e.ClickedItem as ContentMenuItem;
            if (menu != null)
            {
                SelectedMenu = menu.Name;
                LoadLeftSideContent(menu.Name);
            }
        }

        void LoadLeftSideContent(string menuName)
        {
            AnnotationMenuListView.SelectedValue = menuName;
        }

        private void CheckAllTags(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.IsChecked.Value)
            {
                var model = DataContext as ContentPageViewModel;
                NoTag.IsChecked = true;
                model.CheckAllTags();
            }
        }

        private void NoTagUnChencked(object sender, RoutedEventArgs e)
        {
            if (AllTag.IsChecked.Value)
            {
                AllTag.IsChecked = false;
            }
        }

        private void TagUnChencked(object sender, RoutedEventArgs e)
        {
            if (AllTag.IsChecked.Value)
            {
                AllTag.IsChecked = false;
            }
        }
    }
}
