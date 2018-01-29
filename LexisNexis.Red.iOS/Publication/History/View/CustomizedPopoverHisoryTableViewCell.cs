
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public partial class CustomizedPopoverHisoryTableViewCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CustomizedPopoverHisoryTableViewCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CustomizedPopoverHisoryTableViewCell");

		public CustomizedPopoverHisoryTableViewCell (IntPtr handle) : base (handle)
		{
		}

		public static CustomizedPopoverHisoryTableViewCell Create ()
		{
			return (CustomizedPopoverHisoryTableViewCell)Nib.Instantiate (null, null) [0];
		}
	}
}

