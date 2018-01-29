using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using System.Linq;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac
{
	public class SearchResultManager
	{
		#region properties

		public SearchResultViewController ViewController;

		public List<string> GroupItems{ get; set; }
		public List<SearchDisplayResult> DocumentResultList { get; set; }
		public List<SearchDisplayResult> PublicationResultList { get; set; }

		//public Dictionary<string, List<string>> SearchResultList { get; set; }

		public nint CurrentRow { get; set; }
		#endregion

		#region constructor
		public SearchResultManager (SearchResultViewController controller, int bookID, string keyword)
		{
			ViewController = controller;
			GetSearchResultByKeyWord (bookID, keyword);
		}
		#endregion

		#region methods
		public void GetSearchResultByKeyWord(int bookID, string keyWord)
		{
			//PublicationResultList = SearchUtil.Search (bookID, ref keyWord, SearchUtil.ContentCategory.All);
			if (PublicationResultList == null || PublicationResultList.Count == 0) {
				int ncount = 10;
				PublicationResultList = new List<SearchDisplayResult> (0);
				while (ncount > 0) {
					var value = new SearchDisplayResult ();
					value.TocId = 1;
					value.TocTitle = "Update";
					value.GuideCardTitle = "What";
					PublicationResultList.Add (value);

					ncount--;
				}

				DocumentResultList = new List<SearchDisplayResult> (0);
				DocumentResultList.AddRange (PublicationResultList);
			} else {
				var value = new SearchDisplayResult ();
				value.TocId = 1;
				value.TocTitle = "Update";
				value.GuideCardTitle = "What";

				DocumentResultList = new List<SearchDisplayResult> (0);
				DocumentResultList.Add (value);
			}

			GroupLevelItems ();
			//UpdateResultListAndRefreshTableView ();
		}

		//	
		public List<SearchDisplayResult> ChildrenForGroupItem (NSObject item) 
		{
			if (PublicationResultList == null) {
				return null;
			}

			if (item.ToString () == "Document") {
				return DocumentResultList;
			}else {
				return PublicationResultList;
			}


			//List<string> children = new List<string>(0);
			//string key = item.ToString();
			//SearchResultList.TryGetValue (key, out children);
			//return children;
		}

		//get the UI display object type
		public NSObject ChildrenItemAtIndex (List<SearchDisplayResult> valueList, int index)
		{
			SearchDisplayResult value = valueList [index];

			var dict = new NSDictionary ("key1", value.TocId, "key2", value.TocTitle, "key3", value.GuideCardTitle);
			return dict;
		}

		//get the Index object from children list
		public SearchDisplayResult ObjectAtChildrenList (List<SearchDisplayResult> valueList, int index)
		{
			if (index < 0 || index >= valueList.Count) {
				return null;
			}

			SearchDisplayResult value = valueList [index];

			return value;
		}

		//
		public List<string> GroupLevelItems ()
		{
			if (GroupItems == null) {
				GroupItems = new List<string> (2);
				GroupItems.Add ("Document");
				GroupItems.Add ("Publication");
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
			if (item.Class.Name == "__NSDictionaryI") {
				return false;
			} else {
				return true;
			}
		}

		void UpdateResultListAndRefreshTableView()
		{
			if (GroupItems == null) {
				return;
			}

			ViewController.RefreshSearchViewData ();
		}

		#endregion
	}
}

