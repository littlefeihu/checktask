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
	[Register ("LNRedAboutPanelController")]
	partial class LNRedAboutPanelController
	{
		[Outlet]
		AppKit.NSTextField AboutTitle { get; set; }

		[Outlet]
		AppKit.NSTextView ContentTextView { get; set; }

		[Outlet]
		AppKit.NSTextField LastSyncTime { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ContentTextView != null) {
				ContentTextView.Dispose ();
				ContentTextView = null;
			}

			if (AboutTitle != null) {
				AboutTitle.Dispose ();
				AboutTitle = null;
			}

			if (LastSyncTime != null) {
				LastSyncTime.Dispose ();
				LastSyncTime = null;
			}
		}
	}

	[Register ("LNRedAboutPanel")]
	partial class LNRedAboutPanel
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
