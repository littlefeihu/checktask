using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

using Foundation;
using AppKit;
using CoreGraphics;

using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Mac.Data;
using LexisNexis.Red.Common.Entity;


namespace LexisNexis.Red.Mac
{
	public partial class PublicationContentViewController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public PublicationContentViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PublicationContentViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public PublicationContentViewController () : base ("PublicationContentView", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		#region properties
		//strongly typed view accessor
		public new PublicationContentView View {
			get {
				return (PublicationContentView)base.View;
			}
		}

		public string BookTitle { get; set;}
		public int BookID { get; set;}
		public bool IsExpired { get; set;}
		public bool IsFTC { get; set;}
		public string CurrencyDate { get; set;}

		public PublicationTOCDataManager TOCDataManager;
		public TocViewController TOCController {get{ return TOCViewController;}}

		public IndexDataManager IdxDataManager;

		public PageContentViewController PageController{ get; set;}
		List<Dictionary<int,TOCNode>> TocListForBackForward { get; set; }
		int CurrentHistoryIndex { get; set; }
		nfloat CONTENTPAGE_WIDTH = 704;
		nfloat sidebarViewWidth = 260;
		bool isFullContentPage { get; set; }

		enum ContentMode {
			CM_Contents = 1,
			CM_Index = 2,
			CM_Annotations = 3
		};

		ContentMode currentViewMode {get; set;}
		public int ViewMode{get {
				if (currentViewMode == ContentMode.CM_Index) {
					return 2;
				} else {
					return 1;
				}
			}}
		#endregion

		#region public methods

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			PageController = PageViewController;

			if (TocListForBackForward == null) {
				TocListForBackForward = new List<Dictionary<int,TOCNode>> (0);
			}

			//
			LeftButton.Image = NSImage.ImageNamed ("NSGoLeftTemplate");
			RightButton.Image = NSImage.ImageNamed ("NSGoRightTemplate");

			//
			ContentButton.Cell.Bordered = false;
			ContentButton.Cell.SetButtonType (NSButtonType.MomentaryChange); 

			IndexButton.Cell.Bordered = false;
			IndexButton.Cell.SetButtonType (NSButtonType.MomentaryChange); 

			AnnotationButton.Cell.Bordered = false;
			AnnotationButton.Cell.SetButtonType (NSButtonType.MomentaryChange);

			var attributedTitle = Utility.AttributeTitle ("Go to", NSColor.Red, 14);
			GotoButton.AttributedTitle = attributedTitle;
			var alterTitle = Utility.AttributeTitle ("Go to", NSColor.Black, 14);
			GotoButton.AttributedAlternateTitle = alterTitle;
			GotoButton.Hidden = true;

			NSNotificationCenter.DefaultCenter.AddObserver (LNRConstants.LNChangeHistoryItemNotification, 
				HandleHistoryNotification);

			NSNotificationCenter.DefaultCenter.AddObserver (LNRConstants.LNPublicationDidFinishedDownload, 
				HandleFinishDownload);

		}

		//for the same title
		public void HandleHistoryNotification (NSNotification notification)
		{
			var tocID = Int32.Parse(notification.Object.ToString());

			TOCNode rootTocNode = TOCDataManager.RootNodeList[0].ParentNode;
			TOCNode currentTocNode = PublicationContentUtil.Instance.GetTOCByTOCId(tocID,rootTocNode);
			if (currentTocNode != null) {
				TOCDataManager.GetTOCNodeListByCurrentLeafNode (currentTocNode);
				TOCDataManager.CurrentLeafNode = currentTocNode;
				TOCViewController.RefreshTableViewData ();
				TOCViewController.SelectTOCRectByTocID(currentTocNode.ID);
			}
		}

		//deal with update from infomodal
		public void HandleFinishDownload(NSNotification notification) 
		{
			var bookID = Int32.Parse(notification.Object.ToString());
			if (bookID == BookID) {

				Publication BookInfo = PublicationsDataManager.SharedInstance.CurrentPublication;
				BookTitle = BookInfo.Name;
				string currencyDate = "Currency Date " + BookInfo.CurrencyDate.Value.ToString ("dd MMM yyyy");
				CurrencyDate = currencyDate;

				var NSAppDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
				NSAppDelegate.publicationsWindowController.SetToolBarInfoUpdateState ();
			}
		}
		#endregion

