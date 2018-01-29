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
	[Register ("TableOfContentController")]
	partial class TableOfContentController
	{
		[Outlet]
		UIKit.UIView ContainerView { get; set; }

		[Outlet]
		UIKit.UILabel ExpiredDateLabel { get; set; }

		[Outlet]
		UIKit.UIButton ExpireInfoButton { get; set; }

		[Outlet]
		UIKit.UIView ExpireInfoView { get; set; }

		[Outlet]
		UIKit.UISearchBar SearchBar { get; set; }
		
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

			if (SearchBar != null) {
				SearchBar.Dispose ();
				SearchBar = null;
			}

			if (ContainerView != null) {
				ContainerView.Dispose ();
				ContainerView = null;
			}
		}
	}
}
