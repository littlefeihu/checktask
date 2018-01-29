using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using UIKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	/// <summary>
	/// App data util.
	/// This is a singleton class which used to manage (get or set) data used by app, including:
	/// 1. Current opend publication and status;
	/// 2. User's content browser history;
	/// 3. Tag manage;
	/// 4. Annotation manage;
	/// etc.
	/// </summary>
	public class AppDataUtil
	{
		private static readonly AppDataUtil instance;

		private AppDelegate appDelegate;

		public List<Tag> TagList{ get; set; }


		public int AnnotationFilterSelectedIndex{ get; set; }
		public int AnnotationSegmentSelectedIndex{ get; set;}
		public int indexPathRowNumber{ get; set;}


		public List<ContentCategory> SelectedTypeInSearchFilter{ get; set; }//content type which is selected in search result content type filter controller, null means no content type is selected

		public string ScrollToHtmlTagId{ get; set; }

		public string ContentSearchKeyword{ get; set; }//user input text in search bar

		public string HighlightSearchKeyword{ get; set; }//value is set when search result item is selected, and it supposed to set "" when TOC is opend

		private AppDataUtil()
		{
			this.appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;

			TagList = new List<Tag> ();
 			List<AnnotationTag> annoTagList = AnnCategoryTagUtil.Instance.GetTags ();
 			foreach (var annoTag in annoTagList) {
				TagList.Add(new Tag(annoTag, true));
			}
		}

		static AppDataUtil()
		{
			instance = new AppDataUtil();


		}
		public static AppDataUtil Instance
		{
			get
			{
				return instance;
			}
		}

		public void SetCurrentPublication (Publication p, string searchKeyword = "")
		{
			if (appDelegate.CurOpendPublication.P == null || appDelegate.CurOpendPublication.P.BookId != p.BookId) {
				appDelegate.CurOpendPublication.RootTOCNode = GetRootTOCNodeOfPublication(p).Result;
			}
			appDelegate.CurOpendPublication.P = p;

			ContentSearchKeyword = searchKeyword;//reset user input search keyword, when user enter an new publication
		}

		private Task<TOCNode> GetRootTOCNodeOfPublication(Publication p)
		{
			return Task.Run (async ()=>{
				return await PublicationUtil.Instance.GetDlBookTOC (p.BookId);
			});
		}


		public Publication GetCurrentPublication ()
		{
			return appDelegate.CurOpendPublication.P;
		}


		public void SetOpendTOC (TOCNode node)
		{
			if (node != null) {
				appDelegate.CurOpendPublication.OpendTOCNode = node;
				ScrollToHtmlTagId = "";
			}
		}

		public TOCNode GetOpendTOC ()
		{
			return appDelegate.CurOpendPublication.OpendTOCNode;
		}


		public void SetOpendIndex (Index index)
		{
			appDelegate.CurOpendPublication.OpendIndex = index;
		}

		public Index GetOpendIndex ()
		{
			return appDelegate.CurOpendPublication.OpendIndex;
		}

		public void SetOpendContentType (PublicationContentTypeEnum contentType)
		{
			appDelegate.CurOpendPublication.OpendContentType = contentType;
		}

		public PublicationContentTypeEnum GetOpendContentType ()
		{
			return appDelegate.CurOpendPublication.OpendContentType;
		}

		/// <summary>
		/// Gets the content of the current displayed.
		/// TOC content
		/// Index content
		/// </summary>
		/// <returns>The current displayed content.</returns>
		public async Task<string> GetCurrentDisplayedContent ()
		{
			string content = "";

			if (appDelegate.CurOpendPublication.OpendContentType == PublicationContentTypeEnum.TOC) {
				content = await PublicationContentUtil.Instance.GetContentFromTOC(appDelegate.CurOpendPublication.P.BookId, appDelegate.CurOpendPublication.OpendTOCNode);
			} else if (appDelegate.CurOpendPublication.OpendContentType == PublicationContentTypeEnum.Index) {
				content = await PublicationContentUtil.Instance.GetContentFromIndex (appDelegate.CurOpendPublication.P.BookId, appDelegate.CurOpendPublication.OpendIndex);
			} else {
				//GET TOC content, with selected annotation
			}

			return content;
		}

		public void AddOpenedContentObserver (Observer o)
		{
			//appDelegate.CurOpendPublication.ClearObservers ();
			//TODO, avoid repeat to add observer
			appDelegate.CurOpendPublication.AddObserver (o);
		}

		public void ClearOpendPublicationObserver ()
		{
			appDelegate.CurOpendPublication.ClearObservers ();
		}

		#region TOC operation
		/// <summary>
		/// Gets the current publication root node of toc.
		/// </summary>
		/// <returns>The current publication toc root node.</returns>
		public TOCNode GetCurPublicationTocRootNode ()
		{
			return appDelegate.CurOpendPublication.RootTOCNode;
		}

		/// <summary>
		/// Gets the TOC node by I.
		/// </summary>
		/// <returns>The TOC node by I.</returns>
		/// <param name="id">Identifier.</param>
		public TOCNode GetTOCNodeByID (int id)
		{
			//TODO, optimize the way to get TOCNode by toc id
			return PublicationContentUtil.Instance.GetTOCByTOCId(id, appDelegate.CurOpendPublication.RootTOCNode);
		}

		public TOCNode GetHighlightedTOCNode ()
		{
			return appDelegate.CurOpendPublication.HighlightedTOCNode;
		}
		public void SetHighlightedTOCNode(TOCNode node)
		{
			if (node != null) {
				appDelegate.CurOpendPublication.HighlightedTOCNode = node;
			}
		}


		public async void ScrollToTOC (InfiniteScrollCallBack callBack, UIWebView webView, string direction, int id)
		{
			TOCNode node = GetTOCNodeByID (id);
			if (node != null) {
				string content = await PublicationContentUtil.Instance.GetContentFromTOC (appDelegate.CurOpendPublication.P.BookId, node);
				callBack (webView, content, direction);
			}
		}

		public TOCNode GetNextOfTOCNodeWithId(int id, string direction)
		{
			TOCNode node = GetTOCNodeByID (id);
			if (direction == "previous") {
				return PublicationContentUtil.Instance.GetPreviousPageByTreeNode (node);
			} else if (direction == "next") {
				return PublicationContentUtil.Instance.GetNextPageByTreeNode (node);
			}
			return null;
		}

		#endregion 


		#region content navigation
		/// <summary>
		/// Invoked when user touch forward button on content navigation bar
		/// <returns>The next browser record which user accessed</returns>
		/// </summary>
		public BrowserRecord Forward ()
		{
			return NavigationManager.Instance.Forth ();
		}

		/// <summary>
		/// Invoked when user touch backward button on content navigation bar
		/// <returns>The previous browser record which user accessed</returns>
		/// </summary>
		public BrowserRecord Backward ()
		{
			return NavigationManager.Instance.Back ();
		}

		/// <summary>
		/// Adds the browser record when user opend the publication or click leaf node of toc
		/// </summary>
		/// <param name="r">The red component.</param>
		public void AddBrowserRecord (ContentBrowserRecord r)
		{
			NavigationManager.Instance.AddRecord (r);
		}


		public bool CanBack ()
		{
			return NavigationManager.Instance.CanBack;
		}

		public bool CanForward ()
		{
			return NavigationManager.Instance.CanForth;
		}
		#endregion


	}
}

