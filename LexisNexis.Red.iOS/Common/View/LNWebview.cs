using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreGraphics;
using ObjCRuntime;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;

using Xamarin;

namespace LexisNexis.Red.iOS
{
	[Register ("LNWebview")]
	public class LNWebview : UIWebView
	{

		public override bool CanBecomeFirstResponder {
			get {
				return true;
			}
		}
 
		public LNWebview (IntPtr handle) : base(handle)
		{
 			InitWebView ();
  		}

		public LNWebview () : base()
		{ 
			InitWebView ();
		}

		private void InitWebView()
		{
			var menuController = UIMenuController.SharedMenuController;
			List<UIMenuItem> menuItemList = new List<UIMenuItem>{ 
				new UIMenuItem ("Legal Define", new Selector("LegalDefineText")), 
				new UIMenuItem("Copy", new Selector("CopyText")),
				new UIMenuItem ("Note", new Selector("NoteText")), 
				new UIMenuItem ("Highlight", new Selector("HighlightText")) };
			#if !RELEASE
			menuItemList.Add(new UIMenuItem("ViewSource", new Selector("ViewSource")));
			#endif
			menuController.MenuItems = menuItemList.ToArray();

			Delegate = new LNWebViewDelegate ();
			RegisterAsNotificationObserver ();
		}


		public override bool CanPerform (Selector action, NSObject withSender)
		{
			if (action == new Selector ("LegalDefineText"))
				return true;
			if (action == new Selector ("NoteText"))
				return true;
			if (action == new Selector ("HighlightText"))
				return true;
			if (action == new Selector ("CopyText"))
				return true;
			#if !RELEASE
			if (action == new Selector ("ViewSource"))
				return true;
			#endif

			return false;
		}

		[Export("LegalDefineText")]
		public void LegalDefineText ()
		{
			string selectedText = EvaluateJavascript("window.getSelection().toString()");

			CGRect selecteTextFrame = GetSelectedTextFrame ();
			CGRect textFrame = new CGRect (selecteTextFrame.X, selecteTextFrame.Y+44, selecteTextFrame.Width, selecteTextFrame.Height);

			LegalDefineViewController legalVC = new LegalDefineViewController ();
			legalVC.selectText = selectedText;
			UIPopoverController legalDefineVC = new UIPopoverController(legalVC);
			legalDefineVC.SetPopoverContentSize(new CGSize(320, 460), true);
			legalDefineVC.BackgroundColor = UIColor.White;
			AppDisplayUtil.Instance.SetPopoverController (legalDefineVC);
			legalDefineVC.PresentFromRect (textFrame,  Superview, UIPopoverArrowDirection.Any, true);

		}

		[Export("CopyText")]
		public void CopyText ()
		{
			string selectedText = EvaluateJavascript("window.getSelection().toString();");
			UIPasteboard.General.String = selectedText;
		}

		[Export("NoteText")]
		public void NoteText ()
		{
			this.NoteTextController ();	
		}

		[Export("HighlightText")]
		public void HighlightText ()
		{
			CGRect selecteTextFrame = GetSelectedTextFrame ();
			CGRect textFrame = new CGRect (selecteTextFrame.X, selecteTextFrame.Y+44, selecteTextFrame.Width, selecteTextFrame.Height);
			HighlightTableViewControllerController highLightVC = new HighlightTableViewControllerController ();
			UINavigationController highLightNav = new UINavigationController (highLightVC);
			highLightNav.View.BackgroundColor = UIColor.White;
			highLightNav.NavigationBar.TintColor = UIColor.Red;
			UIPopoverController highPopoverController = new UIPopoverController (highLightNav);
			highPopoverController.PopoverContentSize = new CGSize (320, 328);
			AppDisplayUtil.Instance.SetPopoverController (highPopoverController); 
			highPopoverController.PresentFromRect (textFrame,Superview,UIPopoverArrowDirection.Any,true);
		}

		#if !RELEASE
		[Export("ViewSource")]
		public void ViewSource ()
		{
 			CGRect selecteTextFrame = GetSelectedTextFrame ();

			UIViewController htmlSourceController = new UIViewController ();
			htmlSourceController.View.Frame = new CGRect (0, 0, 320, 320);
			UITextView htmlTextView = new UITextView(new CGRect(0, 0, 320, 320));
			htmlTextView.Text = EvaluateJavascript ("(function (){if (window.getSelection) {var range=window.getSelection().getRangeAt(0);var container = document.createElement('div');container.appendChild(range.cloneContents());return container.innerHTML;}})();");
			htmlSourceController.View.AddSubview (htmlTextView);

			UIPopoverController showHtmlSourcePopover = new UIPopoverController(htmlSourceController);
			showHtmlSourcePopover.BackgroundColor = UIColor.White;
			showHtmlSourcePopover.PopoverContentSize = new CGSize (320, 320);
			AppDisplayUtil.Instance.SetPopoverController (showHtmlSourcePopover);
			showHtmlSourcePopover.PresentFromRect (selecteTextFrame,  Superview, UIPopoverArrowDirection.Any,true);
		}
		#endif


