
using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	public class HistoryTableViewController : UITableViewController
	{
	

		public HistoryTableViewController () : base (UITableViewStyle.Plain)
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
			Title = "Recent History";
//			TableView.Source = new HistoryTableViewSource (this.TableView);
 			// Register the TableView's data source

		}

	}
}

