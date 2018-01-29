
using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public class HelpListSource : UITableViewSource
	{
		private UIViewController curViewController;
		public HelpListSource (UIViewController vc)
		{
			curViewController = vc;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			return 3;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (HelpListCell.Key) as HelpListCell;
			if (cell == null)
				cell = new HelpListCell ();
			
			// TODO: populate the cell with the appropriate data based on the indexPath
			switch (indexPath.Row) {
			case 0:
				cell.TextLabel.Text = "View tour";
				break;
			case 1:
				cell.TextLabel.Text = "Contact us";
				break;
			case 2:
				cell.TextLabel.Text = "FAQS";
				break;
			}

			cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			switch (indexPath.Row) {
			case 1:
				ContactUsViewController csVC = new ContactUsViewController ();
				curViewController.NavigationController.PushViewController (csVC, true);
				break;
			}


		}
	}
}

