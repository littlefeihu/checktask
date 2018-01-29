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
	[Register ("OrganizePublicationsPanelController")]
	partial class OrganizePublicationsPanelController
	{
		[Outlet]
		AppKit.NSButton cancelButton { get; set; }

		[Outlet]
		AppKit.NSButton deleteButton { get; set; }

		[Outlet]
		AppKit.NSTextField dragInfoLabel { get; set; }

		[Outlet]
		AppKit.NSButton okButton { get; set; }

		[Outlet]
		AppKit.NSTextField orderInfoLabel { get; set; }

		[Outlet]
		AppKit.NSTableView tableView { get; set; }

		[Action ("CancelButtonClick:")]
		partial void CancelButtonClick (Foundation.NSObject sender);

		[Action ("DeleteButtonClick:")]
		partial void DeleteButtonClick (Foundation.NSObject sender);

		[Action ("OKButtonClick:")]
		partial void OKButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (orderInfoLabel != null) {
				orderInfoLabel.Dispose ();
				orderInfoLabel = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (dragInfoLabel != null) {
				dragInfoLabel.Dispose ();
				dragInfoLabel = null;
			}

			if (deleteButton != null) {
				deleteButton.Dispose ();
				deleteButton = null;
			}

			if (cancelButton != null) {
				cancelButton.Dispose ();
				cancelButton = null;
			}

			if (okButton != null) {
				okButton.Dispose ();
				okButton = null;
			}
		}
	}

	[Register ("OrganizePublicationsPanel")]
	partial class OrganizePublicationsPanel
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
