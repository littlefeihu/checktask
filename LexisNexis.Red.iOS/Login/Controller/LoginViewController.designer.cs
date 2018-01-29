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
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		UIKit.UITextField emailTextField { get; set; }

		[Outlet]
		UIKit.UIView LoginFormContainerView { get; set; }

		[Outlet]
		UIKit.UITextField passwordTextField { get; set; }

		[Outlet]
		UIKit.UIButton regionDropdownBtn { get; set; }

		[Outlet]
		UIKit.UITextField selectedRegionTextField { get; set; }

		[Action ("EmailDidEndOnExit:")]
		partial void EmailDidEndOnExit (Foundation.NSObject sender);

		[Action ("PasswordDidEndOnExit:")]
		partial void PasswordDidEndOnExit (Foundation.NSObject sender);

		[Action ("ShowRegionsSelectorTableView:")]
		partial void ShowRegionsSelectorTableView (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (emailTextField != null) {
				emailTextField.Dispose ();
				emailTextField = null;
			}

			if (LoginFormContainerView != null) {
				LoginFormContainerView.Dispose ();
				LoginFormContainerView = null;
			}

			if (passwordTextField != null) {
				passwordTextField.Dispose ();
				passwordTextField = null;
			}

			if (regionDropdownBtn != null) {
				regionDropdownBtn.Dispose ();
				regionDropdownBtn = null;
			}

			if (selectedRegionTextField != null) {
				selectedRegionTextField.Dispose ();
				selectedRegionTextField = null;
			}
		}
	}
}
