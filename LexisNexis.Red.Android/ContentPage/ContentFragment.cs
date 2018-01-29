
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Droid.Widget.Layout;
using LexisNexis.Red.Droid.WebViewUtility;
using System.Threading.Tasks;
using LexisNexis.Red.Common.BusinessModel;
using System.IO;
using System.Text;
//using LexisNexis.Red.Common.Business.Pdf;
using System.Threading;
using Android.Graphics;
using System.Collections.Generic;
using HtmlAgilityPack;
using Android.Print;
//using Com.Lexisnexis.Printext;
using LexisNexis.Red.Droid.PrintUtility;
using Uri = Android.Net.Uri;

namespace LexisNexis.Red.Droid.ContentPage
{
    public class ContentFragment : Fragment, IExpandableRightPanel, IActionModeTarget
    {
        public const string FragmentTag = "ContentFragment";
        private const string PdfFileName = "LexisNexis_Document.pdf";

        private ImageView ivPreviousPage;
        private ImageView ivNextPage;
        private TextView tvGoToPage;
        private TextView tvPboPageNumLabel;
        private TextView tvPboPageNum;
        private ImageView ivExpand;
        private FrameLayout flWebViewContainer;
        private BoundedFrameLayout bflContentContainer;
        private WebViewExt wvContent;
        private WebViewExt printingWebView;

        private LinearLayout llTopLoadingIndicator;
        private TextView tvTopLoadingTocTitle;
        private LinearLayout llBottomLoadingIndicator;
        private TextView tvBottomLoadingTocTitle;

        private string currentPboPage;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = inflater.Inflate(Resource.Layout.contentpage_content_fragment, container, false);

            if (((ContentActivity)Activity).Publication == null)
            {
                return v;
            }

            ivExpand = v.FindViewById<ImageView>(Resource.Id.ivExpand);
            ivExpand.Click += OnIvExpandClick;

            ivPreviousPage = v.FindViewById<ImageView>(Resource.Id.ivPreviousPage);
            ivNextPage = v.FindViewById<ImageView>(Resource.Id.ivNextPage);

            tvGoToPage = v.FindViewById<TextView>(Resource.Id.tvGoToPage);
            tvPboPageNumLabel = v.FindViewById<TextView>(Resource.Id.tvPboPageNumLabel);
            tvPboPageNum = v.FindViewById<TextView>(Resource.Id.tvPboPageNum);

            bflContentContainer = v.FindViewById<BoundedFrameLayout>(Resource.Id.bflContentContainer);
            bflContentContainer.SetBackgroundResource(Resource.Drawable.miscinfo_frame_background);

            flWebViewContainer = v.FindViewById<FrameLayout>(Resource.Id.flWebViewContainer);

            llTopLoadingIndicator = v.FindViewById<LinearLayout>(Resource.Id.llTopLoadingIndicator);
            tvTopLoadingTocTitle = v.FindViewById<TextView>(Resource.Id.tvTopLoadingTocTitle);
            llBottomLoadingIndicator = v.FindViewById<LinearLayout>(Resource.Id.llBottomLoadingIndicator);
            tvBottomLoadingTocTitle = v.FindViewById<TextView>(Resource.Id.tvBottomLoadingTocTitle);

            tvPboPageNumLabel.Text = tvPboPageNumLabel.Text + " ";

            llTopLoadingIndicator.Visibility = ViewStates.Gone;
            llBottomLoadingIndicator.Visibility = ViewStates.Gone;

            ivPreviousPage.Click += delegate
            {
                var mainFragmentStatus = ((ContentActivity)Activity).GetMainFragment().MainFragmentStatus;
                if (!NavigationManagerHelper.CanBack(mainFragmentStatus))
                {
                    return;
                }

                // ToDo: Use DataCache.INSTATNCE.Toc.GetNavigationItem to find ContentBrowserRecord
                var record = NavigationManager.Instance.CurrentRecord as ContentBrowserRecord;
                if (record != null)
                {
                    record.WebViewScrollPosition = wvContent.ScrollY;
                }

                var fromBookId = NavigationManagerHelper.GetCurrentBookId();
                NavigationManagerHelper.Back(mainFragmentStatus);
                ((ContentActivity)Activity).GetMainFragment().NavigateTo(fromBookId);
            };

