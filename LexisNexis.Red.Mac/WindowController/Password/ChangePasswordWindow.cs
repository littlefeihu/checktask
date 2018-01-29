
using System;

using Foundation;
using AppKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class ChangePasswordWindow : NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public ChangePasswordWindow (IntPtr handle) : base (handle)
		{
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ChangePasswordWindow (NSCoder coder) : base (coder)
		{
		}

		#endregion

		public override void KeyDown (NSEvent theEvent)
		{
			ushort keycode = theEvent.KeyCode;
			if (keycode == 53) {
				AppDelegate appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
				//appDelegate.loginWindowController.CancelLogin ();
			} else if (keycode == 36) {
				AppDelegate appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
				//appDelegate.loginWindowController.ResetPassword ();
			}
		}
	}
}

