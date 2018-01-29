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
	[Register ("AnnotationController")]
	partial class AnnotationController
	{
		[Outlet]
		UIKit.UILabel ExpiredDateLabel { get; set; }

		[Outlet]
		UIKit.UIButton ExpireInfoButton { get; set; }

		[Outlet]
		UIKit.UIView ExpireInfoView { get; set; }

		[Outlet]
		UIKit.UINavigationBar FilterNavigationBar { get; set; }

		[Outlet]
		UIKit.UIBarButtonItem FilterSegmentControl { get; set; }

		[Action ("OnSegmentControlValueChanged:")]
		partial void OnSegmentControlValueChanged (Foundation.NSObject sender);

		
		void ReleaseDesignerOutlets ()
		{
			if (ExpiredDateLabel != null) {
				ExpiredDateLabel.Dispose ();
				ExpiredDateLabel = null;
			}

			if (ExpireInfoButton != null) {
				ExpireInfoButton.Dispose ();
				ExpireInfoButton = null;
			}

			if (ExpireInfoView != null) {
				ExpireInfoView.Dispose ();
				ExpireInfoView = null;
			}

			if (FilterNavigationBar != null) {
				FilterNavigationBar.Dispose ();
				FilterNavigationBar = null;
			}

			if (FilterSegmentControl != null) {
				FilterSegmentControl.Dispose ();
				FilterSegmentControl = null;
			}
		}
	}
}
