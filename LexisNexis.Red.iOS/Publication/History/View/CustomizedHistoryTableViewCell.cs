
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public partial class CustomizedHistoryTableViewCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CustomizedHistoryTableViewCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CustomizedHistoryTableViewCell");

		public CustomizedHistoryTableViewCell (IntPtr handle) : base (handle)
		{
		}

		public static CustomizedHistoryTableViewCell Create ()
		{
			return (CustomizedHistoryTableViewCell)Nib.Instantiate (null, null) [0];
		}
	}
}

