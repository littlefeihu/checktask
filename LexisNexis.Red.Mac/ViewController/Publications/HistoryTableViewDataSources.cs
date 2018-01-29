using System;
using System.Collections.Generic;
using AppKit;
using Foundation;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;


namespace LexisNexis.Red.Mac
{
	public class HistoryTableViewDataSources : NSTableViewDataSource
	{
		public List<RecentHistoryItem> RecentHistoryList { get; set; }

		public HistoryTableViewDataSources (List<RecentHistoryItem> historyList)
		{
			RecentHistoryList = historyList;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return RecentHistoryList == null ? 0 : RecentHistoryList.Count;
		}
	}

	public class HistoryTableViewDelegate : NSTableViewDelegate
	{
		public List<RecentHistoryItem> RecentHistoryList { get; set; }
		public PublicationsViewController ownerViewController {get; set;}
		public HistoryTableViewDelegate (List<RecentHistoryItem> recentHistoryList, PublicationsViewController viewController)
		{
			RecentHistoryList = recentHistoryList;
			ownerViewController = viewController;
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
			//Console.WriteLine ("row:{0}", row);
			var valueKey = tableColumn.Identifier;
			var dataRow = RecentHistoryList[Convert.ToInt32(row)];
			var tableCellView = (NSTableCellView)tableView.MakeView (valueKey,this);
			switch (valueKey) {
			case "HISTORYITEM":
				var viewList = tableCellView.Subviews;
				var colorView = (NSView)viewList [0];
				colorView.WantsLayer = true;
				colorView.Layer.BackgroundColor = Utility.ColorWithHexColorValue (dataRow.ColorPrimary, 1.0f).CGColor;

				NSTextField nameTF = (NSTextField)viewList [1];
				nameTF.StringValue = dataRow.PublicationTitle;
				nameTF.ToolTip = dataRow.PublicationTitle;

				NSTextField despTF = (NSTextField)viewList [2];
				despTF.StringValue = dataRow.TOCTitle;
				despTF.ToolTip = dataRow.TOCTitle;

				NSTextField dateTF = (NSTextField)viewList [3];
				dateTF.StringValue = Utility.FormateLastReadDate (dataRow.LastReadDate);
				dateTF.ToolTip = Utility.FormateLastReadDate (dataRow.LastReadDate);
				return tableCellView;
			}

			return null;
		}
	
		public async override void SelectionDidChange (NSNotification notification)
		{
			var tableView = (NSTableView)notification.Object;
			nint index = tableView.SelectedRow;
			if (index < 0) {
				return;
			}
			var historyItem = RecentHistoryList[Convert.ToInt32(index)];
			var docID = historyItem.DOCID;
			int tocID = await PublicationContentUtil.Instance.GetTOCIDByDocId(historyItem.BookId, docID);
			await ownerViewController.OpenContentViewFromHistory (historyItem.BookId, tocID);
		}

	}

}

