using System;
using Android.Webkit;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.ContentPage;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.WebViewUtility
{
	public class WebViewClientExt: WebViewClient
	{
		private readonly WebViewKeeper keeper;

		public WebViewClientExt(WebViewKeeper keeper)
		{
			this.keeper = keeper;
		}

		public override void OnPageFinished(WebView view, string url)
		{
			keeper.OnWebPageLoaded((WebViewExt)view, url);
		}

		public override bool ShouldOverrideUrlLoading(WebView view, string url)
		{
			var bookId = NavigationManagerHelper.GetCurrentBookId();
			if(bookId < 0)
			{
				return base.ShouldOverrideUrlLoading(view, url);
			}

			var link = PublicationContentUtil.Instance.BuildHyperLink(bookId, url,"");
			if(link == null)
			{
				return true;
			}

			if(link.LinkType != HyperLinkType.AnchorHyperlink)
			{
				keeper.LoadUrl(link);
				return true;
			}

			return base.ShouldOverrideUrlLoading(view, url);
		}
	}
}

