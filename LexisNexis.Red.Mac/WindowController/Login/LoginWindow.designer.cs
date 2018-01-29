// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace LexisNexis.Red.Mac
{
	[Register ("LoginWindowController")]
	partial class LoginWindowController
	{
		[Outlet]
		AppKit.NSView BottomView { get; set; }

		[Outlet]
		AppKit.NSPopUpButton comboBox { get; set; }

		[Outlet]
		AppKit.NSImageView imageView { get; set; }

		[Outlet]
		AppKit.NSButton loginButton { get; set; }

		[Outlet]
		AppKit.NSImageView LogoImageView { get; set; }

		[Outlet]
		AppKit.NSWindow mainWindow { get; set; }

		[Outlet]
		AppKit.NSButton resetButton { get; set; }

		[Outlet]
		AppKit.NSView TopView { get; set; }

		[Outlet]
		AppKit.NSTextField userMail { get; set; }

		[Outlet]
		AppKit.NSTextField userPassword { get; set; }

		[Action ("CloseLoginWindow:")]
		partial void CloseLoginWindow (Foundation.NSObject sender);

		[Action ("ForgetButtonClick:")]
		partial void ForgetButtonClick (Foundation.NSObject sender);

		[Action ("PopupButtonSelectChange:")]
		partial void PopupButtonSelectChange (Foundation.NSObject sender);

		[Action ("RegisterButtonClick:")]
		partial void RegisterButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BottomView != null) {
				BottomView.Dispose ();
				BottomView = null;
			}

			if (comboBox != null) {
				comboBox.Dispose ();
				comboBox = null;
			}

			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (loginButton != null) {
				loginButton.Dispose ();
				loginButton = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}

			if (mainWindow != null) {
				mainWindow.Dispose ();
				mainWindow = null;
			}

			if (resetButton != null) {
				resetButton.Dispose ();
				resetButton = null;
			}

			if (TopView != null) {
				TopView.Dispose ();
				TopView = null;
			}

			if (userMail != null) {
				userMail.Dispose ();
				userMail = null;
			}

			if (userPassword != null) {
				userPassword.Dispose ();
				userPassword = null;
			}
		}
	}

	[Register ("LoginWindow")]
	partial class LoginWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
