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
	[Register ("AddTagsViewController")]
	partial class AddTagsViewController
	{
		[Outlet]
		AppKit.NSButton BackButton { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton1 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton10 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton11 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton12 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton2 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton3 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton4 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton5 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton6 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton7 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton8 { get; set; }

		[Outlet]
		AppKit.NSButton ColorButton9 { get; set; }

		[Outlet]
		AppKit.NSButton DeleteButton { get; set; }

		[Outlet]
		AppKit.NSButton DoneButton { get; set; }

		[Outlet]
		AppKit.NSView HorLine1 { get; set; }

		[Outlet]
		AppKit.NSView HorLine2 { get; set; }

		[Outlet]
		AppKit.NSBox TagColorBox { get; set; }

		[Outlet]
		AppKit.NSBox TagColorLabelBox { get; set; }

		[Outlet]
		AppKit.NSTextField TagColorLableTF { get; set; }

		[Outlet]
		AppKit.NSBox TagNameBox { get; set; }

		[Outlet]
		AppKit.NSBox TagNameLabelBox { get; set; }

		[Outlet]
		AppKit.NSTextField TagNameLableTF { get; set; }

		[Outlet]
		AppKit.NSTextField TagNameTF { get; set; }

		[Outlet]
		AppKit.NSTextField TitleLabelTF { get; set; }

		[Outlet]
		AppKit.NSView VerLine1 { get; set; }

		[Outlet]
		AppKit.NSView VerLine2 { get; set; }

		[Outlet]
		AppKit.NSView VerLine3 { get; set; }

		[Action ("BackClick:")]
		partial void BackClick (Foundation.NSObject sender);

		[Action ("DoneClick:")]
		partial void DoneClick (Foundation.NSObject sender);

		[Action ("RemoveColorName:")]
		partial void RemoveColorName (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BackButton != null) {
				BackButton.Dispose ();
				BackButton = null;
			}

			if (ColorButton1 != null) {
				ColorButton1.Dispose ();
				ColorButton1 = null;
			}

			if (ColorButton10 != null) {
				ColorButton10.Dispose ();
				ColorButton10 = null;
			}

			if (ColorButton11 != null) {
				ColorButton11.Dispose ();
				ColorButton11 = null;
			}

			if (ColorButton12 != null) {
				ColorButton12.Dispose ();
				ColorButton12 = null;
			}

			if (ColorButton2 != null) {
				ColorButton2.Dispose ();
				ColorButton2 = null;
			}

			if (ColorButton3 != null) {
				ColorButton3.Dispose ();
				ColorButton3 = null;
			}

			if (ColorButton4 != null) {
				ColorButton4.Dispose ();
				ColorButton4 = null;
			}

			if (ColorButton5 != null) {
				ColorButton5.Dispose ();
				ColorButton5 = null;
			}

			if (ColorButton6 != null) {
				ColorButton6.Dispose ();
				ColorButton6 = null;
			}

			if (ColorButton7 != null) {
				ColorButton7.Dispose ();
				ColorButton7 = null;
			}

			if (ColorButton8 != null) {
				ColorButton8.Dispose ();
				ColorButton8 = null;
			}

			if (ColorButton9 != null) {
				ColorButton9.Dispose ();
				ColorButton9 = null;
			}

			if (DeleteButton != null) {
				DeleteButton.Dispose ();
				DeleteButton = null;
			}

			if (DoneButton != null) {
				DoneButton.Dispose ();
				DoneButton = null;
			}

			if (HorLine1 != null) {
				HorLine1.Dispose ();
				HorLine1 = null;
			}

			if (HorLine2 != null) {
				HorLine2.Dispose ();
				HorLine2 = null;
			}

			if (TagColorBox != null) {
				TagColorBox.Dispose ();
				TagColorBox = null;
			}

			if (TagColorLabelBox != null) {
				TagColorLabelBox.Dispose ();
				TagColorLabelBox = null;
			}

			if (TagColorLableTF != null) {
				TagColorLableTF.Dispose ();
				TagColorLableTF = null;
			}

			if (TagNameBox != null) {
				TagNameBox.Dispose ();
				TagNameBox = null;
			}

			if (TagNameLabelBox != null) {
				TagNameLabelBox.Dispose ();
				TagNameLabelBox = null;
			}

			if (TagNameLableTF != null) {
				TagNameLableTF.Dispose ();
				TagNameLableTF = null;
			}

			if (TagNameTF != null) {
				TagNameTF.Dispose ();
				TagNameTF = null;
			}

			if (TitleLabelTF != null) {
				TitleLabelTF.Dispose ();
				TitleLabelTF = null;
			}

			if (VerLine1 != null) {
				VerLine1.Dispose ();
				VerLine1 = null;
			}

			if (VerLine2 != null) {
				VerLine2.Dispose ();
				VerLine2 = null;
			}

			if (VerLine3 != null) {
				VerLine3.Dispose ();
				VerLine3 = null;
			}
		}
	}
}
