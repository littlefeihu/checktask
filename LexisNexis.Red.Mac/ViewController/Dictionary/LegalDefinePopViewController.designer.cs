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
	[Register ("LegalDefinePopViewController")]
	partial class LegalDefinePopViewController
	{
		[Outlet]
		AppKit.NSButton DictioaryButton { get; set; }

		[Outlet]
		WebKit.WebView DictionaryWebView { get; set; }

		[Outlet]
		AppKit.NSButton NextButton { get; set; }

		[Outlet]
		AppKit.NSButton PreButton { get; set; }

		[Outlet]
		AppKit.NSTextField TermTF { get; set; }

		[Outlet]
		AppKit.NSButton WikiButton { get; set; }

		[Action ("DictButtonClick:")]
		partial void DictButtonClick (Foundation.NSObject sender);

		[Action ("NextButtonClick:")]
		partial void NextButtonClick (Foundation.NSObject sender);

		[Action ("PreButtonClick:")]
		partial void PreButtonClick (Foundation.NSObject sender);

		[Action ("WikiButtonClick:")]
		partial void WikiButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (TermTF != null) {
				TermTF.Dispose ();
				TermTF = null;
			}

			if (PreButton != null) {
				PreButton.Dispose ();
				PreButton = null;
			}

			if (NextButton != null) {
				NextButton.Dispose ();
				NextButton = null;
			}

			if (DictionaryWebView != null) {
				DictionaryWebView.Dispose ();
				DictionaryWebView = null;
			}

			if (DictioaryButton != null) {
				DictioaryButton.Dispose ();
				DictioaryButton = null;
			}

			if (WikiButton != null) {
				WikiButton.Dispose ();
				WikiButton = null;
			}
		}
	}
}
