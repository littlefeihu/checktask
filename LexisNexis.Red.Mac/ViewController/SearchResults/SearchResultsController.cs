using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;
using System.Threading.Tasks;
using CoreGraphics;
using System.Text.RegularExpressions;
using LexisNexis.Red.Common.BusinessModel;

using LexisNexis.Red.Mac.Data;

namespace LexisNexis.Red.Mac
{
	public partial class SearchResultsController : AppKit.NSViewController
	{
		#region properties
		NSPopover parentPopover;
		public SearchResult SearchResultsData;
		int BookID { get; set; }
		int tocID { get; set; }
		string keyWord { get; set; }
		LoadingViewController loadViewCtr;
		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public SearchResultsController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SearchResultsController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SearchResultsController (NSPopover popover, int bookID, int tocID, string keyword) : base ("SearchResults", NSBundle.MainBundle)
		{
			parentPopover = popover;
			BookID = bookID;
			keyWord = keyword;

			List<ContentCategory> categoryList = new List<ContentCategory> (0);
			categoryList.Add (ContentCategory.All);

			AddTimer ();

			SearchResultsData = SearchUtil.Search (BookID, tocID, keyword, categoryList);

			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new SearchResults View {
			get {
				return (SearchResults)base.View;
			}
		}

		#region methods
		public override void AwakeFromNib ()
		{
			PostInitialization ();
		}

		void PostInitialization ()
		{
			if (SearchResultsData == null || SearchResultsData.SearchDisplayResultList.Count == 0) {
				SearchTableView.Hidden = true;
				NoResultLabel.Hidden = false;
				NoResultLabel.StringValue = "No search results found.";

			} else {
				NoResultLabel.Hidden = true;

				SearchTableView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
				SearchTableView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;
				SearchTableView.EnclosingScrollView.BackgroundColor = NSColor.White;

				SearchTableView.GridStyleMask = NSTableViewGridStyle.SolidHorizontalLine;
				SearchTableView.EnclosingScrollView.BorderType = NSBorderType.BezelBorder;
				SearchTableView.FloatsGroupRows = true;

				SearchTableView.DataSource = new SearchTableDataSource (SearchResultsData);
				SearchTableView.Delegate = new SearchTableDelegate (SearchResultsData,this);
				SearchTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;
			}
		}
			
		partial void PopupButtonSelectChange (NSObject sender)
		{
			var button = (NSPopUpButton)sender;

			nint index = button.IndexOfSelectedItem;
			var rect = button.Frame;
			CGSize newSize;
			CGPoint newPoint = rect.Location;
			string keyword = keyWord;
			ContentCategory contentType = ContentCategory.All;
			nfloat height = rect.Height+10;
			switch (index) {
			case 1:
				newSize = new CGSize(42, height);
				button.SetFrameSize(newSize);
				newPoint.X = 248;
				button.SetFrameOrigin(newPoint);
				contentType = ContentCategory.All;

				break;
			case 2:
				newSize = new CGSize(100, height);
				button.SetFrameSize(newSize);
				newPoint.X = 190;
				button.SetFrameOrigin(newPoint);
				contentType = ContentCategory.LegislationType;

				break;
			case 3:
				newSize = new CGSize(110, height);
				button.SetFrameSize(newSize);
				newPoint.X = 180;
				button.SetFrameOrigin(newPoint);
				contentType = ContentCategory.CommentaryType;

				break;
			case 4:
				newSize = new CGSize(158, height);
				button.SetFrameSize(newSize);
				newPoint.X = 132;
				button.SetFrameOrigin(newPoint);
				contentType = ContentCategory.FormsPrecedentsType;

				break;

			case 5:
				newSize = new CGSize(50, height);
				button.SetFrameSize(newSize);
				newPoint.X = 240;
				button.SetFrameOrigin(newPoint);
				contentType = ContentCategory.CaseType;

				break;
			}

			button.Title = button.TitleOfSelectedItem;

			List<ContentCategory> categoryList = new List<ContentCategory> (0);
			categoryList.Add (contentType);

			SearchResultsData = null;
			AddTimer();

			SearchResultsData = SearchUtil.Search (BookID, tocID, keyword, categoryList);
			//Console.WriteLine ("search end:{0} SearchResultsData:{1}", NSDate.Now.ToString(), SearchResultsData);

			if (SearchResultsData!=null && SearchResultsData.SearchDisplayResultList!=null && SearchResultsData.SearchDisplayResultList.Count != 0){
				((SearchTableDataSource)SearchTableView.DataSource).SearchResults = SearchResultsData;
				((SearchTableDelegate)SearchTableView.Delegate).SearchResults = SearchResultsData;
				SearchTableView.ReloadData();
				SearchTableView.Hidden = false;
				NoResultLabel.Hidden = true;
			}else {
				SearchTableView.Hidden = true;
				NoResultLabel.Hidden = false;
				NoResultLabel.StringValue = "No search results found.";
			}

			RemoveLoadView();
		}

		public async Task OpenPublicationContentAtSearchItem (SearchDisplayResult resultItem)
		{
			parentPopover.Close ();

			int tocID = resultItem.TocId;

			var winController = Utility.GetMainWindowConroller();

			PublicationsDataManager.SharedInstance.CurrentPublication = 
				PublicationsDataManager.SharedInstance.GetCurrentPublicationByBookID (BookID);

			var section = resultItem.isDocument ? resultItem.SnippetContent : resultItem.GuideCardTitle;
			var title = resultItem.isDocument ? resultItem.Head : resultItem.TocTitle;
			var header = resultItem.isDocument ? resultItem.HeadType + "," + resultItem.HeadSequence : null;
			winController.ContentVC.TOCDataManager.UpdateSearchMatchWordList (SearchResultsData.FoundWordList, 
				SearchResultsData.KeyWordList, section, header);

//			var resultItemData = new SearchNavigationData ();
//			resultItemData.FoundWordList.AddRange(SearchResultsData.KeyWordList);
//			resultItemData.SearchItem.ContentType = resultItem.ContentType;
//			resultItemData.SearchItem.Head = resultItem.Head;
//			resultItemData.SearchItem.HeadSequence = resultItem.HeadSequence;
//			resultItemData.SearchItem.HeadType = resultItem.HeadType;
//			resultItemData.SearchItem.isDocument = resultItem.isDocument;
//			resultItemData.SearchItem.SnippetContent = resultItem.SnippetContent;
			
			await winController.ContentVC.HighlightContentView (tocID, true);

		}

		public void AddLoadView ()
		{
			if (loadViewCtr == null) {

				//var frame = View.Frame;
				var frame = SearchTableView.EnclosingScrollView.Frame;
				var location = new CGPoint ();
				location.X = (frame.Width - 200) / 2;
				location.Y = (frame.Height-100)/2;

				loadViewCtr = new LoadingViewController (location, LNRConstants.LOADING_SEARCH_INFO);
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
			
		public void AddTimer ()
		{
			var timeout = new TimeSpan ((long)(((2 * TimeSpan.TicksPerSecond) / 1) + 0.0));

			NSTimer timer = NSTimer.CreateScheduledTimer(timeout, 
				delegate {
					if (SearchResultsData == null) {
					    AddLoadView();
						NSRunLoop.Current.RunUntil(NSDate.FromTimeIntervalSinceNow(0.1));
						RemoveLoadView ();
					}
				});

			NSRunLoop.Current.AddTimer (timer, NSRunLoopMode.Default);
		}

		#endregion
	}
}
