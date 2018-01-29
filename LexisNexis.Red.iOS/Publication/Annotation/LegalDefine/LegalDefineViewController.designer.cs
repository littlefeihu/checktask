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
	[Register ("LegalDefineViewController")]
	partial class LegalDefineViewController
	{
		[Outlet]
		UIKit.UIButton BackButton { get; set; }

		[Outlet]
		UIKit.UIButton ForButton { get; set; }

		[Outlet]
		UIKit.UILabel SelectTitleLabel { get; set; }

		[Outlet]
		UIKit.UIWebView webView { get; set; }

		[Action ("BackButtonClick:")]
		partial void BackButtonClick (Foundation.NSObject sender);

		[Action ("ForButtonClick:")]
		partial void ForButtonClick (Foundation.NSObject sender);

		[Action ("searchWebButton:")]
		partial void searchWebButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BackButton != null) {
				BackButton.Dispose ();
				BackButton = null;
			}

			if (ForButton != null) {
				ForButton.Dispose ();
				ForButton = null;
			}

			if (SelectTitleLabel != null) {
				SelectTitleLabel.Dispose ();
				SelectTitleLabel = null;
			}

			if (webView != null) {
				webView.Dispose ();
				webView = null;
			}
		}
	}
}
