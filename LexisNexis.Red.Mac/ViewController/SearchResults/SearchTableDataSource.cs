using System;
using System.Collections.Generic;

using Foundation;
using AppKit;
using CoreGraphics;

using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;



namespace LexisNexis.Red.Mac
{
	public class SearchTableDataSource : NSTableViewDataSource
	{
		public SearchResult SearchResults {get; set;}
		public SearchTableDataSource (SearchResult results)
		{
			SearchResults = results;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return SearchResults==null ? 0:SearchResults.SearchDisplayResultList.Count+2;
		}
	}

	public class SearchTableDelegate : NSTableViewDelegate
	{
		public SearchResultsController seachViewController { get; set;}
		public SearchResult SearchResults {get; set;}
		private List<SearchDisplayResult> documentList { get; set;}
		private List<SearchDisplayResult> publicationList { get; set;}
		const int vertitalSpace = 1;
		const int topSpace = 12;
		public SearchTableDelegate (SearchResult results, SearchResultsController viewController)
		{
			SearchResults = results;
			seachViewController = viewController;
			if (documentList == null) {
				documentList = new List<SearchDisplayResult> ();
			}

			documentList.Clear ();
			var docList = SearchResults.SearchDisplayResultList.FindAll ((item)=>item.isDocument==true);
			documentList.AddRange (docList);
			//Console.WriteLine("result count:{0}", docList.Count);

			if (publicationList == null) {
				publicationList = new List<SearchDisplayResult> ();
			}

			publicationList.Clear ();
			var pubList = SearchResults.SearchDisplayResultList.FindAll ((item)=>item.isDocument==false);
			publicationList.AddRange (pubList);
		}

		public async override void SelectionDidChange (NSNotification notification)
		{
			var tableView = (NSTableView)notification.Object;
			int index = Convert.ToInt32 (tableView.SelectedRow);
			if (index <= 0 || index == documentList.Count + 1){
				return;
			}

			SearchDisplayResult resultItem = null;
			if (index > 0 && index <= documentList.Count) {
				int rowIndex = index - 1;
				resultItem = documentList [rowIndex];
			} else {
				int rowIndex = index - 1 - documentList.Count -1;
				resultItem = publicationList [rowIndex];
			}

			await seachViewController.OpenPublicationContentAtSearchItem (resultItem);
		}

		public override NSTableRowView CoreGetRowView (NSTableView tableView, nint row)
		{
			var rowView = tableView.GetRowView (row, true);
			if (rowView == null) {
				rowView = new NSTableRowView ();
			}
			return rowView;
		}

		public override bool IsGroupRow (NSTableView tableView, nint row)
		{
			if (row == 0 || row == documentList.Count + 1) {
				return true;
			} else {
				return false;
			}
		}

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			int index = Convert.ToInt32 (row);

			string title = null;
			NSTableCellView tableCellView = null;
			if (row == 0 || row == documentList.Count + 1) {
				tableCellView = (NSTableCellView)tableView.MakeView ( "GROUPITEM",this);
				if (row == 0) {
					title = "Document";
				}
				else if(row == documentList.Count + 1){
					title = "Publication";
				}
				tableCellView.TextField.StringValue = title;
				tableCellView.WantsLayer = true;
				tableCellView.Layer.BackgroundColor = NSColor.Control.CGColor;

				return tableCellView;
			} 

			string valueKey = "SEARCHITEM";
			string section = null;
			tableCellView = (NSTableCellView)tableView.MakeView (valueKey,this);

			if (index > 0 && index <= documentList.Count) {
				int rowIndex = index - 1;
				title = documentList[rowIndex].Head;
				section = documentList[rowIndex].SnippetContent;
			}else if (row>documentList.Count+1) {
				int rowIndex = index - 1 - documentList.Count -1;
				title = publicationList[rowIndex].TocTitle;
				section = publicationList[rowIndex].GuideCardTitle;
			}
				
			var viewList = tableCellView.Subviews;

			NSTextField titleTF = (NSTextField)viewList [0];
			titleTF.Cell.DrawsBackground = true;
			titleTF.Cell.BackgroundColor = NSColor.Clear;

			var trimTitle = title.Length>90?title.Remove(90)+"...":title;
			titleTF.StringValue = trimTitle;
			titleTF.AttributedStringValue = Utility.AttributedTitle(trimTitle,NSColor.Black,"Helvetica Neue", 1, NSTextAlignment.Left);
			titleTF.ToolTip = trimTitle;

			NSTextField sectionTF = (NSTextField)viewList [1];
			sectionTF.Cell.DrawsBackground = true;
			sectionTF.Cell.BackgroundColor = NSColor.Clear;
			sectionTF.AttributedStringValue = Utility.AttributedPartialTitle (section, SearchResults.FoundWordList, NSColor.Black, 
				"Helvetica Neue", 12, NSTextAlignment.Left, NSLineBreakMode.ByWordWrapping);
			sectionTF.ToolTip = section;

			NSTextField pageNumberTF = (NSTextField)viewList [2];
			pageNumberTF.Cell.DrawsBackground = true;
			pageNumberTF.Cell.BackgroundColor = NSColor.Clear;
			pageNumberTF.StringValue = "";

			return tableCellView;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			int index = Convert.ToInt32 (row);

			if (index == 0 || index == documentList.Count + 1) {
				return 23;
			}// else {
//				return 62;
//			}
				
			NSTableColumn tableColumn = (tableView.TableColumns())[0];
			var valueKey = tableColumn.Identifier;

			string title = null;
			string section = null;

			if (index > 0 && index <= documentList.Count) {
				int rowIndex = index - 1;
				title = documentList[rowIndex].Head;
				section = documentList[rowIndex].SnippetContent;
			}else if (row>documentList.Count+1) {
				int rowIndex = index - 1 - documentList.Count -1;
				title = publicationList[rowIndex].TocTitle;
				section = publicationList[rowIndex].GuideCardTitle;
			}
				
			var tableCellView = (NSTableCellView)tableView.MakeView (valueKey,this);
			var viewList = tableCellView.Subviews;
			NSTextField dateTF = (NSTextField)viewList [0];
			var trimTitle = title.Length>90?title.Remove(90)+"...":title;
			dateTF.StringValue = trimTitle;
			NSTextField nameTF = (NSTextField)viewList [1];
			nameTF.StringValue = section;

			nfloat fontSize = 12;
			nfloat cellHeight1 = HeightWrappedToWidth (dateTF, NSFont.FromFontName("Helvetica Neue Medium", fontSize));
			nfloat cellHeight2 = HeightWrappedToWidth (nameTF, NSFont.FromFontName("Helvetica Neue", fontSize));

			nfloat newHeight = topSpace
			                   + cellHeight1 + vertitalSpace
				+ cellHeight2 + vertitalSpace +topSpace;
			return newHeight;
		}

		nfloat HeightWrappedToWidth (NSTextField sourceTextField, NSFont font)
		{
			NSAttributedString textAttrStr = new NSAttributedString (sourceTextField.StringValue, font);
			CGSize maxSize = sourceTextField.Frame.Size;
			maxSize.Height = 1000;
			CGRect boundRect = textAttrStr.BoundingRectWithSize (maxSize,
				NSStringDrawingOptions.TruncatesLastVisibleLine | 
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

