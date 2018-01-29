
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public partial class NoteTextNewTagAnnotationController : UIViewController
	{
  
		public NoteTextNewTagAnnotationController (IntPtr handle) : base (handle)
		{
			
		}

		public NoteTextNewTagAnnotationController () : base ("NoteTextNewTagAnnotationController", null)
		{
		}

		partial void DeleteBtnClick (NSObject sender)
		{
        //Delete Annotation
			TextView.Text = null;
		}

		public override void DidReceiveMemoryWarning ()
		{
 			base.DidReceiveMemoryWarning ();
			
 		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			AppDisplayUtil.Instance.newAnnotationVC = this;
			var todayText = "Today,";
			var hourText = System.DateTime.Now.ToString (" HH:mm t\\M");//hh:mm 
//			var minText = " am";
			DateLabel.Text = string.Format (todayText+hourText);//+minText


 		}


	}
}

