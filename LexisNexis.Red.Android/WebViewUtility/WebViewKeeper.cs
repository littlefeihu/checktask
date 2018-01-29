using System;
using Android.Webkit;
using Android.Views;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.Business;
using Android.App;
using System.Threading;
using System.Threading.Tasks;
using LexisNexis.Red.Common.BusinessModel;
using Android.Content;

namespace LexisNexis.Red.Droid.WebViewUtility
{
	public class WebViewKeeper
	{
		private readonly WebViewClientExt webViewClient;

		private WebViewExt webView;
		private ViewGroup parentView;
		private Action onPageLoaded;
		private Action<string, string, float, float, float, float> onGetSelectedText;
		private Action<int, float> onWebOverScroll;
		private Action<string> onScrollLoadPageCompleted;
		private Action<string, string> onScrollToPage;
		private Action<Hyperlink> onLoadUrl;

		public WebViewKeeper()
		{
			webViewClient = new WebViewClientExt(this);
		}

		public WebViewExt RequestWebView(
			ViewGroup container,
			Action onPageLoaded = null,
			Action<string, string, float, float, float, float> onGetSelectedText = null,
			Action<Hyperlink> onLoadUrl = null,
			Action<int, float> onWebOverScroll = null,
			Action<string> onScrollLoadPageCompleted = null,
			Action<string, string> onScrollToPage = null)
		{
			RequestWebView(
				container.Context,
				onPageLoaded,
				onGetSelectedText,
				onLoadUrl,
				onWebOverScroll,
				onScrollLoadPageCompleted,
				onScrollToPage);

			parentView = container;
			container.AddView(
				webView,
				new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
			webView.RequestLayout();

			return webView;
		}

		public WebViewExt RequestWebView(
			Context context,
			Action onPageLoaded = null,
			Action<string, string, float, float, float, float> onGetSelectedText = null,
			Action<Hyperlink> onLoadUrl = null,
			Action<int, float> onWebOverScroll = null,
			Action<string> onScrollLoadPageCompleted = null,
			Action<string, string> onScrollToPage = null)
		{
			if(webView == null)
			{
				webView = new WebViewExt(context);
				webView.SetLayerType(LayerType.Software, null);
				webView.LayoutParameters = new ViewGroup.LayoutParams(
					ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
				webView.Settings.JavaScriptEnabled = true;
				webView.SetWebViewClient(webViewClient);
				webView.Settings.TextZoom = WebContentFontSizeHelper.ParseZoom((int)SettingsUtil.Instance.GetFontSize());
				webView.AddJavascriptInterface(new RedController(this, webView), "RedController");
				webView.SetOverScrollHandler(OnWebOverScroll);
				webView.ScrollbarFadingEnabled = false;
			}

			if(parentView != null)
			{
				throw new InvalidOperationException("The WebView is loan out.");
			}

			this.onPageLoaded = onPageLoaded;
			this.onGetSelectedText = onGetSelectedText;
			this.onWebOverScroll = onWebOverScroll;
			this.onScrollLoadPageCompleted = onScrollLoadPageCompleted;
			this.onScrollToPage = onScrollToPage;
			this.onLoadUrl = onLoadUrl;

			return webView;
		}

		public void OnWebPageLoaded(WebViewExt view, string url)
		{
			LogHelper.Debug("dbg", "OnWebPageLoaded url=" + url);
			webView.ClearHistory();
			if(onPageLoaded != null)
			{
				LogHelper.Debug("dbg", "call onPageLoaded");
				onPageLoaded();
			}

			/*
			Task.Run(() =>
			{
				Thread.Sleep(100);
				Application.SynchronizationContext.Post(_ =>
				{
					webView.ScrollTo(0, 0);
					LogHelper.Debug("dbg", "!!!Force: webView.ScrollTo(0, 0).");
				}, null);
			});
			*/
		}

		public void OnScrollLoadPageCompleted(string addedPageId, string removedPagedIdList)
		{
			Android.Util.Log.Debug("dbg", "Added Page: " + addedPageId);
			Android.Util.Log.Debug("dbg", "Removed Page: " + removedPagedIdList);

			var tocId = WebViewManager.ExtractTocIdFromDivId(addedPageId);
			var tag = webView.Tag as JavaObjWrapper<WebViewTag>;
			if(tag == null)
			{
				return;
			}

			if(tag.Value.TOCIdList != null)
			{
				if(tag.Value.TOCIdList.FindIndex(id => id == tocId) < 0)
				{
					tag.Value.TOCIdList.Add(tocId);
				}
			}

			if(removedPagedIdList != null && tag.Value.TOCIdList != null)
			{
				var pageIds = removedPagedIdList.Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries);
				foreach(var id in pageIds)
				{
					tocId = WebViewManager.ExtractTocIdFromDivId(id);
					tag.Value.TOCIdList.Remove(tocId);
				}
			}

			if(onScrollLoadPageCompleted != null)
			{
				onScrollLoadPageCompleted(addedPageId);
			}
		}

		public void OnRemovePageCompleted(string pageId)
		{
			var tocId = WebViewManager.ExtractTocIdFromDivId(pageId);
			var tag = webView.Tag as JavaObjWrapper<WebViewTag>;
			if(tag.Value.TOCIdList != null)
			{
				tag.Value.TOCIdList.RemoveAll(id => id == tocId);
			}
		}

		public int GetWebViewHeight()
		{
			return Conversion.Px2Dp(webView.Height);
		}

		public int GetWebViewWidth()
		{
			return Conversion.Px2Dp(webView.Width);
		}

		private void OnWebOverScroll(int arg1, float arg2)
		{
			if(onWebOverScroll != null)
			{
				onWebOverScroll(arg1, arg2);
			}
		}

		public void OnScrollToPage(string pageId, string pboPage)
		{
			if(onScrollToPage != null)
			{
				onScrollToPage(pageId, pboPage);
			}
		}

		public void OnGetSelectedText(
			string purpose,
			string text,
			float left,
			float top,
			float right,
			float bottom)
		{
			if(onGetSelectedText != null)
			{
				onGetSelectedText(purpose, text, left, top, right, bottom);
			}
		}

		public void ReleaseWebView(ViewGroup container)
		{
			if(webView == null)
			{
				throw new InvalidOperationException("The WebView is not loan out.");
			}

			if(container != null)
			{
				if(parentView == null)
				{
					throw new InvalidOperationException("The WebView is not loan out.");
				}

				if(parentView != container)
				{
					throw new InvalidOperationException("Wrong container release the Webview");
				}

				container.RemoveView(webView);
			}

			parentView = null;
			onPageLoaded = null;
			onGetSelectedText = null;
			onWebOverScroll = null;
			onScrollLoadPageCompleted = null;
			onScrollToPage = null;
			onLoadUrl = null;
		}

		public void ClearWebViewStatus()
		{
			if(webView != null)
			{
				webView.Tag = null;
				//onPageLoaded = ClearHistoryAfterPageLoad;
				webView.LoadData("", "text/html", "utf-8");
				webView.LoadingPageCompleted(0);
			}
		}

		public void SetFontSize(WebContentFontSize size)
		{
			if(webView != null)
			{
				webView.Settings.TextZoom = WebContentFontSizeHelper.ParseZoom(size);
			}
		}

		public void LoadUrl(Hyperlink url)
		{
			if(onLoadUrl != null)
			{
				onLoadUrl(url);
			}
		}
	}
}