		#region api with 'PublicationsWindowController'
		public async Task InitializeContentPage(int tocID) 
		{
			Publication BookInfo = PublicationsDataManager.SharedInstance.CurrentPublication;
			BookTitle = BookInfo.Name;

			bool isSameBook = BookID == BookInfo.BookId ? true : false;

			BookID = BookInfo.BookId;

			IsExpired = BookInfo.DaysRemaining < 0 ? true : false;
			if (BookInfo.CurrencyDate == null) {
				return;
			}
			isFullContentPage = false;
			//initalize content view
			PageViewController.InitalizeStatus ();

			if (tocID == -1) {
				if (TocListForBackForward != null) {
					TocListForBackForward.Clear ();
				}
				this.CurrentHistoryIndex = 0;

				NavigationManager.Instance.Clear ();
			}

			string currencyDate = "Currency Date " + BookInfo.CurrencyDate.Value.ToString ("dd MMM yyyy");
			CurrencyDate = currencyDate;

			//initialize toc view
			TOCViewController.BookID = BookID;
			TOCViewController.IsExpired = IsExpired;
			TOCViewController.CurrencyDate = CurrencyDate;
			TOCViewController.InitializeTableView (isSameBook);

			if (TOCDataManager == null) {
				TOCDataManager = new PublicationTOCDataManager (BookID, BookTitle, this);
			}
			TOCDataManager.InitializeTOCDataByBookID(BookID, tocID);
			TOCViewController.TOCDataManager = TOCDataManager;
			await TOCDataManager.GetPublicationTocFromDB ();

			ContentButtonClick (ContentButton);

			//initialize index view
			if (IdxDataManager == null) {
				IdxDataManager = new IndexDataManager (BookID, BookTitle, this);
				IndexViewController.IndexDataManager = IdxDataManager;
			}

			IndexViewController.BookID = BookID;
			IdxDataManager.BookID = BookID;
			IdxDataManager.CurrentRow = 0;
			IdxDataManager.CurrentIndex = null;
			await IndexViewController.IndexDataManager.GetIndexDataFromDB();
			IndexViewController.InitializeOutlineView();
		}
		#endregion

		#region api with PublicationTOCDataManager
		public void RefreshTOCViewData ()
		{
			PageViewController.AddLoadView();
			TOCViewController.RefreshTableViewData ();
		}

		public void SelectTOCRectByTocID(int tocID)
		{
			TOCViewController.SelectTOCRectByTocID(tocID);
		}

		//TocViewController
		public async Task OpenPublicationContentAtTOCNode (int bookID, TOCNode tocNode)
		{
			Console.WriteLine ("bookID:{0}, tocID:{1}", bookID, tocNode.ID);
//			if (TocListForBackForward.Count >= 30) {
//				TocListForBackForward.RemoveAt (0);
//			}
//
//			var dict = new Dictionary<int,TOCNode> ();
//			dict.Add (bookID, tocNode);
//
//			TocListForBackForward.Add (dict);
//			this.CurrentHistoryIndex = TocListForBackForward.Count - 1;
//			EnablePreNextButton ();

			AddNavigationPathAtBookIDandTocID (bookID, tocNode.ID);

			string htmlString = await PublicationContentUtil.Instance.GetContentFromTOC (bookID, tocNode);
			PageViewController.ShowPageContent(htmlString, true, bookID, tocNode.ID.ToString(),tocNode.Title);
		}

		public void AddNavigationPathAtBookIDandTocID(int bookID, int tocID)
		{
			this.CurrentHistoryIndex += 1;
			var record = new ContentBrowserRecord (bookID, tocID, 0);
			NavigationManager.Instance.AddRecord (record);
		}
		#endregion

		#region api with IndexDataManager
		public void RefreshIndexViewData ()
		{
			IndexViewController.RefreshOutlineViewData ();
		}

