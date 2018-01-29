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
	[Register ("AnnotationsManageController")]
	partial class AnnotationsManageController
	{
		[Outlet]
		UIKit.UITableView AnnotationTableView { get; set; }

		[Outlet]
		UIKit.UITableView TagTableView { get; set; }

		[Action ("AnnotationOnSegmentController:")]
		partial void AnnotationOnSegmentController (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AnnotationTableView != null) {
				AnnotationTableView.Dispose ();
				AnnotationTableView = null;
			}

			if (TagTableView != null) {
				TagTableView.Dispose ();
				TagTableView = null;
			}
		}
	}
}
