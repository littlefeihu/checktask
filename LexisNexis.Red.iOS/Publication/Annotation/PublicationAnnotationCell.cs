
using System;

using Foundation;
using UIKit;
 
namespace LexisNexis.Red.iOS
{
	public partial class PublicationAnnotationCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("PublicationAnnotationCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("PublicationAnnotationCell");
  		public PublicationAnnotationCell (IntPtr handle) : base (handle)
		{
		}
		public static PublicationAnnotationCell Create ()
		{
			return (PublicationAnnotationCell)Nib.Instantiate (null, null) [0];
		}
	}
}

