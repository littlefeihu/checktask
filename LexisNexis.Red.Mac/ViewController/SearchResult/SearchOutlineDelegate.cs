using System;
using AppKit;
using Foundation;
using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.Mac
{
	public class SearchOutlineDelegate: NSOutlineViewDelegate
	{
		#region properties
		SearchResultViewController viewController { get; set;}
		SearchResultManager SearchManager { get; set;}
		#endregion

		#region constructor
		public SearchOutlineDelegate (SearchResultViewController controller) : base ()
		{
			viewController = controller;
			SearchManager = controller.DataManager;
		}
		#endregion

		#region NSOutlineViewDelegate
		//"outlineView:rowViewForItem:"
		public override NSTableRowView RowViewForItem (NSOutlineView outlineView, NSObject item)
		{
			var rowView = new IndexTableRowView ();

			rowView.Level = 1;

			return rowView;
		}

		//"outlineViewSelectionDidChange:"
		public override void SelectionDidChange (NSNotification notification)
		{
			var tableView = (NSOutlineView)notification.Object;
			int selRow = Convert.ToInt16(tableView.SelectedRow);
			viewController.OpenPublicationAtTocNode (null);
			return;

			int docCount = SearchManager.DocumentResultList.Count;
			int pubCount = SearchManager.PublicationResultList.Count;
			bool isDocumentItem = selRow - 1 < docCount ? true : false;
			SearchDisplayResult searchItem = null;
			if (isDocumentItem) {
				searchItem = SearchManager.DocumentResultList [selRow - 1];
			} else {
				int index = selRow - 1 - docCount - 1;
				searchItem = SearchManager.PublicationResultList [index];
			}

			if (searchItem != null) {
				viewController.OpenPublicationAtTocNode (searchItem);
			}
		}

		//"outlineView:isGroupItem:"
		public override bool IsGroupItem (NSOutlineView outlineView, NSObject item)
		{
			if (SearchManager.IsGroupItem (item)) {
				return true;
			} else {
				return false;
			}
		}

		//"outlineView:heightOfRowByItem:"
		public override nfloat GetRowHeight (NSOutlineView outlineView, NSObject item)
		{
			if (SearchManager.IsGroupItem (item)) {
				return 23;
			} else {
				return 74;
			}
		}

		//"outlineView:shouldSelectItem:"
		public override bool ShouldSelectItem (NSOutlineView outlineView, NSObject item)
		{
			if (SearchManager.IsGroupItem (item)) {
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
			if (SearchManager.IsGroupItem (item)) {
				var cellView = (NSTableCellView)outlineView.MakeView ("GroupItem",this);
				cellView.TextField.StringValue = item.ToString ();
				return cellView;
			} else {
				NSTableCellView cellView = (NSTableCellView)outlineView.MakeView ("SearchItem",this);
				NSView [] viewList = cellView.Subviews;
				NSTextField titleTF = (NSTextField)viewList [0];
				NSTextField sectionTF = (NSTextField)viewList [1];
				NSTextField numberTF = (NSTextField)viewList [2];
				NSDictionary value = (NSDictionary)item;
				titleTF.StringValue = value.ObjectForKey (NSObject.FromObject("key2")).ToString();
//				titleTF.Cell.DrawsBackground = true;
//				titleTF.Cell.BackgroundColor = NSColor.Clear;
				titleTF.ToolTip = value.ObjectForKey (NSObject.FromObject("key2")).ToString();
				sectionTF.StringValue = value.ObjectForKey (NSObject.FromObject("key3")).ToString();
//				sectionTF.Cell.DrawsBackground = true;
//				sectionTF.Cell.BackgroundColor = NSColor.Clear;
				sectionTF.ToolTip = value.ObjectForKey (NSObject.FromObject("key3")).ToString();
//				numberTF.Cell.DrawsBackground = true;
//				numberTF.Cell.BackgroundColor = NSColor.Clear;
				numberTF.StringValue = "";
				numberTF.ToolTip = "";
				return cellView;
			}
		}

		#endregion
	}
}

