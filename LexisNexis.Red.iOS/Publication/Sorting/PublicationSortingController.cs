
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class PublicationSortingController : UITableViewController
	{
		public MyPublicationViewController CurPublicationListController { get; set;}

		public PublicationSortingController () : base (UITableViewStyle.Grouped)
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
			TableView.Source = new PublicationSortingSource (this);
		}

		public void ReloadPublicationList ()
		{
			CurPublicationListController.ReloadPublicationList();
		}




	}
}

