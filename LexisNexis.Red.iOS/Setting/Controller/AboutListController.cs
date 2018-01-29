using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public class AboutListController : UITableViewController
	{
		public AboutListController () : base (UITableViewStyle.Plain)
		{
			Title = "About";
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Register the TableView's data source
			TableView.Source = new AboutListSource (this);
			TableView.ScrollEnabled = false;
			TableView.TableFooterView = new UIView ();

		}
		 
		public override void ViewWillAppear (bool animated)
		{
			AppDisplayUtil.Instance.SetCurrentPopoverViewSize (new CGSize (320, 265));
			TableView.ReloadData ();
		}


	}
}

