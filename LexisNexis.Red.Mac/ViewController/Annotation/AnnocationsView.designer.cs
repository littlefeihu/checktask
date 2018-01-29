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
	[Register ("AnnocationsViewController")]
	partial class AnnocationsViewController
	{
		[Outlet]
		AppKit.NSButton AllButton { get; set; }

		[Outlet]
		AppKit.NSTableView AnnotationTableView { get; set; }

		[Outlet]
		AppKit.NSButton HighlightsButton { get; set; }

		[Outlet]
		AppKit.NSTextField InfoLabelTF { get; set; }

		[Outlet]
		AppKit.NSView InfoView { get; set; }

		[Outlet]
		AppKit.NSButton NotesButton { get; set; }

		[Outlet]
		AppKit.NSButton TagFilterButton { get; set; }

		[Outlet]
		AppKit.NSPopUpButton TagFilterPopBtn { get; set; }

		[Action ("AllButtonClick:")]
		partial void AllButtonClick (Foundation.NSObject sender);

		[Action ("HighlightButtonClick:")]
		partial void HighlightButtonClick (Foundation.NSObject sender);

		[Action ("NotesButtonClick:")]
		partial void NotesButtonClick (Foundation.NSObject sender);

		[Action ("TagFilterBtnClick:")]
		partial void TagFilterBtnClick (Foundation.NSObject sender);

		[Action ("TagFilterPopBtnClick:")]
		partial void TagFilterPopBtnClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AllButton != null) {
				AllButton.Dispose ();
				AllButton = null;
			}

			if (HighlightsButton != null) {
				HighlightsButton.Dispose ();
				HighlightsButton = null;
			}

			if (NotesButton != null) {
				NotesButton.Dispose ();
				NotesButton = null;
			}

			if (TagFilterButton != null) {
				TagFilterButton.Dispose ();
				TagFilterButton = null;
			}

			if (TagFilterPopBtn != null) {
				TagFilterPopBtn.Dispose ();
				TagFilterPopBtn = null;
			}

			if (InfoView != null) {
				InfoView.Dispose ();
				InfoView = null;
			}

			if (InfoLabelTF != null) {
				InfoLabelTF.Dispose ();
				InfoLabelTF = null;
			}

			if (AnnotationTableView != null) {
				AnnotationTableView.Dispose ();
				AnnotationTableView = null;
			}
		}
	}

	[Register ("AnnocationsView")]
	partial class AnnocationsView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
