using System;
using System.Collections.Generic;

using AppKit;
using Foundation;
using CoreGraphics;

using LexisNexis.Red.Common.BusinessModel;



namespace LexisNexis.Red.Mac
{
	public class AHLTagsTableDataSource: NSTableViewDataSource
	{
		public List<ColorTagState> TagsList { get; set; }
		public AHLTagsTableDataSource (List<ColorTagState> tagsList)
		{
			TagsList = tagsList;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return TagsList==null ? 0:TagsList.Count;
		}
	}

	public class AHLTagsTableDelegate : NSTableViewDelegate
	{
		public object ViewController { get; set;}
		public List<ColorTagState> TagsList { get; set; }
		const int vertitalSpace = 7;
		const int topSpace = 17;
		public AHLTagsTableDelegate (List<ColorTagState> tagsList, object viewController)
		{
			TagsList = tagsList;
			ViewController = viewController;
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var tableView = (NSTableView)notification.Object;
			nint index = tableView.SelectedRow;
			if (index < 0) {
				return;
			}
			var historyItem = TagsList[Convert.ToInt32(index)];

		}

		public override NSTableRowView CoreGetRowView (NSTableView tableView, nint row)
		{
			var rowView = tableView.GetRowView (row, true);
			if (rowView == null) {
				rowView = new NSTableRowView ();
			}
			return rowView;
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var valueKey = tableColumn.Identifier;
			var dataRow = TagsList[Convert.ToInt32(row)].Tag;
			var tableCellView = (NSTableCellView)tableView.MakeView (valueKey,this);
			switch (valueKey) {
			case "TAGSITEM":
				var viewList = tableCellView.Subviews;

				var colorView = (NSView)viewList [0];
				colorView.WantsLayer = true;
				colorView.Layer.BackgroundColor = Utility.ColorWithHexColorValue (dataRow.Color, 1.0f).CGColor;
				colorView.Layer.CornerRadius = 5.0f;

				NSTextField titleTF = (NSTextField)viewList [1];
				titleTF.Cell.Wraps = true;
				titleTF.Cell.DrawsBackground = true;
				titleTF.Cell.BackgroundColor = NSColor.Clear;
				titleTF.StringValue = dataRow.Title;

				NSButton checkButton = (NSButton)viewList [2];
				checkButton.State = TagsList [Convert.ToInt32 (row)].CheckState;
				checkButton.Action = new ObjCRuntime.Selector ("CheckButtonClick:");
				checkButton.Target = this;

				var lineView = (NSView)viewList [3];
				lineView.WantsLayer = true;
				lineView.Layer.BackgroundColor = NSColor.Grid.CGColor;

				return tableCellView;
			}

			return null;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			return 34.0f;
		}

		[Action("CheckButtonClick:")]
		void CheckButtonClick(NSObject sender)
		{
			var button = (NSButton)sender;
			//bool isSelected = button.State == NSCellStateValue.On?true:false;

			var cellView = (NSTableCellView)button.Superview;
			var tableRowView = (NSTableRowView)cellView.Superview;
			var tableView = (NSTableView)tableRowView.Superview;
			nint row = tableView.RowForView(tableRowView);
			int index = Convert.ToInt32 (row);
			if (row < 0 || row >= TagsList.Count) {
				return;
			}

			if (this.ViewController is AddHighlightViewController) {
				var viewController = (AddHighlightViewController)this.ViewController;
				viewController.SetCheckStateAtIndex (index, button.State);
			} else if (this.ViewController is AddAnnotationViewController){
				var viewController = (AddAnnotationViewController)this.ViewController;
				viewController.SetCheckStateAtIndex (index, button.State);
			}
		}
	}
}

