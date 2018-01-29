using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.WindowsStore.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class IndexMenu : UserControl
    {
        public IndexMenu()
        {
            this.InitializeComponent();
        }

        private async void IndexMenuItemClick(object sender, ItemClickEventArgs e)
        {
            var contentViewModel = DataContext as ContentPageViewModel;
            var item = e.ClickedItem as IndexMenuItem;
            if (contentViewModel != null && item != null)
            {          
                await contentViewModel.CurrentSectedIndexChanged(item);
            }

        }
    }
}
