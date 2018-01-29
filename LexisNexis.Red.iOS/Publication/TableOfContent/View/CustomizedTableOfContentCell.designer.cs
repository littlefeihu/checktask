// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace LexisNexis.Red.iOS
{
	[Register ("CustomizedTableOfContentCell")]
	partial class CustomizedTableOfContentCell
	{
		[Outlet]
		public UIKit.UIView LeftColorSideBar { get; set; }

		[Outlet]
		public UIKit.UILabel NameLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LeftColorSideBar != null) {
				LeftColorSideBar.Dispose ();
				LeftColorSideBar = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}
		}
	}
}
