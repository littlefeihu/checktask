using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.WindowsStore.Common;
using LexisNexis.Red.WindowsStore.Controls;
using LexisNexis.Red.WindowsStore.ViewModels;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Prism.StoreApps;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace LexisNexis.Red.WindowsStore.Views
{
    public sealed partial class AnnotationsPage : VisualStateAwarePage, IContentPage
    {

        public AnnotationsPage()
        {
            InitializeComponent();
        }
     
        private void RecentHistorySwitch(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            var navigationService = IoCContainer.Instance.ResolveInterface<INavigationService>();
            navigationService.Navigate("Publications", "");
        }


        private void CheckAllTags(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.IsChecked.Value)
            {
                var model = DataContext as AnnotationsPageViewModel;
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

        private void EditTag(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var guid = (Guid)btn.Tag;
            var model = DataContext as AnnotationsPageViewModel;
            model.EditDialogOpen(guid);
        }

        private void DeleteTag(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var tag = (Guid)btn.Tag;
            var model = DataContext as AnnotationsPageViewModel;
            model.DeleteTag(tag);
        }

        private void TagUnChencked(object sender, RoutedEventArgs e)
        {
            AllTag.IsChecked = false;
        }

     

    }
}
