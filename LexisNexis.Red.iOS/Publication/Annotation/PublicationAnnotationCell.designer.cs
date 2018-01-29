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
	[Register ("PublicationAnnotationCell")]
	partial class PublicationAnnotationCell
	{
		[Outlet]
		public UIKit.UIView bgView { get; private set; }

		[Outlet]
		public UIKit.UILabel conLabel { get; private set; }

		[Outlet]
		public UIKit.UILabel dateLabel { get; private set; }

		[Outlet]
		public UIKit.UIImageView imageIma { get; private set; }

		[Outlet]
		public UIKit.UILabel NoteLabel { get; set; }

		[Outlet]
		public UIKit.UILabel TocLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (bgView != null) {
				bgView.Dispose ();
				bgView = null;
			}

			if (conLabel != null) {
				conLabel.Dispose ();
				conLabel = null;
			}

			if (dateLabel != null) {
				dateLabel.Dispose ();
				dateLabel = null;
			}

			if (TocLabel != null) {
				TocLabel.Dispose ();
				TocLabel = null;
			}

			if (imageIma != null) {
				imageIma.Dispose ();
				imageIma = null;
			}

			if (NoteLabel != null) {
				NoteLabel.Dispose ();
				NoteLabel = null;
			}
		}
	}
}
