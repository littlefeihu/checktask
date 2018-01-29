using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.WindowsStore.ViewModels;
using LexisNexis.Red.WindowsStore.Views;
using Microsoft.Xaml.Interactivity;

namespace LexisNexis.Red.WindowsStore.Behaviors
{
    public class NavigateToContentPageAction : DependencyObject, IAction
    {

        public string EventArgsParameterPath { get; set; }
        public object Execute(object sender, object parameter)
        {

            var args = parameter as ItemClickEventArgs;
            if (args != null)
            {
                var item = args.ClickedItem as PublicationViewModel;
                if (item != null)
                {
                    if (!item.IsDownloading && item.PublicationStatus != PublicationStatusEnum.NotDownloaded)
                    {
                        var frame = GetFrame(sender as DependencyObject);
                        return frame.Navigate(typeof(ContentPage), item);
                    }

                }
            }
            return null;
        }

        private Frame GetFrame(DependencyObject dependencyObject)
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            var parentFrame = parent as Frame;
            if (parentFrame != null) return parentFrame;
            return GetFrame(parent);
        }
    }
}
