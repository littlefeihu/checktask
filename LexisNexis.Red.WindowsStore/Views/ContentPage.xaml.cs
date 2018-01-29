using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LexisNexis.Red.WindowsStore.Common;
using LexisNexis.Red.WindowsStore.ViewModels;
using Microsoft.Practices.Prism.StoreApps;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.WindowsStore.Html;
using System;
using Newtonsoft.Json;
using Windows.Foundation;
using LexisNexis.Red.Common.HelpClass;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using Windows.System;
using Windows.ApplicationModel.Resources;
using LexisNexis.Red.WindowsStore.Controls;
using LexisNexis.Red.WindowsStore.Tools;

//using System.Management.Automation;

namespace LexisNexis.Red.WindowsStore.Views
{
    public sealed partial class ContentPage : VisualStateAwarePage, IContentPage
    {
        private static readonly List<ContentMenuItem> MENU_LIST = new List<ContentMenuItem>
        {
            new ContentMenuItem{ Name = "toc", LabelId = "MenuLabel-TOC"},
            new ContentMenuItem{ Name = "index", LabelId = "MenuLabel-Index"},
            new ContentMenuItem{ Name = "annotations", LabelId = "MenuLabel-Annotations"}
        };

        private BasePrintPage PrintUtil = new BasePrintPage();
        private Point ContentMenuPosition = new Point(120, 200);
        public List<ContentMenuItem> MenuList
        {
            get { return MENU_LIST; }
        }

        private string selectedMenu = "toc";

        public string SelectedMenu
        {
            get { return selectedMenu; }
            set { selectedMenu = value; }
        }

        public ContentPage()
        {
            InitializeComponent();
            Loaded += ContentPageLoaded;
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            PrintUtil.RegisterForPrinting();
        }

        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            PrintUtil.UnregisterForPrinting();
        }

        private void ContextMenuInitial(Point p, double padding)
        {
            ContextMenu Menu = new ContextMenu();
            var AnnotationModel = new AnnotationViewModel();
            Menu.DataContext = AnnotationModel;
            AnnotationModel.UpdateTagCollection += (tagItem, tagEnum) =>
                {
                    var mode = DataContext as ContentPageViewModel;
                    mode.UpdateTagCollections(tagItem, tagEnum);
                };
            AnnotationModel.AddAnnotation += async (annStatue) =>
                {
            
                    switch (annStatue)
                    {
                        case AnnotationStatue.Highlight:
                            await InvokeScript(ContentWebView, "insertTagAtRange();");
                            break;
                        case AnnotationStatue.Note:
                            await InvokeScript(ContentWebView, "insertNodeAtRange();");
                            break;
                    }
                    await InvokeScript(ContentWebView, "highlightSelectedText();");
                };
            Menu.ContextMenuClick += (s) =>
                {
                    switch (s)
                    {
                        case ContextMenuStatue.LegalDefine:
                            var contentPageViewModel = DataContext as ContentPageViewModel;
                            double PaddingLeft = LeftSidePanelGrid.Visibility == Visibility.Visible ? 368 : 48;
                            string countryCode= HtmlHelper.GetTOCCountryCode(contentPageViewModel.SelectedContent);
                            DictionaryDialog DictinaryMenu = new DictionaryDialog(countryCode, ContentMenuPosition, PaddingLeft);
                            DictinaryMenu.Show(SelectContent);

                            break;
                        case ContextMenuStatue.Copy:
                            HtmlHelper.Copy(SelectContent);               
                            break;
                    }
                };
            Menu.Show(p, padding);
        }

        void ContentPageLoaded(object sender, RoutedEventArgs e)
        {
            LoadLeftSideContent(selectedMenu);
            var context = DataContext as ContentPageViewModel;
            SizeChanged += (s, arg) =>
            {
                HistoryPanel.Height = SearchResultPanel.Height = this.ActualHeight;
                HistoryPanel.Width = SearchResultPanel.Width = this.ActualWidth;
            };
        }

