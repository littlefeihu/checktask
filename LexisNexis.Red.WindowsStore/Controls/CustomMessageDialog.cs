using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace LexisNexis.Red.WindowsStore.Controls
{
    public class CustomMessageDialog
    {
        private Popup popup;
        private TaskCompletionSource<IUICommand> taskCompletionSource;
        private StackPanel buttonPanel;


        public CustomMessageDialog(string message)
        {
            Title = string.Empty;
            Commands = new List<IUICommand>();
            CancelCommandIndex = Int32.MaxValue;
            DefaultCommandIndex = 0;
            HeaderBrush = new SolidColorBrush(Colors.White);
            Message = message;
        }


        public CustomMessageDialog(string message, string title)
            : this(message)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Title = title;
            }
        }

        private Button defaultCommandButton;
        public uint DefaultCommandIndex { get; set; }

        public uint CancelCommandIndex { get; set; }

        public IList<IUICommand> Commands { get; private set; }

        public string Message { get; set; }

        public string Title { get; set; }

        public Brush HeaderBrush { get; set; }

        /// <Summary>
        /// Begins an asynchronous operation showing a dialog.
        /// </Summary>
        /// <Returns>     
        /// An object that represents the asynchronous operation. For more on the async
        /// pattern, see Asynchronous programming in the Windows Runtime.
        /// </Returns>
        public IAsyncOperation<IUICommand> ShowAsync()
        {
            popup = new Popup { Child = CreateDialog() };
            if (popup.Child != null)
            {
                SubscribeEvents();
                popup.IsOpen = true;
            }
            defaultCommandButton.Focus(FocusState.Programmatic);
            return AsyncInfo.Run(WaitForInput);
        }

        private Task<IUICommand> WaitForInput(CancellationToken token)
        {
            taskCompletionSource = new TaskCompletionSource<IUICommand>();

            token.Register(OnCanceled);

            return taskCompletionSource.Task;
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

            Style subHeaderTextStyle = Application.Current.Resources["SubheaderTextBlockStyle"] as Style;

            double width = Window.Current.Bounds.Width;
            double height = Window.Current.Bounds.Height;
            var root = new Grid { Width = width, Height = height };
            var overlay = new Grid { Background = new SolidColorBrush(Colors.Black), Opacity = 0.2D };
            root.Children.Add(overlay);

            var dialogPanel = new Grid
            {
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush(Colors.Transparent)
            };

            dialogPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            dialogPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            dialogPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var contentBoder = new Border
            {
                BorderThickness = new Thickness(1D),
                BorderBrush = Application.Current.Resources["RedColorBrush"] as SolidColorBrush,
                Padding = new Thickness(20D),
                Background = new SolidColorBrush(Colors.White)
            };

            Grid.SetColumn(contentBoder, 1);
            dialogPanel.Children.Add(contentBoder);

            var contentPanel = new StackPanel { Orientation = Orientation.Vertical };

            contentBoder.Child = contentPanel;

            var titleTextBlock = new TextBlock
            {
                Text = Title,
                Style = subHeaderTextStyle,
                Margin = new Thickness(0, 0, 0, 20)
            };
            AutomationProperties.SetAutomationId(titleTextBlock, "titleTextBlock");
            contentPanel.Children.Add(titleTextBlock);


            Style bodyTextStyle = Application.Current.Resources["BodyTextBlockStyle"] as Style;
            var messageTextBlock = new TextBlock
            {
                Text = Message,
                Style = bodyTextStyle,
                Margin = new Thickness(0, 0, 0, 20),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            AutomationProperties.SetAutomationId(messageTextBlock, "messageTextBlock");
            contentPanel.Children.Add(messageTextBlock);

            buttonPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Right, Orientation = Orientation.Horizontal };
            contentPanel.Children.Add(buttonPanel);

            var defaultCommandStyle = Application.Current.Resources["DefaultDialogCommandButtonStyle"] as Style;

            if (Commands.Count == 0)
            {
                Button button = new Button
                {
                    Content = "Close",
                    MinWidth = 92,
                    Margin = new Thickness(20, 20, 0, 0),
                    Style = defaultCommandStyle
                };
                AutomationProperties.SetAutomationId(button, "Close");
                button.Click += (okSender, okArgs) => CloseDialog(null);
                buttonPanel.Children.Add(button);
                defaultCommandButton = button;
            }
            else
            {
                int i = 0;
                foreach (var command in Commands)
                {
                    IUICommand currentCommand = command;

                    Button button = new Button
                    {
                        Content = command.Label,
                        Margin = new Thickness(20, 20, 0, 0),
                        MinWidth = 92
                    };
                    AutomationProperties.SetAutomationId(button, command.Label);

                    if (i == DefaultCommandIndex)
                    {
                        button.Style = defaultCommandStyle;
                        defaultCommandButton = button;
                    }
                    button.Click += (okSender, okArgs) => CloseDialog(currentCommand);
                    buttonPanel.Children.Add(button);
                    i++;
                }
            }

            root.Children.Add(dialogPanel);

            return root;
        }

        // adjust for different view states
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
            // Only respond to Esc if there is a cancel index
            if ((e.Key == VirtualKey.Escape) && (CancelCommandIndex < Commands.Count))
            {
                OnCanceled();
            }

            // Only respond to Enter if there is a cancel index
            if ((e.Key == VirtualKey.Enter) && (DefaultCommandIndex < Commands.Count))
            {
                CloseDialog(Commands[(int)DefaultCommandIndex]);
            }
        }

        private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            Window.Current.Activated -= OnWindowActivated;
            SubscribeEvents();
            popup.Child = CreateDialog();
            popup.IsOpen = true;
        }

        private void OnCanceled()
        {
            UnsubscribeEvents();

            IUICommand command = null;
            if (CancelCommandIndex < Commands.Count)
            {
                command = Commands[(int)CancelCommandIndex];
            }
            CloseDialog(command);
        }

        private void CloseDialog(IUICommand command)
        {
            UnsubscribeEvents();

            if (command != null && command.Invoked != null)
            {
                command.Invoked(command);
            }
            popup.IsOpen = false;
            taskCompletionSource.SetResult(command);
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
    }
}
