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
		public List<Publication> offlinePublicationList { get; set; }

		public PopHistoryTableDataSource (List<Publication> bookList)
		{
			offlinePublicationList = bookList;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return offlinePublicationList==null ? 0:offlinePublicationList.Count;
		}
	}

	public class PopHistoryTableDelegate : NSTableViewDelegate
	{
		public List<Publication> offlinePublicationList { get; set; }
		const int vertitalSpace = 7;
		const int topSpace = 17;
		public PopHistoryTableDelegate (List<Publication> bookList)
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

				NSTextField dateTF = (NSTextField)viewList [1];
				dateTF.Cell.Wraps = true;
				dateTF.StringValue = dataRow.LastUpdatedDate.ToString ();
				dateTF.ToolTip = dataRow.LastUpdatedDate.ToString ();

				NSTextField nameTF = (NSTextField)viewList [2];
				nameTF.Cell.Wraps = true;
				nameTF.StringValue = dataRow.Name;
				nameTF.ToolTip = dataRow.Name;

				NSTextField despTF = (NSTextField)viewList [3];
				despTF.Cell.Wraps = true;
				despTF.StringValue = dataRow.Description;
				despTF.ToolTip = dataRow.Description;


				return tableCellView;
			}

			return null;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			NSTableColumn tableColumn = (tableView.TableColumns())[0];

			int index = Convert.ToInt32 (row);

			var valueKey = tableColumn.Identifier;
			var dataRow = offlinePublicationList[index];
			var tableCellView = (NSTableCellView)tableView.MakeView (valueKey,this);
			var viewList = tableCellView.Subviews;
			NSTextField dateTF = (NSTextField)viewList [1];
			dateTF.StringValue = dataRow.LastUpdatedDate.ToString ();
			NSTextField nameTF = (NSTextField)viewList [2];
			nameTF.StringValue = dataRow.Name;
			NSTextField despTF = (NSTextField)viewList [3];
			despTF.StringValue = dataRow.Description;


			nfloat fontSize = 12;
			nfloat cellHeight1 = HeightWrappedToWidth (dateTF, fontSize);
			nfloat cellHeight2 = HeightWrappedToWidth (nameTF, fontSize);
			nfloat cellHeight3 = HeightWrappedToWidth (despTF, fontSize);

			nfloat newHeight = topSpace 
				+ cellHeight1 + vertitalSpace 
				+ cellHeight2 + vertitalSpace 
				+ cellHeight3;
			return newHeight;
		}

		nfloat HeightWrappedToWidth (NSTextField sourceTextField, nfloat fontSize)
		{
			NSTextField textField = new NSTextField(new CGRect(0, 0, sourceTextField.Frame.Size.Width, 1000));
			textField.Font = NSFont.SystemFontOfSize(fontSize);
			textField.StringValue = sourceTextField.StringValue ;

			CGSize size = textField.Cell.CellSizeForBounds(textField.Frame);

			return size.Height;
		}
	}
}

