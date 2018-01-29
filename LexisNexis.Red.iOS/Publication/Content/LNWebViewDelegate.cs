using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Foundation;
using System.Linq;
using UIKit;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using Xamarin;

namespace LexisNexis.Red.iOS
{
	public delegate void InfiniteScrollCallBack(UIWebView webView, string content, string position);

	public class LNWebViewDelegate : UIWebViewDelegate
	{
		public LNWebViewDelegate () : base()
		{
		}
		/// <summary>
		/// Shoulds the start load.
		/// </summary>
		/// <returns><c>true</c>, if start load was shoulded, <c>false</c> otherwise.</returns>
		/// <param name="webView">Web view.</param>
		/// <param name="request">Request.</param>
		/// <param name="navigationType">Navigation type.</param>
		public override bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			string urlString = request.Url.AbsoluteString;//"looseleaf:kp//opendocument?dpsi=02I6&refpt=2006A69S65&remotekey1=REFPTID" for example

			//parse request url
			int endPostOfProtocal = urlString.IndexOf("://");
			string protocalStr = "";//"http", "looseleaf" etc.   
			string addressStr = "";//"opendocument?dpsi=02I6&refpt=2006A69S65&remotekey1=REFPTID" for example
			string functionStr = "";//"opendocument", "loadpage" etc
			Dictionary<string, string> requestParams = new Dictionary<string, string>();
			if (endPostOfProtocal > 0) {
				protocalStr = urlString.Substring (0, endPostOfProtocal);
				addressStr = urlString.Substring (endPostOfProtocal + 3);

				if (addressStr != null) {
					string[] addressStrArr = addressStr.Split (new char[1]{'?'});
					functionStr = addressStrArr.Length > 0 ? addressStrArr [0] : "";

					if (addressStrArr.Length >= 2) {
						string paramsStr = addressStrArr [1];
						string[] paramsArr = paramsStr.Split (new char[1]{ '&' });
						foreach (string aParamPair in paramsArr) {
							string[] nameValue = aParamPair.Split (new char[1] {'='});
							if (nameValue.Length >= 2) {
								requestParams.Add (nameValue [0], nameValue [1]);
							}
						}
					}
				}
			}
			if (navigationType == UIWebViewNavigationType.LinkClicked) {
				var bookId = AppDataUtil.Instance.GetCurrentPublication ().BookId;
				var	hyperLink = PublicationContentUtil.Instance.BuildHyperLink (bookId, urlString);
				switch (hyperLink.LinkType) {
				case HyperLinkType.IntraHyperlink:
					var Intra = hyperLink as IntraHyperlink;
					var tocId = Intra.TOCID;
					AppDataUtil.Instance.SetOpendTOC (AppDataUtil.Instance.GetTOCNodeByID (tocId));
					AppDataUtil.Instance.SetHighlightedTOCNode (AppDataUtil.Instance.GetTOCNodeByID (tocId));
					AppDataUtil.Instance.ScrollToHtmlTagId = Intra.Refpt;
 					AppDataUtil.Instance.AddBrowserRecord (new ContentBrowserRecord(AppDataUtil.Instance.GetCurrentPublication().BookId, tocId, 0));//add browser record
					break;
				case HyperLinkType.InternalHyperlink:
					var inter = hyperLink as InternalHyperlink;
					var booId = inter.BookID;
					var TocId = inter.TOCID;
					AppDataUtil.Instance.ScrollToHtmlTagId = inter.Refpt;
 					List<Publication> cachedPublicationList = PublicationUtil.Instance.GetPublicationOffline ();
					var publication = cachedPublicationList.FirstOrDefault (o => o.BookId == booId);
					AppDataUtil.Instance.SetCurrentPublication (publication);
					AppDataUtil.Instance.SetOpendTOC (AppDataUtil.Instance.GetTOCNodeByID(TocId));
					AppDataUtil.Instance.SetHighlightedTOCNode (AppDataUtil.Instance.GetTOCNodeByID(TocId));

					break;
				case HyperLinkType.ExternalHyperlink:
 					var External = hyperLink as ExternalHyperlink;
					string stringUrl = External.Url;
					UIApplication.SharedApplication.OpenUrl (NSUrl.FromString (stringUrl));
					break;

				case HyperLinkType.AttachmentHyperlink:
					var AttachMent = hyperLink as AttachmentHyperlink;
					if (AttachMent!= null) {
						string targetFileName = AttachMent.TargetFileName;
						string appRootPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/"+targetFileName;
						NSUrl url = new NSUrl(appRootPath);
						NSUrlRequest requestUrl = NSUrlRequest.FromUrl (url);
						AppDataUtil.Instance.AddBrowserRecord (new DocumentBrowserRecord (AppDataUtil.Instance.GetCurrentPublication().BookId,AppDataUtil.Instance.GetOpendTOC ().ID,0,0,AttachMent));
						webView.LoadRequest (requestUrl);
						AppDisplayUtil.Instance.contentVC.ContentNavigationItem.LeftBarButtonItems[1].Enabled = true;
					} 
					break;
				}
			}

