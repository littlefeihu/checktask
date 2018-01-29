
using System;

using Foundation;
using UIKit;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	[Register ("UserTagTableViewController")]
	public partial class UserTagTableViewController : UITableViewController
	{ 
		public  string typeString{ get; set;}
 
 		public UserTagTableViewController (IntPtr handle) : base (handle)
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
			TableView.Source = new UserTagTableViewSource (this, this.TableView);
			TableView.TableFooterView = new UIView ();
			this.NavigationController.NavigationBar.TintColor = UIColor.Red;//backbutton color
			UIBarButtonItem tempBarButton = new UIBarButtonItem ();
			tempBarButton.Title = "Back";
			this.NavigationItem.BackBarButtonItem = tempBarButton;

			if (typeString == "NoteTag") {
				UIBarButtonItem addButton = new UIBarButtonItem ("Done",UIBarButtonItemStyle.Plain,backBtnClik);
				addButton.TintColor = UIColor.Red;
				this.NavigationItem.LeftBarButtonItem = addButton;
				TableView.SetEditing(true, true);		
			}
		}

		public void backBtnClik(object o, EventArgs e){
			this.NavigationController.PopViewController(true);
		}

		partial void addTagClick (NSObject sender)
		{
			UIStoryboard storyboard = UIStoryboard.FromName ("TagEditstory", NSBundle.MainBundle);
			TagEditViewController tagEditVC = (TagEditViewController)storyboard.InstantiateViewController ("FromTable");
			UINavigationController navController = new UINavigationController (tagEditVC);
			navController.NavigationBar.TintColor = UIColor.Red;
			navController.View.BackgroundColor = UIColor.White;
			tagEditVC.Title = "New Tag";
   			this.NavigationController.PushViewController (tagEditVC, true);
		}



		partial void ChangeEditStatus (Foundation.NSObject sender)
		{
			if(TableView.Editing){
				NavigationItem.LeftBarButtonItem.Title = "Edit";
				TableView.SetEditing(false, true);
			}else{
				NavigationItem.LeftBarButtonItem.Title = "Done";
				TableView.SetEditing(true, true);
			}
		}

	}
}

