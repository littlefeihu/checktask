using System;
using Android.Webkit;
using LexisNexis.Red.Droid.App;
using Android.Views;
using System.IO;
using LexisNexis.Red.Common.BusinessModel;
using System.Text.RegularExpressions;
using Android.Content;

namespace LexisNexis.Red.Droid.WebViewUtility
{
	public class WebViewManager
	{
		public const int ShowWaitContentMinLength = 200 * 1024;
		public const int ShowWaitMinATagCount = 100;

		public enum WebViewType
		{
			Content,
			Index,
			Printing,
		}

		public const string PleaseWaitPageLoadDialogExtTagKey = "PleaseWaitPageLoadDialogExtTagKey";
		public const string AssetsHtmlRoot = "file:///android_asset/html/";
		private readonly string HtmlTemplate;
		private readonly string IndexHtmlTemplate;
		private readonly string PrintingHtmlTemplate;

		private static readonly WebViewManager instance = new WebViewManager();

		public static WebViewManager Instance
		{
			get
			{
				return instance;
			}
		}

		private readonly WebViewKeeper contentKeeper = new WebViewKeeper();
		private readonly WebViewKeeper indexKeeper = new WebViewKeeper();
		private readonly WebViewKeeper printingKeeper = new WebViewKeeper();

		private WebViewManager()
		{
			using (var sr = new StreamReader (MainApp.ThisApp.Assets.Open ("html/content_template.html")))
			{
				HtmlTemplate = sr.ReadToEnd();
			}

			using (var sr = new StreamReader (MainApp.ThisApp.Assets.Open ("html/index_template.html")))
			{
				IndexHtmlTemplate = sr.ReadToEnd();
			}

			using (var sr = new StreamReader (MainApp.ThisApp.Assets.Open ("html/printing_template.html")))
			{
				PrintingHtmlTemplate = sr.ReadToEnd();
			}
		}

		public WebViewExt RequestPrintingWebView(
			Context context,
			Action onPageLoaded)
		{
			return printingKeeper.RequestWebView(
				context,
				onPageLoaded);
		}

		public WebViewExt RequestWebView(
			WebViewType type,
			ViewGroup container,
			Action onPageLoaded = null,
			Action<string, string, float, float, float, float> onGetSelectedText = null,
			Action<Hyperlink> onLoadUrl = null,
			Action<int, float> onWebOverScroll = null,
			Action<string> onScrollLoadPageCompleted = null,
			Action<string, string> onScrollToPage = null)
		{
			switch(type)
			{
			case WebViewType.Content:
				return contentKeeper.RequestWebView(
					container,
					onPageLoaded,
					onGetSelectedText,
					onLoadUrl,
					onWebOverScroll,
					onScrollLoadPageCompleted,
					onScrollToPage);
			case WebViewType.Index:
				return indexKeeper.RequestWebView(
					container,
					onPageLoaded,
					onGetSelectedText,
					onLoadUrl,
					onWebOverScroll,
					onScrollLoadPageCompleted,
					onScrollToPage);
			}

			throw new InvalidProgramException("Unknown web view type.");
		}

		public void ReleaseWebView(WebViewType type, ViewGroup container)
		{
			switch(type)
			{
			case WebViewType.Content:
				contentKeeper.ReleaseWebView(container);
				return;
			case WebViewType.Index:
				indexKeeper.ReleaseWebView(container);
				return;
			case WebViewType.Printing:
				printingKeeper.ReleaseWebView(container);
				return;
			}

			throw new InvalidProgramException("Unknown web view type.");
		}

		public void ClearAllWebViewStatus()
		{
			contentKeeper.ClearWebViewStatus();
			indexKeeper.ClearWebViewStatus();
		}

		public void ClearWebViewStatus(WebViewType type)
		{
			if(type == WebViewType.Content)
			{
				contentKeeper.ClearWebViewStatus();
			}
			else if(type == WebViewType.Index)
			{
				indexKeeper.ClearWebViewStatus();
			}
			else if(type == WebViewType.Printing)
			{
				printingKeeper.ClearWebViewStatus();
			}
		}

		public void SetFontSize(WebContentFontSize size)
		{
			contentKeeper.SetFontSize(size);
			indexKeeper.SetFontSize(size);
		}

		public string ApplyTemplate(WebViewType type, string body)
		{
			if(type == WebViewType.Content)
			{
				return HtmlTemplate.Replace("#BODY#", body);
			}
			else if(type == WebViewType.Index)
			{
				return IndexHtmlTemplate.Replace("#BODY#", body);
			}
			else if(type == WebViewType.Printing)
			{
				return PrintingHtmlTemplate.Replace("#BODY#", body);
			}

			throw new InvalidProgramException("Unknown web view type.");
		}

		public string ApplyPrintTemplate(string body, string publicationTitle, string tocTitle)
		{
			return PrintingHtmlTemplate
				.Replace("###PublicationTitle###", publicationTitle)
				.Replace("###TocTitle###", tocTitle)
				.Replace("###CurrentDate###", DateTime.Now.ToString("dd MMMM"))
				.Replace("###CurrentYear###", DateTime.Now.ToString("yyyy"))
				.Replace("#BODY#", body);
		}

		public static string GetTocDivId(int tocId)
		{
			return "toc_" + tocId;
		}

		public static int ExtractTocIdFromDivId(string divId)
		{
			divId = divId.Replace("toc_", "");
			return int.Parse(divId);
		}

		public static int CountATag(string content)
		{
			var rg = new Regex("<a");
			MatchCollection mc = rg.Matches(content);
			return mc.Count;
		}
	}
}

