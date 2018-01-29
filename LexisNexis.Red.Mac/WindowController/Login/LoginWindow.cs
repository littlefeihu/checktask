

using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using AppKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class LoginWindow : NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public LoginWindow (IntPtr handle) : base (handle)
		{
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LoginWindow (NSCoder coder) : base (coder)
		{
		}

		#endregion

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		public override void KeyDown (NSEvent theEvent)
		{
			ushort keycode = theEvent.KeyCode;
			//Console.WriteLine ("keycode:{0}", keycode);

			if (keycode == 53) {
				AppDelegate appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
				appDelegate.loginWindowController.CancelLogin ();
			} else if (keycode == 36) {
				AppDelegate appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
				appDelegate.loginWindowController.Login ();
			}
		}
	}
}