        void LoadLeftSideContent(string menuName)
        {
            MenuListView.SelectedValue = menuName;
            switch (menuName)
            {
                case "toc":
                    {
                        TocContainerGrid.Visibility = Visibility.Visible;
                        IndexContainerGrid.Visibility = Visibility.Collapsed;
                        AnnotationsContainerGrid.Visibility = Visibility.Collapsed;
                        ContentPanel.Visibility = Visibility.Visible;
                        break;
                    }
                case "index":
                    {
                        TocContainerGrid.Visibility = Visibility.Collapsed;
                        IndexContainerGrid.Visibility = Visibility.Visible;
                        AnnotationsContainerGrid.Visibility = Visibility.Collapsed;
                        ContentPanel.Visibility = Visibility.Collapsed;
                        break;
                    }
                case "annotations":
                    {
                        TocContainerGrid.Visibility = Visibility.Collapsed;
                        IndexContainerGrid.Visibility = Visibility.Collapsed;
                        AnnotationsContainerGrid.Visibility = Visibility.Visible;
                        ContentPanel.Visibility = Visibility.Visible;
                        break;
                    }
            }
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            if (pageState == null) return;
            base.SaveState(pageState);
            pageState["CurrentMenu"] = selectedMenu;
        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            if (pageState == null) return;

            base.LoadState(navigationParameter, pageState);

            if (pageState.ContainsKey("CurrentMenu"))
            {
                selectedMenu = pageState["CurrentMenu"].ToString();
            }
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

        private bool Navigating = false;
        private string SelectContent { set; get; }
        private async void ScriptNotify(object sender, NotifyEventArgs e)
        {
            var notify = JsonConvert.DeserializeObject<NotifytModel>(e.Value);
            var contentPageViewModel = DataContext as ContentPageViewModel;
            switch (notify.Type)
            {
                case "POSITION":
                    ContentMenuPosition = HtmlHelper.ConvertToPoint(notify.Value1, notify.Value2);
                    break;
                case "SELECTED":
                    string text = notify.Value1;
                    SelectContent = SearchTools.RemoveBlank(text);
                    double PaddingLeft = LeftSidePanelGrid.Visibility == Visibility.Visible ? 368 : 48;
                    ContextMenuInitial(ContentMenuPosition, PaddingLeft);
                    break;
                case "INFINITE":
                    if (Navigating)
                        return;
                    await Task.Run(() => { Navigating = true; });
                    WebView webView = sender as WebView;

                    switch (notify.Value1)
                    {
                        case "UP":
                            var last = contentPageViewModel.GetBackwardNode();
                            if (last == null)
                                break;
                            contentPageViewModel.LoadingContent(last.Title, RollingDirection.Up);
                            await InvokeScript(webView, "loadingupvisible();");
                            string backwardContent = await contentPageViewModel.GoBackWardContent(last);
                            var recordUP = new NavigationRecord
                            {
                                TOCId = last.ID,
                                Type = NavigationType.TOCDocument
                            };
                            backwardContent = await HtmlHelper.PageSplit(backwardContent, recordUP);
                            string jsonUp = JsonConvert.SerializeObject(new { content = backwardContent, selector = string.Format("#tocId{0}", last.ID) });
                            await InvokeScript(webView, "AppendHtml_Top(" + jsonUp + ");");
                            contentPageViewModel.LoadingCompleted();
                            await InvokeScript(webView, string.Format("ListenHref('#tocId{0} a');", last.ID));
                            break;
                        case "DOWN":
                            var next = contentPageViewModel.GetForwardNode();
                            if (next == null)
                                break;
                            contentPageViewModel.LoadingContent(next.Title, RollingDirection.Down);
                            await InvokeScript(webView, "loadingdownvisible();");
                            string forwardContent = await contentPageViewModel.GoForWardContent(next);
                            var recordDown = new NavigationRecord
                            {
                                TOCId = next.ID,
                                Type = NavigationType.TOCDocument
                            };
                            forwardContent = await HtmlHelper.PageSplit(forwardContent, recordDown);
                            string jsonDowm = JsonConvert.SerializeObject(new { content = forwardContent, selector = string.Format("#tocId{0}", next.ID) });
                            await InvokeScript(webView, "AppendHtml_Bottom(" + jsonDowm + ");");
                            contentPageViewModel.LoadingCompleted();
                            await InvokeScript(webView, string.Format("ListenHref('#tocId{0} a');", next.ID));
                            break;
                    }
                    //avoid scroll event hannpens twice
                    Task.Run(() =>
                    {
                        using (ManualResetEvent manualResetEvent = new ManualResetEvent(false))
                        {
                            manualResetEvent.WaitOne(1000);
                            Navigating = false;
                        }
                    });
                    break;
                case "SCROLL":
                    int tocId = int.Parse(notify.Value1.Substring(5));
                    await contentPageViewModel.ResetTocByScroll(tocId);
                    break;
                case "HREF":
                    if (notify.Value1.StartsWith("about:"))
                    {
                        notify.Value1 = notify.Value1.Replace("about:", "http://");
                    }
                    contentPageViewModel.LinkAnalyze(notify.Value1);
                    break;
                case "PBO":
                    string pageNum = notify.Value1;
                    contentPageViewModel.PageNum = "Page " + pageNum;
                    break;
                case "COUNTRYCODE":
                    break;
            }
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            var navigationService = IoCContainer.Instance.ResolveInterface<INavigationService>();
            navigationService.Navigate("Publications", "");
        }

        #region history
        private void CollapseHistoryPanel(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            HistoryPopPanel.IsOpen = false;
        }
        private void CollapseHistoryPanel(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            HistoryPopPanel.IsOpen = false;
        }
        private void RecentHistorySwitch(object sender, RoutedEventArgs e)
        {
            HistoryPopPanel.IsOpen = true;
        }

        private async void RecentHistoryItemClick(object sender, ItemClickEventArgs e)
        {
            ContentPageViewModel contentPageViewModel = DataContext as ContentPageViewModel;
            var item = e.ClickedItem as RecentHistoryItem;
            if (contentPageViewModel != null && item != null)
            {
                await contentPageViewModel.RecentHistoryToTOC(item);
            }
        }
        #endregion

        #region search
        private void CollapseSearchPanel(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SearchResultPopup.IsOpen = false;
        }
        private void CollapseSearchPanel(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            SearchResultPopup.IsOpen = false;
        }

        private void KeepSearchPanel(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void KeepSearchPanel(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
        }

        private async void HighlightSearch(object sender, ItemClickEventArgs e)
        {
            var searchDisplayResult = e.ClickedItem as SearchResultModel;
            var contentPageViewModel = DataContext as ContentPageViewModel;
            if (searchDisplayResult != null && contentPageViewModel != null)
            {
                await contentPageViewModel.SearchResultToToc(searchDisplayResult);
                LoadLeftSideContent("toc");
            }
        }

        private void GotoPage(object sender, ItemClickEventArgs e)
        {
            var searchDisplayResult = e.ClickedItem as SearchPageModel;
            var contentPageViewModel = DataContext as ContentPageViewModel;
            contentPageViewModel.GotoPage(searchDisplayResult);
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            var keyWords = SearchTextBox.Text.Trim();
            var contentPageViewModel = DataContext as ContentPageViewModel;
            if (!string.IsNullOrEmpty(keyWords) && contentPageViewModel != null)
            {
                contentPageViewModel.SearchDocument(keyWords);
            }
        }

        private async void SearchPageButtonClick(object sender, RoutedEventArgs e)
        {
            string numString = SearchPageBox.Text.Trim();
            var contentPageViewModel = DataContext as ContentPageViewModel;
            int num = 0;
            if (int.TryParse(numString, out num) && contentPageViewModel != null)
            {
                bool isEmpty = await contentPageViewModel.SearchPage(num);
                SearchPageTitle.Text = isEmpty ? "Page Not Found" : "Page Results";
            }
        }

        private void SearchByEnter(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && e.KeyStatus.RepeatCount == 0)
            {
                SearchButtonClick(null, null);
            }
        }

        private void SearchPageByEnter(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (!e.Key.ToString().Contains("Number"))
            {
                if (e.Key == VirtualKey.Enter && e.KeyStatus.RepeatCount == 0)
                {
                    SearchPageButtonClick(null, null);
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        private void KeyWordChanged(object sender, TextChangedEventArgs e)
        {
            var keyWords = SearchTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(keyWords))
            {
                var contentPageViewModel = DataContext as ContentPageViewModel;
                contentPageViewModel.ClearKeywords();
            }
        }

        private void PageNumChanged(object sender, TextChangedEventArgs e)
        {
            var num = SearchPageBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(num))
            {
                var contentPageViewModel = DataContext as ContentPageViewModel;
                contentPageViewModel.ClearPageNum();
                SearchPageTitle.Text = "Go to Page";
            }
        }

        private void PopupResultPanel(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!SearchResultPopup.IsOpen)
            {
                SearchResultPopup.IsOpen = true;
            }
        }

        private void PageSearchPanelOpen(object sender, object e)
        {
            var contentPageViewModel = DataContext as ContentPageViewModel;
            contentPageViewModel.IspageResultShow = false;
            SearchPageBox.Text = string.Empty;
            SearchPageTitle.Text = "Go to Page";
        }

        #endregion

        private async void TOC_Loaded(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            WebView webView = sender as WebView;
            var contentPageViewModel = DataContext as ContentPageViewModel;
            if (webView != null && contentPageViewModel != null)
            {
                var currentRecord = contentPageViewModel.HistoryNavigator.GetCurrentRecord();
                string selector = string.Format("#tocId{0}", currentRecord.TOCId);
                await InvokeScript(webView, "OnLoad('" + selector + "'," + currentRecord.PageNum + ");");
                switch (currentRecord.Type)
                {
                    case NavigationType.Search:
                        var searchResult = currentRecord.Tag as SearchResultModel;
                        string keyWords = string.Join(" ", searchResult.Keywords);
                        ResourceLoader RESOURCE_LOADER = new ResourceLoader();
                        if (searchResult.Type == RESOURCE_LOADER.GetString("Document"))
                        {
                            string elementTostr = searchResult.HeadType;
                            int headIndex = searchResult.HeadIndex;
                            string document = JsonConvert.SerializeObject(new { keyword = keyWords, isDocument = true, element = elementTostr, index = headIndex });
                            await InvokeScript(webView, "ScrollToHighlight(" + document + ");");
                        }
                        else if (searchResult.Type == RESOURCE_LOADER.GetString("Publication"))
                        {
                            string publication = JsonConvert.SerializeObject(new { keyword = keyWords, isDocument = false });
                            await InvokeScript(webView, "ScrollToHighlight(" + publication + ");");
                        }
                        break;
                    case NavigationType.InternalLink:
                    case NavigationType.IntraLink:
                        var refpt = currentRecord.Tag as string;//ScrollToLinkContent
                        await InvokeScript(webView, "ScrollToLinkContent('" + refpt + "');");
                        break;
                }
                if (contentPageViewModel.IsPBO)
                {
                    await InvokeScript(webView, "ListenCurrentPageNum();");
                }
                LoadLeftSideContent("toc");
            }
        }


        private async void Index_Loaded(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            WebView webView = sender as WebView;
            var contentPageViewModel = DataContext as ContentPageViewModel;
            if (webView != null && contentPageViewModel != null)
            {
                await InvokeScript(webView, "GotoElementWithIndex('" + contentPageViewModel.SelectIndexTitle + "');");
            }
        }

        private async Task InvokeScript(WebView webView, string request)
        {
            await webView.InvokeScriptAsync("eval", new string[] { request });
        }

        private async void PrintContent(object sender, RoutedEventArgs e)
        {
            //var contentPageViewModel = DataContext as ContentPageViewModel;
            //string selector = string.Format("#tocId{0}", contentPageViewModel.CurrentTocNode.ID);
            //string id = string.Format("tocId{0}", contentPageViewModel.CurrentTocNode.ID);
            //await ContentWebView.InvokeScriptAsync("eval", new string[] { "ScollToPageStart(\"" + id + "\");" });
            //await ContentWebView.InvokeScriptAsync("eval", new string[] { "SavePageToLocal(\"" + selector + "\");" });

            //await PrintUtil.GetWebPages();
            //await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();
        }

        private async void SaveAsPDF(object sender, RoutedEventArgs e)
        {
            //var contentPageViewModel = DataContext as ContentPageViewModel;

            //string html = "<html>" + contentPageViewModel.SelectedContent + "</html>";

            //await PdfUtil.SaveAsPdf(html, "Cross on Evidence / Chapter 1 / Introduction");    q
            //ShareBase shareUtil = new ShareBase();
            //shareUtil.DTTransferManager.DataRequested += async(s, args) =>
            //    {
            //        DataPackage requestData = args.Request.Data;
            //        requestData.Properties.Title = "LexisNexis Document";
            //        requestData.Properties.Description = "Share a PDF File"; // The description is optional.
            //        Windows.Storage.StorageFile file = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("windows-pdf.pdf");
            //        List<StorageFile> pickedFile = new List<StorageFile>();
            //        pickedFile.Add(file);
            //        requestData.SetStorageItems(pickedFile);
            //    };
            //shareUtil.ShowShareUI();

            //Start-Process -FilePath "C:\file.pdf" –Verb Print
            //using (PowerShell PowerShellInstance = PowerShell.Create())
            //{
            //    // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
            //    // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.
            //    string script = "Start-Process -FilePath \"C:\file.pdf\" –Verb Print";
            //    PowerShellInstance.AddScript(script);
            //    PowerShellInstance.Invoke();

            //    // use "AddParameter" to add a single parameter to the last command/script on the pipeline.
            //    //PowerShellInstance.AddParameter("param1", "parameter 1 value!");
            //}

        }


    }

}
