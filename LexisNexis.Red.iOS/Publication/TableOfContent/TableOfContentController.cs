using System;

using Foundation;
using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

using MBProgressHUD;

namespace LexisNexis.Red.iOS
{
	public partial class TableOfContentController : UIViewController, Observer
	{
		private TableOfContentTableViewController tocTableViewVC;

		private TableOfContentTableViewSource tocTableViewSource;


		public TableOfContentController (IntPtr handle) : base (handle)
		{

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("SearchContentInTOC"), ProcessContentSearchRequest);

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("RemoveContentSearchResultViewInTOC"), delegate(NSNotification obj) {
				if(AppDisplayUtil.Instance.ContentSearchResController != null){
					//AppDisplayUtil.Instance.ContentSearchResController.View.RemoveFromSuperview();
				}
			});

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ExpireInfoView.AddGestureRecognizer (new UITapGestureRecognizer (AppDisplayUtil.Instance.ShowPublicationInfoView));
			ShowExpireInfoView ();

			tocTableViewVC = new TableOfContentTableViewController ();
			TOCNode rootNode = AppDataUtil.Instance.GetCurPublicationTocRootNode ();
			if (rootNode != null && rootNode.ChildNodes != null && rootNode.ChildNodes.Count > 0) {
				tocTableViewSource = new TableOfContentTableViewSource (rootNode);
				tocTableViewVC.TableView.Source = tocTableViewSource;
				tocTableViewVC.TableView.TranslatesAutoresizingMaskIntoConstraints = false;
				tocTableViewVC.TableView.TableFooterView = new UIView ();//hidden redundant line separator

				ContainerView.AddSubview (tocTableViewVC.TableView);
				ContainerView.AddConstraints (new NSLayoutConstraint[]{ 
					NSLayoutConstraint.Create(tocTableViewVC.TableView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 0),
					NSLayoutConstraint.Create(tocTableViewVC.TableView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0),
					NSLayoutConstraint.Create(tocTableViewVC.TableView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Leading, 1, 0),
					NSLayoutConstraint.Create(tocTableViewVC.TableView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Trailing, 1, 0)
				});

				AppDataUtil.Instance.AddOpenedContentObserver (this);//Set current instance as the observer of subject OpendPublication to get notification when opend content changed
			}

			AppDisplayUtil.Instance.TOCVC = this;

