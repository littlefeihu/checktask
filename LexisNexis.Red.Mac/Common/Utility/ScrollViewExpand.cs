using System;
using AppKit;
using Foundation;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class ScrollViewExpand : NSScrollView
	{
		[Export("initWithFrame:")]
		public ScrollViewExpand (CGRect frame) : base(frame)
		{
		}

		public override void ScrollWheel (NSEvent theEvent)
		{
			//base.ScrollWheel (theEvent);
			var view = Superview.Superview.Superview;
			view.ScrollWheel (theEvent);
		}
	}
}

