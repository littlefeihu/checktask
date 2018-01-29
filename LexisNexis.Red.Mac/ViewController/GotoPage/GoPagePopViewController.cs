using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using AppKit;
using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;
using System.Threading.Tasks;
using LexisNexis.Red.Mac.Data;
using System.Text.RegularExpressions;

namespace LexisNexis.Red.Mac
{
	public partial class GoPagePopViewController : AppKit.NSViewController
	{
		#region properties
		NSPopover parentPopover;
		int BookID { get; set; }
		int TocID { get; set; }
		List<PageSearchItem> PageItemList { get; set; }
		const float ViewHeight_Min = 53.0f;
		const float ViewHeight_Max = 287.0f;
		#endregion


		#region Constructors

		// Called when created from unmanaged code
		public GoPagePopViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public GoPagePopViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public GoPagePopViewController (NSPopover popover, int bookID, int tocID) : base ("GoPagePopView", NSBundle.MainBundle)
		{
			parentPopover = popover;
			BookID = bookID;
			TocID = tocID;
			PageItemList = new List<PageSearchItem> (0);

			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new GoPagePopView View {
			get {
				return (GoPagePopView)base.View;
			}
		}

		public override void ViewDidLoad ()
		{
			SearchField.WantsLayer = true;
			SearchField.Layer.CornerRadius = 4;
			SearchField.Layer.BorderColor = NSColor.Grid.CGColor;
			SearchField.Cell.DrawsBackground = true;
			SearchField.Layer.BorderWidth = 1;
			SearchField.Cell.BackgroundColor = NSColor.White;

			SearchField.Cell.SendsWholeSearchString = true;
			SearchField.Cell.PlaceholderString = "Search pages";

			SetViewResultFrame (false);

			InitializeTableView ();
		}

		async partial void SearchNumber (NSObject sender)
		{
			var searchField = (NSSearchField)sender;
			var value = searchField.StringValue;
			if (string.IsNullOrEmpty(value)){
				SetViewResultFrame(false);
				return;
			}

			PageItemList.Clear();

			int pageNumber;
			bool isPageNumber = Int32.TryParse(value, out pageNumber);
			if (isPageNumber) {
				var pageList = await PageSearchUtil.Instance.SeachByPageNum(BookID, pageNumber);
				PageItemList.AddRange(pageList);
			}
				
			RefreshSearchData ();

			SetViewResultFrame(true);
		}

		private void SetViewResultFrame(bool isShow)
		{
			CGRect frame = View.Frame;
			if (isShow) {
				CGSize newSize = new CGSize (frame.Width,ViewHeight_Max);
				parentPopover.ContentSize = newSize;
			} else {
				CGSize newSize = new CGSize (frame.Width,ViewHeight_Min);
				parentPopover.ContentSize = newSize;
			}
		}

		private void InitializeTableView ()
		{
			SearchField.BackgroundColor = NSColor.White;
			SearchTableView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
			SearchTableView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;
			SearchTableView.EnclosingScrollView.BackgroundColor = NSColor.White;

			SearchTableView.GridStyleMask = NSTableViewGridStyle.None;
			SearchTableView.EnclosingScrollView.BorderType = NSBorderType.BezelBorder;

			SearchTableView.DataSource = new GoPageTableDataSource (PageItemList);
			SearchTableView.Delegate = new GoPageTableDelegate (PageItemList,this);
			SearchTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;

		}

		void RefreshSearchData ()
		{
			if (PageItemList == null || PageItemList.Count == 0) {
				SearchTableView.EnclosingScrollView.Hidden = true;
				NoResultLabel.Hidden = false;
				NoResultLabel.StringValue = "Page Not Found.\n" +
					"The page you're looking for\n" +
					" doesn't exist in this publication. \nPlease try again.";

			} else {
				SearchTableView.EnclosingScrollView.Hidden = false;
				NoResultLabel.Hidden = true;
				var aObject = SearchTableView.Delegate;
				if (aObject is GoPageTableDelegate) {
					((GoPageTableDelegate)aObject).UpdateData (PageItemList);
					SearchTableView.ReloadData ();
				}
			}
		}

		async public Task OpenContentPageAtSearchPageItem (PageSearchItem resultItem)
		{
			parentPopover.Close ();

			int tocID = resultItem.TOCID;

			var winController = Utility.GetMainWindowConroller();

			winController.ContentVC.searchPageNumber = SearchField.StringValue;
			await winController.ContentVC.HighlightContentView (tocID, false);
		}
	}
}
