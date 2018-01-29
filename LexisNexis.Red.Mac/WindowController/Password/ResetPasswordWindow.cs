
using System;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class ResetPasswordWindow : MonoMac.AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public ResetPasswordWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ResetPasswordWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		// Override the constructor that takes a rectangle and a style, and change the style to bordeless
		[Export ("initWithContentRect:styleMask:backing:defer:")]
		public ResetPasswordWindow (RectangleF rect, NSWindowStyle style, NSBackingStore backing, bool defer)
			: base (rect, NSWindowStyle.Borderless, backing, defer) 
		{
			// Go transparent
			//BackgroundColor = NSColor.Clear;

			// pull window to front
			//Level = NSWindowLevel.Status;
			IsOpaque = false;
			HasShadow = true;
		}
		#endregion

		public override bool CanBecomeKeyWindow {
			get {
				return true;
			}
		}

		PointF start;
		// Track potential drag operations
		public override void MouseDown (NSEvent theEvent)
		{
			start = theEvent.LocationInWindow;
		}

		public override void MouseDragged (NSEvent theEvent)
		{
			var screenVisibleFrame = NSScreen.MainScreen.VisibleFrame;
			var windowFrame = Frame;
			var newOrigin = Frame.Location;

			// Get mouse location in window coordinates
			var current = theEvent.LocationInWindow;

			// Update the origin with the difference between the new mouse location and the old mouse location.                                      
			newOrigin.X += (current.X - start.X);
			newOrigin.Y += (current.Y - start.Y);

			// Prevent window to go under menubar
			if ((newOrigin.Y + windowFrame.Height) > (screenVisibleFrame.Y + screenVisibleFrame.Height))
				newOrigin.Y = screenVisibleFrame.Y + screenVisibleFrame.Height - windowFrame.Height;

			// Move to new lcoation
			SetFrameOrigin (newOrigin);
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
			}
		}
	}
}

