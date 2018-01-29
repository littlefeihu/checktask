
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class DocTypeFilterTableViewController : UITableViewController
	{
		public DocTypeFilterTableViewController () : base (UITableViewStyle.Plain)
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
			TableView.Source = new DocTypeFilterTableViewSource ();

			TableView.TableFooterView = new UIView ();
			TableView.TintColor = UIColor.Red;
		}
	}
}

