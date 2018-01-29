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
	[Register ("ForgetPasswordWindowController")]
	partial class ForgetPasswordWindowController
	{
		[Outlet]
		AppKit.NSView BottomView { get; set; }

		[Outlet]
		AppKit.NSButton CancelButton { get; set; }

		[Outlet]
		AppKit.NSPopUpButton CountryCombobox { get; set; }

		[Outlet]
		AppKit.NSTextField CountryLabel { get; set; }

		[Outlet]
		AppKit.NSTextField EmailLabel { get; set; }

		[Outlet]
		AppKit.NSTextField EmailTF { get; set; }

		[Outlet]
		AppKit.NSTextField InfoLabel { get; set; }

		[Outlet]
		AppKit.NSImageView LogoImageView { get; set; }

		[Outlet]
		AppKit.NSButton SendButton { get; set; }

		[Outlet]
		AppKit.NSTextField ServiceTF { get; set; }

		[Outlet]
		AppKit.NSView TopView { get; set; }

		[Action ("CancelButtonClick:")]
		partial void CancelButtonClick (Foundation.NSObject sender);

		[Action ("PopupButtonSelectChange:")]
		partial void PopupButtonSelectChange (Foundation.NSObject sender);

		[Action ("SendButtonClick:")]
		partial void SendButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BottomView != null) {
				BottomView.Dispose ();
				BottomView = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}

			if (CountryCombobox != null) {
				CountryCombobox.Dispose ();
				CountryCombobox = null;
			}

			if (CountryLabel != null) {
				CountryLabel.Dispose ();
				CountryLabel = null;
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

			if (LogoImageView != null) {
				LogoImageView.Dispose ();
				LogoImageView = null;
			}

			if (SendButton != null) {
				SendButton.Dispose ();
				SendButton = null;
			}

			if (ServiceTF != null) {
				ServiceTF.Dispose ();
				ServiceTF = null;
			}

			if (TopView != null) {
				TopView.Dispose ();
				TopView = null;
			}
		}
	}
}
