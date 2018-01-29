using System;
using System.Collections.Generic;

using AppKit;
using Foundation;
using CoreGraphics;

using LexisNexis.Red.Common.BusinessModel;



namespace LexisNexis.Red.Mac
{
	public class TagsTableDataSource: NSTableViewDataSource
	{
		public List<AnnotationTag> TagsList { get; set; }
		public TagsTableDataSource (List<AnnotationTag> tagsList)
		{
			TagsList = tagsList;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return TagsList==null ? 0:TagsList.Count;
		}
	}

	public class TagsTableDelegate : NSTableViewDelegate
	{
		public AnnotationOrganiserViewController AnnViewController { get; set;}
		public List<AnnotationTag> TagsList { get; set; }
		const int vertitalSpace = 7;
		const int topSpace = 17;
		public TagsTableDelegate (List<AnnotationTag> tagsList, AnnotationOrganiserViewController viewController)
		{
			TagsList = tagsList;
			AnnViewController = viewController;
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
			//Console.WriteLine ("row:{0}", row);
			var valueKey = tableColumn.Identifier;
			var dataRow = TagsList[Convert.ToInt32(row)];
			var tableCellView = (NSTableCellView)tableView.MakeView (valueKey,this);
			switch (valueKey) {
			case "TAGITEM":
				var viewList = tableCellView.Subviews;
				var colorView = (NSImageView)viewList [0];
				colorView.Cell.Bordered = false;
				colorView.ImageScaling = NSImageScale.None;
				colorView.ImageAlignment = NSImageAlignment.Center;
				colorView.ImageFrameStyle = NSImageFrameStyle.None;

				if (row == 0) {
					colorView.Cell.Image = Utility.ImageWithFilePath ("/Images/Annotation/All_Icon@1x.png");
				}else {
					colorView.Cell.Image = CreateImageWithColor (dataRow.Color);
				}

				NSTextField titleTF = (NSTextField)viewList [1];
				titleTF.Cell.Wraps = true;
				titleTF.Cell.DrawsBackground = true;
				titleTF.Cell.BackgroundColor = NSColor.Clear;
				titleTF.StringValue = dataRow.Title;

				NSButton checkButton = (NSButton)viewList [2];
//				if (row == 0) {
//					checkButton.Hidden = true;
//				} else {

				if (checkButton.State == NSCellStateValue.Off) {
				} else {
					checkButton.Cell.Image = Utility.ImageWithFilePath ("/Images/Annotation/Check@1x.png");
				}

				return tableCellView;
			}

			return null;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			return 44.0f;
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

