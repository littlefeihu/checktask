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
	[Register ("PageContentViewController")]
	partial class PageContentViewController
	{
		[Outlet]
		AppKit.NSMenu ContentMenu { get; set; }

		[Outlet]
		LexisNexis.Red.Mac.PageWebKit ContentWebView { get; set; }

		[Outlet]
		AppKit.NSView CustomView { get; set; }

		[Outlet]
		AppKit.NSTextField InfoLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ContentMenu != null) {
				ContentMenu.Dispose ();
				ContentMenu = null;
			}

			if (ContentWebView != null) {
				ContentWebView.Dispose ();
				ContentWebView = null;
			}

			if (CustomView != null) {
				CustomView.Dispose ();
				CustomView = null;
			}

			if (InfoLabel != null) {
				InfoLabel.Dispose ();
				InfoLabel = null;
			}
		}
	}
}
