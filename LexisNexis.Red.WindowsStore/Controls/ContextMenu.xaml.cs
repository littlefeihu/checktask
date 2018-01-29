using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.WindowsStore.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using System.Linq;
using System;
using Windows.ApplicationModel.Resources;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LexisNexis.Red.WindowsStore.Controls
{
    public sealed partial class ContextMenu : UserControl
    {
        private const double COLLAPSE = 0.00001;
        private Popup popup;
        private Point DialogPosition { set; get; }
        private double PaddingLeft { set; get; }
        public delegate void BtnClickHandle(ContextMenuStatue s);
        public event BtnClickHandle ContextMenuClick;
        public ContextMenu()
        {
            this.InitializeComponent();
            Loaded += ContextMenu_Loaded;
        }

        void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            NoteDate.Text = DateTime.Now.ToString("dd MMM yyyy");
        }

        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (popup.IsOpen == false) return;

            var child = popup.Child as FrameworkElement;
            if (child == null) return;

            Root.Width = e.Size.Width;
            Root.Height = e.Size.Height;
            FixedDialogPosition();
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape || e.Key == VirtualKey.Enter)
            {
                CloseDialog();
            }
        }

        public void Show(Point p, double paddingLeft)
        {
            DialogPosition = p;
            PaddingLeft = paddingLeft;
            DictionaryBtn.Visibility = DictionaryUtil.IsDictionaryDownloaded() ? Visibility.Visible : Visibility.Collapsed;
            popup = new Popup { Child = CreateDialog() };
            if (popup.Child != null)
            {
                SubscribeEvents();
                popup.IsOpen = true;
            }
        }

        private void FixedDialogPosition()
        {
            double marginRight = 0;
            double marginTop = DialogPosition.Y + 50;
            double bound = Window.Current.Bounds.Width - PaddingLeft;
            if (bound > 300)
            {
                marginRight = (bound - 300) / 2;
            }
            ContextMenuDialog.Margin = new Thickness(0, marginTop, marginRight, 0);
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
            Root.Width = Window.Current.Bounds.Width;
            Root.Height = Window.Current.Bounds.Height;
            FixedDialogPosition();
            return this;
        }

        private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            Window.Current.Activated -= OnWindowActivated;
            SubscribeEvents();
            popup.Child = CreateDialog();
            popup.IsOpen = true;
        }

        private void CloseDialog()
        {
            UnsubscribeEvents();
            popup.IsOpen = false;
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

        private void CloseDialog(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            CloseDialog();
        }
        private void Root_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            CloseDialog();
        }

        private void CloseFlyout(object sender, object e)
        {
            var model = DataContext as AnnotationViewModel;
            model.AddAnnotationNotify();
            CloseDialog();
        }

        private void Copy(object sender, RoutedEventArgs e)
        {
            ContextMenuClick.Invoke(ContextMenuStatue.Copy);
        }

        private void LegalDefine(object sender, RoutedEventArgs e)
        {
            ContextMenuClick.Invoke(ContextMenuStatue.LegalDefine);
        }


        private void KeepDialog(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void KeepDialog(object sender, PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void AddHighlight(object sender, RoutedEventArgs e)
        {
            AnnotationDialog(AnnotationStatue.Highlight);
        }

        private void AddNote(object sender, RoutedEventArgs e)
        {
            AnnotationDialog(AnnotationStatue.Note);
        }
        private void AnnotationDialog(AnnotationStatue annotationStatue)
        {
            ContextMenuDialog.Opacity = COLLAPSE;
            var flyout = FlyoutBase.GetAttachedFlyout(FlyoutAttached);
            flyout.ShowAt(FlyoutAttached);
            var model = DataContext as AnnotationViewModel;
            model.Switch(annotationStatue);
        }

        private void EditTag(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var guid = (Guid)btn.Tag;
            var model = DataContext as AnnotationViewModel;
            model.EditTagDialogOpen(guid);
        }

        private void DeleteTag(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var tag = (Guid)btn.Tag;
            var model = DataContext as AnnotationViewModel;
            model.DeleteTag(tag);
        }

    }
}
