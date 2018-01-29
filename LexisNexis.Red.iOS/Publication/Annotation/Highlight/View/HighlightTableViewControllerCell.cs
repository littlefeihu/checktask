
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public partial class HighlightTableViewControllerCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("HighlightTableViewControllerCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("HighlightTableViewControllerCell");

		public HighlightTableViewControllerCell (IntPtr handle) : base (handle)
		{
		}

		public static HighlightTableViewControllerCell Create ()
		{
			return (HighlightTableViewControllerCell)Nib.Instantiate (null, null) [0];
		}
	}
}

