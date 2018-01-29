using System;
using System.Collections.Generic;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;


namespace LexisNexis.Red.Mac.Data
{
	public class PublicationContentManager
	{
		#region properties
		public string BookTitle { get; set;}
		public int BookID { get; set;}
		public List<TOCNode> TOCNodeList { get; set;}
		public List<TOCNode> RootNodeList { get; set;}
		public PublicationContentPanelController PanelController;
		public int CurrentLevel { get; set;}
		#endregion

		#region constructor
		public PublicationContentManager (int bookID, string bookTitle, PublicationContentPanelController controller)
		{
			PanelController = controller;
			BookTitle = bookTitle;
			BookID = bookID;
			RootNodeList = new List<TOCNode>(0);
			GetDlContentFromDB ();

		}
		#endregion

		#region methods
		public  async void GetDlContentFromDB()
		{
			TOCNodeList = await  PublicationUtil.Instance.GetDlBookTOC(BookID);
			RootNodeList.AddRange (TOCNodeList);

			UpdateTOCNodeListAndRefreshTableView ();
		}

		//expand current node
		public void GetSubNodeTOCListByNodeID(int tocNodeID)
		{
			if (tocNodeID >= TOCNodeList.Count) {
				Console.WriteLine ("GetSubNodeTOCListByNodeID node{0} error", tocNodeID);
				return;
			}

			List<TOCNode> subNodeList = new List<TOCNode> (0);
			subNodeList.AddRange (TOCNodeList [tocNodeID].ChildNodes);
			if (subNodeList != null) {

				if (TOCNodeList.Count > 0) {
					TOCNodeList.RemoveAll (item => item!=null);
				}

				TOCNodeList = subNodeList;
				CurrentLevel = TOCNodeList [0].NodeLevel;
					
				UpdateTOCNodeListAndRefreshTableView ();
			}
		}

		//collapse current node
		public void GetParentNodeTOCListByNodeID(int tocNodeID)
		{
			if (tocNodeID >= TOCNodeList.Count) {
				Console.WriteLine ("GetParentNodeTOCListByNodeID node{0} error", tocNodeID);
				return;
			}

			TOCNode parentNode = TOCNodeList [tocNodeID].ParentNode;
			if (parentNode == null) {
				if (TOCNodeList.Count > 0) {
					TOCNodeList.RemoveAll (item => item!=null);
				}
				TOCNodeList.AddRange(RootNodeList);

			} else {
				List<TOCNode> nodeList = new List<TOCNode> (0);
				nodeList.AddRange (parentNode.ChildNodes);

				if (nodeList != null) {
					if (TOCNodeList.Count > 0) {
						TOCNodeList.RemoveAll (item => item!=null);
					}
					TOCNodeList = nodeList;
				}
			}

			CurrentLevel = TOCNodeList [0].NodeLevel;

			UpdateTOCNodeListAndRefreshTableView ();
		}

		void UpdateTOCNodeListAndRefreshTableView()
		{
			if (TOCNodeList != null && TOCNodeList.Count>0) {
				var parentNode = TOCNodeList [0].ParentNode;
				while (parentNode != null) {
					TOCNodeList.Insert (0, parentNode);
					parentNode = TOCNodeList [0].ParentNode;
				}

				var rootNode = new TOCNode ();
				rootNode.Title = "Table of Contents";
				rootNode.NodeLevel = 0;
				TOCNodeList.Insert (0, rootNode);
			} else {
			}

			PanelController.RefreshTOCViewData ();
		}

		public int IndexByTitle(string title)
		{
			int index = 0;
			foreach(TOCNode node in TOCNodeList){
				if (node.Title == title) {
					break;
				}
				index++;
			}
			return index;
		}
			 
		#endregion
	}
}

