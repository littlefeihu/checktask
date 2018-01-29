using System;

using AppKit;
using Foundation;
using LexisNexis.Red.Mac.Data;

namespace LexisNexis.Red.Mac
{
	public class IndexTableViewDataSources : NSOutlineViewDataSource
	{
		IndexViewController viewController { get; set;}
		IndexDataManager indexDataManager {get; set;}
		public IndexTableViewDataSources (IndexViewController controller)
		{
			viewController = controller;
			indexDataManager = viewController.IndexDataManager;
		}

		//"outlineView:child:ofItem:"
		public override NSObject GetChild (NSOutlineView outlineView, nint childIndex, NSObject item)
		{
			int index = Convert.ToInt32 (childIndex);
			if (item == null) {
				NSObject groupItem = indexDataManager.GroupItemAtIndex (index);
				return groupItem;

			} else {
				
				NSObject childrenItem = indexDataManager.ChildrenItemAtIndex (indexDataManager.ChildrenForGroupItem(item),index);
				return childrenItem;
			}
		}

		//"outlineView:isItemExpandable:"
		public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
		{
//			if (item == null)
//				return false;
			
			if (item == null || outlineView.GetParent (item) == null ) {
				return true;
			} else {
				return false;
			}
		}

		//"outlineView:numberOfChildrenOfItem:"
		public override nint GetChildrenCount (NSOutlineView outlineView, NSObject item)
		{
			if (viewController==null||indexDataManager==null ||indexDataManager.IndexNodeList==null) {
				return 0;
			} else if (item == null) {
				return indexDataManager.GroupLevelItems().Count;
			} else {
				var itemList = indexDataManager.ChildrenForGroupItem (item);
				if (itemList != null) {
					return itemList.Count;
				} else {
					return 0;
				}
			}
		}
	}
}

