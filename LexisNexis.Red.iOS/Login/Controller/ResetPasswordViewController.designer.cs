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
	[Register ("ResetPasswordViewController")]
	partial class ResetPasswordViewController
	{
		[Outlet]
		UIKit.UIView CustomizedNavigationBarView { get; set; }

		[Outlet]
		UIKit.UITextField EmailTextField { get; set; }

		[Outlet]
		UIKit.UITextField RegionNameTextField { get; set; }

		[Outlet]
		UIKit.UIView ResetPasswordFormContainerView { get; set; }

		[Action ("SendResetPasswordRequest:")]
		partial void SendResetPasswordRequest (Foundation.NSObject sender);

		[Action ("ShowRegionsSelectorTableView:")]
		partial void ShowRegionsSelectorTableView (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CustomizedNavigationBarView != null) {
				CustomizedNavigationBarView.Dispose ();
				CustomizedNavigationBarView = null;
			}

			if (EmailTextField != null) {
				EmailTextField.Dispose ();
				EmailTextField = null;
			}

			if (RegionNameTextField != null) {
				RegionNameTextField.Dispose ();
				RegionNameTextField = null;
			}

			if (ResetPasswordFormContainerView != null) {
				ResetPasswordFormContainerView.Dispose ();
				ResetPasswordFormContainerView = null;
			}
		}
	}
}
