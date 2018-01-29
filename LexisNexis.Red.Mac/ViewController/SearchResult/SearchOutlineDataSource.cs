using System;
using System.Collections.Generic;

using AppKit;
using Foundation;

using LexisNexis.Red.Common.BusinessModel;


namespace LexisNexis.Red.Mac
{
	public class SearchOutlineDataSource : NSOutlineViewDataSource
	{
		#region properties
		SearchResultManager SearchManager { get; set;}
		#endregion

		#region constructor
		public SearchOutlineDataSource (SearchResultViewController controller) :base ()
		{
			SearchManager = controller.DataManager;
		}
		#endregion

		#region NSOutlineViewDataSource
		//"outlineView:child:ofItem:"
		public override NSObject GetChild (NSOutlineView outlineView, nint childIndex, NSObject item)
		{
			//Console.WriteLine ("GetChild:index{0},item{1}", childIndex, item);
			int index = Convert.ToInt32 (childIndex);
			if (item == null) {
				NSObject groupItem = SearchManager.GroupItemAtIndex (index);
				return groupItem;

			} else {
				NSObject childrenItem = SearchManager.ChildrenItemAtIndex (SearchManager.ChildrenForGroupItem(item),index);
				return childrenItem;
			}
		}

		//"outlineView:isItemExpandable:"
		public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
		{
			//Console.WriteLine ("ItemExpandable:item{0},", item);
			if (item == null || outlineView.GetParent (item) == null ) {
				return true;
			} else {
				return false;
			}
		}

		//"outlineView:numberOfChildrenOfItem:"
		public override nint GetChildrenCount (NSOutlineView outlineView, NSObject item)
		{
			//Console.WriteLine ("GetChildrenCount:item{0},", item);
			if (SearchManager==null ||SearchManager.PublicationResultList==null) {
				return 0;
			} else if (item == null) {
				return SearchManager.GroupLevelItems().Count;
			} else {
				var itemList = SearchManager.ChildrenForGroupItem (item);
				if (itemList != null) {
					return itemList.Count;
				} else {
					return 0;
				}
			}
		}

		#endregion
	}
}

