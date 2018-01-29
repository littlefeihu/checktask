using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace LexisNexis.Red.iOS
{
	partial class HomeTabBarController : UITabBarController
	{
		public HomeTabBarController (IntPtr handle) : base (handle)
		{
			Title = "Publications";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TabBar.TintColor = UIColor.Red;

			//Set text color of navigation bar as red
			NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes (){ ForegroundColor = UIColor.Red };
			//UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment (new UIOffset(0, -600), UIBarMetrics.Default); 
			NavigationItem.Title = "Publications";

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = false;
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);

			//Dismiss current popover controller if it exist
			AppDisplayUtil.Instance.DismissPopoverView ();
		}

		partial void OpenSettingPanel (Foundation.NSObject sender)
		{
 			SettingListController settingListVC = new SettingListController();
			UINavigationController navController = new UINavigationController (settingListVC);
			navController.NavigationBar.TintColor = UIColor.Red;
			navController.View.BackgroundColor = UIColor.White;
			navController.NavigationBar.BarTintColor = UIColor.White;

			UIPopoverController settingPop = new UIPopoverController(navController);
			settingPop.SetPopoverContentSize(new CoreGraphics.CGSize(320, 265), true);
			settingPop.BackgroundColor = UIColor.White;

 			AppDisplayUtil.Instance.SetPopoverController(settingPop);

			UIView viewOfBarButtonItem = (UIView)((UIBarButtonItem)sender).ValueForKey (new NSString("view"));
			settingPop.PresentFromRect (viewOfBarButtonItem.Frame, viewOfBarButtonItem.Superview, UIPopoverArrowDirection.Up, true);
		}
	}
}
