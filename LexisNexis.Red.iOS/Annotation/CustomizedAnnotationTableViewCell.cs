
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public partial class CustomizedAnnotationTableViewCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CustomizedAnnotationTableViewCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CustomizedAnnotationTableViewCell");

		public CustomizedAnnotationTableViewCell (IntPtr handle) : base (handle)
		{
		}

		public static CustomizedAnnotationTableViewCell Create ()
		{
			return (CustomizedAnnotationTableViewCell)Nib.Instantiate (null, null) [0];
		}
	}
}

