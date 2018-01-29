 
using System;

using Foundation;
using UIKit;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	public partial class NoteTextTagViewCell : UITableViewCell
	{
		public static readonly UINib Nib = UINib.FromName ("NoteTextTagViewCell", NSBundle.MainBundle);
		public static readonly NSString Key = new NSString ("NoteTextTagViewCell");

 		public NoteTextTagViewCell (IntPtr handle) : base (handle)
		{
		}

		public static NoteTextTagViewCell Create ()
		{
			return (NoteTextTagViewCell)Nib.Instantiate (null, null) [0];
		}
	}
}

