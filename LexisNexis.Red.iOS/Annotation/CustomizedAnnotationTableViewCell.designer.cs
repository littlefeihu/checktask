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
	[Register ("CustomizedAnnotationTableViewCell")]
	partial class CustomizedAnnotationTableViewCell
	{
		[Outlet]
		public UIKit.UILabel DateLabel { get; set; }

		[Outlet]
		public UIKit.UILabel GuidecardLabel { get; set; }

		[Outlet]
		public UIKit.UILabel HighlightTextLabel { get; set; }

		[Outlet]
		public UIKit.UILabel NoteLabel { get; set; }

		[Outlet]
		public UIKit.UILabel PublicationNameLabel { get; set; }

		[Outlet]
		public UIKit.UIView TagContainerView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (PublicationNameLabel != null) {
				PublicationNameLabel.Dispose ();
				PublicationNameLabel = null;
			}

			if (DateLabel != null) {
				DateLabel.Dispose ();
				DateLabel = null;
			}

			if (GuidecardLabel != null) {
				GuidecardLabel.Dispose ();
				GuidecardLabel = null;
			}

			if (HighlightTextLabel != null) {
				HighlightTextLabel.Dispose ();
				HighlightTextLabel = null;
			}

			if (NoteLabel != null) {
				NoteLabel.Dispose ();
				NoteLabel = null;
			}

			if (TagContainerView != null) {
				TagContainerView.Dispose ();
				TagContainerView = null;
			}
		}
	}
}