		//api with IndexViewController 
		public async void OpenPublicationIndexAtIndexNode (Index indexNode)
		{
			if (indexNode == null) {
				PageViewController.SetIndexBannerLetter (null, false);
				PageViewController.ShowPageContent(null, false, 0, null, null);
			} else {
				if (!string.IsNullOrEmpty (indexNode.Title)) {
					PageViewController.SetIndexBannerLetter (indexNode.Title, true);
				}

				if (!string.IsNullOrEmpty(indexNode.FileName)) {
					string htmlString = await PublicationContentUtil.Instance.GetContentFromIndex (BookID, indexNode);
					PageViewController.ShowPageContent(htmlString, true, 0, null, null);
					IndexViewController.RefreshSelectBackColor ();

				}
			}
		}
		#endregion

		#region infinite scroll api with 'PageContentViewController'
		public string GetNextTocNodeTitle () 
		{
			TOCNode currentToc = TOCDataManager.CurrentLeafNode;
			currentToc = PublicationContentUtil.Instance.GetNextPageByTreeNode (currentToc);
			if (currentToc == null) {
				return null;
			}
			return currentToc.Title;
		}

		public async Task FetchNextPageAtTocNode ()
		{
			TOCNode currentToc = TOCDataManager.CurrentLeafNode;
			currentToc = PublicationContentUtil.Instance.GetNextPageByTreeNode (currentToc);
			TOCDataManager.CurrentLeafNode = currentToc;
			string htmlString = await PublicationContentUtil.Instance.GetContentFromTOC (BookID, currentToc);
			PageViewController.AppendPageContent(htmlString, BookID, currentToc.ID.ToString(),currentToc.Title);

			if (currentToc != null) {
				this.InvokeOnMainThread(()=>
					HighlightLeafTocNode (currentToc)
				);
			}
		}

		public string GetPreTocNodeTitle () 
		{
			TOCNode currentToc = TOCDataManager.CurrentLeafNode;
			currentToc = PublicationContentUtil.Instance.GetPreviousPageByTreeNode (currentToc);
			if (currentToc == null) {
				return null;
			}
			return currentToc.Title;
		}

		public async Task FetchPrePageAtTocNode()
		{
			TOCNode currentToc = TOCDataManager.CurrentLeafNode;
			if (currentToc == null) {
				return;
			}
			currentToc = PublicationContentUtil.Instance.GetPreviousPageByTreeNode (currentToc);
			TOCDataManager.CurrentLeafNode = currentToc;
			string htmlString = await PublicationContentUtil.Instance.GetContentFromTOC (BookID, currentToc);
			PageViewController.PrependPageContent(htmlString, BookID, currentToc.ID.ToString(),currentToc.Title);

			if (currentToc != null) {
				this.InvokeOnMainThread(()=>
					HighlightLeafTocNode (currentToc)
				);
			}
		}

		void HighlightLeafTocNode (TOCNode currentToc)
		{
			TOCDataManager.GetTOCNodeListByCurrentLeafNode (currentToc);
			TOCDataManager.SetHighlightTOCIndexByTocID (currentToc.ID);
			TOCViewController.RefreshTableViewData ();
			TOCViewController.ScrollCurrentLeafNodeToVisible ();
		}

