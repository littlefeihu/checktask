
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	[Register ("NoteTextTagViewController")]
	public partial class NoteTextTagViewController : UITableViewController
	{
		public NoteTextTagViewController (IntPtr handle) : base (handle)
		{
		}

		public NoteTextTagViewController () : base (UITableViewStyle.Plain)
		{
		}
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			TableView.ReloadData();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Register the TableView's data source
			TableView.Source = new NoteTextTagViewSource (this.TableView);
			TableView.TableFooterView = new UIView ();
			this.NavigationController.NavigationBar.BarTintColor = UIColor.White;//backgroundView color
			this.NavigationController.NavigationBar.TintColor = UIColor.Red;//backbutton color

		}

		partial void EditTagClick (NSObject sender)
		{
 			UIStoryboard storyboard = UIStoryboard.FromName ("PopoverTagManage", NSBundle.MainBundle);
			UserTagTableViewController userTagVC = (UserTagTableViewController)storyboard.InstantiateViewController ("NavigationController");
			userTagVC.typeString = "NoteTag";

			UINavigationController navController = new UINavigationController (userTagVC);
			navController.NavigationBar.TintColor = UIColor.Red;
			userTagVC.Title = "Edit Tag";
			this.NavigationController.PushViewController (userTagVC, true);
		}

	}
}

