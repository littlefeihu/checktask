using System;
using System.Collections.Generic;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.ContentPage;

namespace LexisNexis.Red.Droid.Business
{
	public class TOCBreadcrumbs
	{
		public const string GetTOCListTASK = "GetTOCListTASK";

		private bool currentNodeExpanded;
		private readonly ObjHolder<Publication> publication;
		private readonly TOCNode rootTocNode;
		private readonly TOCNode firstPage;
		private readonly TOCNode lastPage;
		private TOCNode showingLeafNode;
		private TOCNode currentTOCNode;
		private Guid navigationRecordId;

		public Guid NavigationRecordId
		{
			get
			{
				return navigationRecordId;
			}
		}

		public TOCNode CurrentTOCNode
		{
			get
			{
				return currentTOCNode;
			}

			set
			{
				currentTOCNode = value;

				if(currentTOCNode != null && !currentTOCNode.IsParent())
				{
					showingLeafNode = currentTOCNode;
				}
			}
		}

		public TOCNode ShowingLeafNode
		{
			get
			{
				if(CurrentTOCNode != null && !CurrentTOCNode.IsParent())
				{
					showingLeafNode = CurrentTOCNode;
				}
				else if(showingLeafNode == null)
				{
					showingLeafNode = firstPage;
				}

				return showingLeafNode;
			}
		}

		public Publication Publication
		{
			get
			{
				return publication.Value;
			}
		}

		public TOCBreadcrumbs(ObjHolder<Publication> publication, TOCNode rootTocNode)
		{
			this.publication = publication;
			this.rootTocNode = rootTocNode;
			firstPage = rootTocNode.GetFirstPage();
			lastPage = rootTocNode.GetLastPage();
			CurrentTOCNode = null;
		}

		public TOCNode RootTocNode
		{
			get
			{
				return rootTocNode;
			}
		}

		public List<TOCNode> GetPresentNodeList()
		{
			List<TOCNode> result = null;
			if(CurrentTOCNode == null
				// If current node is a top level node, and it is collapsed.
				// Then juse show all its brother.
				|| ((!CurrentNodeExpanded) && (CurrentTOCNode.NodeLevel == 1 || CurrentTOCNode.ParentNode == null)))
			{
				result = new List<TOCNode>(rootTocNode.ChildNodes);
				result.Insert(0, null);
				return result;
			}

			result = new List<TOCNode>();

			// All nodes before the spliterNode (include spliterNodeId), should be expanded;
			// All nodes After the spliterNode (exclude spliterNodeId), should be collapsed;
			TOCNode spliterNode = CurrentNodeExpanded ? CurrentTOCNode : CurrentTOCNode.ParentNode;

			foreach(var subnode in spliterNode.ChildNodes)
			{
				result.Add(subnode);
			}

			while(spliterNode != null && spliterNode.NodeLevel != 0)
			{
				result.Insert(0, spliterNode);
				spliterNode = spliterNode.ParentNode;
			}

			result.Insert(0, null);
			return result;
		}

		public bool IsCurrentNode(TOCNode node)
		{
			if(CurrentTOCNode == null)
			{
				return node.ID == GetFirstPage().ID;
			}
			else
			{
				return CurrentTOCNode.ID == node.ID;
			}
		}

		public bool IsExpanded(TOCNode node, List<TOCNode> presentNodeList)
		{
			if(CurrentTOCNode == null
				// If current node is a top level node, and it is collapsed,
				// Then juse show all its brother, and all nodes should be collapsed.
				|| ((!CurrentNodeExpanded) && (CurrentTOCNode.NodeLevel == 1 || CurrentTOCNode.ParentNode == null)))
			{
				return false;
			}

			if(node.ID == CurrentTOCNode.ID)
			{
				return CurrentNodeExpanded;
			}

			// All nodes before the spliterNode (include spliterNodeId), should be expanded;
			// All nodes After the spliterNode (exclude spliterNodeId), should be collapsed;
			var spliterNodeId = CurrentNodeExpanded ? CurrentTOCNode.ID : CurrentTOCNode.ParentId;
			for(int i = 0; i < presentNodeList.Count; ++i)
			{
				var curNode = presentNodeList[i];
				if(curNode == null)
				{
					continue;
				}

				if(node.ID == curNode.ID)
				{
					return true;
				}

				if(spliterNodeId == curNode.ID)
				{
					return false;
				}
			}

			return false;
		}