			SearchBar.Delegate = new TOCSearchBarDelegate ();
			//hide search bar border
			SearchBar.Layer.BorderWidth = 1;
			SearchBar.Layer.BorderColor = UIColor.FromRGB (194, 194, 194).CGColor;
 		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (AppDataUtil.Instance.ContentSearchKeyword != null) {
				SearchBar.Text = AppDataUtil.Instance.ContentSearchKeyword;
			}
			SearchBar.ResignFirstResponder ();

		}

		/// <summary>
		/// Implement method Update(Subject s) which defined in interface Observer
		/// Observer design pattern is used here to get notification when the selected toc is changed
		/// </summary>
		/// <param name="s">S.</param>
		public void Update(Subject s)
		{
			if (AppDataUtil.Instance.GetOpendContentType () == PublicationContentTypeEnum.TOC) {
				TableOfContentTableViewSource source = (TableOfContentTableViewSource)tocTableViewVC.TableView.Source;
				source.Tree = new TOCNodeTree (AppDataUtil.Instance.GetCurPublicationTocRootNode());
				source.HighlightedTOCNode = AppDataUtil.Instance.GetHighlightedTOCNode () ?? AppDataUtil.Instance.GetOpendTOC ();
				source.LatestOpendTOCNode = source.HighlightedTOCNode.ParentNode;
				source.DisplayedTocList = source.Tree.GetDisplayTOCNodeList (source.LatestOpendTOCNode);
				tocTableViewVC.TableView.ReloadData ();

				ShowExpireInfoView ();
			}
		}


		public void SetHighlightedTOCNode (TOCNode node)
		{
			if (node != null) {
				tocTableViewSource.SetSelectTOCNode (node);
				tocTableViewVC.TableView.ReloadData ();
			}
		}

		public void ProcessContentSearchRequest (NSNotification obj)
		{
			var hud = new MTMBProgressHUD (View) {
				LabelText = "Loading",
				RemoveFromSuperViewOnHide = true
			};
			View.AddSubview (hud);
			hud.Show (animated: true);

			string keyword = SearchBar.Text;
			SearchResult res = SearchUtil.Search(AppDataUtil.Instance.GetCurrentPublication().BookId, AppDataUtil.Instance.GetOpendTOC().ID, keyword);

			if (AppDisplayUtil.Instance.ContentSearchResController != null) {
				AppDisplayUtil.Instance.ContentSearchResController.View.RemoveFromSuperview ();
			}
			AppDisplayUtil.Instance.ContentSearchResController = new ResultViewController (res);
			AppDisplayUtil.Instance.ContentSearchResController.View.TranslatesAutoresizingMaskIntoConstraints = false;
			ContainerView.AddSubview (AppDisplayUtil.Instance.ContentSearchResController.View);
			ContainerView.AddConstraints (new NSLayoutConstraint[]{ 
				NSLayoutConstraint.Create(AppDisplayUtil.Instance.ContentSearchResController.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 0),
				NSLayoutConstraint.Create(AppDisplayUtil.Instance.ContentSearchResController.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0),
				NSLayoutConstraint.Create(AppDisplayUtil.Instance.ContentSearchResController.View, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Leading, 1, 0),
				NSLayoutConstraint.Create(AppDisplayUtil.Instance.ContentSearchResController.View, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Trailing, 1, 0)
			});

			hud.Hide (animated: true, delay: 0.2);
		}

		private void ShowExpireInfoView ()
		{
			bool isExpireInfoViewAppear = AppDataUtil.Instance.GetCurrentPublication ().DaysRemaining < 0 ? true : false;

			if (isExpireInfoViewAppear) {
				UILabel expireLabel = new UILabel (new CGRect (15, 5, 100, 20));
				expireLabel.Font = UIFont.SystemFontOfSize (14);
				expireLabel.TextColor = UIColor.White;
				expireLabel.Text = "Expired";

				UILabel dateLabel = new UILabel (new CGRect (15, 25, 256, 14));
				dateLabel.Font = UIFont.SystemFontOfSize (12);
				dateLabel.TextColor = UIColor.LightTextColor;
				dateLabel.Text = "Currency Date " + ((DateTime)AppDataUtil.Instance.GetCurrentPublication ().LastUpdatedDate).ToString ("dd MMM yyyy");

				UIButton infoButton = new UIButton (UIButtonType.DetailDisclosure);
				infoButton.Frame = new CGRect (288, 11, 22, 22);
				infoButton.TintColor = UIColor.White;
				infoButton.TouchUpInside += (object sender, EventArgs e) => AppDisplayUtil.Instance.ShowPublicationInfoView();

				ExpireInfoView.AddSubviews (new UIView[]{expireLabel, dateLabel, infoButton});

				for (int i = 0; i < ExpireInfoView.Constraints.Length; i++) {
					if (ExpireInfoView.Constraints [i].FirstAttribute == NSLayoutAttribute.Height) {
						ExpireInfoView.Constraints [i].Constant = 44;
					}
				}

			} else {
				foreach (var subView in ExpireInfoView.Subviews) {
					subView.RemoveFromSuperview ();
				}
				for (int i = 0; i < ExpireInfoView.Constraints.Length; i++) {
					if (ExpireInfoView.Constraints [i].FirstAttribute == NSLayoutAttribute.Height) {
						ExpireInfoView.Constraints [i].Constant = 0;
					}
				}
			}
		}

	}
}
