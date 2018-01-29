using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	partial class TOCTableViewController : UITableViewController
	{
		public TOCNodeTree Tree{ get; set;}

		/// <summary>
		/// Gets or sets the displayed toc list.
		/// </summary>
		/// <value>The displayed toc list.</value>
		public List<TOCNode> DisplayedTocList{ get; set;}


		/// <summary>
		/// Gets or sets the touched TOC node.
		/// The TOCNode which user touched
		/// </summary>
		/// <value>The touched TOC node.</value>
		public TOCNode HighlightedTOCNode{ get; set;}

		public TOCNode LatestOpendTOCNode{ get; set;}

		public TOCTableViewController (IntPtr handle) : base (handle)
		{
			InitTOC ();
		}

		#region override
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//Add notification observer to handle content search
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("searchContent"), delegate(NSNotification obj) {
				UIStoryboard storyboard = UIStoryboard.FromName ("ContentSearchResult", NSBundle.MainBundle);
				UIViewController controller = storyboard.InstantiateViewController ("ContentSearchResult");
				//List<SearchDisplayResult> res = SearchUtil.Search (AppDataUtil.Instance.GetCurrentPublication ().BookId, ref keyword);

				PresentViewController(controller, true, delegate() {});

			});


			TableView.TableFooterView = new UIView ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}



		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return DisplayedTocList.Count;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "Table of Contents";
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			CustomizedTableOfContentCell cell = tableView.DequeueReusableCell (CustomizedTableOfContentCell.Key) as CustomizedTableOfContentCell;
			if (cell == null)
				cell = CustomizedTableOfContentCell.Create ();

			TOCNode node = this.DisplayedTocList[indexPath.Row];
			cell.TintColor = UIColor.White;
			cell.LeftColorSideBar.BackgroundColor = ColorUtil.GenerateTintColor (node.NodeLevel, Tree.TotalLevel);
			cell.NameLabel.Text = node.Title;

			//Set cell indicator according to the role and status of TOCNode
			if (node.Role == "me") {
				if (HighlightedTOCNode != null && HighlightedTOCNode.ID == node.ID) {
					cell.AccessoryView = new UIImageView (new UIImage ("Images/Publication/TOC/PageIconWhite.png"));
				} else {
					cell.AccessoryView = new UIImageView (new UIImage ("Images/Publication/TOC/PageIcon.png"));
				}
			} else {
				if (IsTOCNodeOpened(node)) {
					cell.AccessoryView = new UIImageView (new UIImage ("Images/Publication/TOC/DownArrow.png"));
				} else {
					cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				}
			}

			//set background color of current toc cell
			if (HighlightedTOCNode != null && HighlightedTOCNode.ID == node.ID) {
				cell.BackgroundColor = cell.LeftColorSideBar.BackgroundColor;
				cell.NameLabel.TextColor = UIColor.White;
				tableView.ScrollToRow (indexPath, UITableViewScrollPosition.Middle, false);
			}

			return cell;
		}


		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{

			HighlightedTOCNode = DisplayedTocList[indexPath.Row];

			if (HighlightedTOCNode.Role == "me" || IsTOCNodeOpened(HighlightedTOCNode)) {
				LatestOpendTOCNode = HighlightedTOCNode.ParentNode;
			} else {
				LatestOpendTOCNode = HighlightedTOCNode;
			}
			DisplayedTocList = Tree.GetDisplayTOCNodeList (LatestOpendTOCNode);

			UIView.Animate (0.2, delegate {
				tableView.ReloadData ();
				//tableView.ScrollToRow(indexPath, UITableViewScrollPosition.None, false);
			});

			//display content of toc page in webview
			if (HighlightedTOCNode.Role == "me") {
				SetDisplayedTOC (HighlightedTOCNode);
			}
		}

		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			UIView headerView = new UIView (new CGRect(0, 0, 320, 44));
			UILabel headerTitleLabel = new UILabel (new CGRect(15, 12, 260, 20));
			headerTitleLabel.Text = "Table of Contents";
			headerTitleLabel.Font = UIFont.SystemFontOfSize (14);
			headerView.AddSubview (headerTitleLabel);

			UIView bottomLine = new UIView (new CGRect(15, 43, 305, 1));
			bottomLine.BackgroundColor = UIColor.LightGray.ColorWithAlpha(0.6f);
			headerView.AddSubview (bottomLine);

			UIView sideBar = new UIView (new CGRect(0, 0, 5, 44));
			sideBar.BackgroundColor = UIColor.FromRGB(255, 200, 200);
			headerView.AddSubview (sideBar);

			headerView.BackgroundColor = UIColor.White;


			UITapGestureRecognizer tapHeaderRecoginzer = new UITapGestureRecognizer ();
			tapHeaderRecoginzer.AddTarget (delegate() {
				HighlightedTOCNode = Tree.GetFirstPageNode().ParentNode;
				LatestOpendTOCNode = HighlightedTOCNode.ParentNode;
				DisplayedTocList = Tree.GetDisplayTOCNodeList(LatestOpendTOCNode);
				tableView.ReloadData();
			});
			headerView.UserInteractionEnabled = true;
			headerView.AddGestureRecognizer (tapHeaderRecoginzer);

			return headerView;

		}


		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return 44;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			TOCNode node = this.DisplayedTocList[indexPath.Row];
			CGSize expectedLableSize = TextDisplayUtil.GetStringBoundRect(node.Title, UIFont.SystemFontOfSize(14), new CGSize (260, 600));

			return ((expectedLableSize.Height + 24) > 44) ? (expectedLableSize.Height + 24) : 44;
		}

		#endregion


		private void InitTOC ()
		{
			//Initialize TOC 
			Tree = new TOCNodeTree (AppDataUtil.Instance.GetCurPublicationTocRootNode());

			if (AppDataUtil.Instance.GetOpendTOC () != null) {
				LatestOpendTOCNode = AppDataUtil.Instance.GetOpendTOC ().ParentNode;
			} else {
				var firstNonPageTOCNode = Tree.GetFirstPageNode ().ParentNode;
				LatestOpendTOCNode = firstNonPageTOCNode.ParentNode == null ? firstNonPageTOCNode : firstNonPageTOCNode.ParentNode;
			}
			DisplayedTocList = Tree.GetDisplayTOCNodeList (LatestOpendTOCNode);
			if (AppDataUtil.Instance.GetOpendTOC () == null) {
				HighlightedTOCNode = Tree.GetFirstPageNode().ParentNode;
				SetDisplayedTOC (Tree.GetFirstPageNode());
			} else {
				HighlightedTOCNode = AppDataUtil.Instance.GetOpendTOC();
				SetDisplayedTOC (HighlightedTOCNode);
			}
		}

		/// <summary>
		/// Determines whether the instance TOC node is opened or not
		/// </summary>
		/// <returns><c>true</c> if this instance is TOC node opened the specified node; otherwise, <c>false</c>.</returns>
		/// <param name="node">Node.</param>
		private bool IsTOCNodeOpened(TOCNode node)
		{
			if (node != null) {
				if (node.ChildNodes != null) {
					foreach(var n in node.ChildNodes){
						var nodeInDisplayedList = DisplayedTocList.Find(o => o.ID == n.ID);
						if (nodeInDisplayedList != null)
							return true;
					}
				}
			}
			return false;

		}

		private void SetDisplayedTOC (TOCNode node)
		{
			if (node != null) {
				AppDataUtil.Instance.SetOpendTOC (node);
				AppDataUtil.Instance.AddBrowserRecord (new ContentBrowserRecord(AppDataUtil.Instance.GetCurrentPublication().BookId, node.ID, 0));//add browser record
			}
		}


		public void SetSelectTOCNode (TOCNode node)
		{
			HighlightedTOCNode = node;
			LatestOpendTOCNode = HighlightedTOCNode.ParentNode;
			DisplayedTocList = Tree.GetDisplayTOCNodeList(LatestOpendTOCNode);
		}
	}
}