		private void AddNoteTableView(NSNotification obj){
			this.NoteTextController ();	

		}

		private void RegisterAsNotificationObserver()
		{

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("addNoteTableView"), AddNoteTableView);

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("changeFont"), delegate(NSNotification obj) {
				if(obj.UserInfo != null ){
					NSObject size = obj.UserInfo.ObjectForKey(new NSString ("size"));
					string jsStr = string.Format("document.getElementsByTagName(\"body\")[0].style.fontSize=\"{0}px\";", size);
					this.EvaluateJavascript(jsStr);
				}
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("HighlightContentSearchKeyword"), delegate(NSNotification obj) {
				if(obj.UserInfo != null ){
					NSObject contentSearchKeyword = obj.UserInfo.ObjectForKey(new NSString ("ContentSearchKeyword"));
					string jsStr = string.Format("highlightSearchKeyword(\"{0}\");", contentSearchKeyword);
					this.EvaluateJavascript(jsStr);
				}
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("scrollToHeading"), delegate(NSNotification obj) {//sent from search result
				if(obj.UserInfo != null ){
					NSObject headingType = obj.UserInfo.ObjectForKey(new NSString ("headingType"));
					NSObject headingNum = obj.UserInfo.ObjectForKey(new NSString ("headingNum"));
					NSObject tocId = obj.UserInfo.ObjectForKey(new NSString ("tocId"));

					string jsStr = string.Format("$(window).scrollTo($(\".page_container[data-tocid='{0}'] {1}:eq({2})\"), 300);", tocId, headingType, headingNum);
					this.EvaluateJavascript(jsStr);
				}
			});


		}

		private void NoteTextController(){
			CGRect selecteTextFrame = GetSelectedTextFrame ();
			CGRect textFrame = new CGRect (selecteTextFrame.X, selecteTextFrame.Y+44, selecteTextFrame.Width, selecteTextFrame.Height);
			UIStoryboard storyboard = UIStoryboard.FromName ("NoteTextNavgation", NSBundle.MainBundle);
			NoteViewController controllerVC = (NoteViewController)storyboard.InstantiateViewController ("NoteTextNavgationer");
			UINavigationController navController = new UINavigationController (controllerVC);
			navController.NavigationBar.Translucent = false;
			navController.EdgesForExtendedLayout = UIRectEdge.None;
			UIPopoverController tagManagePopoverController = new UIPopoverController(navController);
			tagManagePopoverController.BackgroundColor = UIColor.White;
			AppDisplayUtil.Instance.SetPopoverController (tagManagePopoverController);
			tagManagePopoverController.PresentFromRect (textFrame,  Superview,UIPopoverArrowDirection.Any,true);

		}

		private CGRect GetSelectedTextFrame()
		{
			try{
				float x = float.Parse(EvaluateJavascript ("window.getSelection().getRangeAt(0).getBoundingClientRect().left")); 
				float y = float.Parse(EvaluateJavascript ("window.getSelection().getRangeAt(0).getBoundingClientRect().top"));
				float width = float.Parse(EvaluateJavascript ("window.getSelection().getRangeAt(0).getBoundingClientRect().width"));
				float height = float.Parse(EvaluateJavascript ("window.getSelection().getRangeAt(0).getBoundingClientRect().height")) ;
				return new CGRect (x, y, width, height);
			}catch(FormatException fe){
				Insights.Report(fe, new Dictionary<string, string>{
					{"Exception summary","Get selected text rect error"},
					{"Publication ID", AppDataUtil.Instance.GetCurrentPublication().BookId.ToString()},
					{"Publication Name", AppDataUtil.Instance.GetCurrentPublication().Name},
					{"Country", GlobalAccess.Instance.CurrentUserInfo.Country.CountryName},
					{"Email", GlobalAccess.Instance.CurrentUserInfo.Email},
				});
				return new CGRect (0, 0, 0, 0);
			}
		}

	}
}

