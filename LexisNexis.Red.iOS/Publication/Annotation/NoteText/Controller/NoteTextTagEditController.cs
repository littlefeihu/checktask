
using System;

using Foundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using LexisNexis.Red.Common.BusinessModel;
namespace LexisNexis.Red.iOS
{
	public partial class NoteTextTagEditController : UIViewController
	{
		public NoteTextTagEditController (IntPtr handle) : base (handle)
		{
 		}
		public NoteTextTagEditController () : base ("NoteTextTagEditController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			this.Title = "New tag";
 			this.View.Frame = new CGRect (0,0,320,332);

    		}
 		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			this.View.Frame = new CGRect (0,0,320,332);
			this.View.SetNeedsDisplay ();
		}

  	}
}

