using System;
using AppKit;
using Foundation;
using ObjCRuntime;
using LexisNexis.Red.Mac.Data;
using CoreGraphics;
using System.Threading.Tasks;

namespace LexisNexis.Red.Mac
{	public class TocTableViewDelegate : NSTableViewDelegate
	{
		#region properties
		TocViewController viewController { get; set;}
		static readonly nfloat rootFontSize = 14;
		static readonly nfloat defalultFontSize = 14;
		#endregion

		#region public methods
		public TocTableViewDelegate (TocViewController controller)
		{
			viewController = controller;
		}
			
		public override NSTableRowView CoreGetRowView (NSTableView tableView, nint row)
		{
			var rowView = (NSTableRowViewExtend)tableView.GetRowView (row, true);
			if (rowView == null) {
			   rowView = new NSTableRowViewExtend ();
			}

			int level = viewController.TOCDataManager.TOCNodeList [Convert.ToInt32(row)].NodeLevel;
			rowView.Level = level;
			return rowView;
		}

		public async override void SelectionDidChange (NSNotification notification)
		{
			var tableView = (NSTableView)notification.Object;
			nint selectedIndex = tableView.SelectedRow;
			if (selectedIndex < 0) {
				return;
			}

			await ExpandTableViewItemAtIndex (tableView, selectedIndex);
		}
			
		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			if (row < 0) {
				return null;
			}

			var valueKey = tableColumn.Identifier;
			var tableCellView = (NSTableCellView)tableView.MakeView (valueKey,viewController);
			var rowView = (NSTableRowViewExtend)tableView.GetRowView (row, true);
			if (rowView == null) {
				return null;
			}
			int index = Convert.ToInt32 (row);

			var dataManager = (PublicationTOCDataManager)viewController.TOCDataManager;
			var deepLevel = dataManager.CurrentLevel;
			var dataRow = dataManager.TOCNodeList[index];
			int rowIndexHighLight = dataManager.CurrentHighlight;

			//rowView.IsParent = ((dataRow.NodeLevel<deepLevel) &&dataRow.NodeLevel!=0) ? true : false;

			rowView.IsSubSelect = (row == rowIndexHighLight) ? true : false;

			switch (valueKey) {

			case "TOCITEM":
				var viewList = tableCellView.Subviews;

				//color bar
				var colorView = (NSView)viewList [0];
				colorView.WantsLayer = true;
				colorView.Layer.BackgroundColor = Utility.BlendColor (dataRow.NodeLevel, LNRConstants.TOCLEVEL_MAX);

				//title
				if (dataRow.Title != null) {
					nfloat fontSize = row == 0 ? rootFontSize : defalultFontSize; 
					NSTextField textField = tableCellView.TextField;
					textField.Cell.Font = NSFont.SystemFontOfSize (fontSize);
					textField.StringValue = dataRow.Title;
					textField.ToolTip = dataRow.Title;
				}

				//button
				NSButton button = (NSButton)viewList [2];

				if (dataRow.Role == "me") {
					if (row == 0) {
						button.Hidden = true;
					} else {
						button.Image = Utility.ImageWithFilePath ("/Images/TOC/toc_doc.png");
						button.Cell.ImageScale = NSImageScale.AxesIndependently;
						button.Enabled = false;
						button.Bordered = false;
						button.Tag = 3;
					}
				} else {
					button.Hidden = false;
					button.Enabled = false;
					button.Bordered = false;
					int currentLevel = deepLevel;
					if (dataRow.NodeLevel>=currentLevel) {
						button.Tag = 0;   //@2x
						button.Image = Utility.ImageWithFilePath ("/Images/TOC/toc_collapse.png");//NSImage.ImageNamed ("NSGoRightTemplate");
						button.Cell.ImageScale = NSImageScale.None;
					} else {
						button.Tag = 1;
						button.Image =  Utility.ImageWithFilePath ("/Images/TOC/toc_expand.png");
						button.Cell.ImageScale = NSImageScale.None;
					}
					button.Action = new Selector ("ExpandButtonClicked:");
					button.Target = viewController;
				}
				return tableCellView;
			}

			return null;
		}

		public override nfloat GetRowHeight (NSTableView tableView, nint row)
		{
			NSTableColumn tableColumn = (tableView.TableColumns())[0];

			int index = Convert.ToInt32 (row);

			var valueKey = tableColumn.Identifier;
			var dataRow = viewController.TOCDataManager.TOCNodeList[index];
			var tableCellView = (NSTableCellView)tableView.MakeView (valueKey,viewController);
			tableCellView.TextField.StringValue = dataRow.Title;
			nfloat fontSize = row == 0 ? rootFontSize : defalultFontSize;
			nfloat cellHeight = HeightWrappedToWidth (tableCellView.TextField, fontSize);
			nfloat rowHeight = (cellHeight+26) > LNRConstants.TOCITEMHEIGHT_MIN ? cellHeight+26 : LNRConstants.TOCITEMHEIGHT_MIN;

			//Console.WriteLine ("cellheight:{0} rowheight:{1}",cellHeight,rowHeight);

			return rowHeight;
		}
		#endregion

