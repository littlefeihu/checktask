﻿using System;

using AppKit;
using Foundation;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class LNBordlessWindow : NSWindow {
		// Override the constructor that takes a rectangle and a style, and change the style to bordeless
		[Export ("initWithContentRect:styleMask:backing:defer:")]
		public LNBordlessWindow (CGRect rect, NSWindowStyle style, NSBackingStore backing, bool defer)
			: base (rect, NSWindowStyle.Borderless, backing, defer) 
		{
			// Go transparent
			BackgroundColor = NSColor.Clear;

			// pull window to front
			//Level = NSWindowLevel.Status;
			IsOpaque = false;
			HasShadow = true;
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		// Custom windows that use the NSBorderlessWindowMask can't become key by default. 
		// Override this method so that controls in this window will be enabled.
		public override bool CanBecomeKeyWindow {
			get {
				return true;
			}
		}

		CGPoint start;
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
	}

	[Register ("CustomView")]
	public class CustomView : NSView {
		NSImage circle, pentagon;

		public CustomView (IntPtr handle) : base (handle) {}

		public override void AwakeFromNib ()
		{
			circle = NSImage.ImageNamed ("circle");
			pentagon = NSImage.ImageNamed ("pentagon");
			NeedsDisplay = true;
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			NSColor.Clear.Set ();
			NSGraphics.RectFill (Frame);

			var image = Window.AlphaValue > 0.7 ? circle : pentagon;
			image.Draw (new CGPoint (0, 0), Frame, NSCompositingOperation.SourceOver, 1);
			NeedsDisplay = true;
			Window.InvalidateShadow ();
		}
	}
}
