
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
	public partial class PublicationContentPanelController : AppKit.NSWindowController 
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
		public PublicationContentPanelController (int bookID, string bookTitle, bool isExpired, string currencyDate) : base ("PublicationContentPanel")
		{
			BookTitle = bookTitle;
			BookID = bookID;
			IsExpired = isExpired;
			CurrencyDate = currencyDate;
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
		bool isSideBarShow = true;
		nfloat sidebarViewWidth = 260;
		#endregion

		#region public methods

		public override void WindowDidLoad ()
		{
			if (BookTitle!=null) {
				Window.Title = BookTitle;
				TitleTField.StringValue = BookTitle;
				TitleTField.ToolTip = BookTitle;
			}

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

			sidebarViewWidth = TocCustomView.Frame.Width;

			if (IsFTC) {

				int location = BookTitle.IndexOf (" + Cases");
				if (location < 0) {
					BookTitle += " + Cases";
				} 
			}

			SegmentContol.SetSelected (false, 0);

			AddInfoUpdateState ();

			Window.DidResize += HandleWindowDidResize;

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
			using (NSAutoreleasePool pool = new NSAutoreleasePool ()) {
				var infoPanelController = new PublicationInfoPanelController ();
				infoPanelController.BookInfo = PublicationsDataManager.SharedInstance.CurrentPubliction;
				infoPanelController.InitializeInfoView ();

				var infoPanel = infoPanelController.Window;
				infoPanel.MakeFirstResponder (null);

				NSApplication NSApp = NSApplication.SharedApplication;

				infoPanel.WindowShouldClose += (t) => true;
				infoPanel.WillClose += delegate(object asender, EventArgs e) {
					NSApp.StopModal ();
				};

				nint result = NSApplication.SharedApplication.RunModalForWindow (infoPanel);
				infoPanelController.Window.OrderOut (null);
				infoPanelController.Dispose ();

				AddInfoUpdateState ();
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
			NSView currentView;
			if (ContentButton.State == NSCellStateValue.On) {
				currentView = TocCustomView;
			}else if (IndexButton.State == NSCellStateValue.On) {
				currentView = IndexCustomView;
			}else {
				currentView = AnnotationView;
			}
			#if false
			isSideBarShow = !isSideBarShow;
			var sidebarViewDict = SideBarViewAnimation(currentView, isSideBarShow);
			NSDictionary []dict = new NSDictionary[1];
			dict.SetValue(sidebarViewDict,0);
			using (var animation = new NSViewAnimation(dict)) {
				animation.Duration = 0.2;
				animation.AnimationCurve = NSAnimationCurve.EaseIn;
				animation.StartAnimation();
			}
			bool isFull = !isSideBarShow;
			RelayoutContentPageView(isFull);
			#endif

			#if true
			isSideBarShow = !isSideBarShow;
			var sidebarViewDict = SideBarViewAnimation(currentView, isSideBarShow);
			bool isFull = !isSideBarShow;
			var contentViewDict = ContentViewAnimation(BookContentView, isFull);

			NSDictionary []dict = new NSDictionary[2];
			dict.SetValue(sidebarViewDict,0);
			dict.SetValue(contentViewDict,1);
			using (var animation = new NSViewAnimation(dict)) {
				animation.Duration = 0.2;
				animation.AnimationCurve = NSAnimationCurve.Linear;
				animation.StartAnimation();
			}
			#endif
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
			
		partial void ContentButtonClick (NSObject sender)
		{
			ContentButton.State = NSCellStateValue.On;
			IndexButton.State = NSCellStateValue.Off;
			AnnotationButton.State = NSCellStateValue.Off;

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
				PageViewController.ShowPageContent(null, false);
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

			if (updateCount<=0) {
				NSView [] views = superView.Subviews;
				nint count = views.Length;
				if (count > 2) {
					NSButton button = (NSButton)views [count - 1];
					button.RemoveFromSuperview ();
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
				button.Layer.BackgroundColor = Utility.ColorWithRGB (251, 212, 213).CGColor;
				button.Layer.CornerRadius = 5;
			} else {
				var attributedTitle = Utility.AttributeTitle (title, NSColor.Black, fontSize);
				button.AttributedTitle = attributedTitle;
				var alterTitle = Utility.AttributeTitle (title, NSColor.Red, fontSize);
				button.AttributedAlternateTitle = alterTitle;
				button.WantsLayer = true;
				button.Layer.BackgroundColor = Utility.ColorWithRGB (255, 255, 255).CGColor;
				button.Layer.CornerRadius = 5;
			}
		}

		#region sidebar view animation control
		void StartViewAnotation (NSView currentView)
		{
			var dict1 = SideBarViewAnimation (currentView, true);
			NSDictionary []dict = new NSDictionary[1];
			dict.SetValue(dict1,0);
			using (var animation = new NSViewAnimation(dict)) {
				animation.Duration = 0;
				animation.AnimationCurve = NSAnimationCurve.Linear;
				animation.StartAnimation();
			}
		}

		void SetSideBarViewShowByMode ()
		{
			isSideBarShow = true;
			nfloat width = sidebarViewWidth;
			if (ContentButton.State == NSCellStateValue.On) {
				nfloat height = TocCustomView.Frame.Height;
				StartViewAnotation (TocCustomView);

				IndexCustomView.SetFrameSize(new CGSize(0,height));
				AnnotationView.SetFrameSize(new CGSize(0,height));
			}else if (IndexButton.State == NSCellStateValue.On) {
				nfloat height = IndexCustomView.Frame.Height;
				StartViewAnotation (IndexCustomView);

				TocCustomView.SetFrameSize(new CGSize(0,height));
				AnnotationView.SetFrameSize(new CGSize(0,height));
			}else if (AnnotationButton.State == NSCellStateValue.On) {
				nfloat height = AnnotationView.Frame.Height;
				StartViewAnotation (AnnotationView);

				TocCustomView.SetFrameSize(new CGSize(0,height));
				IndexCustomView.SetFrameSize(new CGSize(0,height));
			}

			RelayoutContentPageView (false);
		}

		NSDictionary SideBarViewAnimation (NSView customView, bool isShow)
		{
			CGRect startFrame = customView.Frame;
			CGRect endFrame = startFrame;
			NSObject[] objects;

			if (isShow) {
				endFrame.Width = sidebarViewWidth;
				endFrame.X = 0;
				objects = new NSObject[] {customView, 
					NSValue.FromCGRect(startFrame), 
					NSValue.FromCGRect(endFrame),
					NSViewAnimation.FadeInEffect};
			} else {
				endFrame.Width = 0;
				objects = new NSObject[] {customView, 
					NSValue.FromCGRect(startFrame), 
					NSValue.FromCGRect(endFrame), 
					NSViewAnimation.FadeOutEffect};
			}

			NSObject[] keys = new NSObject[] {NSViewAnimation.TargetKey, 
				NSViewAnimation.StartFrameKey,
				NSViewAnimation.EndFrameKey,
				NSViewAnimation.EffectKey};

			return NSDictionary.FromObjectsAndKeys (objects, keys);
		}

		NSDictionary ContentViewAnimation (NSView customView, bool isFull)
		{
			CGRect startFrame = customView.Frame;
			CGRect endFrame = GetContentPageViewSize(isFull);
			NSObject[] objects = new NSObject[] {customView, 
				NSValue.FromCGRect(startFrame), 
				NSValue.FromCGRect(endFrame)};

			NSObject[] keys = new NSObject[] {NSViewAnimation.TargetKey, 
				NSViewAnimation.StartFrameKey,
				NSViewAnimation.EndFrameKey};
			return NSDictionary.FromObjectsAndKeys (objects, keys);
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

		CGRect GetContentPageViewSize(bool isFull)
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
				CGPoint location = new CGPoint (xstart, 0);
				return new CGRect (location, newSize);

			} else {
				CGSize newSize = BackgroudView.Frame.Size;
				newSize.Height -= FunctionButtonView.Frame.Size.Height;
				nfloat xstart = 0;
				if (newSize.Width > CONTENTPAGE_WIDTH) {
					xstart = (newSize.Width-CONTENTPAGE_WIDTH)/2;
					newSize.Width = CONTENTPAGE_WIDTH;
				}
				CGPoint location = new CGPoint (xstart, 0);
				return new CGRect (location, newSize);
			}
		}
		#endregion

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
	
		void HandleWindowDidResize (object sender, EventArgs e)
		{
			if (ContentButton.State == NSCellStateValue.On) {
				RelayoutContentPageView (!isSideBarShow);
			}else if (IndexButton.State == NSCellStateValue.On){
				RelayoutContentPageView (!isSideBarShow);
			}else if (AnnotationButton.State == NSCellStateValue.On) {
				if (!isSideBarShow) {
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

