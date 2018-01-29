using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class ExpandScrollView : NSScrollView
	{
		#region Constructors

		// Called when created from unmanaged code
		public ExpandScrollView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ExpandScrollView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		[Export("initWithFrame:")]
		public ExpandScrollView (CGRect frame) : base(frame)
		{
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public override void ScrollWheel (NSEvent theEvent)
		{
			//base.ScrollWheel (theEvent);
			var view = Superview.Superview.Superview;
			view.ScrollWheel (theEvent);
		}
	}
}
