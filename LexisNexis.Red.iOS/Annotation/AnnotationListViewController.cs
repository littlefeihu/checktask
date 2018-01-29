using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace LexisNexis.Red.iOS
{
	partial class AnnotationListViewController : UIViewController
	{
		public AnnotationListViewController (IntPtr handle) : base (handle)
		{
			//TabBarController.Title = "Annotations";
			//Title = "Annotations";
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			TabBarController.Title = "Annotations";
		}
	}
}


