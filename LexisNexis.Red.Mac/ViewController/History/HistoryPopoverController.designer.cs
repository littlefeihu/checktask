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
	[Register ("HistoryPopoverController")]
	partial class HistoryPopoverController
	{
		[Outlet]
		AppKit.NSTextField HistoryLabel { get; set; }

		[Outlet]
		AppKit.NSTableView HistoryTableView { get; set; }

		[Outlet]
		AppKit.NSTextField NoHistoryLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HistoryLabel != null) {
				HistoryLabel.Dispose ();
				HistoryLabel = null;
			}

			if (HistoryTableView != null) {
				HistoryTableView.Dispose ();
				HistoryTableView = null;
			}

			if (NoHistoryLabel != null) {
				NoHistoryLabel.Dispose ();
				NoHistoryLabel = null;
			}
		}
	}
}
