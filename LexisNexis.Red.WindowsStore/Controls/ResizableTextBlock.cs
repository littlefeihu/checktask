using LexisNexis.Red.WindowsStore.ViewModels;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Markup;

namespace LexisNexis.Red.WindowsStore.Controls
{
    [ContentProperty(Name = "Inlines")]
    public class ResizableTextBlock : Control
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(ResizableTextBlock), new PropertyMetadata(default(string), OnTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        //public static readonly DependencyProperty InlinesProperty = DependencyProperty.Register(
        //    "Inlines", typeof(List<Inline>), typeof(ResizableTextBlock), new PropertyMetadata(default(List<Inline>)));

        public List<Inline> Inlines
        {
            get;
            private set ; 
        }


        public static readonly DependencyProperty MaxLinesProperty = DependencyProperty.Register(
            "MaxLines", typeof(int), typeof(ResizableTextBlock), new PropertyMetadata(default(int), OnValueChanged));

        public int MaxLines
        {
            get { return (int)GetValue(MaxLinesProperty); }
            set { SetValue(MaxLinesProperty, value); }
        }

        public TextBlockStatus Status { get; private  set; }


        public ResizableTextBlock()
        {
            DefaultStyleKey = typeof(ResizableTextBlock);
            Inlines= new List<Inline>();
        }

        private TextBlock bodyTextBlock;
        private Button extendButton;

        protected override void OnApplyTemplate()
        {
            
            base.OnApplyTemplate();
            bodyTextBlock = GetTemplateChild("Body") as TextBlock;
            extendButton = GetTemplateChild("Button") as Button;
            if (bodyTextBlock != null)
            {
                if (Inlines != null)
                {
                    foreach (var inline in Inlines)
                    {
                        bodyTextBlock.Inlines.Add(inline);
                    }
                }
            }
           // InlineCollection

            //extendButton
            if (extendButton != null)
            {
                extendButton.Visibility = Visibility.Collapsed;
                extendButton.Click += (sender, args) =>
                {
                    Status = Status == TextBlockStatus.Collapsed ? TextBlockStatus.Expanded : TextBlockStatus.Collapsed;
                    SetStatus();
                };
            }


            RefreshBodyTextBlock();
            //this.
        }


        private void OnTextSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height != 0)
            {
                if (MaxLines > 0 && bodyTextBlock.MaxLines == 0 && Status == TextBlockStatus.Collapsed)
                {
                    var cutOffHeight = bodyTextBlock.LineHeight * MaxLines;
                    if (bodyTextBlock.ActualHeight > cutOffHeight)
                    {
                        extendButton.Visibility = Visibility.Visible;
                        SetStatus();
                    }
                }
                bodyTextBlock.SizeChanged -= OnTextSizeChanged;
            }

        }

        public void RefreshBodyTextBlock()
        {
            if (bodyTextBlock!=null)
            {
                bodyTextBlock.MaxLines = 0;
                bodyTextBlock.SizeChanged += OnTextSizeChanged;
            }
          
            Status = TextBlockStatus.Collapsed;
            if (extendButton!=null)
            {
                extendButton.Visibility = Visibility.Collapsed;
            }
            

          
        }

        private void SetStatus()
        {
            ResourceLoader resourceLoader = new ResourceLoader();
            if (Status == TextBlockStatus.Collapsed)
            {
                extendButton.Content = resourceLoader.GetString("MoreText");
                bodyTextBlock.MaxLines = MaxLines;
            }
            else
            {
                extendButton.Content = resourceLoader.GetString("LessText");
                bodyTextBlock.MaxLines = 0;
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ResizableTextBlock)d;

            if (obj != null)
            {
                obj.UpdateLayout();
            }

        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ResizableTextBlock)d;
            if (obj != null)
            {
                obj.RefreshBodyTextBlock();
                obj.UpdateLayout();
            }
        }
    }
}
