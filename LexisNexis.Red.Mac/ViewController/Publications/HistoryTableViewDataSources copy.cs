using System;
using System.Collections.Generic;
using AppKit;
using Foundation;
using LexisNexis.Red.Common.BusinessModel;


namespace LexisNexis.Red.Mac
{
	public class HistoryTableViewDataSources : NSTableViewDataSource
	{
		public List<Publication> offlinePublicationList { get; set; }

		public HistoryTableViewDataSources (List<Publication> bookList)
		{
			offlinePublicationList = bookList;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return offlinePublicationList==null ? 0:offlinePublicationList.Count;
		}
	}

	public class HistoryTableViewDelegate : NSTableViewDelegate
	{
		public List<Publication> offlinePublicationList { get; set; }

		public HistoryTableViewDelegate (List<Publication> bookList)
		{
			offlinePublicationList = bookList;
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
			var dataRow = offlinePublicationList[Convert.ToInt32(row)];
			var tableCellView = (NSTableCellView)tableView.MakeView (valueKey,this);
			switch (valueKey) {
			case "ITEMDATA":
				var viewList = tableCellView.Subviews;
				var colorView = (NSView)viewList [0];
				colorView.WantsLayer = true;
				colorView.Layer.BackgroundColor = Utility.ColorWithHexColorValue (dataRow.ColorPrimary).CGColor;

				NSTextField nameTF = (NSTextField)viewList [1];
				nameTF.StringValue = dataRow.Name;
				nameTF.ToolTip = dataRow.Name;

				NSTextField despTF = (NSTextField)viewList [2];
				despTF.StringValue = dataRow.Description;
				despTF.ToolTip = dataRow.Description;

				NSTextField dateTF = (NSTextField)viewList [3];
				dateTF.StringValue = dataRow.LastUpdatedDate.ToString ();
				dateTF.ToolTip = dataRow.LastUpdatedDate.ToString ();
				return tableCellView;
			}

			return null;
		}
	}

}

