using System;

using Foundation;
using UIKit;
using CoreAnimation;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	/// <summary>
	/// Circular progress view which looks like progress view when install an app in apple store
	/// </summary>
	public class CircularProgressView : UIView
	{

		private readonly CAShapeLayer outerCircleLayer;
		private CAShapeLayer progressLayer;
		private float curProgress = 0.0f; //from 0 to 1

		public CircularProgressView (CGRect frame)
		{
			Frame = frame;
			BackgroundColor = UIColor.Clear;

			UIBezierPath outerCirclePath = UIBezierPath.FromOval (frame);
			outerCircleLayer = new CAShapeLayer ();
			outerCircleLayer.Bounds = Bounds;
			outerCircleLayer.Position = Center;
			outerCircleLayer.Path = outerCirclePath.CGPath;
			outerCircleLayer.FillColor = UIColor.Clear.CGColor;
			outerCircleLayer.LineWidth = 1.0f;
			outerCircleLayer.StrokeColor = UIColor.White.CGColor;
			outerCircleLayer.StrokeStart = 0.05f;
			outerCircleLayer.StrokeEnd = 1.0f;
			Layer.AddSublayer (outerCircleLayer);

			CABasicAnimation rotationAnimation = CABasicAnimation.FromKeyPath ("transform.rotation.z");
			rotationAnimation.To = NSNumber.FromDouble ( Math.PI * 2.0);
			rotationAnimation.Duration = 1;
			rotationAnimation.Cumulative = true;
			rotationAnimation.RepeatDuration = 3600;
			outerCircleLayer.AddAnimation (rotationAnimation, "rotationAnimation");

		}

		/// <summary>
		/// Updates the progress.
		/// </summary>
		/// <param name="progress">Progress. from 0f to 1.0f</param>
		public void UpdateProgress (float progress)
		{
			if (curProgress == 0) {
				Begin ();
			}

			if (progress < 1) {
				curProgress = progress;
				CATransaction.Begin ();
				progress %= 1.0f;//progress should be less than 1.0 and greater than 0
				progressLayer.StrokeEnd = progress;
				CATransaction.Commit ();
			} else {//If download process has been finished then remove current progress view
				RemoveFromSuperview ();
			}

		}

		/// <summary>
		/// When Download has begin, progress layer wil be showned
		/// </summary>
		private void Begin ()
		{
			outerCircleLayer.RemoveAnimation ("rotationAnimation");
			outerCircleLayer.StrokeStart = 0.0f;
			outerCircleLayer.StrokeEnd = 1f;

			UIBezierPath progressCirclePath = UIBezierPath.FromArc(Center, 
													(Frame.Width - 2) /2, //Radius
													(nfloat) (-Math.PI / 2), //start angle
													(nfloat)(1.5 * Math.PI), //end angle
													true
												);
			progressLayer = new CAShapeLayer ();
			progressLayer.Path = progressCirclePath.CGPath;
			progressLayer.FillColor   = UIColor.Clear.CGColor;
			progressLayer.LineWidth   = 3.0f;
			progressLayer.StrokeColor = UIColor.White.CGColor;
			progressLayer.StrokeStart = 0f;
			progressLayer.StrokeEnd   = 0f;

			//Circle progress view center
			CAShapeLayer centerRectLayer = new CAShapeLayer ();
			centerRectLayer.Bounds = new CGRect (0, 0, 10, 10);
			centerRectLayer.Position = Center;
			centerRectLayer.FillColor = UIColor.White.CGColor;
			centerRectLayer.BackgroundColor = UIColor.White.CGColor;

			Layer.AddSublayer (progressLayer);
			Layer.AddSublayer (centerRectLayer);
		}


			
	}
}

