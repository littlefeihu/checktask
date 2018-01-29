using System;

using AppKit;
using Foundation;
using CoreAnimation;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class CircularProgressView : NSView
	{
		CAShapeLayer outerCircleLayer { get; set;}
		CAShapeLayer progressLayer { get; set;}
		float curProgress { get; set;}
		public PublicationView superViewObj { get; set;}

		[Export("initWithFrame:")]
		public CircularProgressView (CGRect frame) : base(frame)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
			WantsLayer = true;

			var center = new CGPoint(Frame.Width/2,Frame.Height/2);
		
			outerCircleLayer = new CAShapeLayer ();

			var path = new CGPath();
			nfloat radis = (Frame.Width-2) / 2;
			path.AddArc (center.X, center.Y, radis, 0, (float)(2*Math.PI), true);

			outerCircleLayer.Path = path;
			outerCircleLayer.FillColor = NSColor.Clear.CGColor;
			outerCircleLayer.LineWidth = 1.0f;
			outerCircleLayer.StrokeColor = NSColor.White.CGColor;
			outerCircleLayer.StrokeStart = 0.0f;
			outerCircleLayer.StrokeEnd = 1.0f;

			Layer.AddSublayer (outerCircleLayer);

			progressLayer = new CAShapeLayer ();

			var progressPath = new CGPath();

			float start = (float)(Math.PI/2);
			float end = (float)(-3*Math.PI/2);
			radis = (Frame.Width-6) / 2;

			progressPath.AddArc (center.X, center.Y, radis, start, end, true);

			progressLayer.Path = progressPath;
			progressLayer.FillColor   = NSColor.Clear.CGColor;
			progressLayer.LineWidth   = 3.0f;
			progressLayer.StrokeColor = NSColor.White.CGColor;
			progressLayer.StrokeStart = 0;
			progressLayer.StrokeEnd = 0.0f;
			Layer.AddSublayer (progressLayer);

			//Circle progress view center
			var centerRectLayer = new CAShapeLayer ();
			centerRectLayer.Bounds = new CGRect (0, 0, 8, 8);
			centerRectLayer.Position = center;
			centerRectLayer.FillColor = NSColor.White.CGColor;
			centerRectLayer.BackgroundColor = NSColor.White.CGColor;
			Layer.AddSublayer (centerRectLayer);
		}

		public void UpdateProgress (float progress)
		{
			if (progress!=1 && ((progress-curProgress)<0.02 || progress > 1.0)) {
				return;
			}

			//Console.WriteLine ("progress:{0}",progress);
			curProgress = progress;

			CATransaction.Begin ();
			progressLayer.StrokeEnd = curProgress;
			CATransaction.Commit ();
		}
			
		public override void MouseUp (NSEvent theEvent)
		{
			//base.MouseUp (theEvent);
            string title = "Cancel Download";
            string errMsg = "Are you sure you want to cancel the download of this LexisNexis Red publication?";
            nint result = AlertSheet.RunConfirmAlert (title,errMsg);
            if (result == 1) {
				if (superViewObj != null) {
					ResetProgress ();
					superViewObj.CancelDownload ();
				}
            }
		}

		public void ResetProgress()
		{
			curProgress = 0;

			CATransaction.Begin ();
			progressLayer.StrokeEnd = curProgress;
			CATransaction.Commit ();
		}
			
	}
}

