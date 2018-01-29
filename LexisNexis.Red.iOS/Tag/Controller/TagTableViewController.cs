
using System;

using Foundation;
using UIKit;


namespace LexisNexis.Red.iOS
{
	public class TagTableViewController : UITableViewController
	{
		public TagTableViewController () : base (UITableViewStyle.Plain)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			// Register the TableView's data source
			TableView.Source = new TagTableViewSource (this.TableView);

			//hidden redundant line separator
			TableView.TableFooterView = new UIView ();

			TableView.TintColor = UIColor.Red;
		}

	}
}