            ivNextPage.Click += delegate
            {
                var mainFragmentStatus = ((ContentActivity)Activity).GetMainFragment().MainFragmentStatus;
                if (!NavigationManagerHelper.CanForth(mainFragmentStatus))
                {
                    return;
                }

                var record = NavigationManager.Instance.CurrentRecord as ContentBrowserRecord;
                if (record != null)
                {
                    record.WebViewScrollPosition = wvContent.ScrollY;
                }

                var fromBookId = NavigationManagerHelper.GetCurrentBookId();
                NavigationManagerHelper.Forth(mainFragmentStatus);
                ((ContentActivity)Activity).GetMainFragment().NavigateTo(fromBookId);
            };

            tvGoToPage.Click += delegate
            {
                var goToPageDialogFragment = GoToPageDialogFragment.NewInstance();
                goToPageDialogFragment.Show(FragmentManager.BeginTransaction(), "goToPageDialogFragment");
            };

            //UpdatePboPage();
            //((ContentActivity)Activity).GetMainFragment().SetLeftPanelStatus();

            return v;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);

            if (hidden)
            {
                // Hidden
                if (wvContent != null)
                {
                    WebViewManager.Instance.ReleaseWebView(
                        WebViewManager.WebViewType.Content, flWebViewContainer);
                    wvContent = null;
                }
            }
            else
            {
                // Show
                if (wvContent == null)
                {
                    wvContent = WebViewManager.Instance.RequestWebView(
                        WebViewManager.WebViewType.Content,
                        flWebViewContainer,
                        OnPageLoaded,
                        OnGetSelectedText,
                        OnLoadUrl,
                        OnWebOverScroll,
                        OnScrollLoadPageCompleted,
                        OnScrollToPage);
                    OpenContentPage();
                }

                //SetExpandableStatus();
                //UpdateNavigationIcon();
            }
        }

        public override void OnStop()
        {
            if (wvContent != null)
            {
                WebViewManager.Instance.ReleaseWebView(
                    WebViewManager.WebViewType.Content, flWebViewContainer);
                wvContent = null;
            }

            base.OnStop();
        }

        public override void OnResume()
        {
            base.OnResume();

            if (((ContentActivity)Activity).Publication == null)
            {
                return;
            }

            if (wvContent == null)
            {
                wvContent = WebViewManager.Instance.RequestWebView(
                    WebViewManager.WebViewType.Content,
                    flWebViewContainer,
                    OnPageLoaded,
                    OnGetSelectedText,
                    OnLoadUrl,
                    OnWebOverScroll,
                    OnScrollLoadPageCompleted,
                    OnScrollToPage);
                OpenContentPage();
                if (wvContent.IsLoadingPage)
                {
                    if (wvContent.LoadingPage == WebViewExt.ScrollOverLoadingPage.Top)
                    {
                        llTopLoadingIndicator.Visibility = ViewStates.Visible;
                        llTopLoadingIndicator.Alpha = 1.0f;
                    }
                    else
                    {
                        llBottomLoadingIndicator.Visibility = ViewStates.Visible;
                        llBottomLoadingIndicator.Alpha = 1.0f;
                    }
                }
            }

            //if (printingWebView != null && printingWebView.Tag == null)
            //{
            //    // printingWebView has been served for physical printer;
            //    WebViewManager.Instance.ClearWebViewStatus(WebViewManager.WebViewType.Printing);
            //    printingWebView = null;
            //}

            //UpdateNavigationIcon();
        }

        private void OnPageLoaded()
        {
            LogHelper.Debug("dbg", "RotateWait::Page loaded event receive, Close Wait");
            /* // The functionality is not needed now.
			var nav = contentFragmentStatus.GetCurrentNavigation();
			if(nav.WebViewScrollPosition != 0)
			{
				wvContent.ScrollTo(0, nav.WebViewScrollPosition);
			}
			*/

            ((ContentActivity)Activity).ClosePleaseWaitPageLoadDialog();
        }

        private void OnGetSelectedText(string purpose, string text, float left, float top, float right, float bottom)
        {
            //if ("copy" == purpose.ToLower())
            //{
            //    text = string.Join(
            //        " ",
            //        text.Split(new string[] { "\n", "\t", "\r", " " }, StringSplitOptions.RemoveEmptyEntries));
            //    var clipboardManager = (ClipboardManager)Activity.GetSystemService(Context.ClipboardService);
            //    clipboardManager.PrimaryClip = ClipData.NewPlainText(null, text);
            //    Toast.MakeText(Activity, "Copied", ToastLength.Short).Show();
            //}
            //else if ("legaldefine" == purpose.ToLower())
            //{
            //    var locations = new int[2];
            //    flWebViewContainer.GetLocationInWindow(locations);

            //    ((ContentActivity)Activity).ShowLegalDefinePopup(
            //        text,
            //        new Rect(
            //            locations[0] + Conversion.Dp2Px((int)left),
            //            locations[1] + Conversion.Dp2Px((int)top),
            //            locations[0] + Conversion.Dp2Px((int)right),
            //            locations[1] + Conversion.Dp2Px((int)bottom)));
            //}
        }

        private void OnLoadUrl(Hyperlink url)
        {
            ((ContentActivity)Activity).GetMainFragment().LoadUrl(url);
        }

        private const int threshold = 10;

        private void OnWebOverScroll(int arg1, float arg2)
        {
            if (wvContent.IsLoadingPage)
            {
                return;
            }

            var pub = ((ContentActivity)Activity).Publication;
            var tag = wvContent.Tag as JavaObjWrapper<WebViewTag>;
            //tag.Value.DebugList();
            //Log.Debug("dbg", "Scroll over event, delta = " + arg1);
            //if (arg1 >= threshold)
            //{
            //    // Over bottom
            //    Log.Debug("dbg", "Scroll over bottom.");
            //    var maxNodeId = tag.Value.GetMaxTOCId(pub.Value.BookId);
            //    var maxNode = PublicationContentUtil.Instance.GetTOCByTOCId(maxNodeId, DataCache.INSTATNCE.Toc.RootTocNode);
            //    if (DataCache.INSTATNCE.Toc.IsLastPage(maxNode))
            //    {
            //        Log.Debug("dbg", "[Reach last page]Current page is:[" + maxNode.ID + "]" + maxNode.Title);
            //        return;
            //    }

            //    wvContent.StarLoadingPage(WebViewExt.ScrollOverLoadingPage.Bottom, false);

            //    var nextNode = PublicationContentUtil.Instance.GetNextPageByTreeNode(maxNode);

            //    Log.Debug("dbg", "Over bottom, load: " + nextNode.ID);

            //    llBottomLoadingIndicator.Visibility = ViewStates.Visible;
            //    llBottomLoadingIndicator.Alpha = 0;
            //    tvBottomLoadingTocTitle.Text = nextNode.Title;
            //    llBottomLoadingIndicator.Animate().SetStartDelay(200).Alpha(1);

            //    wvContent.LoadUrl("javascript:android.red.showbottomloading('" + nextNode.Title + "');");

            //    //*
            //    Task.Run(() =>
            //    {
            //        Thread.Sleep(100);
            //        var nextPage = AsyncHelpers.RunSync<string>(
            //            () => PublicationContentUtil.Instance.GetContentFromTOC(pub.Value.BookId, nextNode, false));

            //        Application.SynchronizationContext.Post(_ =>
            //        {
            //            AsyncUIOperationRepeater.INSTATNCE.SubmitAsyncUIOperation(
            //                (IAsyncTaskActivity)Activity,
            //                a =>
            //            {
            //                var nextPageDivId = WebViewManager.GetTocDivId(nextNode.ID);
            //                nextPage = "<div id='" + nextPageDivId + "' class='tocpagediv' >" + nextPage + "</div>";
            //                string longStringParamId = RedController.AddLongStringParam(nextPage);
            //                wvContent.LoadUrl("javascript:android.red.appendpage('" + longStringParamId + "', '" + nextPageDivId + "');");
            //            });
            //        }, null);
            //    });
            //    //*/
            //}
            //else if (arg1 <= -1 * threshold)
            //{
            //    // Over top
            //    Log.Debug("dbg", "Scroll over top.");

            //    var minNodeId = tag.Value.GetMinTOCId(pub.Value.BookId);
            //    var minNode = PublicationContentUtil.Instance.GetTOCByTOCId(minNodeId, DataCache.INSTATNCE.Toc.RootTocNode);
            //    if (DataCache.INSTATNCE.Toc.IsFirstPage(minNode))
            //    {
            //        Log.Debug("dbg", "[Reach 1st page]Current page is:[" + minNode.ID + "]" + minNode.Title);
            //        return;
            //    }

            //    wvContent.StarLoadingPage(WebViewExt.ScrollOverLoadingPage.Top, true);

            //    var previousNode = PublicationContentUtil.Instance.GetPreviousPageByTreeNode(minNode);
            //    Log.Debug("dbg", "Over top, load: " + previousNode.ID);

            //    llTopLoadingIndicator.Visibility = ViewStates.Visible;
            //    llTopLoadingIndicator.Alpha = 0;
            //    tvTopLoadingTocTitle.Text = previousNode.Title;
            //    llTopLoadingIndicator.Animate().SetStartDelay(200).Alpha(1);

            //    wvContent.LoadUrl("javascript:android.red.showtoploading('" + previousNode.Title + "');");

            //    //*
            //    Task.Run(() =>
            //    {
            //        Thread.Sleep(100);
            //        var previousPage = AsyncHelpers.RunSync<string>(
            //            () => PublicationContentUtil.Instance.GetContentFromTOC(pub.Value.BookId, previousNode, false));

            //        Application.SynchronizationContext.Post(_ =>
            //        {
            //            AsyncUIOperationRepeater.INSTATNCE.SubmitAsyncUIOperation(
            //                (IAsyncTaskActivity)Activity,
            //                a =>
            //            {
            //                var previousPageDivId = WebViewManager.GetTocDivId(previousNode.ID);
            //                previousPage = "<div id='" + previousPageDivId + "' class='tocpagediv' >" + previousPage + "</div>";
            //                string longStringParamId = RedController.AddLongStringParam(previousPage);
            //                wvContent.LoadUrl("javascript:android.red.prependpage('" + longStringParamId + "', '" + previousPageDivId + "');");
            //            });
            //        }, null);
            //    });
                //*/
           // }
        }

        private void OnScrollLoadPageCompleted(string pageId)
        {
            //llTopLoadingIndicator.Visibility = ViewStates.Gone;
            //tvTopLoadingTocTitle.Text = "";
            //llBottomLoadingIndicator.Visibility = ViewStates.Gone;
            //tvBottomLoadingTocTitle.Text = "";
        }

        private void OnScrollToPage(string pageId, string pboPage)
        {
            //var pub = ((ContentActivity)Activity).Publication;
            //var tocId = WebViewManager.ExtractTocIdFromDivId(pageId);
            //if (pboPage != null)
            //{
            //    if (string.IsNullOrEmpty(pboPage))
            //    {
            //        var pboPageItem = AsyncHelpers.RunSync<PageItem>(
            //            () => PageSearchUtil.Instance.GetFirstPageItem(pub.Value.BookId, tocId));
            //        currentPboPage = pboPageItem != null ? pboPageItem.Identifier.ToString() : null;
            //    }
            //    else
            //    {
            //        currentPboPage = pboPage;
            //    }

            //    UpdatePboPage();
            //}
            //else
            //{
            //    var tag = wvContent.Tag as JavaObjWrapper<WebViewTag>;
            //    if (!tag.Value.IsCurrentTOC(pub.Value.BookId, tocId))
            //    {
            //        tag.Value.SetCurrentToc(
            //            pub.Value.BookId,
            //            tocId);

            //        DataCache.INSTATNCE.Toc.SetCurrentTOCNodeById(tocId);
            //        ((ContentActivity)Activity).GetMainFragment().Refresh();
            //    }
            //}
        }

        private void OnIvExpandClick(object sender, EventArgs e)
        {
           // ((ContentActivity)Activity).GetMainFragment().SwitchLeftPanelStatus();
        }

        public void SetExpandableStatus()
        {
            //if (bflContentContainer == null)
            //{
            //    // OnCreateView did not called.
            //    return;
            //}

            //var mainFragmentStatus = ((ContentActivity)Activity).GetMainFragment().MainFragmentStatus;
            //if (mainFragmentStatus.LeftPanelOpen)
            //{
            //    //bflContentContainer.SetBackgroundColor(Color.White);
            //    ivExpand.SetImageResource(Resource.Drawable.expand_content_view);
            //}
            //else
            //{
            //    //bflContentContainer.SetBackgroundResource(Resource.Drawable.miscinfo_frame_background);
            //    ivExpand.SetImageResource(Resource.Drawable.collapse_content_view);
            //}
        }

        public void OpenContentPage()
        {
        
            if (IsHidden)
            {
                // This fragment is hidden, need not open page
                return;
            }

            if (wvContent == null   // WebView does not requested
                || DataCache.INSTATNCE.Toc == null  // Toc has not been retrieved
                )
            {
                return;
            }

      

            var pub = ((ContentActivity)Activity).Publication;
            if (!DataCache.INSTATNCE.Toc.IsCurrentPublication(pub.Value.BookId))
            {
                // The toc is not belong to current publication
                return;
            }

            var tocNode = DataCache.INSTATNCE.Toc.ShowingLeafNode;
            var tag = wvContent.Tag as JavaObjWrapper<WebViewTag>;
            if (tag != null && tag.Value.IsCurrentTOC(pub.Value.BookId, tocNode.ID))
            {
                /*
				wvContent.Tag = new JavaObjWrapper<WebViewTag>(
					WebViewTag.CreateWebViewTagByTOC(
						pub.Value.BookId,
						tocNode.ID));
				*/

                ((ContentActivity)Activity).ClosePleaseWaitPageLoadDialog();
                UpdateNavigationIcon();

                if (!tag.Value.IsCurrentNavigationItem())
                {
                    wvContent.LoadUrl("javascript:android.red.applyHighLight();android.red.scrollByNavigation();");
                    tag.Value.BindNavigationItem();

                    if (((ContentActivity)Activity).IsPbo())
                    {
                        var pboPageItem = AsyncHelpers.RunSync<PageItem>(
                            () => PageSearchUtil.Instance.GetFirstPageItem(pub.Value.BookId, tocNode.ID));
                        currentPboPage = pboPageItem != null ? pboPageItem.Identifier.ToString() : null;
                    }

                    //UpdatePboPage();
                }

                return;
            }

            var result = AsyncHelpers.RunSync<string>(
                () => PublicationContentUtil.Instance.GetContentFromTOC(pub.Value.BookId, tocNode, true, false));

            //result = SlideToc(result);

            // Only large content need show wait dialog
            if (result.Length > WebViewManager.ShowWaitContentMinLength
                || WebViewManager.CountATag(result) > WebViewManager.ShowWaitMinATagCount)
            {
                ((ContentActivity)Activity).ShowPleaseWaitDialog();
                Task.Run(() =>
                {
                    Thread.Sleep(100);

                    result = PublicationContentUtil.Instance.RenderHyperLink(result, pub.Value.BookId);
                    result = "<div id='toc_" + tocNode.ID + "' class='tocpagediv' >" + result + "</div>";

                    Application.SynchronizationContext.Post(_ =>
                    {
                        tag = new JavaObjWrapper<WebViewTag>(
                            WebViewTag.CreateWebViewTagByTOC(
                                pub.Value.BookId,
                                tocNode.ID));
                        tag.Value.BindNavigationItem();
                        wvContent.Tag = tag;

                        wvContent.LoadDataWithBaseURL(
                            "file:///android_asset/html/",
                            WebViewManager.Instance.ApplyTemplate(WebViewManager.WebViewType.Content, result),
                            "text/html",
                            "utf-8",
                            null);

                        if (((ContentActivity)Activity).IsPbo())
                        {
                            var pboPageItem = AsyncHelpers.RunSync<PageItem>(
                                () => PageSearchUtil.Instance.GetFirstPageItem(pub.Value.BookId, tocNode.ID));
                            currentPboPage = pboPageItem != null ? pboPageItem.Identifier.ToString() : null;
                        }

                        UpdatePboPage();
                        UpdateNavigationIcon();
                    }, null);
                });

                return;
            }

            //result = PublicationContentUtil.Instance.RenderHyperLink(result, pub.Value.BookId);
            //result = "<div id='toc_" + tocNode.ID + "' class='tocpagediv' >" + result + "</div>";



            //DumpToc(pub.Value.BookId, tocNode.ID, WebViewManager.Instance.ApplyTemplate(WebViewManager.WebViewType.Content, result));

            //tag = new JavaObjWrapper<WebViewTag>(
            //    WebViewTag.CreateWebViewTagByTOC(
            //        pub.Value.BookId,
            //        tocNode.ID));
            //tag.Value.BindNavigationItem();
            //wvContent.Tag = tag;

            //wvContent.LoadDataWithBaseURL(
            //    "file:///android_asset/html/",
            //    WebViewManager.Instance.ApplyTemplate(WebViewManager.WebViewType.Content, result),
            //    "text/html",
            //    "utf-8",
            //    null);

            //if (((ContentActivity)Activity).IsPbo())
            //{
            //    var pboPageItem = AsyncHelpers.RunSync<PageItem>(
            //        () => PageSearchUtil.Instance.GetFirstPageItem(pub.Value.BookId, tocNode.ID));
            //    currentPboPage = pboPageItem != null ? pboPageItem.Identifier.ToString() : null;
            //}

            //UpdatePboPage();
            //UpdateNavigationIcon();
        }

    
        public void ResetNavigation()
        {
            UpdateNavigationIcon();
        }

        public void DoActionMode(int actionId)
        {
            switch (actionId)
            {
                case Resource.Id.actionContentAnnotate:
                    {

                    }
                    return;
                case Resource.Id.actionContentLegalDefine:
                    {
                        wvContent.LoadUrl("javascript:android.red.getSelection('LegalDefine', true);");
                    }
                    return;
                case Resource.Id.actionContentCopy:
                    {
                        wvContent.LoadUrl("javascript:android.red.getSelection('Copy');");
                    }
                    return;
                default:
                    throw new InvalidOperationException("Unknown action mode.");
            }
        }

        public void AfterDoActionMode(int actionId)
        {
            //switch (actionId)
            //{
            //    case Resource.Id.actionContentAnnotate:
            //        return;
            //    case Resource.Id.actionContentLegalDefine:
            //        {
            //            //*
            //            Task.Run(() =>
            //            {
            //                Thread.Sleep(500);
            //                Application.SynchronizationContext.Post(_ =>
            //                    wvContent.LoadUrl("javascript:android.red.restoreSelection();"), null);
            //            });
            //            //*/
            //            //wvContent.LoadUrl("javascript:android.red.restoreSelection();");
            //        }
            //        return;
            //    case Resource.Id.actionContentCopy:
            //        return;
            //    default:
            //        throw new InvalidOperationException("Unknown action mode.");
            //}
        }

        public void OnUserDismissLegalDefinePopup()
        {
            wvContent.LoadUrl("javascript:android.red.removeSelection();");
        }

        private void UpdateNavigationIcon()
        {
            //var mainFragmentStatus = ((ContentActivity)Activity).GetMainFragment().MainFragmentStatus;
            //if(!NavigationManagerHelper.CanBack(mainFragmentStatus))
            //{
            //	ivPreviousPage.SetImageResource(Resource.Drawable.previous_page_disable);
            //}
            //else
            //{
            //	ivPreviousPage.SetImageResource(Resource.Drawable.previous_page_activy);
            //}

            //if(!NavigationManagerHelper.CanForth(mainFragmentStatus))
            //{
            //	ivNextPage.SetImageResource(Resource.Drawable.next_page_disable);
            //}
            //else
            //{
            //	ivNextPage.SetImageResource(Resource.Drawable.next_page_activy);
            //}
        }

        private void UpdatePboPage()
        {
            //if(((ContentActivity)Activity).IsPbo())
            //{
            //	tvGoToPage.Visibility = ViewStates.Visible;
            //	if(string.IsNullOrEmpty(currentPboPage))
            //	{
            //		tvPboPageNumLabel.Visibility = ViewStates.Invisible;
            //		tvPboPageNum.Visibility = ViewStates.Invisible;
            //	}
            //	else
            //	{
            //		tvPboPageNumLabel.Visibility = ViewStates.Visible;
            //		tvPboPageNum.Visibility = ViewStates.Visible;
            //		tvPboPageNum.Text = currentPboPage;
            //	}
            //}
            //else
            //{
            //	tvGoToPage.Visibility = ViewStates.Invisible;
            //	tvPboPageNumLabel.Visibility = ViewStates.Invisible;
            //	tvPboPageNum.Visibility = ViewStates.Invisible;
            //}
        }

        public void Print(PrintType type)
        {
            //printingWebView = WebViewManager.Instance.RequestPrintingWebView(
            //	this.Activity,
            //	PrintingWebViewOnPageLoaded);

            //var tocNode = DataCache.INSTATNCE.Toc.ShowingLeafNode;
            //var pub = ((ContentActivity)Activity).Publication;

            //if(type == PrintType.PDF)
            //{
            //	printingWebView.Tag = new JavaObjWrapper<string>(PdfFileName);
            //}

            //var result = AsyncHelpers.RunSync<string>(
            //	() => PublicationContentUtil.Instance.GetContentFromTOC(pub.Value.BookId, tocNode, true, false));

            //if(result.Length > WebViewManager.ShowWaitContentMinLength)
            //{
            //	((ContentActivity)Activity).ShowPleaseWaitDialog();
            //}

            //printingWebView.LoadDataWithBaseURL(
            //	"file:///android_asset/html/",
            //	WebViewManager.Instance.ApplyPrintTemplate(result, pub.Value.Name, tocNode.Title),
            //	"text/html",
            //	"utf-8",
            //	null);
        }

        private void PrintingWebViewOnPageLoaded()
        {
            //var pdfFile = printingWebView.Tag as JavaObjWrapper<string>;

            //var printManager = (PrintManager)Activity.GetSystemService(Context.PrintService);
            //var printDocumentAdapter = printingWebView.CreatePrintDocumentAdapter();
            //if(pdfFile == null)
            //{
            //	printManager.Print("Print TOC", printDocumentAdapter, null);
            //	printingWebView.Tag = null;
            //	WebViewManager.Instance.ReleaseWebView(WebViewManager.WebViewType.Printing, null);
            //}
            //else
            //{
            //	//PrintHelper.Layout(
            //	//	printDocumentAdapter,
            //	//	new CancellationSignal(),
            //	//	new PrintLayoutCallback(pdfFile.Value, printDocumentAdapter, PdfFinished));
            //}
        }

        private void PdfFinished(string path)
        {
            //printingWebView.Tag = null;
            //WebViewManager.Instance.ReleaseWebView(WebViewManager.WebViewType.Printing, null);
            //WebViewManager.Instance.ClearWebViewStatus(WebViewManager.WebViewType.Printing);
            //printingWebView = null;

            //((ContentActivity)Activity).ClosePleaseWaitPageLoadDialog();

            //var emailIntent = new Intent(Intent.ActionSend);
            //emailIntent.SetType("message/rfc822");
            //emailIntent.PutExtra(Intent.ExtraEmail, "");
            //var file = new Java.IO.File(path);
            //emailIntent.PutExtra(Intent.ExtraStream, Uri.FromFile(file));
            //Activity.StartActivity(emailIntent);
        }

        private static string FakeToc(string page)
        {
            //const string sessionFolder = "41_3_8_aa";

            //var sdCardRoot = Android.OS.Environment.ExternalStorageDirectory.Path;
            //var tempFolder = System.IO.Path.Combine (sdCardRoot, @"Temp");
            //var tempFolderInfo = new DirectoryInfo(tempFolder);
            //if(!tempFolderInfo.Exists)
            //{
            //	return page;
            //}

            //var sessionFolderInfo = new DirectoryInfo(System.IO.Path.Combine(tempFolder, sessionFolder));
            //if(!sessionFolderInfo.Exists)
            //{
            //	return page;
            //}

            //var tocFiles = new List<FileInfo>(sessionFolderInfo.GetFiles());
            //tocFiles.Sort((a, b) =>{
            //	var tocA = int.Parse(a.Name.Substring(0, a.Name.Length - 5));
            //	var tocB = int.Parse(b.Name.Substring(0, b.Name.Length - 5));
            //	return tocA - tocB;
            //});

            //page = string.Empty;
            //for(int i = 0; i < tocFiles.Count; ++i)
            //{
            //	var t = tocFiles[i];
            //	var tocId = t.Name;
            //	tocId = tocId.Substring(0, tocId.Length - 5);
            //	using(var fs = new FileStream(t.FullName, FileMode.Open, FileAccess.Read))
            //	using(var sr = new StreamReader(fs, Encoding.UTF8))
            //	{
            //		if(i > 0)
            //		{
            //			page += "<div style='margin-left:-5px;margin-right:-5px' class='pagebreakdiv'><div style='width:100%;height:2px;background:#E1E1E1'></div><div style='width:100%;height:13px;background:#EEEEEE'></div></div>";
            //		}

            //		page +=  "<div id='toc_" + tocId + "' class='tocpagediv' >" +  sr.ReadToEnd() + "</div>";
            //	}
            //}

            return page;
        }

        public void DumpToc()
        {
            //var sdCardRoot = Android.OS.Environment.ExternalStorageDirectory.Path;
            //var tempFolder = System.IO.Path.Combine(sdCardRoot, @"Temp");
            //var tempFolderInfo = new DirectoryInfo(tempFolder);
            //if (!tempFolderInfo.Exists)
            //{
            //    tempFolderInfo.Create();
            //}

            //var pub = ((ContentActivity)Activity).Publication;
            //var tag = wvContent.Tag as JavaObjWrapper<WebViewTag>;

            //var sessionFolder = pub.Value.BookId.ToString();
            //foreach (var id in tag.Value.TOCIdList)
            //{
            //    sessionFolder += "_" + id;
            //}
            //sessionFolder = System.IO.Path.Combine(tempFolder, sessionFolder);

            //var sessionFolderInfo = new DirectoryInfo(sessionFolder);
            //if (sessionFolderInfo.Exists)
            //{
            //    sessionFolderInfo.Delete();
            //}
            //sessionFolderInfo.Create();

            //foreach (var id in tag.Value.TOCIdList)
            //{
            //    var tocNode = PublicationContentUtil.Instance.GetTOCByTOCId(
            //        id,
            //        DataCache.INSTATNCE.Toc.RootTocNode);
            //    var result = AsyncHelpers.RunSync<string>(
            //        () => PublicationContentUtil.Instance.GetContentFromTOC(pub.Value.BookId, tocNode));

            //    var dumpFile = System.IO.Path.Combine(sessionFolder, id + ".html");
            //    var fi = new FileInfo(dumpFile);
            //    using (var fs = new FileStream(fi.FullName, FileMode.CreateNew, FileAccess.Write))
            //    using (var sw = new StreamWriter(fs, Encoding.UTF8))
            //    {
            //        //sw.Write(WebViewManager.Instance.ApplyTemplate(WebViewManager.WebViewType.Content, toc));
            //        sw.Write(result);
            //    }
            //}
        }

        private void DumpToc(int bookId, int tocId, string toc)
        {
            //var sdCardRoot = Android.OS.Environment.ExternalStorageDirectory.Path;
            //var tempFolder = System.IO.Path.Combine(sdCardRoot, @"Temp");
            //var tempFolderInfo = new DirectoryInfo(tempFolder);
            //if (!tempFolderInfo.Exists)
            //{
            //    tempFolderInfo.Create();
            //}

            //var dumpFile = System.IO.Path.Combine(tempFolder, "pub_" + bookId + "_" + tocId + ".html");
            //var fi = new FileInfo(dumpFile);
            //fi.Delete();
            //using (var fs = new FileStream(fi.FullName, FileMode.CreateNew, FileAccess.Write))
            //using (var sw = new StreamWriter(fs, Encoding.UTF8))
            //{
            //    //sw.Write(WebViewManager.Instance.ApplyTemplate(WebViewManager.WebViewType.Content, toc));
            //    sw.Write(toc);
            //}
        }

        private string SlideToc(string tocContent)
        {
            tocContent = "<div class='l1'>"
                + "<div class='l2'>asf</div>"
                + "<div class='l2'>asf<span class='l3'>daf</span></div>"
                + "<div class='l2'>asf</div>"
                + "<div class='l2'>asf<span class='l3'>daf</span></div>"
                + "<div class='l2'>asf</div>"
                + "</div>";

            //var doc = new HtmlDocument();
            //doc.LoadHtml(tocContent);
            //var firstGChildren = doc.DocumentNode.Elements(null);
            //Console.WriteLine("------------------");
            //Console.WriteLine(tocContent);
            //Console.WriteLine("------------------");
            //Console.WriteLine(doc.DocumentNode.InnerHtml);
            //foreach (var c in firstGChildren)
            //{
            //    Console.WriteLine("------------------");
            //    Console.WriteLine(c.InnerHtml);
            //}
            //Console.WriteLine("------------------");
            return tocContent;
        }
    }
}