		#region private methods
		private async Task ExpandTableViewItemAtIndex (NSTableView tableView, nint row)
		{
			int index = Convert.ToInt32 (row);
			var dataManager = (PublicationTOCDataManager)viewController.TOCDataManager;

			//if current toc node level is 1, don't deal with this click event
			if (row == 0 && dataManager.CurrentLevel == 1) {
				tableView.ReloadData ();
				return;
			} else if (row == 0 && dataManager.CurrentLevel > 1){
				int tocID = dataManager.TOCNodeList[1].ID;
				dataManager.GetParentNodeTOCListByNodeID (1);
				int rowIndexHighLight = dataManager.IndexByTOCID(tocID);
				SetHighlightIndex (rowIndexHighLight);
				tableView.ReloadData ();
				return;
			}

			var dataRow = dataManager.TOCNodeList[index];

			//select leaf node, open content
			if (dataRow.Role == "me") {

				if (GetHighlightIndex() > 0) {
					RefreshHightRowViewAtTableView (tableView);
				}

				dataManager.CurrentLeafNode = dataRow;
				await viewController.OpenPublicationContentAtTOCNode (dataRow);

				if (GetHighlightIndex() > 0) {
					//RefreshHightRowViewAtTableView (tableView);
				}

				return;
			}

			var rowView = (NSTableRowViewExtend)tableView.GetRowView (row, true);
			var tableCellView = (NSTableCellView)rowView.Subviews[0];
			var viewList = tableCellView.Subviews;
			var expandButton = (NSButton)viewList [2];
			//expand
			if (expandButton.Tag == 0) {
				int tocID = dataRow.ID;
				dataManager.GetSubNodeTOCListByNodeID (index);
				int rowIndexHighLight = dataManager.IndexByTOCID(tocID);
				SetHighlightIndex (rowIndexHighLight);
				tableView.ReloadData ();
			} else {//collapse
				int tocID = dataRow.ID;
				dataManager.GetParentNodeTOCListByNodeID (index);
				int rowIndexHighLight = dataManager.IndexByTOCID(tocID);
				SetHighlightIndex (rowIndexHighLight);
				tableView.ReloadData ();
				tableView.ScrollRowToVisible (rowIndexHighLight);
			}
		}

		int GetHighlightIndex ()
		{
			var dataManager = (PublicationTOCDataManager)viewController.TOCDataManager;
			return dataManager.CurrentHighlight;
		}

		void SetHighlightIndex(int highlightIndex)
		{
			var dataManager = (PublicationTOCDataManager)viewController.TOCDataManager;
			dataManager.CurrentHighlight = highlightIndex;
		}

		private void RefreshHightRowViewAtTableView (NSTableView tableView)
		{

			int rowIndexHighLight = GetHighlightIndex ();
			CGRect rect = tableView.RectForRow (rowIndexHighLight);

			NSRange range = tableView.RowsInRect (tableView.VisibleRect());
			if (rowIndexHighLight >= range.Location && rowIndexHighLight < range.Location + range.Length) {
				var highlightView = (NSTableRowViewExtend)tableView.GetRowView (rowIndexHighLight,true);
				highlightView.IsSubSelect = false;
			}
				
			SetHighlightIndex (-1);
			tableView.SetNeedsDisplayInRect (rect);
		}

		nfloat HeightWrappedToWidth (NSTextField sourceTextField, nfloat fontSize)
		{
			NSAttributedString textAttrStr = new NSAttributedString (sourceTextField.StringValue,NSFont.SystemFontOfSize(fontSize));
			CGSize maxSize = sourceTextField.Frame.Size;
			maxSize.Height = 1000;
			CGRect boundRect = textAttrStr.BoundingRectWithSize (maxSize,
				                   NSStringDrawingOptions.TruncatesLastVisibleLine |
				                   NSStringDrawingOptions.UsesLineFragmentOrigin| 
				                   NSStringDrawingOptions.UsesFontLeading);

			//multiple of 17
			nfloat stringHeight = boundRect.Height;
			nfloat cellHeight = HeightWrappedWidth (sourceTextField, fontSize);
			//Console.WriteLine ("cellHeight:{0} stringHeight:{1}",cellHeight,stringHeight);

			return Math.Max ((float)(cellHeight - 5 + 2), 
				(float)(stringHeight + 2));
		}

		nfloat HeightWrappedWidth (NSTextField sourceTextField, nfloat fontSize)
		{
			NSTextField textField = new NSTextField(new CGRect(0, 0, sourceTextField.Frame.Size.Width, 1000));
			textField.Font = NSFont.SystemFontOfSize(fontSize);
			textField.StringValue = sourceTextField.StringValue ;

			CGSize size = textField.Cell.CellSizeForBounds(textField.Frame);
			return size.Height;
		}

		#endregion
	}
}

