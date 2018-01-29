using System;
using Android.Runtime;
using Java.Interop;
using Android.Webkit;
using System.Collections.Generic;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Droid.Business;
using Android.App;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.ContentPage;

namespace LexisNexis.Red.Droid.WebViewUtility
{
	public class RedController: Java.Lang.Object
	{
		public interface IRedWebViewHost
		{
			void InfinityScrollPageLoaded();
		}

		private static List<Tuple<
			string,	// id
			long,	// timestamp
			string	// value
		>> LongStringParamCache = new List<Tuple<string, long, string>>();

		public static string AddLongStringParam(string value)
		{
			var now = DateTimeHelper.GetTimeStamp();
			LongStringParamCache.RemoveAll(p => now - p.Item2 > 5000);
			var param = new Tuple<string, long, string>(Guid.NewGuid().ToString(), now, value);
			LongStringParamCache.Add(param);
			return param.Item1;
		}

		public static string GetLongStringParam(string id)
		{
			string result = null;
			var found = LongStringParamCache.FindIndex(p => p.Item1 == id);
			if(found >= 0)
			{
				result = LongStringParamCache[found].Item3;
				LongStringParamCache.RemoveAt(found);
			}

			var now = DateTimeHelper.GetTimeStamp();
			LongStringParamCache.RemoveAll(p => now - p.Item2 > 5000);

			return result;
		}

		private WebViewKeeper keeper;
		private readonly WebViewExt webView;
		public RedController(WebViewKeeper keeper, WebViewExt webView)
		{
			this.keeper = keeper;
			this.webView = webView;
		}

		public RedController (IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}

		[Export ("jsError")]
		[JavascriptInterface]
		public void jsError(Java.Lang.String error) {
			Console.WriteLine("jsError: " + error);
		}
		[Export ("jsLog")]
		[JavascriptInterface]
		public void jsLog(Java.Lang.String message) {
			Console.WriteLine("[" + DateTimeHelper.GetTimeStamp() + "] jsLog: " + message);
		}

		[Export ("Selected")]
		[JavascriptInterface]
		public void Selected(
			Java.Lang.String purpose,
			Java.Lang.String text,
			float left, float top, float right, float bottom) {
			Application.SynchronizationContext.Post(_ =>
				keeper.OnGetSelectedText(purpose.ToString(), text.ToString(), left, top, right, bottom),
				null);
		}

		[Export ("OnLoadingPageCompleted")]
		[JavascriptInterface]
		public void OnLoadingPageCompleted(Java.Lang.String addedPageId, Java.Lang.String removedPageIdList, int newWebViewContentHeight) {
			Application.SynchronizationContext.Post(_ =>
				keeper.OnScrollLoadPageCompleted(
					addedPageId.ToString(),
					removedPageIdList == null ? null : removedPageIdList.ToString()),
				null);
			webView.LoadingPageCompleted(newWebViewContentHeight);
		}

		[Export ("OnRemovePageCompleted")]
		[JavascriptInterface]
		public void OnRemovePageCompleted(Java.Lang.String pageId) {
			keeper.OnRemovePageCompleted(pageId.ToString());
		}

		[Export ("GetCachedLongStringParam")]
		[JavascriptInterface]
		public Java.Lang.String GetCachedLongStringParam(Java.Lang.String id) {
			return new Java.Lang.String(GetLongStringParam(id.ToString()));
		}

		[Export ("GetWebViewWidth")]
		[JavascriptInterface]
		public int GetWebViewWidth() {
			return keeper.GetWebViewWidth();
		}

		[Export ("GetWebViewHeight")]
		[JavascriptInterface]
		public int GetWebViewHeight() {
			return keeper.GetWebViewHeight();
		}

		[Export ("ScrollReachBound")]
		[JavascriptInterface]
		public void ScrollReachBound(int delta) {
			Application.SynchronizationContext.Post(_ =>
				webView.ScrollReachBound(delta), null);
		}

