using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using System.Runtime.InteropServices;

using Newtonsoft.Json;

using Foundation;
using AppKit;
using WebKit;
using CoreGraphics;
using ObjCRuntime;
using PdfKit;
using QuartzComposer;

using LexisNexis.Red.Mac.Data;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business.Pdf;
using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.Mac
{
	public partial class PageContentViewController : AppKit.NSViewController
	{
		public ShowMessageClickListener clickListener = new ShowMessageClickListener ();
		public string CurrentSearchHeader { get; set; }
		public bool IsPBOTitle { get; set; }
		public string SearchPageNumber { get; set; }
		private string TitleCountryCode { get; set; }
		#region Constructors

		// Called when created from unmanaged code
		public PageContentViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PageContentViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public PageContentViewController () : base ("PageContentView", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new PageContentView View {
			get {
				return (PageContentView)base.View;
			}
		}
		LoadingViewController loadViewCtr;
		float bodyFontSize = 13;
		bool isInfiniteLoading = false;
		bool isSelectedLoading = false;
		bool isScrollLoading = false;
		bool isHrefLoading = false;
		CGPoint menuLocation { get; set; }
		NSObject notificationProxy;
		public override void ViewDidLoad ()
		{
			bodyFontSize = GetFontSizeFromPlist ();
			if (Math.Abs (bodyFontSize) <= 0) {
				bodyFontSize = 13;
			}
				
			ContentWebView.Preferences.MinimumFontSize = 11;
			ContentWebView.Preferences.DefaultFontSize = (int)bodyFontSize;

			CustomView.WantsLayer = true;
			CustomView.Layer.BackgroundColor = Utility.ColorWithRGB(248,249,248,0.9f).CGColor;
			CustomView.Hidden = true;

			ContentWebView.FrameLoadDelegate = new ContentPageWebFrameLoadDelegate(this);
			ContentWebView.UIDelegate = new ContentPageWebUIDelegate ();

			//ContentWebView.MainFrame.FrameView.CanPrintHeadersAndFooters = true;
//			ContentWebView.UIShouldPerformActionfromSender = (WebView webView, Selector action, NSObject sender) => {
//				return false;
//			};
				
//			ContentWebView.UIGetContextMenuItems = (sender, forElements, defaultItems) => {
//				// Disable contextual (right click) menu for the webView
//				return null;
//			};


			AddScrollNotificationMonitor ();
		}

		public void AddScrollNotificationMonitor ()
		{
			notificationProxy = NSNotificationCenter.DefaultCenter.AddObserver (NSView.BoundsChangedNotification, 
				ScrollToBottomNotification);
		}

		public void RemoveScrollNotificationMonitor ()
		{
			if (notificationProxy != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (notificationProxy);
			}
		}

		public void InitalizeStatus ()
		{
			//Console.WriteLine ("InitalizeStatus");
			SetIndexBannerLetter (null, false);
			isInfiniteLoading = false;
			isSelectedLoading = false;
			isScrollLoading = false;
			isHrefLoading = false;
			IsPBOTitle = false;
			SearchPageNumber = null;
		}

		#region infinite scroll
		public void ScrollToBottomNotification (NSNotification notification)
		{
			if (View == null || View.Window == null) {
				return;
			}

			var controller = (PublicationsWindowController)View.Window.WindowController;
			if (controller == null || controller.ContentVC == null || controller.ContentVC.ViewMode == 2) {
				return;
			}

			var clipView = (NSClipView)notification.Object;

			if (clipView.DocumentView == null || 
				clipView.DocumentView.Class.Name != "WebHTMLView") {
				return;
			}

			var superViewClassName = clipView.Superview.Superview.Superview.Superview.Class.Name;
			if (superViewClassName == "LegalDefinePopView") {
				return;
			}

			nfloat yBottom = clipView.DocumentVisibleRect().Bottom;
			nfloat docHeight = clipView.DocumentRect.Height;
			nfloat yTop = clipView.DocumentVisibleRect ().Top;
			//Console.WriteLine ("yTop:{0}",yTop);
			//Console.WriteLine ("yBottom:{0}",yBottom);
			//Console.WriteLine ("docHeight:{0}",docHeight);
			//Console.WriteLine ("------");
			if (yBottom >= docHeight && yBottom != clipView.Frame.Height) {
				
				if (!isScrollLoading) {
					isScrollLoading = true;
				}

				LoadNextTocContent ();
			}
			else if (yTop == 0) {

				if (isScrollLoading && isSelectedLoading) {
					isSelectedLoading = false;
					isScrollLoading = false;
					return;
				} else if (isSelectedLoading) {
					isSelectedLoading = false;
					return;
				}
					
				if (!isScrollLoading) {
					isScrollLoading = true;
				}

				LoadPreTocContent ();  //select toc node can trigger this notification
			}

			if (IsPBOTitle) {
				//Console.WriteLine ("CallJs:listenCurrentPageNum");
				NSObject[] objects = new NSObject[] { new NSString ("listenCurrentPageNum") };
				CallJs ("listenCurrentPageNum", objects);
			}
		}
			
		public void LoadFinished(string message)
		{
			//Console.WriteLine ("LoadFinished");

			isInfiniteLoading = false;

			var window = View.Window;
			PublicationsWindowController winController = null;
			if (window == null) {
				winController = Utility.GetMainWindowConroller ();
			} else {
				winController = (PublicationsWindowController)View.Window.WindowController;
			}

			if (winController == null) {
				return;
			}

			var wordList =  winController.ContentVC.TOCDataManager.SearchTermWordList;

			if (wordList!= null) {
				HighlightKeyWords (wordList);
			}

			if (!string.IsNullOrEmpty (this.CurrentSearchHeader)) {
				NSObject[] objects = new NSObject[] { new NSString (this.CurrentSearchHeader) };
				this.CurrentSearchHeader = null;
				CallJs ("scrollToSearchHeaderPositon", objects);  //scrollToSearchContentPositon //scrollToSearchHeaderPositon
			}

			if (IsPBOTitle&&SearchPageNumber!=null) {
				//Console.WriteLine ("CallJs:scrollToSearchPage");
				NSObject[] objects = new NSObject[] { new NSString (SearchPageNumber) };
				CallJs ("scrollToSearchPage", objects);
			}
		}

		private async void LoadNextTocContent()
		{
			if (isInfiniteLoading) {
				return;
			}
			isInfiniteLoading = true;

			var controller = (PublicationsWindowController)View.Window.WindowController;
			string nextLeafNodeTitle = controller.ContentVC.GetNextTocNodeTitle ();
			//Console.WriteLine ("nextLeafNodeTit:{0}", nextLeafNodeTitle);
			if (nextLeafNodeTitle == null) {
				isInfiniteLoading = false;
				return;
			}

			NSObject[] objects = new NSObject[] {new NSString(nextLeafNodeTitle)};
			CallJs("addNextPageLoadingDiv", objects);

			//Console.WriteLine ("LoadNextTocContent");

			await controller.ContentVC.FetchNextPageAtTocNode ();
		}

		private async void LoadPreTocContent()
		{
			if (isInfiniteLoading) {
				return;
			}

			isInfiniteLoading = true;
			var controller = (PublicationsWindowController)View.Window.WindowController;
			string preLeafNodeTitle = controller.ContentVC.GetPreTocNodeTitle ();
			//Console.WriteLine ("preLeafNodeTit:{0}", preLeafNodeTitle);
			if (preLeafNodeTitle == null) {
				isInfiniteLoading = false;
				return;
			}

			NSObject[] objects = new NSObject[] {new NSString(preLeafNodeTitle)};
			CallJs("addPrePageLoadingDiv", objects);

			//Console.WriteLine ("LoadPreTocContent");

			await controller.ContentVC.FetchPrePageAtTocNode ();
		}

		public void AppendPageContent(string htmlString, int bookID, string tocID, string tocTitle)
		{
			if (!string.IsNullOrEmpty (htmlString)) {
				string divString = null;
				if (tocID!=null && tocTitle!=null) {
					divString = string.Format("<div id=\"{0}_{1}_{2}\" class=\"leafNodeContent\">",bookID,tocID,tocTitle);
				}

				string divHtmlString;
				if (divString != null) {
					divHtmlString = divString + htmlString + "</div>";
				} else {
					divHtmlString = htmlString;
				}

				//string jsonDowm = JsonConvert.SerializeObject (new {content=divHtmlString});

				NSObject[] objectss = new NSObject[] {new NSString(divHtmlString)};
				CallJs ("appendHtml_NextPage", objectss);
			}
		}

		public void PrependPageContent(string htmlString, int bookID, string tocID, string tocTitle)
		{
			if (!string.IsNullOrEmpty (htmlString)) {
				string divString = null;
				if (tocID!=null && tocTitle!=null) {
					divString = string.Format("<div id=\"{0}_{1}_{2}\" class=\"leafNodeContent\">",bookID,tocID,tocTitle);
				}

				string divHtmlString;
				if (divString != null) {
					divHtmlString = divString + htmlString + "</div>";
				} else {
					divHtmlString = htmlString;
				}

				NSObject[] objectss = new NSObject[] {new NSString(divHtmlString)};
				CallJs ("prependHtml_PrePage", objectss);
			}
		}

		public void HighlightKeyWords(List<string> keyWords)
		{
			var keyWordString = string.Join (" ", keyWords);
			NSObject[] objects = new NSObject[] {new NSString(keyWordString)};

			CallJs ("highlightSearchKeyword", objects);
		}
	
		public void CallJs(string function, NSObject[] arguments)
		{
			var result = ContentWebView.WindowScriptObject.CallWebScriptMethod (function, arguments);
			//Console.WriteLine ("CallJs:function:{0};result:{1}",function,result);
			if (result == null) {
				if (function == "scrollToSearchPage") {
					//this.SearchPageNumber = null;
				}
				return;
			}
			var aobject = JsonConvert.DeserializeObject<Dictionary<string,string>> (result.ToString());
			if (aobject == null) {
				if (function == "listenCurrentPageNum") {
					var controller = (PublicationsWindowController)View.Window.WindowController;
					controller.ContentVC.SetPageNumber ("");
				}
				return;
			}
			string key = aobject.Keys.ElementAt (0);
			if (key == "appendHtml_NextPage" || key == "prependHtml_PrePage") {
				isInfiniteLoading = false;
			}else if (key == "listenCurrentPageNum") {
				string pageNum = aobject.Values.ElementAt (0);
				var controller = (PublicationsWindowController)View.Window.WindowController;
				controller.ContentVC.SetPageNumber (pageNum);
			}
		}

		private void JSChangeFontSize ()
		{
			string jsString = string.Format ("document.body.style.fontSize={0}", bodyFontSize+5);
			ContentWebView.StringByEvaluatingJavaScriptFromString (jsString);

			//NSObject[] objectss = new NSObject[] {new NSString(bodyFontSize.ToString())};
			//CallJs ("setBodyFontSize", objectss);
		}
			
		private bool IsWebViewContentEmpty () 
		{
			var element = (DomHtmlElement)ContentWebView.MainFrame.DomDocument.DocumentElement;
			string htmlString = element.OuterHTML;
			if (string.IsNullOrEmpty (htmlString)) {
				return true;
			} else {
				return false;
			}
		}
		#endregion

		#region api with contentWebViewDelegate
		public void CustomScrollDrag(string message)
		{
			if (message == null || message.Length<15) {   //{TOCNodeID:35_3_Chapter One The Functions of the Judge and Jury}
				return;
			}
			//Console.WriteLine ("{0}",message);

			string divID = message.Substring (11);
			char[] separator = new char[]{'_'};
			var nodelist = divID.Split (separator);
			if (nodelist!=null && nodelist.Length > 2) {
				int nodeID = Int32.Parse (nodelist [1]);
				var controller = (PublicationsWindowController)View.Window.WindowController;
				controller.ContentVC.RefreshTocNodeByCurrentTocID (nodeID);	
			}
		}

		public async void ClickAHref (string message) 
		{
			//Console.WriteLine ("ClickAHref start message:{0}", message);
			if (isHrefLoading) {
				return;
			}
			isHrefLoading = true;

			string hyperlink;
			int iStart = message.IndexOf ("/Resources");
			if (iStart>=0) {
				hyperlink = "http://"+message.Remove (0, iStart + 11);
			}else {
				hyperlink = message;
			}

			int currentBookID = PublicationsDataManager.SharedInstance.CurrentPublication.BookId;
			var link = PublicationContentUtil.Instance.BuildHyperLink(currentBookID, hyperlink);
			if (link is IntraHyperlink) {
				int tocID = ((IntraHyperlink)link).TOCID;
				var controller = (PublicationsWindowController)View.Window.WindowController;
				await controller.ContentVC.ReloadHtmlByTocID (tocID);	
			} else if (link is ExternalHyperlink) {
				var urlString = ((ExternalHyperlink)link).Url;
				NSUrl url = new NSUrl (urlString);
				NSWorkspace.SharedWorkspace.OpenUrl (url);
			} else if (link is InternalHyperlink) {
				//Console.WriteLine ("InternalHyperlink");
				int tocID = ((InternalHyperlink)link).TOCID;
				int bookID = ((InternalHyperlink)link).BookID;
				var controller = (PublicationsWindowController)View.Window.WindowController;
				await controller.UpdateContentView (bookID, tocID);
			} else if (link is AttachmentHyperlink) {
				//Console.WriteLine ("AttachmentHyperlink");
				var fileType = ((AttachmentHyperlink)link).FileType;
				var fileName = ((AttachmentHyperlink)link).TargetFileName;
				string filePath = Utility.GetAppCacheAbsolutePath ()+fileName;
				NSWorkspace.SharedWorkspace.OpenFile (filePath);
			} else if (link is AnchorHyperlink) {
				//Console.WriteLine ("AnchorHyperlink");
				var anchorName = ((AnchorHyperlink)link).AnchorName;
				var url = new NSUrl (anchorName);
				NSWorkspace.SharedWorkspace.OpenUrl (url);

			} else if (!string.IsNullOrEmpty (message)) {

				string filePath = Utility.GetAppUserAbsolutePath () + "DLFiles" +"";
				filePath = message.Replace ("file://dl_folder", filePath);

				Console.WriteLine (message);
			}

			isHrefLoading = false;
		}

		public void CustomMouseUp(string message)
		{
			//Console.WriteLine ("message:{0}", message);
			var domRange = ContentWebView.SelectedDomRange;
			if (domRange == null) {
				return;
			}

			string selectText = domRange.Text;
			if (selectText==null || selectText.Length<=0) {
				return;
			}
				
			var aobject = JsonConvert.DeserializeObject<Dictionary<string,object>> (message);
			var values = aobject.Values;
			nfloat x = Int32.Parse (values.ElementAt(0).ToString());
			nfloat y = View.Frame.Height+50-Int32.Parse (values.ElementAt(1).ToString());


			CGPoint location = new CGPoint (x, y);
			menuLocation = new CGPoint (x, y);

			location = View.ConvertPointToView(location, null);

			bool islegal = Utility.IsValidDictionary();
			//islegal = false;
			ContentMenu.ItemAt (2).Hidden = !islegal;

			bool isHighlight = false;
			ContentMenu.ItemAt (4).Hidden = isHighlight;
			ContentMenu.ItemAt (5).Hidden = !isHighlight;
			ContentMenu.ItemAt (6).Hidden = !isHighlight;

			bool isNote = false;
			ContentMenu.ItemAt (7).Hidden = isNote;
			ContentMenu.ItemAt (8).Hidden = !isNote;
			ContentMenu.ItemAt (9).Hidden = !isNote;

			NSWindow window = NSApplication.SharedApplication.KeyWindow;
			if (window == null) {
				window = NSApplication.SharedApplication.MainWindow;
			}
			nint windowNumber = 1;
			if (window != null) {
				windowNumber = window.WindowNumber;
			} else {
				return;
			}
			NSEvent fakeMouseEvent = 
				NSEvent.MouseEvent (NSEventType.LeftMouseUp,
					location,
					(NSEventModifierMask)NSEventMask.LeftMouseUp,
					0,
					windowNumber,
					window.GraphicsContext, 0, 1, 1);
			NSMenu.PopUpContextMenu(ContentMenu,fakeMouseEvent,View);
		}

		public void ShowMessage(NSString message){
			NSAlert alert = new NSAlert();
			alert.MessageText = "Message from JavaScript";
			alert.InformativeText = (String) message;
			alert.RunModal();
		}
		#endregion

		#region action
		[Action ("LegalDefine:")]
		void LegalDefine (NSObject sender)
		{
			DomRange range = ContentWebView.SelectedDomRange;
			string selectedText = range.Text;
			//ContentWebView.Editable = true;
			ContentWebView.ReplaceSelectionWithMarkupString("this");
			//ContentWebView.Editable = false;
//			NSString *markupString = [NSString stringWithFormat:
//				@"<span style='color: red; font-style: italic'>%@</span>",
//				@”SomeString”];
//			[webView replaceSelectionWithMarkupString:markupString];

			this.Invoke (() => {
				var popover = new NSPopover ();
				popover.Behavior = NSPopoverBehavior.Transient;
				popover.ContentViewController = new LegalDefinePopViewController (selectedText, this.TitleCountryCode);
				CGRect rect = new CGRect(menuLocation.X,menuLocation.Y-60,5,5);
				popover.Show (rect, ContentWebView, NSRectEdge.MinYEdge);
			}, 0.0f);


		}

		[Action ("copy:")]
		void copy (NSObject sender)
		{
			var element = (DomHtmlElement)ContentWebView.MainFrame.DomDocument.DocumentElement;
			string htmlString = element.OuterHTML;

			var pboard = NSPasteboard.GeneralPasteboard;
			string range = ContentWebView.SelectedDomRange.Text;

			string[] typeArray = {"NSStringPboardType"};
			pboard.DeclareTypes(typeArray,this);
			pboard.SetStringForType(range,"NSStringPboardType");
		}

		[Action ("CopyContent:")]
		void CopyContent (NSObject sender)
		{
			var pboard = NSPasteboard.GeneralPasteboard;
			string range = ContentWebView.SelectedDomRange.Text;

			string[] typeArray = {"NSStringPboardType"};
			pboard.DeclareTypes(typeArray,this);
			pboard.SetStringForType(range,"NSStringPboardType");
		}

		[Action ("AddHighlightMenuClick:")]
		void AddHighlightMenuClick (NSObject sender)
		{
			this.Invoke (() => {
				var popover = new NSPopover ();
				popover.Behavior = NSPopoverBehavior.Transient;
				popover.ContentViewController = new AddHighlightViewController(popover);
				CGRect rect = new CGRect(menuLocation.X,menuLocation.Y-60,5,5);
				popover.Show (rect, ContentWebView, NSRectEdge.MinYEdge);
			}, 0.1f);
		}

		[Action ("EditHighlightMenuClick:")]
		void EditHighlightMenuClick (NSObject sender)
		{
			this.Invoke (() => {
				var popover = new NSPopover ();
				popover.Behavior = NSPopoverBehavior.Transient;
				popover.ContentViewController = new AddHighlightViewController(popover);
				CGRect rect = new CGRect(menuLocation.X,menuLocation.Y-60,5,5);
				popover.Show (rect, ContentWebView, NSRectEdge.MinYEdge);
			}, 0.1f);
		}

		[Action ("DeleteHighlightMenuClick:")]
		void DeleteHighlightMenuClick (NSObject sender)
		{
		}

		[Action ("AddNoteMenuClick:")]
		void AddNoteMenuClick (NSObject sender)
		{
			this.Invoke (() => {
				var popover = new NSPopover ();
				popover.Behavior = NSPopoverBehavior.Transient;
				popover.ContentViewController = new AddAnnotationViewController(popover);
				CGRect rect = new CGRect(menuLocation.X,menuLocation.Y-60,5,5);
				popover.Show (rect, ContentWebView, NSRectEdge.MinYEdge);
			}, 0.1f);
		}

		[Action ("EditAnnotate:")]
		void EditAnnotate (NSObject sender)
		{
			var element = (DomHtmlElement)ContentWebView.MainFrame.DomDocument.DocumentElement;
			string htmlString = element.OuterHTML;
			//string htmlText = element.OuterText;  //plainText:{1}
			//string range = ContentWebView.SelectedDomRange.Text;
			//Console.WriteLine ("htmlText:{0}", htmlString);

			string range = ContentWebView.SelectedDomRange.Text;
			string markupString = string.Format ("<span style='color: red; font-style: italic'>{0}</span>", range);

			ContentWebView.ReplaceSelectionWithMarkupString (markupString);
		}

		[Action ("EditAnnotateMenuClick:")]
		void EditAnnotateMenuClick (NSObject sender)
		{
			this.Invoke (() => {
				var popover = new NSPopover ();
				popover.Behavior = NSPopoverBehavior.Transient;
				popover.ContentViewController = new AddAnnotationViewController(popover);
				CGRect rect = new CGRect(menuLocation.X,menuLocation.Y-60,5,5);
				popover.Show (rect, ContentWebView, NSRectEdge.MinYEdge);
			}, 0.1f);
		}

		[Action ("DeleteAnnotateMenuClick:")]
		void DeleteAnnotateMenuClick (NSObject sender)
		{
		}

		#endregion

		#region public methods
		public void SetIndexBannerLetter (string indexValue, bool isShow)
		{
			if (isShow) {
				CustomView.Hidden = false;
				if (!string.IsNullOrEmpty (indexValue)) {
					if (indexValue.Length == 2) {
						InfoLabel.StringValue = indexValue.First ().ToString ();
					} else {
						InfoLabel.StringValue = "";
					}
				}
			} else {
				CustomView.Hidden = true;
			}
		}

		public void SetPageContentEmpty(bool isShow)
		{
			isSelectedLoading = true;
			if (isShow) {
				ContentWebView.MainFrame.LoadHtmlString ("", null);
				if (ContentWebView.Hidden) {
					ContentWebView.Hidden = false;
				}

				CurrentSearchHeader = null;

			} else {
				ContentWebView.Hidden = true;
			}
		}
			
		public void ShowPageContent(string htmlString, bool isShow, int bookID, string tocID, string tocTitle)
		{
			isSelectedLoading = true;
			if (isShow) {
				ContentWebView.Hidden = false;
				if (!string.IsNullOrEmpty (htmlString)) {

					if (File.Exists ("htmlTemplate/htmlDoc.html")) {
						var contentString = File.ReadAllText ("htmlTemplate/htmlDoc.html");
						string divString = null;
						if (tocID!=null && tocTitle!=null) {
							divString = string.Format("<div id=\"{0}_{1}_{2}\" class=\"leafNodeContent\">",bookID,tocID,tocTitle);
					    }

						string divHtmlString;
						if (divString != null) {
							divHtmlString = divString + htmlString + "</div>";
						} else {
							divHtmlString = htmlString;
						}

						contentString = contentString.Replace ("#CONTENT#", divHtmlString);
						contentString = contentString.Replace ("#FONTSIZE#", bodyFontSize.ToString());
						ContentWebView.MainFrame.LoadHtmlString (contentString, NSBundle.MainBundle.ResourceUrl);

						GetTOCCountryCode (htmlString);
					}
				}
				RemoveLoadView ();
			} else {
				ContentWebView.Hidden = true;
			}
		}

		private string GetTOCCountryCode(string strHtml)
		{
			string CountryCode = "AU";

			int index = strHtml.IndexOf ("data-doc.country=\"");
			if (index > 0) {
				int startIndex = index + "data-doc.country=\"".Length;
				CountryCode = strHtml.Substring (startIndex, 2);
			}

			this.TitleCountryCode = CountryCode;

			return CountryCode;
		}

		public int ZoomInFontSize () 
		{
			if (bodyFontSize >= 17) {
				return ContentWebView.Preferences.DefaultFontSize;
			}

			ContentWebView.Preferences.DefaultFontSize ++;
			bodyFontSize += 1;

			JSChangeFontSize ();

			SaveFontSizeToPlist ();

			return ContentWebView.Preferences.DefaultFontSize;
		}

		public int ZoomOutFontSize () 
		{
			if (bodyFontSize <= 11) {
				return ContentWebView.Preferences.DefaultFontSize;
			}

			ContentWebView.Preferences.DefaultFontSize --;
			bodyFontSize -= 1;

			JSChangeFontSize ();

			SaveFontSizeToPlist ();
			return ContentWebView.Preferences.DefaultFontSize;
		}

		public int FitFontSize() 
		{
			ContentWebView.Preferences.DefaultFontSize = 13;

			bodyFontSize = 13;

			JSChangeFontSize ();

			return ContentWebView.Preferences.DefaultFontSize;
		}

		void SaveFontSizeToPlist () 
		{
			SettingsUtil.Instance.SaveFontSize (bodyFontSize);
			return;
		}

		float GetFontSizeFromPlist ()
		{
			return (float)SettingsUtil.Instance.GetFontSize ();
		}
		#endregion

		#region private methods
		public void AddLoadView ()
		{
			if (loadViewCtr == null) {

				var frame = View.Frame;
				var location = new CGPoint ();
				location.X = (frame.Width - 200) / 2;
				location.Y = (frame.Height-100)/2;

				loadViewCtr = new LoadingViewController (location, LNRConstants.LOADING_INFO);
			}

			loadViewCtr.View.WantsLayer = true;
			View.WantsLayer = true;
			View.Layer.AddSublayer (loadViewCtr.View.Layer);
		}

		public void RemoveLoadView ()
		{
			NSRunLoop.Current.RunUntil(NSRunLoopMode.Default,NSDate.FromTimeIntervalSinceReferenceDate(5));

			if (loadViewCtr != null) {
				loadViewCtr.View.Layer.RemoveFromSuperLayer ();
				loadViewCtr.Dispose ();
				loadViewCtr = null;
			}
		}
			
		// Install the click handler for the Show Message HTML button
		public void installClickHandlers()
		{
			var dom = ContentWebView.MainFrameDocument;
			var element = dom.GetElementById ("message_button");
			element.AddEventListener ("click", clickListener, true);
		}
		#endregion
	
		#if false
		async public Task SavehtmlToPDF()
		{
	     
//			var savePaenl = new NSSavePanel ();
//			savePaenl.CanCreateDirectories = true;
//			savePaenl.CanSelectHiddenExtension = false;
//			savePaenl.Message = "Choose the path to save the document";
//			savePaenl.Prompt = "Save";
//			savePaenl.NameFieldStringValue = "LexisNexis-Document";
//
//			string title = PublicationsDataManager.SharedInstance.CurrentPublication.Name;
//			
//			savePaenl.BeginSheet (NSApplication.SharedApplication.KeyWindow, (nint result) => {
//				if (result == 1) {
//					string fileName = savePaenl.Filename;
//					int length = fileName.Length;
//					if (length>4 && fileName.Substring(length-3).ToLower()=="pdf") {
//					}else {
//						fileName += ".pdf";
//					}
//
//					string filePath = Path.Combine (Utility.GetAppCacheAbsolutePath(), fileName);
//				}
//			});
		}

		#endif

		#if true
		async public Task SavehtmlToPDF()
		{
			var savePaenl = new NSSavePanel ();
			savePaenl.CanCreateDirectories = true;
			savePaenl.CanSelectHiddenExtension = false;
			savePaenl.Message = "Choose the path to save the document";
			savePaenl.Prompt = "Save";
			savePaenl.NameFieldStringValue = "LexisNexis-Document";

			string title = PublicationsDataManager.SharedInstance.CurrentPublication.Name;


			savePaenl.BeginSheet (NSApplication.SharedApplication.KeyWindow, (nint result) => {
				if (result == 1) {

					PdfDocument pdfDocument = new PdfDocument();

					CGRect documentRect = ContentWebView.MainFrame.FrameView.DocumentView.Frame;
					CGRect frameRect = ContentWebView.MainFrame.FrameView.Frame;
					CGRect pageRect = new CGRect(0,0,frameRect.Width,frameRect.Height);
					CGRect pRect = ContentWebView.MainFrame.FrameView.DocumentView.RectForPage(1);
					int index = 0;
					while(pageRect.Y<documentRect.Height) {
						var pdfData = ContentWebView.MainFrame.FrameView.DocumentView.DataWithPdfInsideRect (pageRect); 
						var image = new NSImage(pdfData);
						string footer = "Currency date: " + PublicationsDataManager.SharedInstance.CurrentPublication.CurrencyDate.Value.ToString ("dd MMM yyyy") +
							"                " +
							"© 2015 LexisNexis"+
							"                          " +
							"Printed page "+
							string.Format("{0}",index+1);
						var footerPage = new FooterPdfPage(image);
						footerPage.PageHeader = title;
						footerPage.PageFooter = footer;

						pdfDocument.InsertPage(footerPage,index);

						index++;
						pageRect.Offset(0,frameRect.Height);
					}
						
					string fileName = savePaenl.Filename;
					int length = fileName.Length;
					if (length>4 && fileName.Substring(length-3).ToLower()=="pdf") {
					}else {
						fileName += ".pdf";
					}

					string filePath = Path.Combine (Utility.GetAppCacheAbsolutePath(), fileName);

					pdfDocument.Write (filePath);
				}
			});
		}
		#endif

		#if false
		public void SavehtmlToPDF()
		{
			var savePaenl = new NSSavePanel ();
			savePaenl.CanCreateDirectories = true;
			savePaenl.CanSelectHiddenExtension = false;
			savePaenl.Message = "Choose the path to save the document";
			savePaenl.Prompt = "Save";
			savePaenl.NameFieldStringValue = "LexisNexis-Document";

			savePaenl.BeginSheet (NSApplication.SharedApplication.KeyWindow, (nint result) => {
				if (result == 1) {
					var pdfData = (NSData)ContentWebView.MainFrame.FrameView.DocumentView.DataWithPdfInsideRect (
						ContentWebView.MainFrame.FrameView.DocumentView.Frame); 
						
					//PdfDocument pdfDocument = new PdfDocument(pdfData);

					string fileName = savePaenl.Filename;
					int length = fileName.Length;
					if (length>4 && fileName.Substring(length-3).ToLower()=="pdf") {
					}else {
						fileName += ".pdf";
					}

//					for(int i=0; i<pdfDocument.PageCount; i++) {
//						var page = pdfDocument.GetPage(i);
//					}

					string filePath = Path.Combine (Utility.GetAppCacheAbsolutePath(), fileName);

					pdfData.Save(filePath,true);
					//pdfDocument.Write (filePath);
				}
			});
		}
		#endif

		public void PrintPDF(NSUrl fileUrl) 
		{
			// Create the print settings.
			ContentWebView.MainFrame.FrameView.PrintDocumentView();
			//printOperationWithPrintInfo: printInfo]
				
			NSPrintInfo sharePrintInfo = NSPrintInfo.SharedPrintInfo;
			var sharedDict = sharePrintInfo.Dictionary;

			NSMutableDictionary printInfoDict = NSMutableDictionary.FromDictionary(sharedDict);
			printInfoDict.Add (NSObject.FromObject("NSPrintHeaderAndFooter"),NSObject.FromObject (true));
			printInfoDict.Add (NSObject.FromObject("NSPrintJobDisposition"), NSObject.FromObject("NSPrintSpoolJob"));  //  NSPrintSaveJob

			var printInfo = new NSPrintInfo (printInfoDict);
			printInfo.TopMargin = 72.0f;
			printInfo.BottomMargin = 72.0f;
			printInfo.LeftMargin = 0.0f;
			printInfo.RightMargin = 0.0f;
			printInfo.HorizontalPagination = NSPrintingPaginationMode.Fit;
			printInfo.VerticalPagination = NSPrintingPaginationMode.Auto;
			printInfo.HorizontallyCentered = true;
			printInfo.VerticallyCentered = false;
			printInfo.ScalingFactor = 0.5f;
			var rect = printInfo.ImageablePageBounds;
			// Create the document reference.
			var pdfData = ContentWebView.MainFrame.FrameView.DocumentView.DataWithPdfInsideRect (
				ContentWebView.MainFrame.FrameView.DocumentView.Frame);  

			PdfDocument pdfDocument = new PdfDocument(pdfData);

			PdfView pdfView = new PdfView ();
			pdfView.SetFrameSize (ContentWebView.MainFrame.FrameView.Frame.Size);
			pdfView.SetFrameOrigin (new CGPoint (0, 0));
			pdfView.Document = pdfDocument;
			
			var op = NSPrintOperation.FromView (pdfView.DocumentView, printInfo);
			op.ShowsProgressPanel = true;
			op.ShowsPrintPanel = true;
			op.RunOperation ();

			//op.RunOperationModal(View.Window,null);
		}

		#if false
		public void PrintPDF(NSUrl fileUrl) 
		{
			// Create the print settings.

			NSPrintInfo printInfo = NSPrintInfo.SharedPrintInfo;
			printInfo.TopMargin = 0.0f;
			printInfo.BottomMargin = 0.0f;
			printInfo.LeftMargin = 0.0f;
			printInfo.RightMargin = 0.0f;
			printInfo.HorizontalPagination = NSPrintingPaginationMode.Fit;
			printInfo.VerticalPagination = NSPrintingPaginationMode.Auto;
			printInfo.HorizontallyCentered = true;
			printInfo.VerticallyCentered = false;
			printInfo.JobDisposition = "NSPrintSaveJob";

            NSPrintInfo sharePrintInfo = NSPrintInfo.SharedPrintInfo;

            var sharedDict = sharePrintInfo.Dictionary;

            NSMutableDictionary printInfoDict = NSMutableDictionary.FromDictionary(sharedDict);
			printInfoDict.Add (NSObject.FromObject("NSPrintHeaderAndFooter"),NSObject.FromObject (true));
            printInfoDict.Add (NSObject.FromObject("NSPrintJobDisposition"), NSObject.FromObject("NSPrintSaveJob"));
            var printInfo = new NSPrintInfo (printInfoDict);

			// Create the document reference.
			var pdfData = ContentWebView.MainFrame.FrameView.DocumentView.DataWithPdfInsideRect (
				ContentWebView.MainFrame.FrameView.DocumentView.Frame);  

			PdfDocument pdfDocument = new PdfDocument(pdfData);

			PdfView pdfView = new PdfView ();
			pdfView.SetFrameSize (ContentWebView.MainFrame.FrameView.Frame.Size);
			pdfView.SetFrameOrigin (new CGPoint (0, 0));
			pdfView.Document = pdfDocument;

			var op = NSPrintOperation.FromView (pdfView.DocumentView, printInfo);
			op.ShowsProgressPanel = true;
			op.ShowsPrintPanel = true;
			op.RunOperation ();
			//op.RunOperationModal(View.Window,null);
		}
		#endif
	}

	public class ShowMessageClickListener : DomEventListener {

		// The user just clicked the Show Message NSButton, so we show him/her
		// a greeting. This code shows you how to execute C# code when a click is done in
		// and HTML button.
		public override void HandleEvent (DomEvent evt)
		{
			var alert = new NSAlert () {
				MessageText = "Hello there",
				InformativeText = "Saying hello from C# code. Event type: " + evt.Type
			};
			alert.RunModal();
		}
	}

	class AutoPinner : IDisposable
	{
		GCHandle _pinnedArray;
		public AutoPinner(Object obj)
		{
			_pinnedArray = GCHandle.Alloc(obj, GCHandleType.Pinned);
		}
		public static implicit operator IntPtr(AutoPinner ap)
		{
			return ap._pinnedArray.AddrOfPinnedObject(); 
		}
		public void Dispose()
		{
			_pinnedArray.Free();
		}
	}
}