
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public partial class SettingTableViewItem : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("SettingTableViewItem", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("SettingTableViewItem");

		public SettingTableViewItem (IntPtr handle) : base (handle)
		{
		}

		public static SettingTableViewItem Create ()
		{
			return (SettingTableViewItem)Nib.Instantiate (null, null) [0];
		}
	}
}

