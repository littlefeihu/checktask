
using System;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.iOS
{
	public class SettingListController : UITableViewController
	{
		public SettingListController () : base (UITableViewStyle.Plain)
		{
			Title = "Settings";

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
			TableView.BackgroundColor = UIColor.White;
			
			// Register the TableView's data source
			TableView.Source = new SettingListSource (this);
			TableView.SectionHeaderHeight = 0;
			TableView.RowHeight = 44;


		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			TableView.ScrollEnabled = false;
		}
	}
}

