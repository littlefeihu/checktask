
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Foundation;
using AppKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Mac.Data;
using CoreGraphics;
using LexisNexis.Red.Common.HelpClass;
using CoreAnimation;

namespace LexisNexis.Red.Mac
{
	public enum PublicationCategory
	{
		FILTER_ALL = 1,
		FILTER_LOAN = 2,
		FILTER_SUBSCRIPTION = 3
	}

	public partial class PublicationsViewController : NSViewController
	{
		#region const and properties
		const int PUBLICATION_VIEW_HEIGHT = 350;
		const int PUBLICATION_COVER_WIDTH = 200;
		const int PUBLICATION_COVER_HORIZONTAL_SPACING = 30;

		const int PUBLICATION_FILTER_ALL = 1;
		const int PUBLICATION_FILTER_LOAN = 2;
		const int PUBLICATION_FILTER_SUBSCRIPTION = 3;

		public List<Publication> offlinePublicationList;
		List<Publication> onlinePublicationList;

		List<NSView> allPublicationsView { get; set;}

		public bool IsHandleNoditifation { get; set;}
		int publicationCategory {get; set;} 
		LoadingViewController loadViewCtr;

		//strongly typed view accessor
		public new PublicationsView View {
			get {
				return (PublicationsView)base.View;
			}
		}

		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public PublicationsViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PublicationsViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public PublicationsViewController () : base ("PublicationsView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		async void Initialize ()
		{
			if (allPublicationsView == null) {
				allPublicationsView = new List<NSView>(0);
			}

			offlinePublicationList = PublicationsDataManager.SharedInstance.GetLocalDBPublicationsList();

			await PublicationsDataManager.SharedInstance.InitAndLoadDB ();
			onlinePublicationList = PublicationsDataManager.SharedInstance.onlinePublicationList;

			if (onlinePublicationList != null) {
				offlinePublicationList = onlinePublicationList;
				SetPublicationViewHidden (false);
				ShowPublicationsInScrollView (offlinePublicationList, PUBLICATION_FILTER_ALL);
				RefreshHistoryView ();
			} else {
				if (offlinePublicationList == null || 
					!offlinePublicationList.Any()) {
					InitializeControlTitle ();
					SetPublicationViewHidden (true);
				}
			}

			RemoveLoadView ();
		}

		public override void LoadView ()
		{
			base.LoadView ();

			AddLoadView ();

			MainScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
			MainScrollView.VerticalScroller.ControlSize = NSControlSize.Small;
			MainScrollView.BackgroundColor = NSColor.White;
			MainScrollView.HorizontalScrollElasticity = NSScrollElasticity.None;
			MainScrollView.VerticalScrollElasticity = NSScrollElasticity.None;

			MainScrollView.DocumentView = MainDocumentView;

			bookScrollView.AutohidesScrollers = true;
			bookScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
			bookScrollView.HorizontalScroller.ControlSize = NSControlSize.Small;
			bookScrollView.VerticalScrollElasticity = NSScrollElasticity.None;
			RefreshPublicationsView (false);
			RefreshHistoryView ();

			if (offlinePublicationList.Any ()) {
				RemoveLoadView ();
			}
		}

		private void AddLoadView ()
		{
			if (loadViewCtr == null) {
				PublicationsCustomView.WantsLayer = true;
				var scrollframe = PublicationsCustomView.Frame;
				var location = new CGPoint ();
				location.X = (scrollframe.Width - 200) / 2;
				location.Y = (scrollframe.Height-100)/2;

				loadViewCtr = new LoadingViewController (location, LNRConstants.LOADING_INFO);
			}

			loadViewCtr.View.WantsLayer = true;
			PublicationsCustomView.Layer.AddSublayer (loadViewCtr.View.Layer);
		}

		private void RemoveLoadView ()
		{
			NSRunLoop.Current.RunUntil(NSRunLoopMode.Default,NSDate.FromTimeIntervalSinceReferenceDate(5));

			if (loadViewCtr != null) {
				loadViewCtr.View.Layer.RemoveFromSuperLayer ();
				loadViewCtr.Dispose ();
				loadViewCtr = null;
			}
		}

		private void ReframeMainScrollViewWithHeight (nfloat viewHeight)
		{
			var view = (NSView)MainScrollView.DocumentView;
			var scrollHeight = MainScrollView.Frame.Height;

			nfloat docHeight = viewHeight<scrollHeight?scrollHeight:viewHeight;
				
			view.SetFrameSize (new CGSize(PublicationsCustomView.Frame.Width, docHeight));

			ScrollToTop ();
		}

		void ScrollToTop ()
		{
			// assume that the scrollview is an existing variable
			var documentView = (NSView)MainScrollView.DocumentView;
			if (documentView == null) {
				return;
			}

			CGPoint newScrollOrigin;
			if (documentView.IsFlipped) {
				newScrollOrigin = new CGPoint(0.0f, 0.0f);
			} else {
				newScrollOrigin = new CGPoint(0.0f, documentView.Frame.Height
					-MainScrollView.ContentView.Bounds.Height);
			}

			documentView.ScrollPoint(newScrollOrigin);
		}
		#endregion

		#region action
		partial void PopupButtonSelectChange (NSObject sender)
		{
			var button = (NSPopUpButton)sender;

			nint index = button.IndexOfSelectedItem;
			var rect = button.Frame;
			CGSize newSize;
			switch (index) {
			case 1:
				newSize = new CGSize(45, rect.Height);
				button.SetFrameSize(newSize);
				break;
			case 2:
				newSize = new CGSize(60, rect.Height);
				button.SetFrameSize(newSize);
				break;
			case 3:
				newSize = new CGSize(115, rect.Height);
				button.SetFrameSize(newSize);
				break;
			}

			button.Title = button.TitleOfSelectedItem;

			FilterAndShowPublicationsInScrollView(offlinePublicationList, Convert.ToInt32(index));
		}
		#endregion

		//organize publications
		public void HandleDeletePublictaionByBookID(int bookID)
		{
			offlinePublicationList = PublicationsDataManager.SharedInstance.GetLocalDBPublicationsList ();

			foreach (PublicationView bookView in  allPublicationsView) {
				if (bookView.BookInfo.BookId == bookID) {
					allPublicationsView.Remove (bookView);
					break;
				}
			}

			FilterAndShowPublicationsInScrollView (offlinePublicationList, publicationCategory);
			RefreshHistoryView ();
		}

		public void HandleSortPublictaions(List<int> bookIDArray)
		{
			offlinePublicationList = PublicationsDataManager.SharedInstance.GetLocalDBPublicationsList ();

			for (int index = 0; index < bookIDArray.Count; index++)
			{
				int bookID = bookIDArray[index];
				int idx = allPublicationsView.FindIndex (n => (((PublicationView)n).BookInfo.BookId == bookID));
				var bookview = allPublicationsView.Find (n => (((PublicationView)n).BookInfo.BookId == bookID));
				allPublicationsView.Add (bookview);
				allPublicationsView.RemoveAt (idx);
			}
				
			FilterAndShowPublicationsInScrollView (offlinePublicationList, publicationCategory);
		}

		void RefreshPublicationsView (bool isOnlineLoadFinished)
		{
			if ((offlinePublicationList == null || 
				!offlinePublicationList.Any())) {
				if (isOnlineLoadFinished) {
					SetPublicationViewHidden (true);
					InitializeControlTitle ();
				} else {
					publicationEmptyLabel.Hidden = true;
					bookScrollView.Hidden = true;
				}

				popupButton.SelectItemWithTag (PUBLICATION_FILTER_ALL);
			} else {
				SetPublicationViewHidden (false);

				popupButton.SelectItemWithTag (PUBLICATION_FILTER_ALL);
				ShowPublicationsInScrollView (offlinePublicationList, PUBLICATION_FILTER_ALL);
			}
		}

		public void RefreshHistoryView () 
		{
			var currentUser = GlobalAccess.Instance.CurrentUserInfo;
			if (currentUser == null) {
				return;
			}

			List<RecentHistoryItem> recentHistoryList = PublicationContentUtil.Instance.GetRecentHistory();

			if (recentHistoryList == null || !recentHistoryList.Any ()) {
				InitializeControlTitle ();
				SetHistoryViewHidden (true);
				return;
			} 

			if (recentHistoryList.Count > 1) {
				nfloat height = recentHistoryList.Count * historyTableView.RowHeight;
				nfloat totalHeight = PublicationsCustomView.Frame.Height+48+height;
				ReframeMainScrollViewWithHeight (totalHeight);
			}
				
			SetHistoryViewHidden (false);

			historyTableView.DataSource = new HistoryTableViewDataSources (recentHistoryList);
			historyTableView.Delegate = new HistoryTableViewDelegate (recentHistoryList, this);
			historyTableView.EnclosingScrollView.BorderType = NSBorderType.NoBorder;
			historyTableView.GridStyleMask = NSTableViewGridStyle.None;
			historyTableView.EnclosingScrollView.ScrollsDynamically = false;
			historyTableView.EnclosingScrollView.HorizontalScrollElasticity = NSScrollElasticity.None;
			historyTableView.EnclosingScrollView.VerticalScrollElasticity = NSScrollElasticity.None;
			historyTableView.AllowsColumnSelection = false;
			historyTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;

			historyTableView.EnclosingScrollView.HasHorizontalScroller = false;
			historyTableView.EnclosingScrollView.HasVerticalScroller = false;
			historyTableView.AllowsColumnResizing = true;
			historyTableView.ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Uniform;

			historyTableView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
			historyTableView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;

			historyTableView.RowHeight = 66;

		}

		private Publication PublicationByBookID(int bookID) 
		{
			Publication currentBook = null;
			foreach (var publication in offlinePublicationList) {
				if (publication.BookId == bookID) {
					PublicationsDataManager.SharedInstance.CurrentPublication = publication;
					currentBook = publication;
					break;
				}
			}
			return currentBook;
		}

		public async Task OpenContentViewFromHistory (int bookID, int tocID)
		{
			PublicationsDataManager.SharedInstance.CurrentPublication = 
			PublicationsDataManager.SharedInstance.GetCurrentPublicationByBookID (bookID);

			var bookview = (PublicationView)allPublicationsView.Find (n => (((PublicationView)n).BookInfo.BookId == bookID));
			PublicationsDataManager.SharedInstance.CurrentPublicationView = bookview;

			var controller = (PublicationsWindowController)Utility.GetMainWindowConroller();
			await controller.SwitchToContentView (tocID);
		}

		//PublicationScrollView hidden or show

		private void SetPublicationViewHidden(bool isHidden)
		{
			publicationEmptyLabel.Hidden = !isHidden;
			bookScrollView.Hidden = isHidden;
		}

		private void SetHistoryViewHidden(bool isHidden)
		{
			histroyEmptyLabel.Hidden = !isHidden;
			historyTableView.EnclosingScrollView.Hidden = isHidden;
		}

		private void InitializeControlTitle ()
		{
			publicationEmptyLabel.StringValue = "You have no titles, Please contact your Relationship Manager for support.";
			histroyEmptyLabel.StringValue = "Recent history items will be generated as you navigate through the content.";
		}

		private void ShowPublicationsInScrollView (List<Publication> publicationList, int publicationCategoryIndex)
		{
			publicationCategory = publicationCategoryIndex;

			if (bookScrollView != null && bookScrollView.DocumentView != null) {
				var documentView = (NSView)bookScrollView.DocumentView;
				NSView[] viewList = documentView.Subviews;

				foreach (PublicationView view in documentView.Subviews) {
					view.RemoveFromSuperview ();
				}

				allPublicationsView.Clear ();
			}

			int frameX = 0;
			int index = 0;
			var frame = new CGRect(0,0,PUBLICATION_COVER_WIDTH,PUBLICATION_VIEW_HEIGHT);
			var conentView = new NSView (frame);
			foreach (var curPublication in publicationList) {
				if (curPublication.IsLoan && publicationCategoryIndex == PUBLICATION_FILTER_SUBSCRIPTION) {
					continue;
				}

				if (!curPublication.IsLoan && publicationCategoryIndex == PUBLICATION_FILTER_LOAN) {
					continue;
				}

				NSView bookView = CreateBookViewWithIndex (curPublication,index);
				conentView.AddSubview (bookView);
				allPublicationsView.Add (bookView);
				index++;
				frameX += (PUBLICATION_COVER_WIDTH + PUBLICATION_COVER_HORIZONTAL_SPACING);
			}

			if (index > 0) {
				SetPublicationViewHidden (false);

				var scrollContentSize = new CGSize ();
				scrollContentSize.Width = (frameX-PUBLICATION_COVER_HORIZONTAL_SPACING);
				scrollContentSize.Height = bookScrollView.ContentSize.Height;
				frame.Size = scrollContentSize;
				conentView.SetFrameOrigin(new CGPoint (0,0));
				conentView.SetFrameSize (scrollContentSize);

				bookScrollView.DocumentView = conentView;
				var view = (NSView)bookScrollView.DocumentView;
				view.SetFrameSize (scrollContentSize);

				view = view.Superview;
				bookScrollView.AutohidesScrollers = true;

			} else {
				SetPublicationViewHidden (true);
			}
		}

		private NSView CreateBookViewWithIndex (Publication publication, int index)
		{
			var bookView = new PublicationView (BookFrameByIndex(index));
			bookView.InitializeValue (publication, index);

			return bookView;
		}

		private CGRect BookFrameByIndex (int index)
		{
			var frame = new CGRect(0,0,PUBLICATION_COVER_WIDTH,PUBLICATION_VIEW_HEIGHT);
			var location = new CGPoint (0,0);
			if (index != 0) {
				location.X = index * (PUBLICATION_COVER_WIDTH+PUBLICATION_COVER_HORIZONTAL_SPACING);
				nfloat ySpace = (bookScrollView.Bounds.Size.Height-PUBLICATION_VIEW_HEIGHT)/2;
				location.Y = ySpace>0?ySpace:0;
			}

			frame.Location = location;
			return frame;
		}

		//switch filter combobox
		private void FilterAndShowPublicationsInScrollView (List<Publication> publicationList, int publicationCategoryIndex )
		{
			publicationCategory = publicationCategoryIndex;

			if (bookScrollView != null && bookScrollView.DocumentView != null) {
				CreateNewContentViewFromAllView (publicationCategoryIndex);
			}
		}
			
		private void CreateNewContentViewFromAllView (int publicationCategoryIndex)
		{
			int frameX = 0;
			int index = 0;
			var frame = new CGRect(0,0,PUBLICATION_COVER_WIDTH,PUBLICATION_VIEW_HEIGHT);
			NSView conentView = new NSView (frame);

			var documentView = (NSView)bookScrollView.DocumentView;
			if (documentView != null) {
				NSView[] viewList = documentView.Subviews;
				foreach (PublicationView view in documentView.Subviews) {
					view.RemoveFromSuperview ();
				}
				documentView = null;
			}

			foreach (PublicationView view in allPublicationsView) {
				if (!view.BookInfo.IsLoan && publicationCategoryIndex == PUBLICATION_FILTER_LOAN) {
					continue;
				}

				if (view.BookInfo.IsLoan && publicationCategoryIndex == PUBLICATION_FILTER_SUBSCRIPTION) {
					continue;
				}
				view.Frame = BookFrameByIndex (index);
				conentView.AddSubview (view);

				index++;
				frameX += (PUBLICATION_COVER_WIDTH + PUBLICATION_COVER_HORIZONTAL_SPACING);
			}
			if (index > 0) {
				SetPublicationViewHidden (false);

				var scrollContentSize = new CGSize ();
				scrollContentSize.Width = (frameX-PUBLICATION_COVER_HORIZONTAL_SPACING); //PUBLICATION_COVER_WIDTH + PUBLICATION_COVER_HORIZONTAL_SPACING
				scrollContentSize.Height = bookScrollView.ContentSize.Height;
				frame.Size = scrollContentSize;
				conentView.SetFrameOrigin(new CGPoint (0,0));
				conentView.SetFrameSize (scrollContentSize);

				bookScrollView.DocumentView = conentView;
				bookScrollView.AutohidesScrollers = true;

			} else {
				if (allPublicationsView.Count == 0) {
					SetPublicationViewHidden (true);
				}
			}
		}

		public void HandleWindowDidResize (object sender, EventArgs e)
		{

			nfloat height = historyTableView.RowCount * historyTableView.RowHeight;
			nfloat viewHeight = PublicationsCustomView.Frame.Height+48+height;
			var scrollHeight = MainScrollView.Frame.Height;
			nfloat docHeight = viewHeight < scrollHeight ? scrollHeight : viewHeight;
				
			var view = (NSView)MainScrollView.DocumentView;
			view.SetFrameSize (new CGSize(PublicationsCustomView.Frame.Width, docHeight));
		}
	}
}

