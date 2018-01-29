using System;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Business;


namespace LexisNexis.Red.iOS
{
	public class SettingListSource : UITableViewSource
	{

		private UIViewController curViewController;

		public SettingListSource (UIViewController controller)
		{
			curViewController = controller;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			//return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			//return the actual number of items in the section
			return 4;
		}



		public override string TitleForFooter (UITableView tableView, nint section)
		{
			return "Latest server sync " + ((DateTime)SettingsUtil.Instance.GetLastSyncedTime()).ToString("hh:mm t\\M, dd MMM yyyy ");
		}



		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			SettingTableViewItem cell = tableView.DequeueReusableCell (SettingTableViewItem.Key) as SettingTableViewItem;
			if (cell == null)
				cell = SettingTableViewItem.Create();
			
			//populate the cell with the appropriate data based on the indexPath
			switch (indexPath.LongRow) {
			case 0:
				cell.SettingNameLabel.Text = "About";
				cell.SettingItemImage.Image = new UIImage("Images/Setting/About.png");
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				break;
			case 1:
				cell.SettingNameLabel.Text = "Publication Text";
				cell.SettingItemImage.Image = new UIImage ("Images/Setting/Text.png");
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				break;
			case 2:
				cell.SettingNameLabel.Text = "Help";
				cell.SettingItemImage.Image = new UIImage ("Images/Setting/Help.png");
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				break;
			case 3:
				cell.SettingNameLabel.Text = "Log Out";
				cell.SettingDescLabel.Text = GlobalAccess.Instance.CurrentUserInfo.FirstName;
				cell.SettingItemImage.Image = new UIImage ("Images/Setting/Account.png");
				break;
			}

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			
			switch (indexPath.LongRow) {
			case 0://About
				AboutListController aboutListController = new AboutListController ();
				curViewController.NavigationController.PushViewController (aboutListController, true);
				break;
			case 1://Setting publication text
				PublicationTextSettingController publicationSettingController = new PublicationTextSettingController();
				curViewController.NavigationController.PushViewController (publicationSettingController, true);
				break;
			case 2://Help
				HelpListController helpListController = new HelpListController();
				curViewController.NavigationController.PushViewController (helpListController, true);
				break;
			case 3://Logout
				var logoutAlert = UIAlertController.Create ("Log Out", "Thank you for using LexisNexis Red application. Are you sure you want to exit ?", UIAlertControllerStyle.Alert);
				logoutAlert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));
				logoutAlert.AddAction (UIAlertAction.Create ("Confirm", UIAlertActionStyle.Default, action => ConfirmLogout ()));
				curViewController.PresentViewController (logoutAlert, true, null);
				tableView.CellAt (indexPath).Selected = false;
				break;
			}
		}

		void ConfirmLogout ()
		{
			LoginUtil.Instance.Logout();
			curViewController.PresentingViewController.DismissViewController(false, null);

			//Go back to login view
			AppDisplayUtil.Instance.GoToLoginView ();

		}


		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return new nfloat(0);
		}


		public override UIView GetViewForFooter (UITableView tableView, nint section)
		{
			UIView view = new UIView (new CoreGraphics.CGRect (0, 0, 320, 44));

			UIView bottomLine = new UIView (new CoreGraphics.CGRect (0, 0, 320, 0.5));
			bottomLine.BackgroundColor = UIColor.FromRGB (188,186, 193);

			UILabel syncDateLabel = new UILabel (new CoreGraphics.CGRect (0, 10, 320, 20));
			syncDateLabel.Text = "Latest server sync " + ((DateTime)SettingsUtil.Instance.GetLastSyncedTime()).ToString("hh:mm t\\M, dd MMM yyyy ");
			syncDateLabel.Font = UIFont.SystemFontOfSize (13);
			syncDateLabel.TextColor = ColorUtil.ConvertFromHexColorCode ("#6d6d72");
			syncDateLabel.TextAlignment = UITextAlignment.Center;

			view.AddSubview (bottomLine);
			view.AddSubview (syncDateLabel);

			return view;
		}
	}
}

