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
	[Register ("EditTagPanelController")]
	partial class EditTagPanelController
	{
		[Outlet]
		AppKit.NSButton AddButton { get; set; }

		[Outlet]
		AppKit.NSButton DoneButton { get; set; }

		[Outlet]
		AppKit.NSTextField OrgTagLabel { get; set; }

		[Outlet]
		AppKit.NSTableView TagTableView { get; set; }

		[Action ("AddButtonClick:")]
		partial void AddButtonClick (Foundation.NSObject sender);

		[Action ("DoneButtonClick:")]
		partial void DoneButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddButton != null) {
				AddButton.Dispose ();
				AddButton = null;
			}

			if (DoneButton != null) {
				DoneButton.Dispose ();
				DoneButton = null;
			}

			if (OrgTagLabel != null) {
				OrgTagLabel.Dispose ();
				OrgTagLabel = null;
			}

			if (TagTableView != null) {
				TagTableView.Dispose ();
				TagTableView = null;
			}
		}
	}
}
