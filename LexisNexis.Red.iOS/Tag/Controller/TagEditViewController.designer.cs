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
	[Register ("TagEditViewController")]
	partial class TagEditViewController
	{
		[Outlet]
		UIKit.UIView line2View { get; set; }

		[Outlet]
		UIKit.UIView lineView { get; set; }

		[Outlet]
		public UIKit.UITextField TagNameTextField { get; private set; }

		[Action ("TagEditDone:")]
		partial void TagEditDone (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (line2View != null) {
				line2View.Dispose ();
				line2View = null;
			}

			if (lineView != null) {
				lineView.Dispose ();
				lineView = null;
			}

			if (TagNameTextField != null) {
				TagNameTextField.Dispose ();
				TagNameTextField = null;
			}
		}
	}
}
