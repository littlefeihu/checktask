
using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public class HelpListController : UITableViewController
	{
		public HelpListController () : base (UITableViewStyle.Plain)
		{
			Title = "Help";
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
			TableView.Source = new HelpListSource (this);
			TableView.ScrollEnabled = false;
			TableView.TableFooterView = new UIView ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			AppDisplayUtil.Instance.SetCurrentPopoverViewSize (new CGSize (320, 265));

		}
	}
}

