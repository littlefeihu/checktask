using System;
using System.Collections.Generic;

using Foundation;
using AppKit;
using CoreGraphics;
using System.Linq;
using System.Linq.Expressions;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;



namespace LexisNexis.Red.Mac
{
	public class AnnotationsTableDataSource : NSTableViewDataSource
	{
		private List<Annotation> AnnotationList { get; set; }
		private int GuidCardCount { get; set;}
		public AnnotationsTableDataSource (List<Annotation> itemList)
		{
			AnnotationList = itemList;
			var d = (from annotation in AnnotationList
				group annotation  by annotation.GuideCardName into annotationgroup
				select annotationgroup.Key);
			
			GuidCardCount = d.Count();
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return AnnotationList==null ? 0:AnnotationList.Count+this.GuidCardCount;
		}
	}

	public class AnnotationsTableDelegate : NSTableViewDelegate
	{
		private object ViewController { get; set;}
		private List<Annotation> AnnotationList { get; set; }
		private List<string> GuidCardList { get; set;}
		private NSMutableIndexSet GroupIndexSet { get; set;}

		const int vertitalSpace = 1;
		const int topSpace = 12;

		public AnnotationsTableDelegate (List<Annotation> itemList, object viewController)
		{
			AnnotationList = itemList;
			ViewController = viewController;

			var d = (from annotation in AnnotationList.ToList()
				     group annotation  by annotation.GuideCardName into annotationgroup
				     select annotationgroup.Key);

			if (GuidCardList == null) {
				GuidCardList = new List<string>();
			}else {
				GuidCardList.Clear();
			}
			GuidCardList.AddRange (d.ToList());


			if (GroupIndexSet == null) {
				GroupIndexSet = new NSMutableIndexSet ();
			} else {
				GroupIndexSet.Clear ();
			}
			GroupIndexSet.Add (0);
			foreach (var guideCard in GuidCardList) {
				var cardlist = AnnotationList.FindAll ((item)=>item.GuideCardName == guideCard);
				GroupIndexSet.Add ((nuint)cardlist.Count+1);
			}
				
			Console.WriteLine("result count:{0}", d);
		}

		public async override void SelectionDidChange (NSNotification notification)
		{
			var tableView = (NSTableView)notification.Object;
			nint row = tableView.SelectedRow;
			int index = Convert.ToInt32 (row);
			if (GroupIndexSet.Contains((nuint)row)){
				return;
			}

//			SearchDisplayResult resultItem = null;
//			if (index > 0 && index <= BookTitleList.Count) {
//				int rowIndex = index - 1;
//				resultItem = BookTitleList [rowIndex];
//			} else {
//				int rowIndex = index - 1 - documentList.Count -1;
//				resultItem = publicationList [rowIndex];
//			}

			//await ViewController.OpenPublicationContentAtSearchItem (resultItem);
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
			if (GroupIndexSet.Contains((nuint)row)){
				return true;
			} else {
				return false;
			}
		}

		private int GetTitleIndexWithRowValue (nint row)
		{
			int titleIndex = 0;
			var indexList = GroupIndexSet.ToList ();
			for (int i = 0; i < indexList.Count; i++) {
				if (row == (nint)indexList[i]) {
					titleIndex = i;
					break;
				}
			}

			return titleIndex;
		}
			

		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			return null;

			string title = null;
			NSTableCellView tableCellView = null;
			if (GroupIndexSet.Contains((nuint)row)) {
				tableCellView = (NSTableCellView)tableView.MakeView ( "GROUPITEM",this);

				int idx = GetTitleIndexWithRowValue (row);

				tableCellView.TextField.StringValue = GuidCardList[idx];
				tableCellView.WantsLayer = true;
				tableCellView.Layer.BackgroundColor = NSColor.Control.CGColor;

				return tableCellView;
			} 

			int index = Convert.ToInt32 (row);
			var dataRow = AnnotationList[index];

			string valueKey = "ANNOTATIONITEM";
			tableCellView = (NSTableCellView)tableView.MakeView (valueKey,this);

			var viewList = tableCellView.Subviews;

			NSTextField dateTF = (NSTextField)viewList [0];
			dateTF.Cell.Wraps = true;
			dateTF.Cell.DrawsBackground = true;
			dateTF.Cell.BackgroundColor = NSColor.Clear;
			dateTF.StringValue = Utility.FormateLastReadDate (dataRow.UpdatedTime);
			dateTF.ToolTip = Utility.FormateLastReadDate (dataRow.UpdatedTime);

			NSTextField tocTF = (NSTextField)viewList [1];
			tocTF.Cell.Wraps = true;
			tocTF.Cell.DrawsBackground = true;
			tocTF.Cell.BackgroundColor = NSColor.Clear;
			tocTF.StringValue = dataRow.TOCTitle;
			tocTF.ToolTip = dataRow.TOCTitle;

