using Windows.UI.Xaml.Controls;
using LexisNexis.Red.WindowsStore.ViewModels;
using Windows.UI.Xaml;
using Microsoft.Practices.Prism.PubSubEvents;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.WindowsStore.Events;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Collections.Generic;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LexisNexis.Red.WindowsStore.Views
{
    public sealed partial class TOCMenu : UserControl
    {
        //private IEventAggregator eventAggregator;
        //protected IEventAggregator EventAggregator
        //{
        //    get
        //    {
        //        return eventAggregator ?? (eventAggregator = IoCContainer.Instance.ResolveInterface<IEventAggregator>());
        //    }
        //}
        public TOCMenu()
        {
            InitializeComponent();
        }

        private bool IsProcessing = false;
        private async void TOCItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as BreadcrumbNav;
            if (!IsProcessing && item != null)
            {
                IsProcessing = true;
                var contentViewModel = DataContext as ContentPageViewModel;
                if (contentViewModel != null)
                {
                    await contentViewModel.BreadcrumItemExpandStatueChange(item);
                }
                IsProcessing = false;

            }

        }

    }

}
