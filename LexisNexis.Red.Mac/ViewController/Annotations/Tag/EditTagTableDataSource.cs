using System;
using System.Collections.Generic;

using AppKit;

using LexisNexis.Red.Common.BusinessModel;
using Foundation;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class EditTagTableDataSource: NSTableViewDataSource
	{
		private List<AnnotationTag> TagsList { get; set; }
		private object ViewController { get; set;}
		public EditTagTableDataSource (List<AnnotationTag> tagsList, object viewController)
		{
			TagsList = tagsList;
			ViewController = viewController;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return TagsList==null ? 0:TagsList.Count;
		}

		public override NSDragOperation ValidateDrop (NSTableView tableView, NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
		{
			tableView.SetDropRowDropOperation (row, dropOperation);
			if (dropOperation == NSTableViewDropOperation.On) {
				return NSDragOperation.None;
			}
			return NSDragOperation.Move;
		}

		public override bool WriteRows (NSTableView tableView, NSIndexSet rowIndexes, NSPasteboard pboard)
		{

			// Copy the row numbers to the pasteboard.
			NSData zNSIndexSetData = NSKeyedArchiver.ArchivedDataWithRootObject(rowIndexes);
			string[] typeArray = {"NSStringPboardType"};
			pboard.DeclareTypes(typeArray,this);
			pboard.SetDataForType(zNSIndexSetData,"NSStringPboardType");
			return true;
		}

		public override bool AcceptDrop (NSTableView tableView, NSDraggingInfo info, nint row, NSTableViewDropOperation dropOperation)
		{
			if (row < 0) {
				return false;
			}

			NSPasteboard pboard = info.DraggingPasteboard;
			NSData rowData = pboard.GetDataForType("NSStringPboardType");
			var rowIndexes = NSKeyedUnarchiver.UnarchiveObject(rowData);
			int dragRow = (int)((NSIndexSet)rowIndexes).FirstIndex;

			if (dragRow == row) {
				return false;
			}

			int index = Convert.ToInt32 (row);

			if (dropOperation == NSTableViewDropOperation.Above) {
				// 插入
				if (ViewController is EditTagsViewController) {
					var viewController = (EditTagsViewController)this.ViewController;
					viewController.DragItemFromIndexToIndex(tableView, dragRow, index);
				}

				tableView.ReloadData ();

			}  else if (dropOperation == NSTableViewDropOperation.On){
				// 替换
			} else {
				//Console.WriteLine ("unexpected operation {0} in {1}", dropOperation, __FUNCTION__);
			}

			return true;
		}
	}

	public class EditTagTableDelegate : NSTableViewDelegate
	{
		public object editTagViewController { get; set;}
		public List<AnnotationTag> TagsList { get; set; }

		public EditTagTableDelegate (List<AnnotationTag> tagsList, object viewController)
		{
			TagsList = tagsList;
			editTagViewController = viewController;
		}

		public override void SelectionDidChange (NSNotification notification)
		{
			var tableView = (NSTableView)notification.Object;
			nint index = tableView.SelectedRow;
			if (index < 0) {
				return;
			}
			var tagItem = TagsList[Convert.ToInt32(index)];

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

				var removeButton = (NSButton)viewList [0];
				removeButton.Cell.ImageScale = NSImageScale.AxesIndependently;
				removeButton.Image = Utility.ImageWithFilePath ("/Images/Annotation/Remove.png");
				removeButton.Target = this;
				removeButton.Action = new ObjCRuntime.Selector ("RemoveTag:");

//				var colorView = (NSImageView)viewList [1];
//				colorView.Cell.Bordered = false;
//				colorView.ImageScaling = NSImageScale.None;
//				colorView.ImageAlignment = NSImageAlignment.Center;
//				colorView.ImageFrameStyle = NSImageFrameStyle.None;
//				colorView.Image = CreateImageWithColor (dataRow.Color);

				var colorView = (NSView)viewList [1];
				colorView.WantsLayer = true;
				colorView.Layer.BackgroundColor = Utility.ColorWithHexColorValue (dataRow.Color, 1.0f).CGColor;
				colorView.Layer.CornerRadius = 5.0f;

				NSTextField titleTF = (NSTextField)viewList [2];
				titleTF.Cell.LineBreakMode = NSLineBreakMode.TruncatingTail;
				titleTF.Cell.DrawsBackground = true;
				titleTF.Cell.BackgroundColor = NSColor.Clear;
				titleTF.StringValue = dataRow.Title;
				titleTF.ToolTip = dataRow.Title;

				var editButton = (NSButton)viewList [3];
				editButton.Cell.ImageScale = NSImageScale.AxesIndependently;
				editButton.Image = Utility.ImageWithFilePath ("/Images/Annotation/Forward.png");
				editButton.Target = this;
				editButton.Action = new ObjCRuntime.Selector ("EditTag:");

				var lineView = (NSView)viewList [4];
				lineView.WantsLayer = true;
				lineView.Layer.BackgroundColor = NSColor.Grid.CGColor;
				return tableCellView;
			}

			return null;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			return 34.0f;
		}

		[Action("RemoveTag:")]
		private void RemoveTag(NSObject sender)
		{
			var button = (NSButton)sender;
			var cellView = (NSTableCellView)button.Superview;
			var tableRowView = (NSTableRowView)cellView.Superview;
			var tableView = (NSTableView)tableRowView.Superview;
			nint row = tableView.RowForView(tableRowView);
			int index = Convert.ToInt32 (row);
			if (row < 0 || row >= TagsList.Count) {
				return;
			}

			if (this.editTagViewController is EditTagsViewController) {
				var viewController = (EditTagsViewController)this.editTagViewController;
				viewController.RemoveTagAtRow (index);
			}
		}

		[Action("EditTag:")]
		private void EditTag(NSObject sender)
		{
			var button = (NSButton)sender;
			var cellView = (NSTableCellView)button.Superview;
			var tableRowView = (NSTableRowView)cellView.Superview;
			var tableView = (NSTableView)tableRowView.Superview;
			nint row = tableView.RowForView(tableRowView);
			int index = Convert.ToInt32 (row);
			if (row < 0 || row >= TagsList.Count) {
				return;
			}

			if (this.editTagViewController is EditTagsViewController) {
				var viewController = (EditTagsViewController)this.editTagViewController;
				viewController.EditTagAtRow (tableRowView, index);
			}
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

