using System;
using System.Collections.Generic;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using System.Linq;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Mac.Data
{
	public class PublicationTOCDataManager
	{
		#region properties
		public string BookTitle { get; set;}
		public int BookID { get; set;}
		public List<TOCNode> TOCNodeList { get; set;}
		public List<TOCNode> RootNodeList { get; set;}
		public PublicationContentPanelController PanelController;
		public int CurrentLevel { get; set;}
		public TOCNode CurrentLeafNode { get; set;}
		#endregion

		#region constructor
		public PublicationTOCDataManager (int bookID, string bookTitle, PublicationContentPanelController controller)
		{
			PanelController = controller;
			BookTitle = bookTitle;
			BookID = bookID;
			RootNodeList = new List<TOCNode>(0);
			GetPublicationTocFromDB ();

		}
		#endregion

		#region methods
		public  async void GetPublicationTocFromDB()
		{
			TOCNode rootNodeList = await PublicationContentUtil.Instance.GetTOCByBookId (BookID);
			CurrentLeafNode = rootNodeList.GetFirstPage ();

			TOCNodeList = rootNodeList.ChildNodes;
			if (TOCNodeList != null) {
				RootNodeList.AddRange (TOCNodeList);
			}

			if (TOCNodeList.Any ()) {
				CurrentLevel = TOCNodeList [0].NodeLevel;
			}

			UpdateTOCNodeListAndRefreshTableView ();
			PanelController.TOCController.OpenPublicationContentAtTOCNode(CurrentLeafNode);
		}

		//expand current node
		public void GetSubNodeTOCListByNodeID(int tocNodeID)
		{
			if (TOCNodeList == null) {
				return;
			}

			if (tocNodeID >= TOCNodeList.Count) {
				Console.WriteLine ("GetSubNodeTOCListByNodeID node{0} error", tocNodeID);
				return;
			}

			List<TOCNode> subNodeList = new List<TOCNode> (0);
			subNodeList.AddRange (TOCNodeList [tocNodeID].ChildNodes);
			if (subNodeList != null) {

				if (TOCNodeList.Count > 0) {
					TOCNodeList.Clear ();
				}

				TOCNodeList = subNodeList;
				if (TOCNodeList.Any ()) {
					CurrentLevel = TOCNodeList [0].NodeLevel;
				}
					
				UpdateTOCNodeListAndRefreshTableView ();
			}
		}

		//collapse current node
		public void GetParentNodeTOCListByNodeID(int tocNodeID)
		{
			if (TOCNodeList == null) {
				return;
			}

			if (tocNodeID >= TOCNodeList.Count) {
				Console.WriteLine ("GetParentNodeTOCListByNodeID node{0} error", tocNodeID);
				return;
			}

			TOCNode parentNode = TOCNodeList [tocNodeID].ParentNode;
			if (parentNode == null || parentNode.NodeLevel == 0) {
				if (TOCNodeList.Count > 0) {
					TOCNodeList.Clear ();
		
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

			if (TOCNodeList.Any ()) {
				CurrentLevel = TOCNodeList [0].NodeLevel;
			}

			UpdateTOCNodeListAndRefreshTableView ();
		}
		List<TOCNode> l=new List<TOCNode> ();
		void UpdateTOCNodeListAndRefreshTableView()
		{
			if (TOCNodeList == null) {
				return;
			}

			if (TOCNodeList != null && TOCNodeList.Count>0) {
				var parentNode = TOCNodeList [0].ParentNode;
			
				while (parentNode != null) {
					if (parentNode.NodeLevel == 0) {
						parentNode.Title = "Table of Contents";
					} 
					//TOCNodeList.Insert (0, parentNode);
					l.Add (parentNode);
					parentNode = l [0].ParentNode;
				}
				l.AddRange (TOCNodeList);
				TOCNodeList.Clear ();
				TOCNodeList.AddRange (l);
			} else {
			}

			PanelController.RefreshTOCViewData ();
		}

		public int IndexByTitle(string title)
		{
			if (TOCNodeList == null) {
				return 0;
			}

			int index = 0;
			foreach(TOCNode node in TOCNodeList){
				if (node.Title == title) {
					break;
				}
				index++;
			}
			return index;
		}

		public string FirstLevelNodeTitleAtNode(TOCNode nodeData)
		{
			int nodeLevel = nodeData.NodeLevel;
			TOCNode parentNode = nodeData;
			string title = parentNode.Title;

			while (nodeLevel>1) {
				parentNode = parentNode.ParentNode;
				title = parentNode.Title;
			}

			return title;
		}
			 
		#endregion
	}
}