		public int GetMaxLevel(List<TOCNode> presentNodeList)
		{
			return presentNodeList[presentNodeList.Count - 1].NodeLevel;
		}

		public bool CurrentNodeExpanded
		{
			set
			{
				if(CurrentTOCNode == null)
				{
					throw new InvalidOperationException("Current node is null.");
				}

				currentNodeExpanded = value;
			}

			get
			{
				if(CurrentTOCNode == null)
				{
					throw new InvalidOperationException("Current node is null.");
				}

				return CurrentTOCNode.IsParent() && currentNodeExpanded;
			}
		}

		public TOCNode GetFirstPage()
		{
			if(firstPage == null)
			{
				throw new ApplicationException(String.Format(
					"No page is found in Book [{0}:{1}].",
					publication.Value.BookId,
					publication.Value.Name));
			}

			return firstPage;
		}

		public bool IsFirstPage(TOCNode node)
		{
			if(firstPage == null)
			{
				throw new ApplicationException(String.Format(
					"No page is found in Book [{0}:{1}].",
					publication.Value.BookId,
					publication.Value.Name));
			}

			return firstPage.ID == node.ID;
		}

		public bool IsLastPage(TOCNode node)
		{
			if(lastPage == null)
			{
				throw new ApplicationException(String.Format(
					"No page is found in Book [{0}:{1}].",
					publication.Value.BookId,
					publication.Value.Name));
			}

			return lastPage.ID == node.ID;
		}

		public bool IsCurrentPublication(int bookId)
		{
			return publication.Value.BookId == bookId;
		}

		public void SetCurrentTOCNodeById(int tocId)
		{
			if(tocId < 0)
			{
				CurrentTOCNode = null;
				return;
			}

			var requestedNode = PublicationContentUtil.Instance.GetTOCByTOCId(tocId, rootTocNode);
			if(requestedNode == null)
			{
				throw new ApplicationException(String.Format(
					"Unable to found requested TOC in Book [TOCId:{0} | {1}:{2}].",
					tocId,
					publication.Value.BookId,
					publication.Value.Name));
			}

			CurrentTOCNode = requestedNode;
		}

		public void BindNavigationItem()
		{
			navigationRecordId = NavigationManager.Instance.CurrentRecord.RecordID;
		}

		public void ResetNavigationItem()
		{
			navigationRecordId = Guid.Empty;
		}

		public bool IsBindNavigationItem()
		{
			return NavigationManager.Instance.Records.FindIndex(r => r.RecordID == navigationRecordId) >= 0;
		}

		public bool IsCurrentNavigationItem()
		{
			if(NavigationManager.Instance.CurrentRecord == null)
			{
				return false;
			}

			return navigationRecordId == NavigationManager.Instance.CurrentRecord.RecordID;
		}

		public BrowserRecord GetNavigationItem()
		{
			return NavigationManagerHelper.GetRecord(navigationRecordId);
		}

		public TOCNode GetTocNode(int tocId)
		{
			var requestedNode = PublicationContentUtil.Instance.GetTOCByTOCId(tocId, rootTocNode);
			if(requestedNode == null)
			{
				throw new ApplicationException(String.Format(
					"Unable to found requested TOC in Book [TOCId:{0} | {1}:{2}].",
					tocId,
					publication.Value.BookId,
					publication.Value.Name));
			}

			return requestedNode;
		}
	}
}

