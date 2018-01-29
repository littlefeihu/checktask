
using System;

using Foundation;
using UIKit;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	public partial class CustomizeTagTableViewCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("CustomizeTagTableViewCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("CustomizeTagTableViewCell");
		public AnnotationTag annotationTagId{ get; set;}

		public CustomizeTagTableViewCell (IntPtr handle) : base (handle)
		{
		}

		public static CustomizeTagTableViewCell Create ()
		{
			return (CustomizeTagTableViewCell)Nib.Instantiate (null, null) [0];
		}
	}
}

