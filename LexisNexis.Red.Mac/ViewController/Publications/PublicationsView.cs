
using System;

using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class PublicationsView : NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public PublicationsView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PublicationsView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		/*
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
				Console.WriteLine ("KeyDown");
			
			}
		}
*/

	}
}

