using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Foundation;
using UIKit;

using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;

using MBProgressHUD;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public partial class ContentViewController : UIViewController,Observer
	{

		public LNWebview ContentWebView{ get; set;}
		public UINavigationItem IndexNavigationItem{ get; set; }
		public UINavigationItem ContentNavigationItem{ get; set; }
		private string pageString;
		private string emptyPageNumString = "Page ";

		public ContentViewController (IntPtr handle) : base (handle)
		{
 		}
 
		 
  		public override void LoadView()
		{
			base.LoadView ();

			ContentWebView = new LNWebview ();
			ContentWebView.DataDetectorTypes = UIDataDetectorType.None;//
			ContentWebView.BecomeFirstResponder();
			View.AddSubview (ContentWebView);
  		}

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
  
			AppDisplayUtil.Instance.contentVC = this;

 			GotoBarButton.Hidden = true;
			PageNumLabel.Hidden = true;

			NSLayoutConstraint leadingContraint = NSLayoutConstraint.Create (ContentWebView, NSLayoutAttribute.Leading, NSLayoutRelation.GreaterThanOrEqual, View, NSLayoutAttribute.Leading, 1, 0);
			leadingContraint.Priority = 750;

			NSLayoutConstraint trailingContraint = NSLayoutConstraint.Create (ContentWebView, NSLayoutAttribute.Trailing, NSLayoutRelation.GreaterThanOrEqual, View, NSLayoutAttribute.Trailing, 1, 0);
			trailingContraint.Priority = 750;

			NSLayoutConstraint widthContraint = NSLayoutConstraint.Create (ContentWebView, NSLayoutAttribute.Width, NSLayoutRelation.LessThanOrEqual, View, NSLayoutAttribute.Width, 0, 703);
			widthContraint.Priority = 1000;

			View.AddConstraints (new NSLayoutConstraint[]{
				leadingContraint,
				trailingContraint,
				widthContraint,
				NSLayoutConstraint.Create(ContentWebView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, View, NSLayoutAttribute.CenterX, 1, 0),//center X
				NSLayoutConstraint.Create(ContentWebView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 45),//top 45
				NSLayoutConstraint.Create(ContentWebView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0),//bottom 0
			});
			View.TranslatesAutoresizingMaskIntoConstraints = false;
			ContentWebView.TranslatesAutoresizingMaskIntoConstraints = false;

			IndexNavigationItem = new UINavigationItem ();
			IndexNavigationItem.LeftBarButtonItem = new UIBarButtonItem ("A", UIBarButtonItemStyle.Plain, null);
			IndexNavigationItem.LeftBarButtonItem.SetTitlePositionAdjustment (new UIOffset (20, 0), UIBarMetrics.Default);

			ContentNavigationItem = new UINavigationItem ();
			UIBarButtonItem spaceBarButtonItem = new UIBarButtonItem ("", UIBarButtonItemStyle.Plain, null);

			UIBarButtonItem forwardBarButtonItem = new UIBarButtonItem (new UIImage ("Images/Navigation/GrayForwardArrow.png"), UIBarButtonItemStyle.Plain, delegate {
				NavigateForward();
			});
			forwardBarButtonItem.Enabled = false;

			UIBarButtonItem backwardBarButtonItem = new UIBarButtonItem (new UIImage ("Images/Navigation/RedBackArrow.png"), UIBarButtonItemStyle.Plain, delegate {
				NavigateBackward();
			});
			backwardBarButtonItem.Enabled = false;


			ContentNavigationItem.LeftBarButtonItems = new UIBarButtonItem[]{ spaceBarButtonItem, backwardBarButtonItem, forwardBarButtonItem };
			ContentNavigationBar.PushNavigationItem (ContentNavigationItem, false);
			AppDataUtil.Instance.AddOpenedContentObserver (this);//Set current instance as the observer of subject OpendPublication to get notification when opend content changed

			if (await PageSearchUtil.Instance.IsPBO (AppDataUtil.Instance.GetCurrentPublication ().BookId)) {
				this.JudgePBOBook("YES");	 
			}

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("JudgePBOBook"), delegate(NSNotification obj) {
				if (obj.UserInfo != null) {
					string	boolStr = obj.UserInfo.ObjectForKey (new NSString ("PBOBook")).ToString ();
					this.JudgePBOBook(boolStr);
				}
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("ExecuteJS"), delegate(NSNotification obj) {
				if (obj.UserInfo != null) {
					string	jsStr = obj.UserInfo.ObjectForKey (new NSString ("jsStr")).ToString ();
					ContentWebView.EvaluateJavascript(jsStr);
				}
			});
		}

		private  void JudgePBOBook(string boolStr){

			if (boolStr == "YES") {
				GotoBarButton.Hidden = false;
				PageNumLabel.Hidden = false;

				NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("inputPageNumber"), delegate(NSNotification obj) {
					if (obj.UserInfo != null) {
						pageString = emptyPageNumString + obj.UserInfo.ObjectForKey (new NSString ("page")).ToString ();
						if (pageString == emptyPageNumString) {
							PageNumLabel.Text = "";
						} else {
							PageNumLabel.Text = pageString;
						}
					}
				});

				if (string.IsNullOrEmpty(pageString)) {
					PageNumLabel.Text = "";
				}
			} else {
				GotoBarButton.Hidden = true;
				PageNumLabel.Hidden = true;
			}
		}

		partial void ShowGoToTableView (NSObject sender)
		{
			UIStoryboard storyboard = UIStoryboard.FromName ("PBOPageNavigation", NSBundle.MainBundle);
			UIViewController navController = (UIViewController)storyboard.InstantiateViewController ("PBOPageNavigation");
 			UIPopoverController tagManagePopoverController = new UIPopoverController(navController);
			tagManagePopoverController.BackgroundColor = UIColor.White;
			AppDisplayUtil.Instance.SetPopoverController (tagManagePopoverController);
			tagManagePopoverController.SetPopoverContentSize(new CGSize(640,256),false);
			tagManagePopoverController.PresentFromRect(new CGRect(ContentWebView.Frame.Size.Width- 480,-225,640,256),this.View,UIPopoverArrowDirection.Up,true);//-225
  		}


		/// <summary>
		/// Update when content which are going to to be displayed changed
		/// </summary>
		/// <param name="s">S.</param>
		public async void Update(Subject s)
		{
			var hud = new MTMBProgressHUD (View) {
				LabelText = "Loading",
				RemoveFromSuperViewOnHide = true
			};
			View.AddSubview (hud);
			hud.Show (animated: true);


			OpenedPublication pcs = (OpenedPublication)s;
			ContentNavigationBar.PopNavigationItem (false);

			string content = "";

			string htmlFilePath = NSBundle.MainBundle.PathForResource ("Html/htmlDoc", "html");
			string htmlStr = File.ReadAllText (htmlFilePath);
			NSUrl url = new NSUrl (htmlFilePath);

			if (pcs.OpendContentType == PublicationContentTypeEnum.TOC) {
				if (await PageSearchUtil.Instance.IsPBO (AppDataUtil.Instance.GetCurrentPublication ().BookId)) {
					GotoBarButton.Hidden = false;
					PageNumLabel.Hidden = false;
				}
				ContentNavigationBar.PushNavigationItem (ContentNavigationItem, false);
				content = await PublicationContentUtil.Instance.GetContentFromTOC (pcs.P.BookId, pcs.OpendTOCNode);
				content = string.Format ("<div data-tocid='{0}' data-toctitle='{1}' class='page_container'>{2}</div>", pcs.OpendTOCNode.ID, pcs.OpendTOCNode.Title, content);
				ContentNavigationItem.LeftBarButtonItems [1].Enabled = AppDataUtil.Instance.CanBack () == true;
				ContentNavigationItem.LeftBarButtonItems [2].Enabled = AppDataUtil.Instance.CanForward () == true;

				TOCNode nextPageNode = AppDataUtil.Instance.GetNextOfTOCNodeWithId (pcs.OpendTOCNode.ID, "next");
				if (nextPageNode != null) {
					htmlStr = htmlStr.Replace ("#NEXT_PAGE_TITLE#", nextPageNode.Title).Replace ("#NEXT_PAGE_TOCID#", nextPageNode.ID.ToString ());
				} else {
					htmlStr = htmlStr.Replace ("#NEXT_PAGE_TITLE#", "You are on the end.").Replace ("#NEXT_PAGE_TOCID#", "-1");//"-1" here means no next page any more
				}

				TOCNode previousPageNode = AppDataUtil.Instance.GetNextOfTOCNodeWithId (pcs.OpendTOCNode.ID, "previous");
				if (previousPageNode != null) {
					htmlStr = htmlStr.Replace ("#PREVIOUS_PAGE_TITLE#", previousPageNode.Title).Replace ("#PREVIOUS_PAGE_TOCID#", previousPageNode.ID.ToString ());
				} else {
					htmlStr = htmlStr.Replace ("#PREVIOUS_PAGE_TITLE#", "You are on the begining.").Replace ("#PREVIOUS_PAGE_TOCID#", "-1");//"-1" here means no next page any more
				}

				htmlStr = htmlStr.Replace ("#PLACE_HOLDER_HIGHLIGHTED_TOCID#", pcs.OpendTOCNode.ID.ToString ());
				htmlStr = htmlStr.Replace ("#PLACE_HOLDER_HIGHLIGHTED_KEYWORD#", AppDataUtil.Instance.HighlightSearchKeyword);
				AppDataUtil.Instance.HighlightSearchKeyword = "";//

				htmlStr = htmlStr.Replace ("#PLACE_HOLDER_SCROLL_SCRIPT#", "<script type='text/javascript' src='./JS/infinitescroll.js'></script>");

				//When jump to content from index by click content link, selected index of tab bar supposed to be changed to "Contents"
				NSNotificationCenter.DefaultCenter.PostNotificationName ("ChangeContentTabBarSelectedIndex", this, new NSDictionary());//Suppose to be handled by ContentLeftTabBarController
			} else if (pcs.OpendContentType == PublicationContentTypeEnum.Index) {
				//TODO,
				GotoBarButton.Hidden = true;
				PageNumLabel.Hidden = true;
				ContentNavigationBar.PushNavigationItem (IndexNavigationItem, false);
				if (pcs.OpendIndex == null) {
					IndexNavigationItem.LeftBarButtonItem.Title = "";
					content = "<div class='nocontent'><span>No Index Files Available</span><div>";
				} else {
					IndexNavigationItem.LeftBarButtonItem.Title = pcs.OpendIndex.Title [0].ToString ();
					content = await PublicationContentUtil.Instance.GetContentFromIndex (pcs.P.BookId, pcs.OpendIndex);
					content = string.Format ("<div class='page_container'>{0}</div>", content);

					//Scroll to selected index
					var scrollToSelectedIndexJS = "(function(){if($(\".main\").first().data(\"filename\") != '" + pcs.OpendIndex.FileName +"'){$(window).scrollTo($(\".main[data-filename='" + pcs.OpendIndex.FileName +"']\"), 300);}})();";
					htmlStr = htmlStr.Replace ("#DOCUMENT_READY_SCRIPT#", scrollToSelectedIndexJS);
				}
				htmlStr = htmlStr.Replace ("#PLACE_HOLDER_SCROLL_SCRIPT#", "");

			} else if (pcs.OpendContentType == PublicationContentTypeEnum.Annotation) {
				//TODO
				GotoBarButton.Hidden = true;
				PageNumLabel.Hidden = true;
				htmlStr = htmlStr.Replace ("#PLACE_HOLDER_SCROLL_SCRIPT#", "");
				content = "<div class='nocontent'><span>You have no annotations, highlight text in your publications to add one</span><div>";
			}
			if (content != null) {
				float defaultFontSize = SettingsUtil.Instance.GetFontSize () > 0 ? SettingsUtil.Instance.GetFontSize () : 14;
				htmlStr = htmlStr.Replace ("#PLACEHOLDER_DEFAULT_FONT_SIZE#","" + defaultFontSize);
				htmlStr = htmlStr.Replace ("#PLACE_HOLDER_SCROLL_TO_ID#", AppDataUtil.Instance.ScrollToHtmlTagId);
				htmlStr = htmlStr.Replace ("#PLACE_HOLDER_HIGHLIGHTED_KEYWORD#", AppDataUtil.Instance.HighlightSearchKeyword);
				content = ContentFormatUtil.Format (content);
				content = Regex.Replace (content, @"<td />", "");
				content = Regex.Replace (content, @"<div[^/^>]*?/>", "");

				htmlStr = htmlStr.Replace ("#CONTENT#", content);
				ContentWebView.LoadHtmlString(htmlStr, url);
			}
			hud.Hide (animated: true, delay: 0.2);
		}


		private void NavigateBackward ()
		{
			BrowserRecord backwardBrowserRecord  = AppDataUtil.Instance.Backward();
			if(backwardBrowserRecord != null){
				List<Publication> cachedPublicationList =  PublicationUtil.Instance.GetPublicationOffline ();
				Publication navigateToP = cachedPublicationList.Find(o => o.BookId == backwardBrowserRecord.BookID);
				if (backwardBrowserRecord.RecordType == RecordType.ContentRecord) {
					AppDataUtil.Instance.SetCurrentPublication (navigateToP);
					ContentBrowserRecord contentBR = (ContentBrowserRecord)backwardBrowserRecord;
					AppDataUtil.Instance.SetOpendTOC (AppDataUtil.Instance.GetTOCNodeByID (contentBR.TOCID));
				} else if (backwardBrowserRecord.RecordType == RecordType.SearchResultRecord) {
					SearchBrowserRecord searchBR = (SearchBrowserRecord)backwardBrowserRecord;
					AppDataUtil.Instance.SetCurrentPublication (navigateToP, searchBR.KeyWords);
					AppDataUtil.Instance.SetOpendTOC (AppDataUtil.Instance.GetTOCNodeByID (searchBR.TOCID));
				}else if(backwardBrowserRecord.RecordType == RecordType.AnnotationNavigator){
					//TODO, Back to annotation navigator
				}
			}
		}

		private void NavigateForward ()
		{
			BrowserRecord forwardBrowserRecord  = AppDataUtil.Instance.Forward();
			if(forwardBrowserRecord != null){
				List<Publication> cachedPublicationList =  PublicationUtil.Instance.GetPublicationOffline ();
				Publication navigateToP = cachedPublicationList.Find(o => o.BookId == forwardBrowserRecord.BookID);
				if(forwardBrowserRecord.RecordType == RecordType.ContentRecord){
					AppDataUtil.Instance.SetCurrentPublication(navigateToP);
					ContentBrowserRecord contentBR = (ContentBrowserRecord)forwardBrowserRecord;
					AppDataUtil.Instance.SetOpendTOC(AppDataUtil.Instance.GetTOCNodeByID(contentBR.TOCID));
				}else if(forwardBrowserRecord.RecordType == RecordType.SearchResultRecord){
					SearchBrowserRecord searchBR = (SearchBrowserRecord)forwardBrowserRecord;
					AppDataUtil.Instance.SetCurrentPublication(navigateToP, searchBR.KeyWords);
					AppDataUtil.Instance.SetOpendTOC(AppDataUtil.Instance.GetTOCNodeByID(searchBR.TOCID));
				}else if(forwardBrowserRecord.RecordType == RecordType.AnnotationNavigator){
					//TODO, Back to annotation navigator
				}
			}
		}

	}
}
