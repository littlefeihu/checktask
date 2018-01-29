
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public partial class CustomizedTableOfContentCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CustomizedTableOfContentCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CustomizedTableOfContentCell");

		public CustomizedTableOfContentCell (IntPtr handle) : base (handle)
		{
		}

		public static CustomizedTableOfContentCell Create ()
		{
			return (CustomizedTableOfContentCell)Nib.Instantiate (null, null) [0];
		}


	}
}

