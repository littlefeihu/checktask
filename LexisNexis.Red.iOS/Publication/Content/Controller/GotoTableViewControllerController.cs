
using System;

using Foundation;
using UIKit;
 

namespace LexisNexis.Red.iOS
{
	[Register ("GotoTableViewControllerController")]
	public partial class GotoTableViewControllerController : UITableViewController
	{ 
 
		public GotoTableViewControllerController (IntPtr handle) : base (handle)
		{
		}
		public GotoTableViewControllerController () : base (UITableViewStyle.Plain)
		{
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
			TableView.Source = new GotoTableViewControllerSource (this.TableView);
			TableView.BackgroundColor = UIColor.White;
 			TableView.TableFooterView = new UIView ();
  		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
 		} 		 
	}
}

