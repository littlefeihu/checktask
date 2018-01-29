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
	[Register ("GoPagePopViewController")]
	partial class GoPagePopViewController
	{
		[Outlet]
		AppKit.NSView bgView { get; set; }

		[Outlet]
		AppKit.NSTextField NoResultLabel { get; set; }

		[Outlet]
		AppKit.NSSearchField SearchField { get; set; }

		[Outlet]
		AppKit.NSTableView SearchTableView { get; set; }

		[Action ("SearchNumber:")]
		partial void SearchNumber (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (bgView != null) {
				bgView.Dispose ();
				bgView = null;
			}

			if (NoResultLabel != null) {
				NoResultLabel.Dispose ();
				NoResultLabel = null;
			}

			if (SearchField != null) {
				SearchField.Dispose ();
				SearchField = null;
			}

			if (SearchTableView != null) {
				SearchTableView.Dispose ();
				SearchTableView = null;
			}
		}
	}
}