		[Export ("ScrollToPage")]
		[JavascriptInterface]
		public void ScrollToPage(Java.Lang.String pageId, Java.Lang.String pboPage) {
			Application.SynchronizationContext.Post(_ =>
				keeper.OnScrollToPage(
					pageId.ToString(),
					pboPage == null ? null : pboPage.ToString()), null);
		}

		[Export ("GetHighLightKeywords")]
		[JavascriptInterface]
		public Java.Lang.String GetHighLightKeywords() {
			//*
			Console.WriteLine("+++++++++");
			Console.WriteLine("Webview nav id: " + DataCache.INSTATNCE.Toc.NavigationRecordId);
			foreach(var r in NavigationManager.Instance.Records)
			{
				Console.WriteLine(r.RecordID + " - " + r.RecordType.ToString());
			}
			Console.WriteLine("----------");
			//*/

			var record = NavigationManagerHelper.GetRecord(DataCache.INSTATNCE.Toc.NavigationRecordId) as SearchBrowserRecord;
			if(record == null
				|| record.SpliteKeywords == null
				|| record.SpliteKeywords.Count == 0)
			{
				return null;
			}

			return new Java.Lang.String(string.Join(" ", record.SpliteKeywords));
		}

		[Export ("GetPboPage")]
		[JavascriptInterface]
		public Java.Lang.String GetPboPage()
		{
			var record = NavigationManagerHelper.GetRecord(DataCache.INSTATNCE.Toc.NavigationRecordId) as ContentBrowserRecord;
			if(record == null
				|| record.RecordType != RecordType.ContentRecord
				|| record.PageNum == 0)
			{
				return null;
			}

			return new Java.Lang.String(record.PageNum.ToString());
		}

		[Export ("GetScrollOp")]
		[JavascriptInterface]
		public Java.Lang.String GetScrollOp()
		{
			var record = NavigationManagerHelper.GetRecord(DataCache.INSTATNCE.Toc.NavigationRecordId);
			if(record.RecordType == RecordType.SearchResultRecord)
			{
				var searchBrowserRecord = (SearchBrowserRecord)record;
				if(string.IsNullOrEmpty(searchBrowserRecord.HeadType))
				{
					return new Java.Lang.String(new ScrollOp{
						Type = ScrollOp.TypeHighLight,
						TocId = "toc_" + searchBrowserRecord.TOCID
					}.Serialize());
				}

				return new Java.Lang.String(new ScrollOp{
					Type = ScrollOp.TypeHighLight,
					TocId = "toc_" + searchBrowserRecord.TOCID,
					HeadType = searchBrowserRecord.HeadType,
					HeadSequence = searchBrowserRecord.HeadSequence
				}.Serialize());
			}
			else if(record.RecordType == RecordType.ContentRecord)
			{
				var contentRecord = (ContentBrowserRecord)record;
				if(contentRecord.PageNum > 0)
				{
					return new Java.Lang.String(new ScrollOp{
						Type = ScrollOp.TypePboPage,
						TocId = "toc_" + contentRecord.TOCID,
						PageNum = contentRecord.PageNum
					}.Serialize());
				}

				if(!string.IsNullOrEmpty(contentRecord.RefptID))
				{
					return new Java.Lang.String(new ScrollOp{
						Type = ScrollOp.TypePboPage,
						TocId = "toc_" + contentRecord.TOCID,
						RefptId = contentRecord.RefptID
					}.Serialize());
				}
			}

			return new Java.Lang.String(new ScrollOp{
				Type = ScrollOp.TypeTop
			}.Serialize());
		}


		[Export ("GetIndexScrollOp")]
		[JavascriptInterface]
		public Java.Lang.String GetIndexScrollOp()
		{
			if(DataCache.INSTATNCE.IndexList != null)
			{
				if(!string.IsNullOrEmpty(DataCache.INSTATNCE.IndexList.UserSelectedIndexTitle))
				{
					return new Java.Lang.String(new ScrollOp{
						Type = ScrollOp.TypeIndex,
						Title = DataCache.INSTATNCE.IndexList.UserSelectedIndexTitle
					}.Serialize());
				}
			}

			return new Java.Lang.String(new ScrollOp{
				Type = ScrollOp.TypeTop
			}.Serialize());
		}
	}
}

