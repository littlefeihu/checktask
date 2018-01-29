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
	[Register ("PublicationDetailController")]
	partial class PublicationDetailController
	{
		[Outlet]
		UIKit.UIView contentContainerView { get; set; }

		[Outlet]
		UIKit.UIView LeftContainerView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint LeftContainerViewLeadingConstraint { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LeftContainerView != null) {
				LeftContainerView.Dispose ();
				LeftContainerView = null;
			}

			if (contentContainerView != null) {
				contentContainerView.Dispose ();
				contentContainerView = null;
			}

			if (LeftContainerViewLeadingConstraint != null) {
				LeftContainerViewLeadingConstraint.Dispose ();
				LeftContainerViewLeadingConstraint = null;
			}
		}
	}
}
