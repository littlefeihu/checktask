using System;
using System.Collections;
using System.Collections.Generic;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Services;

namespace LexisNexis.Red.iOS
{
	public class RegionsTableViewSource : UITableViewSource
	{

		private List<Country> regions;

		SetTextFieldText setSelectedRegion;

		public string SelectedRegionName{ get; set;}

		public RegionsTableViewSource (SetTextFieldText setTextFiledAction, string selectedRegionName = "" )
		{
			regions = ConfigurationService.GetAllCountryMap ();
			setSelectedRegion = setTextFiledAction;
			SelectedRegionName = selectedRegionName;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return regions == null ? 0 : regions.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (RegionsTableViewCell.Key) as RegionsTableViewCell;
			if (cell == null)
				cell = new RegionsTableViewCell ();
			
			//populate the cell with the appropriate data based on the indexPath

			cell.TextLabel.Font = UIFont.SystemFontOfSize (15);
			cell.TextLabel.TextColor = UIColor.DarkGray;

			cell.TextLabel.Text = regions [indexPath.Row].CountryName;
			cell.ImageView.Image = new UIImage (string.Format("Images/NationalFlag/{0}.png", regions [indexPath.Row].CountryCode));

			if (SelectedRegionName == regions [indexPath.Row].CountryName) {
				cell.Accessory = UITableViewCellAccessory.Checkmark;
			}
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			setSelectedRegion (regions [indexPath.Row].CountryName);
			AppDisplayUtil.Instance.DismissPopoverView();
		}
	}
}