			if (protocalStr == "looseleaf") {

				switch (functionStr) {
				case "loadpage":
					AppDataUtil.Instance.ScrollToTOC (AppendPageContent, webView, requestParams ["page"], Convert.ToInt32 (requestParams ["tocid"]));
					TOCNode targetNode = AppDataUtil.Instance.GetNextOfTOCNodeWithId (Convert.ToInt32 (requestParams ["tocid"]), requestParams ["page"]);
					if (targetNode != null) {
						string js = string.Format ("setLoadingTOCTitle(\"{0}\",\"{1}\",\"{2}\")", targetNode.ID, targetNode.Title, requestParams ["page"]);
						webView.EvaluateJavascript (js);
					} else {
						webView.EvaluateJavascript (string.Format ("setLoadingTOCTitle(\"-1\",\"You can't scroll to a new page\", \"{0}\")", requestParams ["page"]));
					}
					break;
				case "highlighttoc":
					if (requestParams ["tocid"] != AppDataUtil.Instance.GetHighlightedTOCNode ().ID.ToString ()) {
						try{
							TOCNode toBeHighlightedTOCNode = AppDataUtil.Instance.GetTOCNodeByID (Convert.ToInt32 (requestParams ["tocid"]));
							AppDataUtil.Instance.SetHighlightedTOCNode(toBeHighlightedTOCNode);
							AppDisplayUtil.Instance.TOCVC.SetHighlightedTOCNode(toBeHighlightedTOCNode);
						}catch(Exception e){
							Insights.Report(e, new Dictionary<string, string>{
								{"Exception summary","Highlight next or previous toc when scrolling up or down"},
								{"Publication ID", AppDataUtil.Instance.GetCurrentPublication().BookId.ToString()},
								{"Publication Name", AppDataUtil.Instance.GetCurrentPublication().Name},
								{"Country", GlobalAccess.Instance.CurrentUserInfo.Country.CountryName},
								{"Email", GlobalAccess.Instance.CurrentUserInfo.Email},
							});
						} 
					}
					break;
				case "setCurrentPBOPageNum":
					var PageNum = requestParams["pageNum"];
					NSNotificationCenter.DefaultCenter.PostNotificationName ("inputPageNumber", this, new NSDictionary ("page", PageNum));

					break;
				}
				return false;
			}

			return true;
		}

		public void AppendPageContent(UIWebView webView, string content, string direction)
		{
			if (content != null) {
				content = ContentFormatUtil.Format (content);

				content = content.Replace("\"", "##");
				content = Regex.Replace (content, @"[\r|\n]", "");
				webView.EvaluateJavascript ("appendPageContent(\"" + content + "\", \"" + direction + "\");");
			}
		}
	}


}

