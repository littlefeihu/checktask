using System;

using Foundation;
using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.iOS
{
	public class AboutListSource : UITableViewSource
	{
		private UIViewController curViewController;

		public AboutListSource (UIViewController controller)
		{
			curViewController = controller;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return 3;
		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (AboutListCell.Key) as AboutListCell;
			if (cell == null)
				cell = new AboutListCell ();
			
			// TODO: populate the cell with the appropriate data based on the indexPath
			switch (indexPath.LongRow) {
			case 0:
				cell.TextLabel.Text = "LexisNexis Red";
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				break;
			case 1:
				cell.TextLabel.Text = "LexisNexis Legal and Professional";
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				break;
			case 2:
				cell.TextLabel.Text = "Terms and Conditions";
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				break;
			}
			
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			UIViewController aboutContentVC = new UIViewController ();
			aboutContentVC.Title = "About";

			AppDisplayUtil.Instance.SetCurrentPopoverViewSize (new CGSize (320, 465));
			aboutContentVC.View.Frame = new CGRect (0, 0, 320, 465);

			UIWebView webView = new UIWebView ();
			webView.Frame = curViewController.View.Frame;
			webView.BackgroundColor = UIColor.White;
			aboutContentVC.View.AddSubview (webView);

			string aboutContent = "";
			switch (indexPath.LongRow) {
			case 0://About LexisNexis Red
				aboutContent = SettingsUtil.Instance.GetLexisNexisRedInfo ();
				break;
			case 1://About LexisNexis Legal and Professional
				aboutContent = SettingsUtil.Instance.GetLexisNexisInfo ();
				break;
			case 2://Terms and Conditions
				aboutContent = SettingsUtil.Instance.GetTermsAndConditions ();
				break;
			}
			webView.LoadHtmlString (aboutContent, NSUrl.FromString (""));
		
			curViewController.NavigationController.PushViewController (aboutContentVC, true);
		}
	}
}

