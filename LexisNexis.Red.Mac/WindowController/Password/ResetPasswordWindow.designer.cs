
// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace LexisNexis.Red.Mac
{
	[Register ("ResetPasswordWindow")]
	partial class ResetPasswordWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("ResetPasswordWindowController")]
	partial class ResetPasswordWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSImageView BottonImageView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton CancelButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton CloseButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSComboBox CountryCombobox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField CountryLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField EmailLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField EmailTF { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField InfoLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView LogoImageView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton SendButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField ServiceTF { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView TopImageView { get; set; }

		[Action ("CancelButtonClick:")]
		partial void CancelButtonClick (MonoMac.Foundation.NSObject sender);

		[Action ("CloseWindow:")]
		partial void CloseWindow (MonoMac.Foundation.NSObject sender);

		[Action ("SendButtonClick:")]
		partial void SendButtonClick (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

			if (CountryLabel != null) {
				CountryLabel.Dispose ();
				CountryLabel = null;
			}

			if (CountryCombobox != null) {
				CountryCombobox.Dispose ();
				CountryCombobox = null;
			}

			if (EmailLabel != null) {
				EmailLabel.Dispose ();
				EmailLabel = null;
			}

			if (EmailTF != null) {
				EmailTF.Dispose ();
				EmailTF = null;
			}

			if (InfoLabel != null) {
				InfoLabel.Dispose ();
				InfoLabel = null;
			}

			if (ServiceTF != null) {
				ServiceTF.Dispose ();
				ServiceTF = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (SendButton != null) {
				SendButton.Dispose ();
				SendButton = null;
			}

			if (TopImageView != null) {
				TopImageView.Dispose ();
				TopImageView = null;
			}

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}

			if (BottonImageView != null) {
				BottonImageView.Dispose ();
				BottonImageView = null;
			}
		}
	}
}
