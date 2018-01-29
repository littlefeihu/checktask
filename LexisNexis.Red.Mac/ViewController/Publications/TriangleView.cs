
using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class TriangleView : NSView
	{
		const float PUBLICATION_VIEW_HEIGHT = 350.0f;
		const float PUBLICATION_COVER_WIDTH = 200.0f;
		const float PUBLICATION_DOWN_WIDTH = 90.0f;
		const float PUBLICATION_DOWN_ORGX = PUBLICATION_COVER_WIDTH-PUBLICATION_DOWN_WIDTH;
		const float PUBLICATION_DOWN_ORGY = PUBLICATION_VIEW_HEIGHT-PUBLICATION_DOWN_WIDTH;

		public NSColor BackgroudColor{ get; set;}

		#region Constructors

		// Called when created from unmanaged code
		public TriangleView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public TriangleView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		[Export("initWithFrame:")]
		public TriangleView (CGRect frame) : base(frame)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		/// <summary>
		/// Draw the triangle
		/// </summary>
		/// <param name="rect">Rect.</param>
		public override void DrawRect (CGRect dirtyRect)
		{
			base.DrawRect (dirtyRect);

			NSGraphicsContext.GlobalSaveGraphicsState();

			BackgroudColor.Set ();

			CGContext context = NSGraphicsContext.CurrentContext.GraphicsPort;
			context.BeginPath ();
			context.MoveTo (PUBLICATION_DOWN_WIDTH,PUBLICATION_DOWN_WIDTH);

			context.AddLineToPoint (PUBLICATION_DOWN_WIDTH, 0.0f);
			context.AddLineToPoint (0.0f, PUBLICATION_DOWN_WIDTH);
			context.AddLineToPoint (PUBLICATION_DOWN_WIDTH, PUBLICATION_DOWN_WIDTH);
			context.ClosePath ();

			context.DrawPath (CGPathDrawingMode.Fill);

			NSGraphicsContext.GlobalRestoreGraphicsState();
		}

		public override void MouseUp (NSEvent theEvent)
		{
			Console.WriteLine ("MouseUp");
		}
	}
}

