
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
		const int SPLITVIEW_MAXHEIGHT = 460;
		const int PUBLICATION_VIEW_HEIGHT = 350;
		const int PUBLICATION_COVER_HEIGHT = 270;
		const int PUBLICATION_COVER_WIDTH = 200;
		const int PUBLICATION_COVER_HORIZONTAL_SPACING = 30;

		const int PUBLICATION_FILTER_ALL = 1;
		const int PUBLICATION_FILTER_LOAN = 2;
		const int PUBLICATION_FILTER_SUBSCRIPTION = 3;

		public List<Publication> offlinePublicationList;
		List<Publication> onlinePublicationList;
		List<NSView> categoryViewList { get; set;}
		//NSMutableArray categoryViewsArray;

		//NSView allPublicationsView;
		//NSView subPublicationsView;
		//NSView loanPublicationsView;

		public bool IsHandleNoditifation { get; set;}
		int publicationCategory {get; set;} 

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
			offlinePublicationList = PublicationsDataManager.SharedInstance.GetLocalDBPublicationsList();

			await PublicationsDataManager.SharedInstance.InitAndLoadDB ();
			onlinePublicationList = PublicationsDataManager.SharedInstance.onlinePublicationList;

			if (onlinePublicationList != null) {
				offlinePublicationList = onlinePublicationList;
				RefreshHistoryView ();
				RefreshPublicationsView ();
			}
		}

		#endregion

		#region action
		partial void PopupButtonSelectChange (NSObject sender)
		{
			var button = (NSPopUpButton)sender;

			nint index = button.IndexOfSelectedItem;

			button.Title = button.TitleOfSelectedItem;

			//ShowPublicationsInScrollView (offlinePublicationList, Convert.ToInt32(index));
			FilterAndShowPublicationsInScrollView(offlinePublicationList, Convert.ToInt32(index));
		}
		#endregion

		public override void LoadView ()
		{
			base.LoadView ();

			bookScrollView.AutohidesScrollers = true;
			bookScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
			bookScrollView.HorizontalScroller.ControlSize = NSControlSize.Small;

			RefreshHistoryView ();
			RefreshPublicationsView ();

			NSNotificationCenter.DefaultCenter.AddObserver (LNRConstants.LNPublicationDidDeleteNotification, 
				HandleNotification);
		}

		//organize publications
		public void HandleNotification (NSNotification notification)
		{
			if (IsHandleNoditifation) {
				return;
			}
				
			IsHandleNoditifation = true;
			offlinePublicationList = PublicationsDataManager.SharedInstance.GetLocalDBPublicationsList ();
		    
			ShowPublicationsInScrollView(offlinePublicationList, publicationCategory);
			IsHandleNoditifation = false;
		}

		void RefreshPublicationsView ()
		{
			if ((offlinePublicationList == null || 
				!offlinePublicationList.Any()) &&
				(onlinePublicationList == null ||
					!onlinePublicationList.Any()) ) {
				SetPublicationViewHidden (true);
				InitializeControlTitle ();

				popupButton.SelectItemWithTag (PUBLICATION_FILTER_ALL);
			} else {
				SetPublicationViewHidden (false);

				popupButton.SelectItemWithTag (PUBLICATION_FILTER_ALL);
				ShowPublicationsInScrollView (offlinePublicationList, PUBLICATION_FILTER_ALL);

				//all
				var docView = (NSView)bookScrollView.DocumentView;
				if (categoryViewList == null) {
					categoryViewList = new List<NSView>(3);
				}
				categoryViewList.Add (docView);
				Console.WriteLine ("count:{0}",docView.Subviews.Length);
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
				SetHistoryViewHidden (true);
				return;
			} 
				
			SetHistoryViewHidden (false);

			historyTableView.DataSource = new HistoryTableViewDataSources (recentHistoryList);
			historyTableView.Delegate = new HistoryTableViewDelegate (recentHistoryList);
			historyTableView.EnclosingScrollView.BorderType = NSBorderType.NoBorder;
			historyTableView.GridStyleMask = NSTableViewGridStyle.None;

			historyTableView.AllowsColumnSelection = false;
			historyTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;

			historyTableView.EnclosingScrollView.HasHorizontalScroller = false;
			historyTableView.AllowsColumnResizing = true;
			historyTableView.ColumnAutoresizingStyle = NSTableViewColumnAutoresizingStyle.Uniform;

			historyTableView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
			historyTableView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;

			historyTableView.RowHeight = 66;

		}

		//PublicationScrollView hidden or show
		void InitializeControlStatas (bool isHidden)
		{
			publicationEmptyLabel.Hidden = !isHidden;
			histroyEmptyLabel.Hidden = !isHidden;

			bookScrollView.Hidden = isHidden;
			historyTableView.EnclosingScrollView.Hidden = isHidden;
		}

		void SetPublicationViewHidden(bool isHidden)
		{
			publicationEmptyLabel.Hidden = !isHidden;
			bookScrollView.Hidden = isHidden;
		}

		void SetHistoryViewHidden(bool isHidden)
		{
			histroyEmptyLabel.Hidden = !isHidden;
			historyTableView.EnclosingScrollView.Hidden = isHidden;
		}

		void InitializeControlTitle ()
		{
			publicationEmptyLabel.StringValue = "You have no titles, Please contact your Relationship Manager for support.";
			histroyEmptyLabel.StringValue = "You have no recent history.";
		}

		void ShowPublicationsInScrollView (List<Publication> publicationList, int publicationCategoryIndex )
		{
			publicationCategory = publicationCategoryIndex;

			if (bookScrollView != null && bookScrollView.DocumentView != null) {
				var documentView = (NSView)bookScrollView.DocumentView;
				NSView[] viewList = documentView.Subviews;

				foreach (PublicationView view in documentView.Subviews) {
					if (view.PublicationDownloadingProgress() > 0) {
						view.CancelDownload ();
					}
					view.RemoveFromSuperview ();
				}
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

				var docView = (NSView)bookScrollView.DocumentView;
				docView.SetFrameSize (scrollContentSize);
				bookScrollView.DocumentView = conentView;
				bookScrollView.AutohidesScrollers = true;

			} else {
				SetPublicationViewHidden (true);
			}
		}

		NSView CreateBookViewWithIndex (Publication publication, int index)
		{
			var frame = new CGRect(0,0,PUBLICATION_COVER_WIDTH,PUBLICATION_VIEW_HEIGHT);
			var location = new CGPoint ();
			if (index != 0) {
				location.X = index * (PUBLICATION_COVER_WIDTH+PUBLICATION_COVER_HORIZONTAL_SPACING);
				nfloat ySpace = (bookScrollView.Bounds.Size.Height-PUBLICATION_VIEW_HEIGHT)/2;
				location.Y = ySpace>0?ySpace:0;
			}

			frame.Location = location;

			var bookView = new PublicationView (frame);
			bookView.InitializeValue (publication, index);

			return bookView;
		}

		//switch filter combobox
		void FilterAndShowPublicationsInScrollView (List<Publication> publicationList, int publicationCategoryIndex )
		{
			publicationCategory = publicationCategoryIndex;

			if (bookScrollView != null && bookScrollView.DocumentView != null) {

				NSView conentView = null;
				switch (publicationCategoryIndex) {
				case PUBLICATION_FILTER_ALL:
					conentView = categoryViewList[0];
					break;

				case PUBLICATION_FILTER_LOAN:
					if (categoryViewList.Count >= 2) {
						conentView = categoryViewList [1];
					} else {
						CreateNewContentView (publicationCategoryIndex);
					}
					break;

				case PUBLICATION_FILTER_SUBSCRIPTION:
					if (categoryViewList.Count >= 3) {
						conentView = categoryViewList [2];
					} else {
						CreateNewContentView (publicationCategoryIndex);
					}
					break;
				}

				if (conentView != null) {
					var scrollContentSize = new CGSize ();
					scrollContentSize.Width = conentView.Frame.Width;
					scrollContentSize.Height = bookScrollView.ContentSize.Height;
					conentView.SetFrameOrigin(new CGPoint (0,0));
					conentView.SetFrameSize (scrollContentSize);

					var docView = (NSView)bookScrollView.DocumentView;
					docView.SetFrameSize (scrollContentSize);
					bookScrollView.DocumentView = conentView;
					bookScrollView.AutohidesScrollers = true;
				}
			}
		}

		void RelayoutContentViewAtFilter (int publicationCategoryIndex)
		{
			int frameX = 0;
			int index = 0;
			var frame = new CGRect(0,0,PUBLICATION_COVER_WIDTH,PUBLICATION_VIEW_HEIGHT);
			NSView conentView = new NSView (frame);
			var documentView = (NSView)bookScrollView.DocumentView;

			foreach (PublicationView view in categoryViewList[0].Subviews) {
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

				var docView = (NSView)bookScrollView.DocumentView;
				docView.SetFrameSize (scrollContentSize);
				bookScrollView.DocumentView = conentView;
				bookScrollView.AutohidesScrollers = true;

			} else {
				SetPublicationViewHidden (true);
			}
		}

		CGRect BookFrameByIndex (int index)
		{
			var frame = new CGRect(0,0,PUBLICATION_COVER_WIDTH,PUBLICATION_VIEW_HEIGHT);
			var location = new CGPoint ();
			if (index != 0) {
				location.X = index * (PUBLICATION_COVER_WIDTH+PUBLICATION_COVER_HORIZONTAL_SPACING);
				nfloat ySpace = (bookScrollView.Bounds.Size.Height-PUBLICATION_VIEW_HEIGHT)/2;
				location.Y = ySpace>0?ySpace:0;
			}

			frame.Location = location;
			return frame;
		}

		NSView CreateNewContentView (int publicationCategoryIndex)
		{
			int frameX = 0;
			int index = 0;
			var frame = new CGRect(0,0,PUBLICATION_COVER_WIDTH,PUBLICATION_VIEW_HEIGHT);
			NSView conentView = new NSView (frame);
			var documentView = (NSView)bookScrollView.DocumentView;

			foreach (PublicationView view in categoryViewList[0].Subviews) {
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

				var docView = (NSView)bookScrollView.DocumentView;
				docView.SetFrameSize (scrollContentSize);
				bookScrollView.DocumentView = conentView;
				bookScrollView.AutohidesScrollers = true;

				categoryViewList.Add (conentView);

			} else {
				SetPublicationViewHidden (true);
			}

			return conentView;
		}

		//		NSView GetCurrentViewObjectByViewID(int publicationCategoryIndex)
		//		{
		//			foreach (NSDictionary dict in categoryViewsArray) {
		//				NSNumber numberValue = (NSNumber)dict.Keys [0];
		//				if (numberValue.Int32Value == publicationCategoryIndex) {
		//					return dict.ObjectForKey(numberValue);
		//				}
		//			}
		//
		//			return null;
		//		}
	}

}

