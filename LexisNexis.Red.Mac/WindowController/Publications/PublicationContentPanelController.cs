
using System;
using Foundation;
using AppKit;

using LexisNexis.Red.Mac.Data;
using CoreGraphics;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using System.IO;
using WebKit;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac
{
	public partial class PublicationContentPanelController: AppKit.NSWindowController 
	{
		#region Constructors

		// Called when created from unmanaged code
		public PublicationContentPanelController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PublicationContentPanelController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public PublicationContentPanelController (int bookID, string bookTitle, bool isExpired, string currencyDate, CGRect newFrame) : base ("PublicationContentPanel")
		{
			BookTitle = bookTitle;
			BookID = bookID;
			IsExpired = isExpired;
			CurrencyDate = currencyDate;
			Window.SetFrame (newFrame, true);

			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			//Window.Title = "Publication Content";
			Window.BackgroundColor = NSColor.White;
			Window.TitleVisibility = NSWindowTitleVisibility.Hidden;
			Window.MakeFirstResponder (null);
		}

		#endregion

		#region properties 
		//strongly typed window accessor
		public new PublicationContentPanel Window {
			get {
				return (PublicationContentPanel)base.Window;
			}
		}

		public string BookTitle { get; set;}
		public int BookID { get; set;}
		public bool IsExpired { get; set;}
		public bool IsFTC { get; set;}
		public string CurrencyDate { get; set;}

		public PublicationTOCDataManager TOCDataManager;
		public TocViewController TOCController {get; set;}

		public IndexDataManager IdxDataManager;

		public PageContentViewController PageController{ get; set;}

		public bool IsSwitchAnnotation { get; set;}
		nfloat CONTENTPAGE_WIDTH = 704;
		nfloat sidebarViewWidth = 260;
		#endregion

		#region public methods

		public override void WindowDidLoad ()
		{
			TOCController = TOCViewController;
			PageController = PageViewController;

			//
			LeftButton.Image = NSImage.ImageNamed ("NSGoLeftTemplate");
			RightButton.Image = NSImage.ImageNamed ("NSGoRightTemplate");

			SplitSwithButton.Cell.ImageScale = NSImageScale.None;
			SplitSwithButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/Sidebar-Icon.png");

			InfoButton.Cell.ImageScale = NSImageScale.None;
			InfoButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/Info-Icon.png");

			ShareButton.Cell.ImageScale = NSImageScale.None;
			//ShareButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/Share-Icon.png");
			ShareButton.Image = NSImage.ImageNamed("NSShareTemplate");

			HistoryButton.Cell.ImageScale = NSImageScale.None;
			HistoryButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/History-Icon.png");

			//
			ContentButton.Cell.Bordered = false;
			ContentButton.Cell.SetButtonType (NSButtonType.MomentaryChange); 

			IndexButton.Cell.Bordered = false;
			IndexButton.Cell.SetButtonType (NSButtonType.MomentaryChange); 

			AnnotationButton.Cell.Bordered = false;
			AnnotationButton.Cell.SetButtonType (NSButtonType.MomentaryChange);

			Window.DidResize += HandleWindowDidResize;

			var attributedTitle = Utility.AttributeTitle ("Go to", NSColor.Red, 14);
			GotoButton.AttributedTitle = attributedTitle;
			var alterTitle = Utility.AttributeTitle ("Go to", NSColor.Black, 14);
			GotoButton.AttributedAlternateTitle = alterTitle;
			GotoButton.Hidden = true;

			NSNotificationCenter.DefaultCenter.AddObserver (LNRConstants.LNChangeHistoryItemNotification, 
				HandleHistoryNotification);
		}

		public void HandleHistoryNotification (NSNotification notification)
		{
			var tocID = Int32.Parse(notification.Object.ToString());
			//Console.WriteLine ("nodeID:{0}",nodeID);
			TOCNode rootTocNode = TOCDataManager.RootNodeList[0].ParentNode;
			TOCNode currentTocNode = PublicationContentUtil.Instance.GetTOCByTOCId(tocID,rootTocNode);
			if (currentTocNode != null) {
				OpenPublicationContentAtTOCNode (BookID, currentTocNode);
				TOCDataManager.GetTOCNodeListByCurrentLeafNode (currentTocNode);
				TOCDataManager.CurrentLeafNode = currentTocNode;
				TOCViewController.RefreshTableViewData ();
				TOCViewController.HighlightTOCRectByTocNode (currentTocNode.TocID);
			}
		}

		public void IntializeControlState (CGRect newFrame)
		{
			if (IsFTC) {
				int location = BookTitle.IndexOf ("+ Cases");
				if (location < 0) {
					location = BookTitle.IndexOf ("+Case");
					if (location < 0) {
						BookTitle += " + Cases";
					}
				}
			}

			if (BookTitle!=null) {
				Window.Title = BookTitle;
				TitleTField.StringValue = BookTitle;
				TitleTField.ToolTip = BookTitle;
			}

			SegmentContol.SetSelected (false, 0);

			AddInfoUpdateState ();

			Window.SetFrame (newFrame, false);
		}

		public void InitializeContentPage() 
		{
			ContentButtonClick (ContentButton);
		}

		public void RefreshTOCViewData ()
		{
			TOCViewController.RefreshTableViewData ();
		}

		public void RefreshIndexViewData ()
		{
			IndexViewController.RefreshOutlineViewData ();
		}

		public async void OpenPublicationIndexAtIndexNode (Index indexNode)
		{
			if (indexNode == null) {
				PageViewController.SetIndexBannerLetter (null, false);
				PageViewController.ShowPageContent(null, false);
			} else {
				if (!string.IsNullOrEmpty (indexNode.Title)) {
					PageViewController.SetIndexBannerLetter (indexNode.Title, true);
				}

				if (!string.IsNullOrEmpty(indexNode.FileName)) {
					string htmlString = await PublicationContentUtil.Instance.GetContentFromIndex (BookID, indexNode);
					PageViewController.ShowPageContent(htmlString, true);
				}
			}
		}

		public async void OpenPublicationContentAtTOCNode (int bookID, TOCNode tocNode)
		{
			string htmlString = await PublicationContentUtil.Instance.GetContentFromTOC (bookID, tocNode);
			PageViewController.ShowPageContent(htmlString, true);
		}

		public void OpenInfoModal ()
		{
			CGPoint orgPoint = Utility.GetModalPanelLocation();
			using (NSAutoreleasePool pool = new NSAutoreleasePool ()) {
				using (var infoPanelController = new PublicationInfoPanelController (orgPoint)) {
					infoPanelController.BookInfo = PublicationsDataManager.SharedInstance.CurrentPubliction;
					infoPanelController.InitializeInfoView ();

					var infoPanel = infoPanelController.Window;
					infoPanel.MakeFirstResponder (null);

					NSApplication NSApp = NSApplication.SharedApplication;

					infoPanel.WindowShouldClose += (t) => true;
					infoPanel.WillClose += delegate(object asender, EventArgs e) {
						NSApp.StopModal ();
					};

					NSApplication.SharedApplication.RunModalForWindow (infoPanel);
					infoPanelController.Window.OrderOut (null);
					infoPanelController.Dispose ();

					AddInfoUpdateState ();
				}
			}
		}

		#endregion

		public int ZoomInFontSize () 
		{
			return PageController.ZoomInFontSize();
		}

		public int ZoomOutFontSize () 
		{
			return PageController.ZoomOutFontSize();
		}

		public int FitFontSize() 
		{
			return PageController.FitFontSize();
		}


		#region Action
		partial void SegmentClick (NSObject sender)
		{
			var control = (NSSegmentedControl)sender;
			nint index = control.SelectedSegment;
			if (index == 0) {
				IsSwitchAnnotation = false;
				Window.PerformClose(sender);
			}else {
				IsSwitchAnnotation = true;
				Window.PerformClose(sender);
			}
		}

		partial void SplitButtonClick (NSObject sender)
		{
			if (ContentButton.State == NSCellStateValue.On) {
				//SetSidebarImageByState(TocCustomView.Hidden);
				if (TocCustomView.Hidden) {
					TocCustomView.Hidden = false;
					RelayoutTOCCustomView (true);
				} else {
					TocCustomView.Hidden = true;
					RelayoutTOCCustomView (false);
				}

			}else if (IndexButton.State == NSCellStateValue.On) {
				//SetSidebarImageByState(IndexCustomView.Hidden);
				if (IndexCustomView.Hidden) {
					IndexCustomView.Hidden = false;
					RelayoutIndexCustomView (true);
				} else {
					IndexCustomView.Hidden = true;
					RelayoutIndexCustomView (false);
				}
			}else {
				//SetSidebarImageByState(AnnotationView.Hidden);
				if (AnnotationView.Hidden) {
					AnnotationView.Hidden = false;
					RelayoutAnnotaionView (true);
				} else {
					AnnotationView.Hidden = true;
					RelayoutAnnotaionView (false);
				}
			}
		}

		private void SetSidebarImageByState(bool isStateOn)
		{
			if (!isStateOn) {
				SplitSwithButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/Sidebar-Icon-White.png");
			} else {
				SplitSwithButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/Sidebar-Icon.png");
			}
		}

		private void HidePreNextButton (bool isHide)
		{
			LeftButton.Hidden = isHide;
			RightButton.Hidden = isHide;
			if (AnnotationButton.State == NSCellStateValue.On) {
				GotoButton.Hidden = false;
			} else {
				GotoButton.Hidden = true;
			}

			PageNumber.Hidden = isHide;
		}
			
		partial void InfoButtonClick (NSObject sender)
		{
			OpenInfoModal ();
		}

		partial void ShareButtonClick (NSObject sender)
		{
		}

		partial void HistoryButtonClick (NSObject sender)
		{
			var popover = new NSPopover ();
			popover.Behavior = NSPopoverBehavior.Transient;
			popover.ContentViewController = new HistoryPopoverController (popover);
			popover.Show (new CGRect (0, 0, 0, 0), (NSView)sender, NSRectEdge.MinYEdge);
		}

		partial void PreButtonClick (NSObject sender)
		{
		}

		partial void NextButtonClick (NSObject sender)
		{
		}

		partial void GotoPageNumber (NSObject sender)
		{
		}
			
		partial void ContentButtonClick (NSObject sender)
		{
			ContentButton.State = NSCellStateValue.On;
			IndexButton.State = NSCellStateValue.Off;
			AnnotationButton.State = NSCellStateValue.Off;

			HidePreNextButton(false);

			if (TOCDataManager == null) {
				TOCDataManager = new PublicationTOCDataManager (BookID, BookTitle, this);
				TOCViewController.BookID = BookID;
				TOCViewController.IsExpired = IsExpired;
				TOCViewController.CurrencyDate = CurrencyDate;
				TOCViewController.TOCDataManager = TOCDataManager;
				TOCViewController.InitializeTableView ();
			}

			PageViewController.SetIndexBannerLetter (null, false);

			if (TOCDataManager.CurrentLeafNode == null) {
			    PageViewController.ShowPageContent(null, true);
			} else {
				OpenPublicationContentAtTOCNode(BookID, TOCDataManager.CurrentLeafNode);
			}

			SetSideBarViewShowByMode();
				
			SetButtonAttributedTitle(ContentButton,LNRConstants.TITLE_CONTENT,true);
			SetButtonAttributedTitle(IndexButton,LNRConstants.TITLE_INDEX,false);
			SetButtonAttributedTitle(AnnotationButton,LNRConstants.TITLE_ANNOTATIONS,false);

		}
			
		async partial  void IndexButtonClick (NSObject sender)
		{
			ContentButton.State = NSCellStateValue.Off;
			IndexButton.State = NSCellStateValue.On;
			AnnotationButton.State = NSCellStateValue.Off;
			HidePreNextButton(true);

			if (IdxDataManager == null) {
				IdxDataManager = new IndexDataManager (BookID, BookTitle, this);
				IndexViewController.BookID = BookID;
				IndexViewController.IndexDataManager = IdxDataManager;
				await IndexViewController.IndexDataManager.GetIndexDataFromDB();
				IndexViewController.InitializeOutlineView();
			} else {
				if (IdxDataManager.CurrentIndex != null) {
					OpenPublicationIndexAtIndexNode(IdxDataManager.CurrentIndex);
					PageViewController.SetIndexBannerLetter(null, true);
				}
			}

			PageViewController.View.Hidden = false;

			if (IdxDataManager.IndexNodeList == null) {
				
				PageViewController.SetIndexBannerLetter("No index files available.", true);
				PageViewController.ShowPageContent("No index files available.", true);
			}
		
			SetSideBarViewShowByMode();

			SetButtonAttributedTitle(ContentButton,LNRConstants.TITLE_CONTENT,false);
			SetButtonAttributedTitle(IndexButton,LNRConstants.TITLE_INDEX,true);
			SetButtonAttributedTitle(AnnotationButton,LNRConstants.TITLE_ANNOTATIONS,false);

		}

		partial void AnnotationButtonClick (NSObject sender)
		{
			ContentButton.State = NSCellStateValue.Off;
			IndexButton.State = NSCellStateValue.Off;
			AnnotationButton.State = NSCellStateValue.On;
			HidePreNextButton(false);

			PageViewController.SetIndexBannerLetter (null, false);
			PageViewController.ShowPageContent(null, false);

			SetSideBarViewShowByMode();

			SetButtonAttributedTitle(ContentButton,LNRConstants.TITLE_CONTENT,false);
			SetButtonAttributedTitle(IndexButton,LNRConstants.TITLE_INDEX,false);
			SetButtonAttributedTitle(AnnotationButton,LNRConstants.TITLE_ANNOTATIONS,true);
		}
		#endregion

		#region private methods
		void AddInfoUpdateState ()
		{
			NSView superView = InfoButton.Superview;
			nint updateCount = PublicationsDataManager.SharedInstance.CurrentPubliction.UpdateCount;
			//Console.WriteLine ("AddInfoUpdateState CurrentPubliction:update{0}", updateCount);
			if (updateCount<=0) {
				NSView [] views = superView.Subviews;
				nint count = views.Length;
				//Console.WriteLine ("AddInfoUpdateState subviews:update{0}", count);
				while (count > 2) {
					NSButton button = (NSButton)views [count - 1];
					button.RemoveFromSuperview ();
					count--;
				}
				return;
			}

			CGRect frame = InfoButton.Frame;
			CGRect newFrame = new CGRect (frame.Right - 13, frame.Bottom - 12, 15, 15);
			NSButton newButton = new NSButton (newFrame);
			newButton.Cell.BezelStyle = NSBezelStyle.Circular;
			newButton.Cell.Bordered = false;
			newButton.WantsLayer = true;
			newButton.Layer.BackgroundColor = NSColor.Red.CGColor;
			newButton.Layer.CornerRadius = 7.5f;
			newButton.Alignment = NSTextAlignment.Center;
			//button.Enabled = false;

			string infoTitle = updateCount.ToString ();
			newButton.AttributedTitle = Utility.AttributeTitle (infoTitle, NSColor.White, 8);
			superView.AddSubview (newButton);
		}

		void SetButtonAttributedTitle(NSButton button, string title, bool isStateOn)
		{
			float fontSize = 14;
			if (isStateOn) {
				var attributedTitle = Utility.AttributeTitle (title, NSColor.Red, fontSize);
				button.AttributedTitle = attributedTitle;
				var alterTitle = Utility.AttributeTitle (title, NSColor.Black, fontSize);
				button.AttributedAlternateTitle = alterTitle;
				button.WantsLayer = true;
				button.Layer.BackgroundColor = Utility.ColorWithRGB (251, 212, 213, 1.0f).CGColor;
				button.Layer.CornerRadius = 5;
			} else {
				var attributedTitle = Utility.AttributeTitle (title, NSColor.Black, fontSize);
				button.AttributedTitle = attributedTitle;
				var alterTitle = Utility.AttributeTitle (title, NSColor.Red, fontSize);
				button.AttributedAlternateTitle = alterTitle;
				button.WantsLayer = true;
				button.Layer.BackgroundColor = Utility.ColorWithRGB (255, 255, 255, 1.0f).CGColor;
				button.Layer.CornerRadius = 5;
			}
		}

		void SetSideBarViewShowByMode ()
		{
			if (ContentButton.State == NSCellStateValue.On) {
				if (TocCustomView.Hidden) {
					TocCustomView.Hidden = false;
				}

				if (!IndexCustomView.Hidden) {
					IndexCustomView.Hidden = true;
				}

				if (!AnnotationView.Hidden) {
					AnnotationView.Hidden = true;
				}
			}else if (IndexButton.State == NSCellStateValue.On) {
				if (!TocCustomView.Hidden) {
					TocCustomView.Hidden = true;
				}

				if (IndexCustomView.Hidden) {
					IndexCustomView.Hidden = false;
				}

				if (!AnnotationView.Hidden) {
					AnnotationView.Hidden = true;
				}
			}else if (AnnotationButton.State == NSCellStateValue.On) {
				if (!TocCustomView.Hidden) {
					TocCustomView.Hidden = true;
				}

				if (!IndexCustomView.Hidden) {
					IndexCustomView.Hidden = true;
				}

				if (AnnotationView.Hidden) {
					AnnotationView.Hidden = false;
				}
			}

			RelayoutContentPageView (false);
		}

		void RelayoutTOCCustomView (bool isShow)
		{
			if (isShow) {

				CGSize tocViewSize = TocCustomView.Frame.Size;

				CGSize newSize = BackgroudView.Frame.Size;
				newSize.Width -= tocViewSize.Width;
				newSize.Height -= FunctionButtonView.Frame.Size.Height;

				nfloat xstart = tocViewSize.Width;

				if (newSize.Width > CONTENTPAGE_WIDTH) {
					xstart += (newSize.Width - CONTENTPAGE_WIDTH) / 2;
					newSize.Width = CONTENTPAGE_WIDTH;
				}

				BookContentView.SetFrameSize(newSize);
				BookContentView.SetFrameOrigin(new CGPoint(xstart,0));

			} else {

				CGSize newSize = BackgroudView.Frame.Size;
				newSize.Height -= FunctionButtonView.Frame.Size.Height;
				nfloat xstart = 0;
				if (newSize.Width > CONTENTPAGE_WIDTH) {
					xstart = (newSize.Width-CONTENTPAGE_WIDTH)/2;
					newSize.Width = CONTENTPAGE_WIDTH;
				}

				BookContentView.SetFrameSize(newSize);
				BookContentView.SetFrameOrigin(new CGPoint(xstart,0));
			}
		}

		void RelayoutIndexCustomView (bool isShow)
		{
			if (isShow) {

				CGSize indexViewSize = IndexCustomView.Frame.Size;

				CGSize newSize = BackgroudView.Frame.Size;
				newSize.Width -= indexViewSize.Width;
				newSize.Height -= FunctionButtonView.Frame.Size.Height;

				nfloat xstart = indexViewSize.Width;

				if (newSize.Width > CONTENTPAGE_WIDTH) {
					xstart += (newSize.Width - CONTENTPAGE_WIDTH) / 2;
					newSize.Width = CONTENTPAGE_WIDTH;
				}

				BookContentView.SetFrameSize(newSize);
				BookContentView.SetFrameOrigin(new CGPoint(xstart,0));

			} else {

				CGSize newSize = BackgroudView.Frame.Size;
				newSize.Height -= FunctionButtonView.Frame.Size.Height;
				nfloat xstart = 0;
				if (newSize.Width > CONTENTPAGE_WIDTH) {
					xstart = (newSize.Width-CONTENTPAGE_WIDTH)/2;
					newSize.Width = CONTENTPAGE_WIDTH;
				}

				BookContentView.SetFrameSize(newSize);
				BookContentView.SetFrameOrigin(new CGPoint(xstart,0));
			}
		}

		void RelayoutAnnotaionView (bool isShow)
		{
			if (isShow) {

				CGSize viewSize = AnnotationView.Frame.Size;

				CGSize newSize = BookContentView.Frame.Size;
				newSize.Width -= viewSize.Width;
				BookContentView.SetFrameSize(newSize);
				BookContentView.SetFrameOrigin(new CGPoint(viewSize.Width,0));

			} else {
				
				CGSize newSize = BackgroudView.Frame.Size;
				newSize.Height -= FunctionButtonView.Frame.Size.Height;

				BookContentView.SetFrameSize(newSize);
				BookContentView.SetFrameOrigin(new CGPoint(0,0));
			}
		}
	
		void RelayoutContentPageView(bool isFull)
		{
			if (!isFull) {
				CGSize newSize = BackgroudView.Frame.Size;
				newSize.Width -= sidebarViewWidth;
				newSize.Height -= FunctionButtonView.Frame.Size.Height;

				nfloat xstart = sidebarViewWidth;

				if (newSize.Width > CONTENTPAGE_WIDTH) {
					xstart += (newSize.Width - CONTENTPAGE_WIDTH) / 2;
					newSize.Width = CONTENTPAGE_WIDTH;
				}
				BookContentView.SetFrameSize(newSize);
				BookContentView.SetFrameOrigin(new CGPoint(xstart,0));

			} else {
				CGSize newSize = BackgroudView.Frame.Size;
				newSize.Height -= FunctionButtonView.Frame.Size.Height;
				nfloat xstart = 0;
				if (newSize.Width > CONTENTPAGE_WIDTH) {
					xstart = (newSize.Width-CONTENTPAGE_WIDTH)/2;
					newSize.Width = CONTENTPAGE_WIDTH;
				}
				BookContentView.SetFrameSize(newSize);
				BookContentView.SetFrameOrigin(new CGPoint(xstart,0));
			}
		}

		void HandleWindowDidResize (object sender, EventArgs e)
		{
			if (ContentButton.State == NSCellStateValue.On) {
				if (TocCustomView.Hidden) {
					RelayoutTOCCustomView (false);
				} else {
					RelayoutTOCCustomView (true);
				}

			}else if (IndexButton.State == NSCellStateValue.On){
				if (IndexCustomView.Hidden) {
					RelayoutIndexCustomView (false);
				} else {
					RelayoutIndexCustomView (true);
				}
			}else if (AnnotationButton.State == NSCellStateValue.On) {
				if (AnnotationView.Hidden) {
					RelayoutAnnotaionView (false);
				} else {
					RelayoutAnnotaionView (true);
				}
			}
		}
		
		#endregion

		protected override void Dispose (bool disposing)
		{
			BookTitle = null;

			CurrencyDate = null;

			TOCDataManager = null;
			TOCController = null;

			IdxDataManager = null;

			PageController = null;
		}
	}
}

