using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using LexisNexis.Red.Mac.Data;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using System.Threading.Tasks;

namespace LexisNexis.Red.Mac
{
	public partial class HistoryPopoverController : AppKit.NSViewController
	{
		#region properties
		NSPopover parentPopover;
		public List<RecentHistoryItem> RecentHistoryList;
		int BookID { get; set; }
		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public HistoryPopoverController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public HistoryPopoverController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public HistoryPopoverController (NSPopover popover, int bookID) : base ("HistoryPopover", NSBundle.MainBundle)
		{
			parentPopover = popover;
			BookID = bookID;
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
			RecentHistoryList = PublicationContentUtil.Instance.GetRecentHistory();
			//RecentHistoryList = PublicationContentUtil.Instance.GetRecentHistory(BookID, 20);
		}

		#endregion

		//strongly typed view accessor
		public new HistoryPopover View {
			get {
				return (HistoryPopover)base.View;
			}
		}

		#region methods
		public override void AwakeFromNib ()
		{
			PostInitialization ();
		}

		void PostInitialization ()
		{
			//HistoryLabel.StringValue = "Recent History";
			if (RecentHistoryList == null || RecentHistoryList.Count == 0) {
				HistoryTableView.Hidden = true;
				NoHistoryLabel.Hidden = false;
				NoHistoryLabel.StringValue = "You have no recent history.";
				
			} else {
				NoHistoryLabel.Hidden = true;

				HistoryTableView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
				HistoryTableView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;
				HistoryTableView.EnclosingScrollView.BackgroundColor = NSColor.White;

				HistoryTableView.GridStyleMask = NSTableViewGridStyle.None;
				HistoryTableView.EnclosingScrollView.BorderType = NSBorderType.NoBorder;
		
				HistoryTableView.DataSource = new PopHistoryTableDataSource (RecentHistoryList);
				HistoryTableView.Delegate = new PopHistoryTableDelegate (RecentHistoryList,this);
				HistoryTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;
			}

		}

		public async Task OpenPublicationContentAtHistory (RecentHistoryItem recentHistoryItem)
		{
			parentPopover.Close ();

			int bookID = recentHistoryItem.BookId;
			var docID = recentHistoryItem.DOCID;
			int tocID = await PublicationContentUtil.Instance.GetTOCIDByDocId(bookID, docID);

			var winController = Utility.GetMainWindowConroller();
			await winController.UpdateContentView (bookID, tocID);

		}
		#endregion
	}
}
