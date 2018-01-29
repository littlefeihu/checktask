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
	[Register ("AddAnnotationViewController")]
	partial class AddAnnotationViewController
	{
		[Outlet]
		AppKit.NSTextField AssignLabelTF { get; set; }

		[Outlet]
		AppKit.NSTextField DateLabelTF { get; set; }

		[Outlet]
		AppKit.NSButton DelAnnotationBtn { get; set; }

		[Outlet]
		AppKit.NSButton DelNoteButton { get; set; }

		[Outlet]
		AppKit.NSButton EditButton { get; set; }

		[Outlet]
		AppKit.NSView HSeprator { get; set; }

		[Outlet]
		AppKit.NSView NoteBkgView { get; set; }

		[Outlet]
		AppKit.NSButton NoteButton { get; set; }

		[Outlet]
		AppKit.NSTextView NoteTextView { get; set; }

		[Outlet]
		AppKit.NSTableView TagsTableView { get; set; }

		[Outlet]
		AppKit.NSTextField TitleLabelTF { get; set; }

		[Outlet]
		AppKit.NSView VSeprator { get; set; }

		[Action ("DelAnnotationBtnClick:")]
		partial void DelAnnotationBtnClick (Foundation.NSObject sender);

		[Action ("DelButtonClick:")]
		partial void DelButtonClick (Foundation.NSObject sender);

		[Action ("EditButtonClick:")]
		partial void EditButtonClick (Foundation.NSObject sender);

		[Action ("NoteButtonClick:")]
		partial void NoteButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AssignLabelTF != null) {
				AssignLabelTF.Dispose ();
				AssignLabelTF = null;
			}

			if (DateLabelTF != null) {
				DateLabelTF.Dispose ();
				DateLabelTF = null;
			}

			if (DelAnnotationBtn != null) {
				DelAnnotationBtn.Dispose ();
				DelAnnotationBtn = null;
			}

			if (DelNoteButton != null) {
				DelNoteButton.Dispose ();
				DelNoteButton = null;
			}

			if (EditButton != null) {
				EditButton.Dispose ();
				EditButton = null;
			}

			if (HSeprator != null) {
				HSeprator.Dispose ();
				HSeprator = null;
			}

			if (NoteBkgView != null) {
				NoteBkgView.Dispose ();
				NoteBkgView = null;
			}

			if (NoteButton != null) {
				NoteButton.Dispose ();
				NoteButton = null;
			}

			if (NoteTextView != null) {
				NoteTextView.Dispose ();
				NoteTextView = null;
			}

			if (TagsTableView != null) {
				TagsTableView.Dispose ();
				TagsTableView = null;
			}

			if (TitleLabelTF != null) {
				TitleLabelTF.Dispose ();
				TitleLabelTF = null;
			}

			if (VSeprator != null) {
				VSeprator.Dispose ();
				VSeprator = null;
			}
		}
	}
}
