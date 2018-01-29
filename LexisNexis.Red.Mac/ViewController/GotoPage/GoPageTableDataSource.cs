using System;
using Foundation;
using AppKit;
using System.Collections.Generic;
using LexisNexis.Red.Common.BusinessModel;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class GoPageTableDataSource : NSTableViewDataSource
	{
		public List<PageSearchItem> SearchResults {get; set;}
		public GoPageTableDataSource (List<PageSearchItem> results)
		{
			SearchResults = results;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return SearchResults==null||SearchResults.Count==0 ? 0:SearchResults.Count+1;
		}
	}

	public class GoPageTableDelegate : NSTableViewDelegate
	{
		public GoPagePopViewController seachViewController { get; set;}
		public List<PageSearchItem> SearchResults {get; set;}
		const int vertitalSpace = 1;
		const int topSpace = 12;
		public GoPageTableDelegate (List<PageSearchItem> results, GoPagePopViewController viewController)
		{
			seachViewController = viewController;
			if (SearchResults == null) {
				SearchResults = new List<PageSearchItem> ();
			}

			SearchResults.Clear ();
		}

		public void UpdateData (List<PageSearchItem> results)
		{
			SearchResults.Clear ();
			SearchResults.AddRange(results);
		}

		public async override void SelectionDidChange (NSNotification notification)
		{
			var tableView = (NSTableView)notification.Object;
			int index = Convert.ToInt32 (tableView.SelectedRow);

			if (index == 0){
				return;
			}
				
			PageSearchItem resultItem = null;
			int rowIndex = index - 1;
			resultItem = SearchResults [rowIndex];

			await seachViewController.OpenContentPageAtSearchPageItem (resultItem);
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
			int index = Convert.ToInt32 (row);

			string title = null;
			NSTableCellView tableCellView = null;
			if (row == 0) {
				title = "Page Results";

				tableCellView = (NSTableCellView)tableView.MakeView ( "GROUPITEM",this);

				var textField = (NSTextField)tableCellView.Subviews [0];
				textField.StringValue = title;

				var gridView = tableCellView.Subviews [1];

				gridView.WantsLayer = true;
				gridView.Layer.BackgroundColor = NSColor.Grid.CGColor;

				tableCellView.WantsLayer = true;
				tableCellView.Layer.BackgroundColor = NSColor.Control.CGColor;

				return tableCellView;
			} 
				
			tableCellView = (NSTableCellView)tableView.MakeView ("PAGEITEM",this);

			int rowIndex = index - 1;
			title = SearchResults [rowIndex].FileTitle;
			string section = SearchResults [rowIndex].GuideCardTitle;

			var viewList = tableCellView.Subviews;

			NSTextField titleTF = (NSTextField)viewList [0];
			titleTF.Cell.DrawsBackground = true;
			titleTF.Cell.BackgroundColor = NSColor.Clear;
			titleTF.Cell.Font = NSFont.FromFontName ("Helvetica Neue Medium",12);
			titleTF.StringValue = title;
			titleTF.ToolTip = title;

			NSTextField sectionTF = (NSTextField)viewList [1];
			sectionTF.Cell.DrawsBackground = true;
			sectionTF.Cell.BackgroundColor = NSColor.Clear;
			sectionTF.Cell.Font = NSFont.FromFontName ("Helvetica Neue",12);
			sectionTF.StringValue = section;
			sectionTF.ToolTip = section;

			var boxView = (NSView)viewList [2];
			boxView.WantsLayer = true;
			boxView.Layer.BackgroundColor = NSColor.Grid.CGColor;

			return tableCellView;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			int index = Convert.ToInt32 (row);

			if (index == 0) {
				return 24;
			}

			int rowIndex = index - 1;
			string title = SearchResults [rowIndex].FileTitle;
			string section = SearchResults [rowIndex].GuideCardTitle;

			var tableCellView = (NSTableCellView)tableView.MakeView ("PAGEITEM",this);
			var viewList = tableCellView.Subviews;
			NSTextField titleTF = (NSTextField)viewList [0];
			titleTF.StringValue = title;
			NSTextField sectionTF = (NSTextField)viewList [1];
			sectionTF.StringValue = section;

			//nfloat fontSize = 14;
			//NSFont.FromFontName("Helvetica Neue Medium", fontSize);
			//NSFont.FromFontName("Helvetica Neue", fontSize)

			nfloat cellHeight1 = HeightWrappedToWidth (titleTF, titleTF.Font);
			nfloat cellHeight2 = HeightWrappedToWidth (sectionTF, sectionTF.Font);

			nfloat newHeight = topSpace
				+ cellHeight1 + vertitalSpace
				+ cellHeight2 +topSpace;
			//Console.WriteLine ("cellHeight1:{0};cellHeight2:{1};newHeight:{2}", cellHeight1,cellHeight2,newHeight);
			return newHeight;
		}

		nfloat HeightWrappedToWidth (NSTextField sourceTextField, NSFont font)
		{
			NSAttributedString textAttrStr = new NSAttributedString (sourceTextField.StringValue, font);
			CGSize maxSize = sourceTextField.Frame.Size;
			maxSize.Height = 1000;
			CGRect boundRect = textAttrStr.BoundingRectWithSize (maxSize,
				NSStringDrawingOptions.UsesDeviceMetrics|
				NSStringDrawingOptions.UsesLineFragmentOrigin | 
				NSStringDrawingOptions.UsesFontLeading);

			nfloat stringHeight = boundRect.Height;
			nfloat cellHeight = HeightWrappedWidth (sourceTextField, font);

			return Math.Max ((float)(cellHeight - 5), 
				(float)(stringHeight));
		}

		nfloat HeightWrappedWidth (NSTextField sourceTextField, NSFont font)
		{
			NSTextField textField = new NSTextField(new CGRect(0, 0, sourceTextField.Frame.Size.Width, 1000));
			textField.Font = font;
			textField.StringValue = sourceTextField.StringValue ;

			CGSize size = textField.Cell.CellSizeForBounds(textField.Frame);
			return size.Height;
		}
	}
}

