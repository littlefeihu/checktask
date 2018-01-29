
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class RegionsTableViewController : UITableViewController
	{
		SetTextFieldText setSelectedRegion;

		public string SelectedRegionName{ get; set;}

		public RegionsTableViewController (SetTextFieldText setTextFiledAction, string selectedRegionName = "") : base (UITableViewStyle.Plain)
		{
			setSelectedRegion = setTextFiledAction;
			SelectedRegionName = selectedRegionName;
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
			TableView.Source = new RegionsTableViewSource (setSelectedRegion, SelectedRegionName);
			TableView.TableFooterView = new UIView ();

		}
	}
}

