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
	[Register ("PreferenceWindowController")]
	partial class PreferenceWindowController
	{
		[Outlet]
		AppKit.NSButton autoLauchCheckbox { get; set; }

		[Outlet]
		AppKit.NSButton autoLoginCheckbox { get; set; }

		[Outlet]
		AppKit.NSButton changePwdButton { get; set; }

		[Outlet]
		AppKit.NSImageView imageView { get; set; }

		[Outlet]
		AppKit.NSButton logoutButton { get; set; }

		[Outlet]
		AppKit.NSWindow preferenceWindow { get; set; }

		[Outlet]
		AppKit.NSTabView tabView { get; set; }

		[Outlet]
		AppKit.NSTextField userMailLabel { get; set; }

		[Outlet]
		AppKit.NSTextField userMailTF { get; set; }

		[Outlet]
		AppKit.NSTextField userPwdLabel { get; set; }

		[Outlet]
		AppKit.NSTextField userPwdTF { get; set; }

		[Action ("changeButtonClick:")]
		partial void changeButtonClick (NSObject sender);

		[Action ("logoutButtonClick:")]
		partial void logoutButtonClick (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (preferenceWindow != null) {
				preferenceWindow.Dispose ();
				preferenceWindow = null;
			}

			if (tabView != null) {
				tabView.Dispose ();
				tabView = null;
			}

			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (userMailLabel != null) {
				userMailLabel.Dispose ();
				userMailLabel = null;
			}

			if (userPwdLabel != null) {
				userPwdLabel.Dispose ();
				userPwdLabel = null;
			}

			if (userMailTF != null) {
				userMailTF.Dispose ();
				userMailTF = null;
			}

			if (userPwdTF != null) {
				userPwdTF.Dispose ();
				userPwdTF = null;
			}

			if (changePwdButton != null) {
				changePwdButton.Dispose ();
				changePwdButton = null;
			}

			if (autoLauchCheckbox != null) {
				autoLauchCheckbox.Dispose ();
				autoLauchCheckbox = null;
			}

			if (autoLoginCheckbox != null) {
				autoLoginCheckbox.Dispose ();
				autoLoginCheckbox = null;
			}

			if (logoutButton != null) {
				logoutButton.Dispose ();
				logoutButton = null;
			}
		}
	}

	[Register ("PreferenceWindow")]
	partial class PreferenceWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
