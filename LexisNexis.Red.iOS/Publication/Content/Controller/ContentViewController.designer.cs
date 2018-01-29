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
	[Register ("ContentViewController")]
	partial class ContentViewController
	{
		[Outlet]
		UIKit.UINavigationBar ContentNavigationBar { get; set; }

		[Outlet]
		UIKit.UIButton GotoBarButton { get; set; }

		[Outlet]
		UIKit.UILabel PageNumLabel { get; set; }

		[Action ("ShowGoToTableView:")]
		partial void ShowGoToTableView (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (GotoBarButton != null) {
				GotoBarButton.Dispose ();
				GotoBarButton = null;
			}

			if (PageNumLabel != null) {
				PageNumLabel.Dispose ();
				PageNumLabel = null;
			}

			if (ContentNavigationBar != null) {
				ContentNavigationBar.Dispose ();
				ContentNavigationBar = null;
			}
		}
	}
}
