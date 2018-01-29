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
	[Register ("SearchResultsController")]
	partial class SearchResultsController
	{
		[Outlet]
		AppKit.NSPopUpButton FilterComboBox { get; set; }

		[Outlet]
		AppKit.NSTextField NoResultLabel { get; set; }

		[Outlet]
		AppKit.NSTextField ResultLabel { get; set; }

		[Outlet]
		AppKit.NSTableView SearchTableView { get; set; }

		[Action ("PopupButtonSelectChange:")]
		partial void PopupButtonSelectChange (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (FilterComboBox != null) {
				FilterComboBox.Dispose ();
				FilterComboBox = null;
			}

			if (NoResultLabel != null) {
				NoResultLabel.Dispose ();
				NoResultLabel = null;
			}

			if (ResultLabel != null) {
				ResultLabel.Dispose ();
				ResultLabel = null;
			}

			if (SearchTableView != null) {
				SearchTableView.Dispose ();
				SearchTableView = null;
			}
		}
	}
}
