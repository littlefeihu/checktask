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
	[Register ("IndexViewController")]
	partial class IndexViewController
	{
		[Outlet]
		AppKit.NSTextField IndexInfoLabel { get; set; }

		[Outlet]
		AppKit.NSTextField IndexLabel { get; set; }

		[Outlet]
		AppKit.NSOutlineView IndexOutlineView { get; set; }

		[Outlet]
		AppKit.NSView InfoCustomView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (IndexLabel != null) {
				IndexLabel.Dispose ();
				IndexLabel = null;
			}

			if (IndexInfoLabel != null) {
				IndexInfoLabel.Dispose ();
				IndexInfoLabel = null;
			}

			if (InfoCustomView != null) {
				InfoCustomView.Dispose ();
				InfoCustomView = null;
			}

			if (IndexOutlineView != null) {
				IndexOutlineView.Dispose ();
				IndexOutlineView = null;
			}
		}
	}
}
