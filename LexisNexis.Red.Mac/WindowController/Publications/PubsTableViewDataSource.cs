using System;
using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public class PubsTableViewDataSource : NSTableViewDataSource
	{
		private OrganizePublicationsPanelController panelController{ get; set;}
		public PubsTableViewDataSource (OrganizePublicationsPanelController controller)
		{
			panelController = controller;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return panelController == null ? 0 : panelController.publicationArray.Count;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var valueKey = (NSString)tableColumn.Identifier;

			int index = Convert.ToInt32 (row);
			var dataRow = panelController.publicationArray[index];

			switch (valueKey)
			{
			case "BOOKTITLE":
				return (NSString)dataRow.Name;
			}
			throw new Exception(string.Format("Incorrect value requested '{0}'", valueKey));
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
			NSData rowData = pboard.GetDataForType("NSStringPboardType");  //NSStringPboardType
			var rowIndexes = NSKeyedUnarchiver.UnarchiveObject(rowData);
			int dragRow = (int)((NSIndexSet)rowIndexes).FirstIndex;

			if (dragRow == row) {
				return false;
			}

			int index = Convert.ToInt32 (row);

			if (dropOperation == NSTableViewDropOperation.Above) {
				// 插入
				panelController.DragItemFromIndexToIndex(tableView, dragRow, index);
				tableView.ReloadData ();

			}  else if (dropOperation == NSTableViewDropOperation.On){
				// 替换
			} else {
				//Console.WriteLine ("unexpected operation {0} in {1}", dropOperation, __FUNCTION__);
			}

			return true;
		}
	}
}