		public void RefreshTocNodeByCurrentTocID (int tocID)
		{
			TOCNode currentToc = TOCDataManager.CurrentLeafNode;
			if (currentToc!=null&&currentToc.ID == tocID) {
				return;
			}
			//Console.WriteLine ("tocID:{0} currentID:{1}",tocID,currentToc.ID);
			bool isScrollBottom = (tocID > currentToc.ID) ? true : false;
			if (TOCDataManager.GetTOCNodeListByCurrentLeafNodeID (tocID,isScrollBottom)) {
				TOCDataManager.SetHighlightTOCIndexByTocID (tocID);
				TOCViewController.RefreshTableViewData ();
				TOCViewController.ScrollCurrentLeafNodeToVisible ();
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

		public void SplitButtonClick (NSObject sender)
		{
			if (ContentButton.State == NSCellStateValue.On) {
				if (isFullContentPage) {
					TocCustomView.Hidden = false;
					isFullContentPage = false;
				} else {
					TocCustomView.Hidden = true;
					isFullContentPage = true;
				}
				RelayoutContentPageView (isFullContentPage);

			}else if (IndexButton.State == NSCellStateValue.On) {
				if (isFullContentPage) {
					IndexCustomView.Hidden = false;
					isFullContentPage = false;
				} else {
					IndexCustomView.Hidden = true;
					isFullContentPage = true;
				}
				RelayoutContentPageView (isFullContentPage);
			}else {
				if (isFullContentPage) {
					AnnotationView.Hidden = false;
					isFullContentPage = false;
				} else {
					AnnotationView.Hidden = true;
					isFullContentPage = true;
				}
			}
		}

		public void ShareButtonClick (NSObject sender)
		{
		}

		public void HistoryButtonClick (NSObject sender)
		{
			var popover = new NSPopover ();
			popover.Behavior = NSPopoverBehavior.Transient;
			popover.ContentViewController = new HistoryPopoverController (popover, BookID);
			popover.Show (new CGRect (0, 0, 0, 0), (NSView)sender, NSRectEdge.MinYEdge);
		}

		public void SearchFieldClick (Foundation.NSObject sender)
		{
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

		private void EnablePreNextButton () 
		{
			if (this.CurrentHistoryIndex>0) {
				LeftButton.Enabled = true;
			}else {
				LeftButton.Enabled = false;
			}

			if (this.CurrentHistoryIndex<TocListForBackForward.Count-1 && this.CurrentHistoryIndex>=0) {
				RightButton.Enabled = true;
			}else {
				RightButton.Enabled = false;
			}
		}

		partial void PreButtonClick (NSObject sender)
		{
			this.CurrentHistoryIndex --;
			EnablePreNextButton();

			//var leafNode = TocListForBackForward[this.CurrentHistoryIndex];
			//var bookID = leafNode.Keys.First();
			//var tocNode = leafNode.Values.First();

			//string htmlString = await PublicationContentUtil.Instance.GetContentFromTOC (bookID, tocNode);
			//PageViewController.ShowPageContent(htmlString, true, bookID, tocNode.ID.ToString(),tocNode.Title);

			var record = (ContentBrowserRecord)NavigationManager.Instance.Back();
			var bookID = record.BookID;
			var tocNode = record.TOCID;

			PublicationsDataManager.SharedInstance.CurrentPublication = 
			PublicationsDataManager.SharedInstance.GetCurrentPublicationByBookID (bookID);

			var NSApp = NSApplication.SharedApplication;
			var winController = (PublicationsWindowController)NSApp.MainWindow.WindowController;
			winController.UpdateTitleBar ();

		}

		partial void NextButtonClick (NSObject sender)
		{
			this.CurrentHistoryIndex ++;
			EnablePreNextButton();

			var leafNode = TocListForBackForward[this.CurrentHistoryIndex];
			var bookID = leafNode.Keys.First();
			var tocNode = leafNode.Values.First();

			PublicationsDataManager.SharedInstance.CurrentPublication = 
			PublicationsDataManager.SharedInstance.GetCurrentPublicationByBookID (bookID);

			var NSApp = NSApplication.SharedApplication;
			var winController = (PublicationsWindowController)NSApp.MainWindow.WindowController;
			winController.UpdateTitleBar ();

			//string htmlString = await PublicationContentUtil.Instance.GetContentFromTOC (bookID, tocNode);
			//PageViewController.ShowPageContent(htmlString, true, bookID, tocNode.ID.ToString(),tocNode.Title);
		}

		partial void GotoPageNumber (NSObject sender)
		{
		}

		partial void ContentButtonClick (NSObject sender)
		{
			currentViewMode = ContentMode.CM_Contents;
			ContentButton.State = NSCellStateValue.On;
			IndexButton.State = NSCellStateValue.Off;
			AnnotationButton.State = NSCellStateValue.Off;

			HidePreNextButton(false);

			PageViewController.SetPageContentEmpty(true);
			PageViewController.SetIndexBannerLetter (null, false);
			BookContentView.Hidden = false;

			if (TOCDataManager.CurrentLeafNode != null) {
				PageViewController.AddLoadView();
				OpenPublicationContentAtTOCNode(BookID,TOCDataManager.CurrentLeafNode);
			}

			SetSideBarViewShowByMode();

			SetButtonAttributedTitle(ContentButton,LNRConstants.TITLE_CONTENT,true);
			SetButtonAttributedTitle(IndexButton,LNRConstants.TITLE_INDEX,false);
			SetButtonAttributedTitle(AnnotationButton,LNRConstants.TITLE_ANNOTATIONS,false);

		}

		partial void IndexButtonClick (NSObject sender)
		{
			if (currentViewMode == ContentMode.CM_Index) {
				return;
			}

			currentViewMode = ContentMode.CM_Index;
			ContentButton.State = NSCellStateValue.Off;
			IndexButton.State = NSCellStateValue.On;
			AnnotationButton.State = NSCellStateValue.Off;
			HidePreNextButton(true);

			if (IdxDataManager.CurrentRow == 0) {
				IndexViewController.SelectItemByRow(1);
			} else {
				OpenPublicationIndexAtIndexNode(IdxDataManager.CurrentIndex);
			}

			BookContentView.Hidden = false;

			if (IdxDataManager.IndexNodeList == null) {

				PageViewController.SetIndexBannerLetter("No index files available.", true);
				string infoString = "<br>" + "<br>" + "<br>" + "<br>" + "No index files available.";
				PageViewController.ShowPageContent(infoString, true, 0, null, null);
			}

			SetSideBarViewShowByMode();

			SetButtonAttributedTitle(ContentButton,LNRConstants.TITLE_CONTENT,false);
			SetButtonAttributedTitle(IndexButton,LNRConstants.TITLE_INDEX,true);
			SetButtonAttributedTitle(AnnotationButton,LNRConstants.TITLE_ANNOTATIONS,false);

		}

		partial void AnnotationButtonClick (NSObject sender)
		{
			if (currentViewMode == ContentMode.CM_Annotations) {
				return;
			}

			currentViewMode = ContentMode.CM_Annotations;
			ContentButton.State = NSCellStateValue.Off;
			IndexButton.State = NSCellStateValue.Off;
			AnnotationButton.State = NSCellStateValue.On;
			HidePreNextButton(false);

			BookContentView.Hidden = true;

			SetSideBarViewShowByMode();

			SetButtonAttributedTitle(ContentButton,LNRConstants.TITLE_CONTENT,false);
			SetButtonAttributedTitle(IndexButton,LNRConstants.TITLE_INDEX,false);
			SetButtonAttributedTitle(AnnotationButton,LNRConstants.TITLE_ANNOTATIONS,true);
		}
		#endregion

		#region private methods
		void SetButtonAttributedTitle(NSButton button, string title, bool isStateOn)
		{
			float fontSize = 13;
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
				if (!isFullContentPage) {
					TocCustomView.Hidden = false;
				} else {
					TocCustomView.Hidden = true;
				}

				if (!IndexCustomView.Hidden) {
					IndexCustomView.Hidden = true;
				}

				if (!AnnotationView.Hidden) {
					AnnotationView.Hidden = true;
				}

				RelayoutContentPageView (isFullContentPage);
			}else if (IndexButton.State == NSCellStateValue.On) {
				if (!TocCustomView.Hidden) {
					TocCustomView.Hidden = true;
				}

				if (!isFullContentPage) {
					IndexCustomView.Hidden = false;
				} else {
					IndexCustomView.Hidden = true;
				}

				if (!AnnotationView.Hidden) {
					AnnotationView.Hidden = true;
				}

				RelayoutContentPageView (isFullContentPage);
			}else if (AnnotationButton.State == NSCellStateValue.On) {
				if (!TocCustomView.Hidden) {
					TocCustomView.Hidden = true;
				}

				if (!IndexCustomView.Hidden) {
					IndexCustomView.Hidden = true;
				}

				if (!isFullContentPage) {
					AnnotationView.Hidden = false;
				} else {
					AnnotationView.Hidden = true;
				}
			}
		}

		void RelayoutContentPageView(bool isFull)
		{
			if (!isFull) {
				CGSize newSize = BackgroudView.Frame.Size;
				newSize.Width -= sidebarViewWidth;
				newSize.Height -= FunctionButtonView.Frame.Size.Height;

				nfloat xstart = sidebarViewWidth;

//				if (newSize.Width > CONTENTPAGE_WIDTH) {
//					xstart += (newSize.Width - CONTENTPAGE_WIDTH) / 2;
//					newSize.Width = CONTENTPAGE_WIDTH;
//				}
				BookContentView.SetFrameSize(newSize);
				BookContentView.SetFrameOrigin(new CGPoint(xstart,0));

			} else {
				CGSize newSize = BackgroudView.Frame.Size;
				newSize.Height -= FunctionButtonView.Frame.Size.Height;
				nfloat xstart = 0;
//				if (newSize.Width > CONTENTPAGE_WIDTH) {
//					xstart = (newSize.Width-CONTENTPAGE_WIDTH)/2;
//					newSize.Width = CONTENTPAGE_WIDTH;
//				}
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

		public void HandleWindowDidResize (object sender, EventArgs e)
		{
			if (ContentButton.State == NSCellStateValue.On ||
			    IndexButton.State == NSCellStateValue.On) {
				RelayoutContentPageView (isFullContentPage);
			} else if (AnnotationButton.State == NSCellStateValue.On){
			}
		}
			
		#endregion

		#region infterface for content history switch content
		public async Task RefreshContentViewAtBookIDandTocNode(int tocID) 
		{
			Publication BookInfo = PublicationsDataManager.SharedInstance.CurrentPublication;
			BookTitle = BookInfo.Name;

			bool isSameBook = BookID == BookInfo.BookId ? true : false;

			BookID = BookInfo.BookId;

			IsExpired = BookInfo.DaysRemaining < 0 ? true : false;
			if (BookInfo.CurrencyDate == null) {
				return;
			}
			isFullContentPage = false;
			//initalize content view
			PageViewController.InitalizeStatus ();

			string currencyDate = "Currency Date " + BookInfo.CurrencyDate.Value.ToString ("dd MMM yyyy");
			CurrencyDate = currencyDate;

			//initialize toc view
			TOCViewController.BookID = BookID;
			TOCViewController.IsExpired = IsExpired;
			TOCViewController.CurrencyDate = CurrencyDate;
			TOCViewController.InitializeTableView (isSameBook);

			TOCDataManager.InitializeTOCDataByBookID(BookID, tocID);
			TOCViewController.TOCDataManager = TOCDataManager;
			await TOCDataManager.GetPublicationTocFromDB ();

			SwitchToContentView ();

			//initialize index view
			IndexViewController.BookID = BookID;
			IdxDataManager.BookID = BookID;
			IdxDataManager.CurrentRow = 0;
			IdxDataManager.CurrentIndex = null;
			await IndexViewController.IndexDataManager.GetIndexDataFromDB();
			IndexViewController.InitializeOutlineView();
		}

		private void SwitchToContentView ()
		{
			//			if (currentViewMode == ContentMode.CM_Contents) {
			//				SetSideBarViewShowByMode();
			//				return;
			//			}

			currentViewMode = ContentMode.CM_Contents;
			ContentButton.State = NSCellStateValue.On;
			IndexButton.State = NSCellStateValue.Off;
			AnnotationButton.State = NSCellStateValue.Off;

			HidePreNextButton(false);

			PageViewController.SetPageContentEmpty(true);
			PageViewController.SetIndexBannerLetter (null, false);
			BookContentView.Hidden = false;

			SetSideBarViewShowByMode();

			SetButtonAttributedTitle(ContentButton,LNRConstants.TITLE_CONTENT,true);
			SetButtonAttributedTitle(IndexButton,LNRConstants.TITLE_INDEX,false);
			SetButtonAttributedTitle(AnnotationButton,LNRConstants.TITLE_ANNOTATIONS,false);
		}
		#endregion

		protected override void Dispose (bool disposing)
		{
			BookTitle = null;

			CurrencyDate = null;

			TOCDataManager = null;

			IdxDataManager = null;

			PageController = null;
		}
	}
}
