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
	[Register ("CustomizedPopoverHisoryTableViewCell")]
	partial class CustomizedPopoverHisoryTableViewCell
	{
		[Outlet]
		public UIKit.UIView CellColorHintView { get; set; }

		[Outlet]
		public UIKit.UILabel GuideCardNameLabel { get; set; }

		[Outlet]
		public UIKit.UILabel TimeLabel { get; set; }

		[Outlet]
		public UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CellColorHintView != null) {
				CellColorHintView.Dispose ();
				CellColorHintView = null;
			}

			if (TimeLabel != null) {
				TimeLabel.Dispose ();
				TimeLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (GuideCardNameLabel != null) {
				GuideCardNameLabel.Dispose ();
				GuideCardNameLabel = null;
			}
		}
	}
}