			NSTextField highlightTF = (NSTextField)viewList [2];
			if (!string.IsNullOrEmpty (dataRow.HighlightText)) {
				highlightTF.StringValue = string.Empty;
				highlightTF.Hidden = true;
			} else {
				highlightTF.Cell.Wraps = true;
				highlightTF.Cell.DrawsBackground = true;
				highlightTF.Cell.BackgroundColor = NSColor.Clear;
				highlightTF.StringValue = dataRow.HighlightText;
				highlightTF.ToolTip = dataRow.HighlightText;
			}

			NSTextField noteTF = (NSTextField)viewList [3];
			if (!string.IsNullOrEmpty (dataRow.NoteText)) {
				noteTF.StringValue = string.Empty;
				noteTF.Hidden = true;
			}else {
				noteTF.Cell.Wraps = true;
				noteTF.Cell.DrawsBackground = true;
				noteTF.Cell.BackgroundColor = NSColor.Clear;
				noteTF.StringValue = dataRow.NoteText;
				noteTF.ToolTip = dataRow.NoteText;

				NSImageView imageView = (NSImageView)viewList [5];
				imageView.Image = Utility.ImageWithFilePath ("/Images/Annotation/note");
			} 
				

			NSView tagView = (NSView)viewList [4];

			var taglist = dataRow.CategoryTagIDs;
			for (int i=0; i<taglist.Count; i++) {
				NSButton tagButton = new NSButton (MakeButtonRectAtIndex(i));
				AnnotationTag tag = new AnnotationTag();//AnnCategoryTagUtil.Instance.GetTagByGuid ();
				tagButton.Image = CreateImageWithColor ("#00ff00");  //tag.Color;
				tagButton.Title = tag.Title;
				tagView.AddSubview (tagButton);
			}

			return tableCellView;
		}

		private CGRect MakeButtonRectAtIndex(int index)
		{
			return new CGRect (0,0,0,0);
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			if (GroupIndexSet.Contains((nuint)row)) {
				return 23;
			} 
//			else {
//				return 62;
//			}
				
			int index = Convert.ToInt32 (row);
			var dataRow = AnnotationList[index];

			string valueKey = "ANNOTATIONITEM";
			NSTableCellView tableCellView = (NSTableCellView)tableView.MakeView (valueKey,this);

			var viewList = tableCellView.Subviews;
			NSTextField dateTF = (NSTextField)viewList [0];
			dateTF.StringValue = "11 May 2015";

			NSTextField tocTF = (NSTextField)viewList [1];
			tocTF.StringValue = dataRow.TOCTitle;
				
			NSTextField highlightTF = (NSTextField)viewList [2];
			highlightTF.StringValue = dataRow.HighlightText == null ? string.Empty : dataRow.HighlightText;

			NSTextField noteTF = (NSTextField)viewList [3];
			noteTF.StringValue = dataRow.NoteText == null ? string.Empty : dataRow.NoteText;

			NSView tagView = (NSView)viewList [4];


			string title = string.Empty;
			string section = string.Empty;
				
			dateTF.StringValue = title;
			tocTF.StringValue = section;

			nfloat fontSize = 12;
			nfloat cellHeight0 = HeightWrappedToWidth (dateTF, NSFont.FromFontName("Helvetica Neue", fontSize));
			nfloat cellHeight1 = HeightWrappedToWidth (tocTF, NSFont.FromFontName("Helvetica Neue Medium", fontSize));

			var specfontSize = 1;
			if (highlightTF.StringValue.Length > 0) {
				specfontSize = 12;
			}
			nfloat cellHeight2 = HeightWrappedToWidth (highlightTF, NSFont.FromFontName("Helvetica Neue", specfontSize));

			specfontSize = 1;
			if (noteTF.StringValue.Length > 0) {
				specfontSize = 12;
			}
			nfloat cellHeight3 = HeightWrappedToWidth (noteTF, NSFont.FromFontName("Helvetica Neue Medium", specfontSize));

			nfloat cellHeight4 = (dataRow.CategoryTagIDs.Count+2)/3*16;

			nfloat newHeight = topSpace
			                   + cellHeight0 + vertitalSpace
				               + cellHeight1 + vertitalSpace 
				               + cellHeight2 + vertitalSpace 
				               + cellHeight3 + vertitalSpace +
				               + cellHeight4 + vertitalSpace + topSpace;
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

		private NSImage CreateImageWithColor(string colorValue)
		{
			NSGraphicsContext.GlobalSaveGraphicsState ();
			CGSize size = new CGSize(12, 12);
			NSImage tintImage = new NSImage (size);
			tintImage.LockFocus ();

			float cornerRadius = 5f;
			CGRect rect = new CGRect (0,0,10,10);
			NSBezierPath path = NSBezierPath.FromRoundedRect (rect,cornerRadius,cornerRadius); 
			if (string.IsNullOrEmpty (colorValue)) {
				NSColor.Grid.Set ();
				path.Stroke ();
			} else {
				Utility.ColorWithHexColorValue (colorValue, 1.0f).SetFill ();
				path.Fill ();
			}

			tintImage.UnlockFocus ();
			CGContext context = NSGraphicsContext.CurrentContext.CGContext;

			return tintImage;
		}
	}
}

