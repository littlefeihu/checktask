// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace LexisNexis.Red.iOS
{
	[Register ("MyPublicationViewController")]
	partial class MyPublicationViewController
	{
		[Outlet]
		UIKit.UIView HistoryContainerView { get; set; }

		[Outlet]
		UIKit.UIView MyPublication { get; set; }

		[Outlet]
		UIKit.UILabel noPublicationLabel { get; set; }

		[Outlet]
		UIKit.UISegmentedControl publicationFilterSegmentControl { get; set; }

		[Outlet]
		UIKit.UIScrollView publicationViewScrollContainer { get; set; }

		[Action ("OnPublicationFilterSelectedChanged:")]
		partial void OnPublicationFilterSelectedChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (MyPublication != null) {
				MyPublication.Dispose ();
				MyPublication = null;
			}

			if (noPublicationLabel != null) {
				noPublicationLabel.Dispose ();
				noPublicationLabel = null;
			}

			if (publicationFilterSegmentControl != null) {
				publicationFilterSegmentControl.Dispose ();
				publicationFilterSegmentControl = null;
			}

			if (publicationViewScrollContainer != null) {
				publicationViewScrollContainer.Dispose ();
				publicationViewScrollContainer = null;
			}

			if (HistoryContainerView != null) {
				HistoryContainerView.Dispose ();
				HistoryContainerView = null;
			}
		}
	}
}
