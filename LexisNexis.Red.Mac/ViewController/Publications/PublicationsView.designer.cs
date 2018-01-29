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
	[Register ("PublicationsViewController")]
	partial class PublicationsViewController
	{
		[Outlet]
		AppKit.NSScrollView bookScrollView { get; set; }

		[Outlet]
		AppKit.NSTableView historyTableView { get; set; }

		[Outlet]
		AppKit.NSTextField historyTF { get; set; }

		[Outlet]
		AppKit.NSTextField histroyEmptyLabel { get; set; }

		[Outlet]
		AppKit.NSView MainDocumentView { get; set; }

		[Outlet]
		AppKit.NSScrollView MainScrollView { get; set; }

		[Outlet]
		AppKit.NSPopUpButton popupButton { get; set; }

		[Outlet]
		AppKit.NSTextField publicationEmptyLabel { get; set; }

		[Outlet]
		AppKit.NSView PublicationsCustomView { get; set; }

		[Action ("PopupButtonSelectChange:")]
		partial void PopupButtonSelectChange (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (bookScrollView != null) {
				bookScrollView.Dispose ();
				bookScrollView = null;
			}

			if (historyTableView != null) {
				historyTableView.Dispose ();
				historyTableView = null;
			}

			if (historyTF != null) {
				historyTF.Dispose ();
				historyTF = null;
			}

			if (histroyEmptyLabel != null) {
				histroyEmptyLabel.Dispose ();
				histroyEmptyLabel = null;
			}

			if (MainDocumentView != null) {
				MainDocumentView.Dispose ();
				MainDocumentView = null;
			}

			if (MainScrollView != null) {
				MainScrollView.Dispose ();
				MainScrollView = null;
			}

			if (popupButton != null) {
				popupButton.Dispose ();
				popupButton = null;
			}

			if (publicationEmptyLabel != null) {
				publicationEmptyLabel.Dispose ();
				publicationEmptyLabel = null;
			}

			if (PublicationsCustomView != null) {
				PublicationsCustomView.Dispose ();
				PublicationsCustomView = null;
			}
		}
	}

	[Register ("PublicationsView")]
	partial class PublicationsView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
