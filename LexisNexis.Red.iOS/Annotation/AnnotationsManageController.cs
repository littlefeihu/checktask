using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	partial class AnnotationsManageController : UIViewController
	{
		public AnnotationsManageController (IntPtr handle) : base (handle)
		{
 		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			UIBarButtonItem historyBarButtonItem = new UIBarButtonItem (new UIImage ("Images/Navigation/HistoryIcon.png"), UIBarButtonItemStyle.Plain, ShowPopoverRecentHistory);
 			UIBarButtonItem tagBarButtonItem = null;
			tagBarButtonItem = new UIBarButtonItem (new UIImage ("Images/Navigation/TagsIcon.png"), UIBarButtonItemStyle.Plain, delegate(object sender, EventArgs e) {
				UIStoryboard storyboard = UIStoryboard.FromName ("PopoverTagManage", NSBundle.MainBundle);
 				UserTagTableViewController userTagVC = (UserTagTableViewController)storyboard.InstantiateViewController ("NavigationController");
 				UINavigationController navController = new UINavigationController (userTagVC);
   				navController.NavigationBar.BarTintColor = UIColor.White;
				UIPopoverController tagManagePopoverController = new UIPopoverController(navController);
				tagManagePopoverController.BackgroundColor = UIColor.White;
				tagManagePopoverController.SetPopoverContentSize(new CoreGraphics.CGSize(320, 320), true);
				AppDisplayUtil.Instance.SetPopoverController (tagManagePopoverController);

				UIView viewOfBarButtonItem = (UIView)((UIBarButtonItem)sender).ValueForKey (new NSString("view"));
				tagManagePopoverController.PresentFromRect (viewOfBarButtonItem.Frame, viewOfBarButtonItem.Superview, UIPopoverArrowDirection.Up, true);
			});

			NavigationItem.RightBarButtonItems = new UIBarButtonItem[]{TabBarController.NavigationItem.RightBarButtonItem, historyBarButtonItem, tagBarButtonItem};


			TagTableViewController tagTableVC = new TagTableViewController ();
			tagTableVC.TableView = TagTableView;
			tagTableVC.TableView.Source = new TagTableViewSource (TagTableView);
			tagTableVC.TableView.TableFooterView = new UIView ();

			AnnotationTableViewController annotationTableVC = new AnnotationTableViewController ();
			annotationTableVC.TableView = AnnotationTableView;
			annotationTableVC.TableView.Source = new AnnotationTableViewSource (annotationTableVC.TableView);
			annotationTableVC.TableView.TableFooterView = new UIView ();
		}

		partial void AnnotationOnSegmentController (NSObject sender)
		{
			var controller = (UISegmentedControl)sender;
			var selectIndex = controller.SelectedSegment;
			AppDataUtil.Instance.AnnotationSegmentSelectedIndex = (int)selectIndex;
     		NSNotificationCenter.DefaultCenter.PostNotificationName("filterAnnotationInAnnotation", this); 
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			TabBarController.Title = "Annotations";
			TabBarController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("", UIBarButtonItemStyle.Plain, null);
			TabBarController.NavigationItem.RightBarButtonItems = NavigationItem.RightBarButtonItems;
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);

			//Dismiss current popover controller if it exist
			AppDisplayUtil.Instance.DismissPopoverView ();
		}

		public void ShowPopoverRecentHistory (object sender, EventArgs e)
		{
			UINavigationController navController = new UINavigationController (new PopoverHistoryTableViewController ());
			navController.NavigationBar.BarTintColor = UIColor.White;

			UIPopoverController historyPop = new UIPopoverController(navController);
			historyPop.BackgroundColor = UIColor.White;
			AppDisplayUtil.Instance.SetPopoverController (historyPop);

			UIView viewOfBarButtonItem = (UIView)((UIBarButtonItem)sender).ValueForKey (new NSString("view"));
			historyPop.PresentFromRect (viewOfBarButtonItem.Frame, viewOfBarButtonItem.Superview, UIPopoverArrowDirection.Up, true);
		}

	}
}
