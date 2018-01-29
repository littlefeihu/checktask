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
	[Register ("AddHighlightViewController")]
	partial class AddHighlightViewController
	{
		[Outlet]
		AppKit.NSButton AddNoteButton { get; set; }

		[Outlet]
		AppKit.NSTextField AssignLabelTF { get; set; }

		[Outlet]
		AppKit.NSButton DelNoteButton { get; set; }

		[Outlet]
		AppKit.NSButton EditButton { get; set; }

		[Outlet]
		AppKit.NSTableView TagsTableView { get; set; }

		[Outlet]
		AppKit.NSTextField TitleLabelTF { get; set; }

		[Action ("AddButtonClick:")]
		partial void AddButtonClick (Foundation.NSObject sender);

		[Action ("DelButtonClick:")]
		partial void DelButtonClick (Foundation.NSObject sender);

		[Action ("EditButtonClick:")]
		partial void EditButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddNoteButton != null) {
				AddNoteButton.Dispose ();
				AddNoteButton = null;
			}

			if (AssignLabelTF != null) {
				AssignLabelTF.Dispose ();
				AssignLabelTF = null;
			}

			if (DelNoteButton != null) {
				DelNoteButton.Dispose ();
				DelNoteButton = null;
			}

			if (EditButton != null) {
				EditButton.Dispose ();
				EditButton = null;
			}

			if (TagsTableView != null) {
				TagsTableView.Dispose ();
				TagsTableView = null;
			}

			if (TitleLabelTF != null) {
				TitleLabelTF.Dispose ();
				TitleLabelTF = null;
			}
		}
	}
}
