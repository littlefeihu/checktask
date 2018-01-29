using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace LexisNexis.Red.WindowsStore.Services
{
    public class ModalDialogService
    {
        private Popup popup;
        private TaskCompletionSource<IUICommand> taskCompletionSource;
        public object Content { get; set; }
        public ModalDialogService(object content)
        {
            Content = content;
        }

        private UIElement CreateDialog()
        {
            var content = Window.Current.Content as FrameworkElement;
            if (content == null)
            {
                // The dialog is being shown before content has been created for the window
                Window.Current.Activated += OnWindowActivated;
                return null;
            }
            double width = Window.Current.Bounds.Width;
            double height = Window.Current.Bounds.Height;
            var root = new Grid { Width = width, Height = height };
            var overlay = new Grid { Background = new SolidColorBrush(Colors.Black), Opacity = 0.5D };
            root.Children.Add(overlay);

            var dialogPanel = new Grid { VerticalAlignment = VerticalAlignment.Stretch, HorizontalAlignment =  HorizontalAlignment.Stretch ,RequestedTheme = ElementTheme.Light, IsTapEnabled = true, Background =  new SolidColorBrush(Colors.Transparent)};
            dialogPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            dialogPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            dialogPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            dialogPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            dialogPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            dialogPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            dialogPanel.Tapped += (sender, args) =>
            {
                //to do

                if (args.OriginalSource== dialogPanel)
                {
                    CloseDialog();
                }
            };
            //dialogPanel.

            var contentPresenter = new ContentPresenter { Content = Content };
            Grid.SetColumn(contentPresenter, 1);
            Grid.SetRow(contentPresenter, 1);
            dialogPanel.Children.Add(contentPresenter);
            root.Children.Add(dialogPanel);

            return root;
        }


        private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            Window.Current.Activated -= OnWindowActivated;
            SubscribeEvents();
            popup.Child = CreateDialog();
            popup.IsOpen = true;
        }

        private void SubscribeEvents()
        {
            Window.Current.SizeChanged += OnWindowSizeChanged;
            Window.Current.Content.KeyDown += OnKeyDown;

        }


        private void UnsubscribeEvents()
        {
            Window.Current.SizeChanged -= OnWindowSizeChanged;
            Window.Current.Content.KeyDown -= OnKeyDown;

        }
        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (popup.IsOpen == false) return;

            var child = popup.Child as FrameworkElement;
            if (child == null) return;

            child.Width = e.Size.Width;
            child.Height = e.Size.Height;
        }


        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                CloseDialog();
            }


        }

        private void CloseDialog()
        {
            UnsubscribeEvents();

            popup.IsOpen = false;
        }


        public IAsyncOperation<IUICommand> ShowAsync()
        {
            popup = new Popup { Child = CreateDialog() };
            if (popup.Child != null)
            {
                SubscribeEvents();
                popup.IsOpen = true;
            }
            return AsyncInfo.Run(WaitForInput);
        }

        public void Close()
        {
            CloseDialog();
        }


        private Task<IUICommand> WaitForInput(CancellationToken token)
        {
            taskCompletionSource = new TaskCompletionSource<IUICommand>();

            token.Register(CloseDialog);

            return taskCompletionSource.Task;
        }

    }
}
