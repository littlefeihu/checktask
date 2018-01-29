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
	[Register ("ContactUsPanelController")]
	partial class ContactUsPanelController
	{
		[Outlet]
		AppKit.NSButton EmailButton { get; set; }

		[Outlet]
		AppKit.NSImageView EmailImage { get; set; }

		[Outlet]
		AppKit.NSTextField EmailTF { get; set; }

		[Outlet]
		AppKit.NSTextField FaxusTF { get; set; }

		[Outlet]
		AppKit.NSTextField InternationalCallerTF { get; set; }

		[Outlet]
		AppKit.NSImageView LocationImage { get; set; }

		[Outlet]
		AppKit.NSTextField PhoneNumberTF { get; set; }

		[Outlet]
		AppKit.NSTextField PostusTF { get; set; }

		[Outlet]
		AppKit.NSTextField SendDX { get; set; }

		[Outlet]
		AppKit.NSImageView TeleImage { get; set; }

		[Action ("EmailButtonClick:")]
		partial void EmailButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (EmailImage != null) {
				EmailImage.Dispose ();
				EmailImage = null;
			}

			if (EmailTF != null) {
				EmailTF.Dispose ();
				EmailTF = null;
			}

			if (FaxusTF != null) {
				FaxusTF.Dispose ();
				FaxusTF = null;
			}

			if (InternationalCallerTF != null) {
				InternationalCallerTF.Dispose ();
				InternationalCallerTF = null;
			}

			if (LocationImage != null) {
				LocationImage.Dispose ();
				LocationImage = null;
			}

			if (PhoneNumberTF != null) {
				PhoneNumberTF.Dispose ();
				PhoneNumberTF = null;
			}

			if (PostusTF != null) {
				PostusTF.Dispose ();
				PostusTF = null;
			}

			if (SendDX != null) {
				SendDX.Dispose ();
				SendDX = null;
			}

			if (TeleImage != null) {
				TeleImage.Dispose ();
				TeleImage = null;
			}

			if (EmailButton != null) {
				EmailButton.Dispose ();
				EmailButton = null;
			}
		}
	}
}
