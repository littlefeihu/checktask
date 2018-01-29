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
	[Register ("PublicationContentPanelController")]
	partial class PublicationContentPanelController
	{
		[Outlet]
		AppKit.NSButton AnnotationButton { get; set; }

		[Outlet]
		AppKit.NSView AnnotationView { get; set; }

		[Outlet]
		AppKit.NSView BackgroudView { get; set; }

		[Outlet]
		AppKit.NSView BookContentView { get; set; }

		[Outlet]
		AppKit.NSButton ContentButton { get; set; }

		[Outlet]
		AppKit.NSView FunctionButtonView { get; set; }

		[Outlet]
		AppKit.NSButton GotoButton { get; set; }

		[Outlet]
		AppKit.NSButton HistoryButton { get; set; }

		[Outlet]
		AppKit.NSButton IndexButton { get; set; }

		[Outlet]
		AppKit.NSView IndexCustomView { get; set; }

		[Outlet]
		LexisNexis.Red.Mac.IndexViewController IndexViewController { get; set; }

		[Outlet]
		AppKit.NSButton InfoButton { get; set; }

		[Outlet]
		AppKit.NSButton LeftButton { get; set; }

		[Outlet]
		AppKit.NSTextField PageNumber { get; set; }

		[Outlet]
		LexisNexis.Red.Mac.PageContentViewController PageViewController { get; set; }

		[Outlet]
		AppKit.NSButton RightButton { get; set; }

		[Outlet]
		AppKit.NSSearchField SearchField { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl SegmentContol { get; set; }

		[Outlet]
		AppKit.NSButton ShareButton { get; set; }

		[Outlet]
		AppKit.NSButton SplitSwithButton { get; set; }

		[Outlet]
		AppKit.NSTextField TitleTField { get; set; }

		[Outlet]
		AppKit.NSView TocCustomView { get; set; }

		[Outlet]
		LexisNexis.Red.Mac.TocViewController TOCViewController { get; set; }

		[Action ("AnnotationButtonClick:")]
		partial void AnnotationButtonClick (Foundation.NSObject sender);

		[Action ("ContentButtonClick:")]
		partial void ContentButtonClick (Foundation.NSObject sender);

		[Action ("GotoPageNumber:")]
		partial void GotoPageNumber (Foundation.NSObject sender);

		[Action ("HistoryButtonClick:")]
		partial void HistoryButtonClick (Foundation.NSObject sender);

		[Action ("IndexButtonClick:")]
		partial void IndexButtonClick (Foundation.NSObject sender);

		[Action ("InfoButtonClick:")]
		partial void InfoButtonClick (Foundation.NSObject sender);

		[Action ("NextButtonClick:")]
		partial void NextButtonClick (Foundation.NSObject sender);

		[Action ("PreButtonClick:")]
		partial void PreButtonClick (Foundation.NSObject sender);

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
			if (AnnotationButton != null) {
				AnnotationButton.Dispose ();
				AnnotationButton = null;
			}

			if (AnnotationView != null) {
				AnnotationView.Dispose ();
				AnnotationView = null;
			}

			if (BackgroudView != null) {
				BackgroudView.Dispose ();
				BackgroudView = null;
			}

			if (BookContentView != null) {
				BookContentView.Dispose ();
				BookContentView = null;
			}

			if (ContentButton != null) {
				ContentButton.Dispose ();
				ContentButton = null;
			}

			if (FunctionButtonView != null) {
				FunctionButtonView.Dispose ();
				FunctionButtonView = null;
			}

			if (HistoryButton != null) {
				HistoryButton.Dispose ();
				HistoryButton = null;
			}

			if (IndexButton != null) {
				IndexButton.Dispose ();
				IndexButton = null;
			}

			if (IndexCustomView != null) {
				IndexCustomView.Dispose ();
				IndexCustomView = null;
			}

			if (IndexViewController != null) {
				IndexViewController.Dispose ();
				IndexViewController = null;
			}

			if (InfoButton != null) {
				InfoButton.Dispose ();
				InfoButton = null;
			}

			if (LeftButton != null) {
				LeftButton.Dispose ();
				LeftButton = null;
			}

			if (PageNumber != null) {
				PageNumber.Dispose ();
				PageNumber = null;
			}

			if (PageViewController != null) {
				PageViewController.Dispose ();
				PageViewController = null;
			}

			if (RightButton != null) {
				RightButton.Dispose ();
				RightButton = null;
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

			if (TocCustomView != null) {
				TocCustomView.Dispose ();
				TocCustomView = null;
			}

			if (TOCViewController != null) {
				TOCViewController.Dispose ();
				TOCViewController = null;
			}

			if (GotoButton != null) {
				GotoButton.Dispose ();
				GotoButton = null;
			}
		}
	}

	[Register ("PublicationContentPanel")]
	partial class PublicationContentPanel
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
