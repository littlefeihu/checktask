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
	[Register ("NoteTextNewTagAnnotationController")]
	partial class NoteTextNewTagAnnotationController
	{
		[Outlet]
		public	UIKit.UILabel DateLabel { get; set; }

		[Outlet]
		UIKit.UIButton DeleteBtn { get; set; }

		[Outlet]
		UIKit.UIButton DoneBtn { get; set; }

		[Outlet]
		public	UIKit.UITextView TextView { get; set; }

		[Action ("DeleteBtnClick:")]
		partial void DeleteBtnClick (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (DateLabel != null) {
				DateLabel.Dispose ();
				DateLabel = null;
			}

			if (DeleteBtn != null) {
				DeleteBtn.Dispose ();
				DeleteBtn = null;
			}

			if (DoneBtn != null) {
				DoneBtn.Dispose ();
				DoneBtn = null;
			}

			if (TextView != null) {
				TextView.Dispose ();
				TextView = null;
			}
		}
	}
}
