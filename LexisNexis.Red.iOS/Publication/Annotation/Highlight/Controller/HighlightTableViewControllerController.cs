
using System;

using Foundation;
using UIKit;
using System.Collections.Generic;

namespace LexisNexis.Red.iOS
{
	public class HighlightTableViewControllerController : UITableViewController
	{
		public	List<Guid> getGuidList;


		public HighlightTableViewControllerController () : base (UITableViewStyle.Plain)
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
			this.Title = "Highlight";
			AppDisplayUtil.Instance.highlightVC = this;
 			getGuidList = new List<Guid>();

			TableView.Source = new HighlightTableViewControllerSource (this,this.TableView);
			TableView.BackgroundColor = UIColor.White;
			TableView.TableFooterView = new UIView ();



			UIBarButtonItem addButton = new UIBarButtonItem ("Add Note",UIBarButtonItemStyle.Plain,addNoteButttonCLick);
			addButton.TintColor = UIColor.Red;
			this.NavigationItem.LeftBarButtonItem = addButton;


			UIBarButtonItem deleteButton = new UIBarButtonItem ("Delete",UIBarButtonItemStyle.Plain,deleteButttonCLick);
			deleteButton.TintColor = UIColor.Red;
			this.NavigationItem.RightBarButtonItem = deleteButton;
		}

		public void deleteButttonCLick(object o, EventArgs e){
			AppDisplayUtil.Instance.DismissPopoverView ();
		}

		public void addNoteButttonCLick(object o,EventArgs e){
			NSNotificationCenter.DefaultCenter.PostNotificationName("addNoteTableView", this);
			AppDisplayUtil.Instance.DismissPopoverView ();

		}
	}
}

