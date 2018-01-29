using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
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
		public PublicationContentViewController PanelController;
		public int CurrentLevel { get; set;}
		public TOCNode CurrentLeafNode { get; set;}
		public int StartTocID { get; set; }
		public int EndTocID { get; set; }
		public int CurrentHighlight { get; set;}
		public int CurrentTocID { get; set;}


		public List<string> SearchTermWordList { get; set; }
		public string CurrentSearchHeader { get; set; }

		private List<string> SearchKeyWordList { get; set; }
		private string CurrentSearchContent { get; set; }

		#endregion

		#region constructor
		public PublicationTOCDataManager (int bookID, string bookTitle, PublicationContentViewController controller)
		{
			PanelController = controller;
			BookTitle = bookTitle;
			BookID = bookID;
			RootNodeList = new List<TOCNode>(0);
			TOCNodeList = new List<TOCNode> (0);
		}
		#endregion

		#region methods
		public void InitializeTOCDataByBookID (int bookID, int tocID)
		{
			BookID = bookID;
			CurrentTocID = tocID;
			CurrentHighlight = 1;
			CurrentLeafNode = null;
			CurrentSearchContent = null;

			if (RootNodeList != null) {
				RootNodeList.Clear ();
			}
			if (TOCNodeList != null) {
				TOCNodeList.Clear ();
			}
		}

		public void ClearMemory ()
		{
			BookTitle = null;
			BookID = 0;
			if (TOCNodeList != null) {
				TOCNodeList.Clear ();
			}
			if (RootNodeList != null) {
				RootNodeList.Clear ();
			}

			CurrentLevel = 1;
			CurrentLeafNode = null;
			CurrentHighlight = 1;
			CurrentTocID = -1;
			StartTocID = 0;
			EndTocID = 0;
			if (SearchTermWordList != null) {
				SearchTermWordList.Clear ();
			}
			if (SearchKeyWordList != null) {
				SearchKeyWordList.Clear ();
			}
			CurrentSearchContent = null;
			CurrentSearchHeader = null;
		}

		public  async Task GetPublicationTocFromDB()
		{
			TOCNode rootNode = await PublicationContentUtil.Instance.GetTOCByBookId (BookID);
			if (rootNode == null) {
				return;
			}
				
			List<TOCNode> nodelist = rootNode.ChildNodes;
			if (nodelist == null) {
				return;
			}

			if (TOCNodeList.Count > 0) {
				TOCNodeList.Clear ();
			}
			TOCNodeList.AddRange(nodelist);

			if (RootNodeList.Count > 0) {
				RootNodeList.Clear ();
			}
			RootNodeList.AddRange (nodelist);

			if (TOCNodeList.Any ()) {
				CurrentLevel = TOCNodeList [0].NodeLevel;
			}

			if (CurrentTocID < 0) {
				UpdateTOCNodeList ();
				PanelController.RefreshTOCViewData ();
				CurrentLeafNode = rootNode.GetFirstPage ();
				StartTocID = CurrentLeafNode.ID;
				EndTocID = CurrentLeafNode.ID;
			} else {
				TOCNode rootTocNode = RootNodeList[0].ParentNode;
				TOCNode currentTocNode = PublicationContentUtil.Instance.GetTOCByTOCId(CurrentTocID,rootTocNode);
				GetTOCNodeListByCurrentLeafNode (currentTocNode);
				CurrentLeafNode = currentTocNode;
				StartTocID = CurrentTocID;
				EndTocID = CurrentTocID;
				PanelController.RefreshTOCViewData ();
				PanelController.SelectTOCRectByTocID(currentTocNode.ID);
			}
		}

		//expand current node
		public void GetSubNodeTOCListByNodeID(int tocNodeID)
		{
			if (TOCNodeList == null) {
				return;
			}

			if (tocNodeID >= TOCNodeList.Count) {
				//Console.WriteLine ("GetSubNodeTOCListByNodeID node{0} error", tocNodeID);
				return;
			}

			List<TOCNode> subNodeList = new List<TOCNode> (0);
			subNodeList.AddRange (TOCNodeList [tocNodeID].ChildNodes);
			if (subNodeList != null) {

				if (TOCNodeList.Count > 0) {
					TOCNodeList.Clear ();
				}

				TOCNodeList.AddRange(subNodeList);
				subNodeList.Clear ();
				subNodeList = null;

				if (TOCNodeList.Any ()) {
					CurrentLevel = TOCNodeList [0].NodeLevel;
				}

				UpdateTOCNodeList ();
			}
		}

		//collapse current node
		public void GetParentNodeTOCListByNodeID(int tocNodeID)
		{
			if (TOCNodeList == null) {
				return;
			}

			if (tocNodeID >= TOCNodeList.Count) {
				//Console.WriteLine ("GetParentNodeTOCListByNodeID node{0} error", tocNodeID);
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
						TOCNodeList.Clear ();
					}
					TOCNodeList.AddRange(nodeList);
					nodeList.Clear ();
					nodeList = null;
				}
			}

			if (TOCNodeList.Any ()) {
				CurrentLevel = TOCNodeList [0].NodeLevel;
			}

			UpdateTOCNodeList ();
		}

		public bool GetTOCNodeListByCurrentLeafNodeID(int nodeID,bool isScrollBottom)
		{
			while(CurrentLeafNode.ID != nodeID){
				if (isScrollBottom) {
					CurrentLeafNode = PublicationContentUtil.Instance.GetNextPageByTreeNode (CurrentLeafNode);
				} else {
					CurrentLeafNode = PublicationContentUtil.Instance.GetPreviousPageByTreeNode (CurrentLeafNode);
				}
			}
				
			if (CurrentLeafNode == null) {
				return false;
			}
			//Console.WriteLine ("CustomScrollDrag_CurrentLeafNode:{0}",CurrentLeafNode.Title);
			List<TOCNode> nodeList = new List<TOCNode> (0);
			nodeList.AddRange (CurrentLeafNode.ParentNode.ChildNodes);

			if (nodeList != null) {
				if (TOCNodeList.Count > 0) {
					TOCNodeList.Clear ();
				}
				TOCNodeList.AddRange(nodeList);
				nodeList.Clear ();
				nodeList = null;

				if (TOCNodeList.Any ()) {
					CurrentLevel = TOCNodeList [0].NodeLevel;
				}

				UpdateTOCNodeList ();
			}

			CurrentHighlight = -1;
			return true;
		}

		public void GetTOCNodeListByCurrentLeafNode(TOCNode leafNode)
		{
			List<TOCNode> nodeList = new List<TOCNode> (0);
			nodeList.AddRange (leafNode.ParentNode.ChildNodes);

			if (nodeList != null) {
				if (TOCNodeList.Count > 0) {
					TOCNodeList.Clear ();
				}
				TOCNodeList.AddRange(nodeList);
				nodeList.Clear ();
				nodeList = null;

				if (TOCNodeList.Any ()) {
					CurrentLevel = TOCNodeList [0].NodeLevel;
				}

				UpdateTOCNodeList ();
			}

			CurrentHighlight = -1;
		}

		void UpdateTOCNodeList ()
		{
			if (TOCNodeList == null) {
				return;
			}

			if (TOCNodeList.Count>0) {
				var parentNode = TOCNodeList [0].ParentNode;

				while (parentNode != null) {
					if (parentNode.NodeLevel == 0) {
						break;
					} 
					TOCNodeList.Insert (0, parentNode);
					parentNode = TOCNodeList [0].ParentNode;
				}
			}

			var rootNode = new TOCNode ();
			rootNode.Title = "Table of Contents";
			rootNode.NodeLevel = 0;
			TOCNodeList.Insert (0, rootNode);
		}

		public int IndexByTOCID(int tocID)
		{
			if (TOCNodeList == null) {
				return 0;
			}

			int index = 0;
			foreach(TOCNode node in TOCNodeList){
				if (node.ID == tocID) {
					break;
				}
				index++;
			}
			return index;
		}

		public void SetHighlightTOCIndexByTocID(int tocNodeID)
		{
			int rowIndexHighLight = IndexByTOCID (tocNodeID);
			if (rowIndexHighLight <1 || rowIndexHighLight>= TOCNodeList.Count) {
				return;
			}

			CurrentHighlight = rowIndexHighLight;
		}

		public void UpdateSearchMatchWordList(List<string> termList, List<string> keywordList, string searchContent, string searchHeader)
		{
			//
			if (SearchTermWordList == null) {
				SearchTermWordList = new List<string> ();
			}

			SearchTermWordList.Clear ();
			if (termList != null) {
				SearchTermWordList.AddRange (termList);
			}

			//
			if (SearchKeyWordList == null) {
				SearchKeyWordList = new List<string> ();
			}

			SearchKeyWordList.Clear ();
			if (keywordList != null) {
				SearchKeyWordList.AddRange (keywordList);
			}

			//
			string newString = string.Empty;
			char [] param = {'.'};
			if (searchContent!=null) {
				newString = searchContent.Trim (param);
			}
			this.CurrentSearchContent = newString;

			//
			string newTitle = string.Empty;
			if (searchHeader!=null) {
				newTitle = searchHeader.Trim (param);
			}
			this.CurrentSearchHeader = newTitle.Trim ();
		}

		public void ClearSearchResult ()
		{
			UpdateSearchMatchWordList (null, null, null, null);
		}
			 
		#endregion
	}
}

