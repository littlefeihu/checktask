using System;
using System.Collections.Generic;

using Foundation;
using AppKit;
using CoreGraphics;

using LexisNexis.Red.Common.BusinessModel;



namespace LexisNexis.Red.Mac
{
	public class PopHistoryTableDataSource : NSTableViewDataSource
	{
		public List<RecentHistoryItem> RecentHistoryList { get; set; }

		public PopHistoryTableDataSource (List<RecentHistoryItem> historyList)
		{
			RecentHistoryList = historyList;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return RecentHistoryList==null ? 0:RecentHistoryList.Count;
		}
	}

	public class PopHistoryTableDelegate : NSTableViewDelegate
	{
		public HistoryPopoverController HistoryViewController { get; set;}
		public List<RecentHistoryItem> RecentHistoryList { get; set; }
		const int vertitalSpace = 7;
		const int topSpace = 17;
		public PopHistoryTableDelegate (List<RecentHistoryItem> bookList, HistoryPopoverController viewController)
		{
			RecentHistoryList = bookList;
			HistoryViewController = viewController;
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var tableView = (NSTableView)notification.Object;
			nint index = tableView.SelectedRow;
			if (index < 0) {
				return;
			}
			var historyItem = RecentHistoryList[Convert.ToInt32(index)];

			HistoryViewController.OpenPublicationContentAtHistory (historyItem);
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

				NSTextField dateTF = (NSTextField)viewList [1];
				dateTF.Cell.Wraps = true;
				dateTF.Cell.DrawsBackground = true;
				dateTF.Cell.BackgroundColor = NSColor.Clear;
				dateTF.StringValue = Utility.FormateLastReadDate (dataRow.LastReadDate);
				dateTF.ToolTip = Utility.FormateLastReadDate (dataRow.LastReadDate);
				//Console.WriteLine ("dateTF:{0}", dateTF.StringValue);

				NSTextField nameTF = (NSTextField)viewList [2];
				nameTF.Cell.Wraps = true;
				nameTF.Cell.DrawsBackground = true;
				nameTF.Cell.BackgroundColor = NSColor.Clear;
				nameTF.StringValue = dataRow.PublicationTitle;
				nameTF.ToolTip = dataRow.PublicationTitle;
				//Console.WriteLine ("nameTF:{0}", nameTF.StringValue);

				NSTextField despTF = (NSTextField)viewList [3];
				despTF.Cell.Wraps = true;
				despTF.Cell.DrawsBackground = true;
				despTF.Cell.BackgroundColor = NSColor.Clear;
				despTF.StringValue = dataRow.TOCTitle;
				despTF.ToolTip = dataRow.TOCTitle;
				//Console.WriteLine ("despTF:{0}", despTF.StringValue);

				var boxView = (NSView)viewList [4];
				boxView.WantsLayer = true;
				boxView.Layer.BackgroundColor = NSColor.Grid.CGColor;
					
				return tableCellView;
			}

			return null;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			NSTableColumn tableColumn = (tableView.TableColumns())[0];

			int index = Convert.ToInt32 (row);

			var valueKey = tableColumn.Identifier;
			var dataRow = RecentHistoryList[index];
			var tableCellView = (NSTableCellView)tableView.MakeView (valueKey,this);
			var viewList = tableCellView.Subviews;
			NSTextField dateTF = (NSTextField)viewList [1];
			dateTF.StringValue = dataRow.LastReadDate.ToString ();
			NSTextField nameTF = (NSTextField)viewList [2];
			nameTF.StringValue = dataRow.PublicationTitle;
			NSTextField despTF = (NSTextField)viewList [3];
			despTF.StringValue = dataRow.TOCTitle;


			nfloat fontSize = 12;
			nfloat cellHeight1 = HeightWrappedToWidth (dateTF, fontSize);
			nfloat cellHeight2 = HeightWrappedToWidth (nameTF, fontSize);
			nfloat cellHeight3 = HeightWrappedToWidth (despTF, fontSize);

			nfloat newHeight = topSpace 
				+ cellHeight1 + vertitalSpace 
				+ cellHeight2 + vertitalSpace 
				+ cellHeight3+topSpace;
			return newHeight;
		}

		nfloat HeightWrappedToWidth (NSTextField sourceTextField, nfloat fontSize)
		{
			NSAttributedString textAttrStr = new NSAttributedString (sourceTextField.StringValue,NSFont.SystemFontOfSize(fontSize));
			CGSize maxSize = sourceTextField.Frame.Size;
			maxSize.Height = 1000;
			CGRect boundRect = textAttrStr.BoundingRectWithSize (maxSize,
				NSStringDrawingOptions.TruncatesLastVisibleLine | 
				NSStringDrawingOptions.UsesLineFragmentOrigin | 
				NSStringDrawingOptions.UsesFontLeading);

			nfloat stringHeight = boundRect.Height;
			nfloat cellHeight = HeightWrappedWidth (sourceTextField, fontSize);

			return Math.Max ((float)(cellHeight - 5), 
				(float)(stringHeight));
		}

		nfloat HeightWrappedWidth (NSTextField sourceTextField, nfloat fontSize)
		{
			NSTextField textField = new NSTextField(new CGRect(0, 0, sourceTextField.Frame.Size.Width, 1000));
			textField.Font = NSFont.SystemFontOfSize(fontSize);
			textField.StringValue = sourceTextField.StringValue ;

			CGSize size = textField.Cell.CellSizeForBounds(textField.Frame);
			return size.Height;
		}
	}
}

