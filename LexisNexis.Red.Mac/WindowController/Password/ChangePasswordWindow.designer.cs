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
	[Register ("ChangePasswordWindow")]
	partial class ChangePasswordWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("ChangePasswordWindowController")]
	partial class ChangePasswordWindowController
	{
		[Outlet]
		AppKit.NSView BottomView { get; set; }

		[Outlet]
		AppKit.NSButton cancelButton { get; set; }

		[Outlet]
		AppKit.NSWindow changePasswordWindow { get; set; }

		[Outlet]
		AppKit.NSTextField infoLabel { get; set; }

		[Outlet]
		AppKit.NSImageView LogoImageView { get; set; }

		[Outlet]
		AppKit.NSTextField newerpwdLabel { get; set; }

		[Outlet]
		AppKit.NSTextField newerPwdTF { get; set; }

		[Outlet]
		AppKit.NSTextField retypeNewPwdTF { get; set; }

		[Outlet]
		AppKit.NSTextField retypepwdLabel { get; set; }

		[Outlet]
		AppKit.NSButton sendButton { get; set; }

		[Outlet]
		AppKit.NSView TopView { get; set; }

		[Outlet]
		AppKit.NSTextField warningLabel { get; set; }

		[Action ("CancelButtonClick:")]
		partial void CancelButtonClick (Foundation.NSObject sender);

		[Action ("CloseWindow:")]
		partial void CloseWindow (Foundation.NSObject sender);

		[Action ("SendButtonClick:")]
		partial void SendButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BottomView != null) {
				BottomView.Dispose ();
				BottomView = null;
			}

			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}

			if (changePasswordWindow != null) {
				changePasswordWindow.Dispose ();
				changePasswordWindow = null;
			}

			if (infoLabel != null) {
				infoLabel.Dispose ();
				infoLabel = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}

			if (newerpwdLabel != null) {
				newerpwdLabel.Dispose ();
				newerpwdLabel = null;
			}

			if (newerPwdTF != null) {
				newerPwdTF.Dispose ();
				newerPwdTF = null;
			}

			if (retypeNewPwdTF != null) {
				retypeNewPwdTF.Dispose ();
				retypeNewPwdTF = null;
			}

			if (retypepwdLabel != null) {
				retypepwdLabel.Dispose ();
				retypepwdLabel = null;
			}

			if (sendButton != null) {
				sendButton.Dispose ();
				sendButton = null;
			}

			if (TopView != null) {
				TopView.Dispose ();
				TopView = null;
			}

			if (warningLabel != null) {
				warningLabel.Dispose ();
				warningLabel = null;
			}
		}
	}
}
