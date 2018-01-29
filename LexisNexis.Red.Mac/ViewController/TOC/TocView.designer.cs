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
	[Register ("TocViewController")]
	partial class TocViewController
	{
		[Outlet]
		AppKit.NSView ExpiredInfoView { get; set; }

		[Outlet]
		AppKit.NSTableView TocTableView { get; set; }

		[Action ("ExpandButtonClick:")]
		partial void ExpandButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (TocTableView != null) {
				TocTableView.Dispose ();
				TocTableView = null;
			}

			if (ExpiredInfoView != null) {
				ExpiredInfoView.Dispose ();
				ExpiredInfoView = null;
			}
		}
	}

	[Register ("TocView")]
	partial class TocView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
