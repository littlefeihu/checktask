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
	[Register ("ContactUsViewController")]
	partial class ContactUsViewController
	{
		[Outlet]
		UIKit.UILabel DateLabel { get; set; }

		[Outlet]
		UIKit.UITextView EmailTextView { get; set; }

		[Outlet]
		UIKit.UILabel FaxLabel { get; set; }

		[Outlet]
		UIKit.UILabel InternationalTelLabel { get; set; }

		[Outlet]
		UIKit.UITextView PostToUsTextView { get; set; }

		[Outlet]
		UIKit.UITextView SendByDXTextView { get; set; }

		[Outlet]
		UIKit.UILabel SendDXLabel { get; set; }

		[Outlet]
		UIKit.UILabel TelLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TelLabel != null) {
				TelLabel.Dispose ();
				TelLabel = null;
			}

			if (DateLabel != null) {
				DateLabel.Dispose ();
				DateLabel = null;
			}

			if (InternationalTelLabel != null) {
				InternationalTelLabel.Dispose ();
				InternationalTelLabel = null;
			}

			if (FaxLabel != null) {
				FaxLabel.Dispose ();
				FaxLabel = null;
			}

			if (EmailTextView != null) {
				EmailTextView.Dispose ();
				EmailTextView = null;
			}

			if (PostToUsTextView != null) {
				PostToUsTextView.Dispose ();
				PostToUsTextView = null;
			}

			if (SendByDXTextView != null) {
				SendByDXTextView.Dispose ();
				SendByDXTextView = null;
			}

			if (SendDXLabel != null) {
				SendDXLabel.Dispose ();
				SendDXLabel = null;
			}
		}
	}
}
