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
	[Register ("PublicationsWindowController")]
	partial class PublicationsWindowController
	{
		[Outlet]
		AppKit.NSButton EditAnnotationButton { get; set; }

		[Outlet]
		AppKit.NSButton HistoryButton { get; set; }

		[Outlet]
		AppKit.NSButton InfoButton { get; set; }

		[Outlet]
		AppKit.NSView MainView { get; set; }

		[Outlet]
		AppKit.NSWindow MainWindow { get; set; }

		[Outlet]
		AppKit.NSSearchField SearchField { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl SegmentContol { get; set; }

		[Outlet]
		AppKit.NSButton ShareButton { get; set; }

		[Outlet]
		AppKit.NSMenu ShareCustomMenu { get; set; }

		[Outlet]
		AppKit.NSButton SplitSwithButton { get; set; }

		[Outlet]
		AppKit.NSTextField TitleTField { get; set; }

		[Action ("EditAnnotaionBtnClick:")]
		partial void EditAnnotaionBtnClick (Foundation.NSObject sender);

		[Action ("EmailDocument:")]
		partial void EmailDocument (Foundation.NSObject sender);

		[Action ("HistoryBtnClick:")]
		partial void HistoryBtnClick (Foundation.NSObject sender);

		[Action ("InfoButtonClick:")]
		partial void InfoButtonClick (Foundation.NSObject sender);

		[Action ("PrintDocument:")]
		partial void PrintDocument (Foundation.NSObject sender);

		[Action ("SearchFieldClick:")]
		partial void SearchFieldClick (Foundation.NSObject sender);

		[Action ("SegmentClick:")]
		partial void SegmentClick (Foundation.NSObject sender);

		[Action ("ShareButtonClick:")]
		partial void ShareButtonClick (Foundation.NSObject sender);

		[Action ("SplitButtonClick:")]
		partial void SplitButtonClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (EditAnnotationButton != null) {
				EditAnnotationButton.Dispose ();
				EditAnnotationButton = null;
			}

			if (HistoryButton != null) {
				HistoryButton.Dispose ();
				HistoryButton = null;
			}

			if (InfoButton != null) {
				InfoButton.Dispose ();
				InfoButton = null;
			}

			if (MainView != null) {
				MainView.Dispose ();
				MainView = null;
			}

			if (MainWindow != null) {
				MainWindow.Dispose ();
				MainWindow = null;
			}

			if (SearchField != null) {
				SearchField.Dispose ();
				SearchField = null;
			}

			if (SegmentContol != null) {
				SegmentContol.Dispose ();
				SegmentContol = null;
			}

			if (ShareButton != null) {
				ShareButton.Dispose ();
				ShareButton = null;
			}

			if (SplitSwithButton != null) {
				SplitSwithButton.Dispose ();
				SplitSwithButton = null;
			}

			if (TitleTField != null) {
				TitleTField.Dispose ();
				TitleTField = null;
			}

			if (ShareCustomMenu != null) {
				ShareCustomMenu.Dispose ();
				ShareCustomMenu = null;
			}
		}
	}

	[Register ("PublicationsWindow")]
	partial class PublicationsWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
