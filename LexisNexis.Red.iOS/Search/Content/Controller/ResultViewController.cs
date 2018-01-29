
using System;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	public partial class ResultViewController : UIViewController
	{
		public SearchResult SearchRes{get;set;}

		public UIPopoverController DocTypeFilterPopover{ get; set;}

		public ResultTableViewController ResTVC{ get; set;}

		
		public ResultViewController (SearchResult res) : base ("ResultViewController", null)
		{
			SearchRes = res;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
			SearchRes = null;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
			if (SearchRes != null && SearchRes.SearchDisplayResultList.Count > 0) {
				ShowResultTableViewInView ();
				ResultFilterContainerView.Hidden = false;
			} else {
				ResultFilterContainerView.Hidden = true;
			}

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("ChangeSearchFilterButtonText"), delegate(NSNotification obj) {//Sent from DocTypeFilterTableViewController
				if(obj.UserInfo != null ){
					ContentFilterButton.SetTitle(obj.UserInfo.ObjectForKey(new NSString ("title")).ToString(), UIControlState.Normal);
				}
			});


			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("ApplyContentTypeFilterToSearchResult"), delegate(NSNotification obj) {//Sent from DocTypeFilterTableViewController
				if(AppDataUtil.Instance.SelectedTypeInSearchFilter == null || AppDataUtil.Instance.SelectedTypeInSearchFilter.Count == 0){
					if(ResTVC != null){
						ResTVC.View.RemoveFromSuperview();
					}
				}else{
					ShowResultTableViewInView ();
				}
			});

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("RemoveSearchResultView"), delegate(NSNotification obj) {//Handle notification which sent from search bar delegate include TOCSearchBarDelegate and IndexSearchBarDelegate
				View.RemoveFromSuperview();
			});
		}

		partial void ShowDocTypeFilter (Foundation.NSObject sender)
		{
			if(DocTypeFilterPopover == null){
				DocTypeFilterTableViewController docTypeFilterTVC  = new DocTypeFilterTableViewController();
				DocTypeFilterPopover = new UIPopoverController(docTypeFilterTVC);
				DocTypeFilterPopover.BackgroundColor = UIColor.White;
				DocTypeFilterPopover.SetPopoverContentSize(new CoreGraphics.CGSize(320, 300), false);
			}

			AppDisplayUtil.Instance.SetPopoverController(DocTypeFilterPopover);
			DocTypeFilterPopover.PresentFromRect(((UIButton)sender).Frame, ((UIView)sender).Superview, UIPopoverArrowDirection.Up, true);
		}

		private void ShowResultTableViewInView ()
		{
			if(ResTVC == null){
				ResTVC = new ResultTableViewController(SearchRes);
				ResTVC.View.TranslatesAutoresizingMaskIntoConstraints = false;
			}
			View.AddSubview(ResTVC.View);
			View.AddConstraints (new NSLayoutConstraint[]{ 
				NSLayoutConstraint.Create(ResTVC.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, View, NSLayoutAttribute.Top, 1, 44),
				NSLayoutConstraint.Create(ResTVC.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, View, NSLayoutAttribute.Bottom, 1, 0),
				NSLayoutConstraint.Create(ResTVC.View, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, View, NSLayoutAttribute.Leading, 1, 0),
				NSLayoutConstraint.Create(ResTVC.View, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, 0)
			});


			NSNotificationCenter.DefaultCenter.PostNotificationName (new NSString ("HighlightContentSearchKeyword"), this, new NSDictionary ("ContentSearchKeyword", String.Join(" ",SearchRes.FoundWordList)));
		}

	}
}

