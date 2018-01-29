using System;
using System.Collections.Generic;

using Foundation;
using AppKit;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;
using System.Linq;
using System.Threading.Tasks;

namespace LexisNexis.Red.Mac.Data
{
	public class IndexDataManager
	{
		#region properties
		public string BookTitle { get; set;}
		public int BookID { get; set;}
		public Dictionary<string, List<Index>> IndexNodeList { get; set; }
		public List<string> GroupItems{ get; set; }
		public PublicationContentViewController PanelController;
		public Index CurrentIndex { get; set; }
		public nint CurrentRow { get; set; }
		#endregion

		#region constructor
		public IndexDataManager (int bookID, string bookTitle, PublicationContentViewController controller)
		{
			PanelController = controller;
			BookTitle = bookTitle;
			BookID = bookID;
		}
		#endregion

		#region methods
		public async Task GetIndexDataFromDB()
		{
			IndexNodeList = await PublicationContentUtil.Instance.GetIndexsByBookId(BookID);
			GroupLevelItems ();

			UpdateIndexListAndRefreshTableView ();
		}

		//	
		public List<Index> ChildrenForGroupItem (NSObject item) 
		{
			if (IndexNodeList == null) {
				return null;
			}

			List<Index> children = new List<Index>(0);
			string key = item.ToString();
			IndexNodeList.TryGetValue (key, out children);

			return children;
		}

		//get the UI display object type
		public NSObject ChildrenItemAtIndex (List<Index> valueList, int index)
		{
			Index value = valueList [index];
			return NSObject.FromObject(value.Title);
		}

		//get the Index object from children list
		public Index IndexObjectAtChildrenList (List<Index> valueList, int index)
		{
			if (index < 0 || index >= valueList.Count) {
				return null;
			}

			Index value = valueList [index];

			return value;
		}

		//
		public List<string> GroupLevelItems ()
		{
			if (GroupItems == null && IndexNodeList!= null) {
				GroupItems = IndexNodeList.Keys.ToList ();
			}
			return GroupItems;
		}

		public NSObject GroupItemAtIndex(int index)
		{
			string itemTitle = GroupItems [index];
			return NSObject.FromObject(itemTitle);
		}

		public bool IsGroupItem(NSObject item)
		{
			if (GroupItems.Contains (item.ToString())) {
				return true;
			} else {
				return false;
			}
		}

		void UpdateIndexListAndRefreshTableView()
		{
			if (GroupItems == null) {
				return;
			}

			PanelController.RefreshIndexViewData ();
		}
			
		#endregion
	}
}

