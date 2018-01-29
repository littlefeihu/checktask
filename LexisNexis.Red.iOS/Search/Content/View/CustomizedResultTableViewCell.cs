
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public partial class CustomizedResultTableViewCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CustomizedResultTableViewCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CustomizedResultTableViewCell");

		public CustomizedResultTableViewCell (IntPtr handle) : base (handle)
		{
		}

		public static CustomizedResultTableViewCell Create ()
		{
			return (CustomizedResultTableViewCell)Nib.Instantiate (null, null) [0];
		}
	}
}

