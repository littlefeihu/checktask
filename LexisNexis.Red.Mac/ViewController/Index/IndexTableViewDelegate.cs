using System;
using AppKit;
using Foundation;
using CoreGraphics;
using LexisNexis.Red.Mac.Data;

namespace LexisNexis.Red.Mac
{
	public class IndexTableViewDelegate: NSOutlineViewDelegate
	{
		IndexViewController viewController { get; set;}
		IndexDataManager indexDataManager {get; set;}

		public IndexTableViewDelegate (IndexViewController controller)
		{
			viewController = controller;
			indexDataManager = viewController.IndexDataManager;
		}

		public override NSTableRowView RowViewForItem (NSOutlineView outlineView, NSObject item)
		{
			var rowView = new IndexTableRowView ();

			int level;
			if (indexDataManager.IsGroupItem (item)) {
				level = 1;
			} else {
				level = 2;
			}

			rowView.Level = level;

			return rowView;
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var tableView = (NSOutlineView)notification.Object;
			nint selRow = tableView.SelectedRow;
			indexDataManager.CurrentRow = selRow;

			var item = tableView.ItemAtRow (selRow);
			var parentItem = tableView.GetParent (item);

			nint parentRow = selRow;
			if (parentItem != null) {
				parentRow = tableView.RowForItem (parentItem);
				int index = Convert.ToInt32 (selRow - parentRow) - 1;

				var indexItem = indexDataManager.IndexObjectAtChildrenList (indexDataManager.ChildrenForGroupItem(parentItem), index);
				indexDataManager.CurrentIndex = indexItem;

				if (indexItem != null) {
					viewController.OpenPublicationIndexAtIndexNode (indexItem);
				}
			}
		}

		//"outlineView:isGroupItem:"
		public override bool IsGroupItem (NSOutlineView outlineView, NSObject item)
		{
			if (indexDataManager.IsGroupItem (item)) {
				return true;
			} else {
				return false;
			}
		}

		//"outlineView:heightOfRowByItem:"
		public override nfloat GetRowHeight (NSOutlineView outlineView, NSObject item)
		{
			if (indexDataManager.IsGroupItem (item)) {
				return 23;
			} else {
				return 44;
			}
		}

		//"outlineView:shouldSelectItem:"
		public override bool ShouldSelectItem (NSOutlineView outlineView, NSObject item)
		{
			if (indexDataManager.IsGroupItem (item)) {
				return false;
			} else {
				return true;
			}
		}

		//"outlineView:shouldShowOutlineCellForItem:"
		public override bool ShouldShowOutlineCell (NSOutlineView outlineView, NSObject item)
		{
			return false;
		}

		//"outlineView:viewForTableColumn:item:"
		public override NSView GetView (NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
		{
			string identifier = tableColumn == null? "IndexItem":tableColumn.Identifier;
			if (indexDataManager.IsGroupItem (item)) {
				var cellView = (NSTableCellView)outlineView.MakeView (identifier,this);
				cellView.TextField.StringValue = item.ToString ();
				return cellView;
			} else {
				var cellView = (NSTableCellView)outlineView.MakeView (identifier,this);
				cellView.TextField.StringValue = item.ToString ();
				return cellView;
			}
		}
	}
}

