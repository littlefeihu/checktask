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
	[Register ("ChangePasswordController")]
	partial class ChangePasswordController
	{
		[Outlet]
		UIKit.UITextField passwordTextField { get; set; }

		[Outlet]
		UIKit.UITextField retypePasswordField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (passwordTextField != null) {
				passwordTextField.Dispose ();
				passwordTextField = null;
			}

			if (retypePasswordField != null) {
				retypePasswordField.Dispose ();
				retypePasswordField = null;
			}
		}
	}
}
