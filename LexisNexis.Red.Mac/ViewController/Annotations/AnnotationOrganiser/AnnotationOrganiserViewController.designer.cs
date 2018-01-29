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
	[Register ("AnnotationOrganiserViewController")]
	partial class AnnotationOrganiserViewController
	{
		[Outlet]
		AppKit.NSButton AllButton { get; set; }

		[Outlet]
		AppKit.NSView AnnotationCustomView { get; set; }

		[Outlet]
		AppKit.NSTableView AnnotationTableView { get; set; }

		[Outlet]
		AppKit.NSView FunctionButtonView { get; set; }

		[Outlet]
		AppKit.NSButton HighlightsButton { get; set; }

		[Outlet]
		AppKit.NSTextField InfoLabelTF { get; set; }

		[Outlet]
		AppKit.NSButton NotesButton { get; set; }

		[Outlet]
		AppKit.NSButton OrphansButton { get; set; }

		[Outlet]
		AppKit.NSTableView TagsTableView { get; set; }

		[Outlet]
		AppKit.NSView TagsView { get; set; }

		[Action ("AllButtonClick:")]
		partial void AllButtonClick (Foundation.NSObject sender);

		[Action ("HighlightButtonClick:")]
		partial void HighlightButtonClick (Foundation.NSObject sender);

		[Action ("NotesButtonClick:")]
		partial void NotesButtonClick (Foundation.NSObject sender);

		[Action ("OrphansButtonClick:")]
		partial void OrphansButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AllButton != null) {
				AllButton.Dispose ();
				AllButton = null;
			}

			if (AnnotationCustomView != null) {
				AnnotationCustomView.Dispose ();
				AnnotationCustomView = null;
			}

			if (AnnotationTableView != null) {
				AnnotationTableView.Dispose ();
				AnnotationTableView = null;
			}

			if (FunctionButtonView != null) {
				FunctionButtonView.Dispose ();
				FunctionButtonView = null;
			}

			if (HighlightsButton != null) {
				HighlightsButton.Dispose ();
				HighlightsButton = null;
			}

			if (NotesButton != null) {
				NotesButton.Dispose ();
				NotesButton = null;
			}

			if (OrphansButton != null) {
				OrphansButton.Dispose ();
				OrphansButton = null;
			}

			if (TagsTableView != null) {
				TagsTableView.Dispose ();
				TagsTableView = null;
			}

			if (TagsView != null) {
				TagsView.Dispose ();
				TagsView = null;
			}

			if (InfoLabelTF != null) {
				InfoLabelTF.Dispose ();
				InfoLabelTF = null;
			}
		}
	}
}
