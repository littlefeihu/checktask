using System;

using Foundation;
using AppKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class ForgetPasswordWindow : NSWindow
	{
		public ForgetPasswordWindow (IntPtr handle) : base (handle)
		{
			//Console.WriteLine("{0}",Frame.Top);
			//SetFrameOrigin (new CGPoint(500,130));
		}

		[Export ("initWithCoder:")]
		public ForgetPasswordWindow (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		public override void KeyDown (NSEvent theEvent)
		{
			ushort keycode = theEvent.KeyCode;
			if (keycode == 53) {
				AppDelegate appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
				appDelegate.loginWindowController.CancelLogin ();
			} else if (keycode == 36) {
				AppDelegate appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
				//appDelegate.loginWindowController.ResetPassword ();
			}else if (keycode == 39) {
			}
		}
	}
}
