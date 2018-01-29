using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.WindowsStore.Html;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LexisNexis.Red.WindowsStore.Controls
{
    public sealed partial class DictionaryDialog : UserControl
    {
        private NavigationDictionaryManager NavigatorManager { set; get; }
        private Popup popup;
        private Point DialogPosition { set; get; }
        private double PaddingLeft { set; get; }
        private string CountryCode { set; get; }
        public DictionaryDialog(string countryCode, Point p, double paddingLeft)
        {
            this.InitializeComponent();

            CountryCode = countryCode;
            DialogPosition = p;
            PaddingLeft = paddingLeft;  
        }

        // adjust for different view states
        private void OnWindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            if (popup.IsOpen == false) return;

            var child = popup.Child as FrameworkElement;
            if (child == null) return;

            DialogRoot.Width = e.Size.Width;
            DialogRoot.Height = e.Size.Height;
            FixedDialogPosition();
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape || e.Key == VirtualKey.Enter)
            {
                CloseDialog();
            }
        }

        public void Show(string dictionaryTitle)
        {
            NavigatorManager = new NavigationDictionaryManager();
            NavigatorManager.Record(dictionaryTitle.Trim());
            SearchDictionary(dictionaryTitle.Trim());
              
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
            double lineHeight = 20;
            double paddingTop = 48 + 48 + 20;
            double marginTop = DialogPosition.Y + paddingTop + lineHeight;
            if ((paddingTop + DialogPosition.Y) * 2 > Window.Current.Bounds.Height)
            {
                marginTop = DialogPosition.Y + paddingTop - lineHeight - 340;
            }

            double bound = Window.Current.Bounds.Width - PaddingLeft;
            if (bound > 300)
            {
                marginRight = (bound - 300) / 2;
            }
            DialogBorder.Margin = new Thickness(0, marginTop, marginRight, 0);
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
            DialogRoot.Width = Window.Current.Bounds.Width;
            DialogRoot.Height = Window.Current.Bounds.Height;
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

        private async void SearchOnWeb(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.google.com/search?q=" + Title.Text));
        }

        private async Task<String> BuildHtml(LegalDefinitionItem definitions, string term,string version)
        {
            String contextualDefinitions = String.Empty;

            foreach (LegalContextualDefinitionItem definition in definitions.ContextualDefinitions)
            {
                contextualDefinitions += definitions.ContextualDefinitions.Count > 1 && definition.Context.Length > 0 ? "<span class='context'>" + definition.Context + "</span> " : String.Empty;
                contextualDefinitions += definition.DefinitionHtml;

                String relatedKeywords = string.Join("; ", definition.AllRelatedKeywords.Select(k => k.HyperlinkTermKeyword));
                String relatedKeywordsLabel = relatedKeywords.Length > 0 ? (contextualDefinitions.Length > 0 ? "See also: " : "See: ") : String.Empty;
                String relatedKeywordClosing = relatedKeywords.Length > 0 ? "." : "";
                contextualDefinitions += "<br/><br/>" + relatedKeywordsLabel + relatedKeywords + relatedKeywordClosing + "<br/><br/>";               
            }
            string h = "<h3>" + term + "</h3>";
            string v = "<div class='version'>" + version + "</div>";
            string html= await HtmlHelper.DictionaryTemplateToString();
            html = html.Replace("#BODY#", h + contextualDefinitions + v);
            return html;
        }

        private void ScriptNotify(object sender, NotifyEventArgs e)
        {
            var notify = JsonConvert.DeserializeObject<NotifytModel>(e.Value);
            switch (notify.Type)
            {
                case "href":
                    string words = notify.Value1;
                    SeeAlso(words);
                    break;
            }
        }

        private async void SearchDictionary(string words)
        {
            BackBtn.IsEnabled = NavigatorManager.BackWard;
            NextBtn.IsEnabled = NavigatorManager.ForWard;
            Title.Text = words;
            LegalDefinitionItem legalDefine = DictionaryUtil.SearchDictionary(Title.Text, CountryCode);
            if (legalDefine == null || legalDefine.ContextualDefinitions == null || legalDefine.ContextualDefinitions.Count == 0)
            {
                NoResultTitle.Visibility = Visibility.Visible;
            }
            else
            {
                NoResultTitle.Visibility = Visibility.Collapsed;
                string html = await BuildHtml(legalDefine, legalDefine.ContextualDefinitions[0].Term, legalDefine.DictionaryVersionText);
                LegalDefineDialog.NavigateToString(html);
            }
        }
        private void SeeAlso(string words)
        {
            NavigatorManager.Record(words);
            SearchDictionary(words);
        }

        private void BackWord(object sender, RoutedEventArgs e)
        {
            var words=NavigatorManager.GoBack();
            SearchDictionary(words);
        }

        private void ForWord(object sender, RoutedEventArgs e)
        {
            var words = NavigatorManager.GoForward();
            SearchDictionary(words);
        }

        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            CloseDialog();
        }
    }

    public class NavigationDictionaryManager
    {
        public NavigationDictionaryManager()
        {
            HistoryRecords = new List<string>();
            CurrenRecordIndex = -1;
        }

        public List<string> HistoryRecords { get; set; }
        public int CurrenRecordIndex { get; set; }
        public bool BackWard { get; set; }
        public bool ForWard { get; set; }
    
        public void Record(string words)
        {
           
            if (CurrenRecordIndex == HistoryRecords.Count - 1)
            {
                CurrenRecordIndex++;
                HistoryRecords.Add(words);
            }
            else if (CurrenRecordIndex < HistoryRecords.Count - 1)
            {
                CurrenRecordIndex++;
                HistoryRecords.RemoveRange(CurrenRecordIndex, HistoryRecords.Count - CurrenRecordIndex);
                HistoryRecords.Add(words);
            }
            BackWard = CurrenRecordIndex > 0 ? true : false;
            ForWard = false;
        }

        public string GoForward()
        {
            CurrenRecordIndex++;
            ForWard = CurrenRecordIndex < HistoryRecords.Count - 1 ? true : false;
            BackWard = true;
            return HistoryRecords[CurrenRecordIndex];
        }
        public string GoBack()
        {
            CurrenRecordIndex--;
            ForWard = true;
            BackWard = CurrenRecordIndex > 0 ? true : false;
            return HistoryRecords[CurrenRecordIndex];
        }
    }
}
