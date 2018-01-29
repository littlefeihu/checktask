using System;

using AppKit;
using Foundation;
using CoreGraphics;
using PdfKit;

namespace LexisNexis.Red.Mac
{
	public class FooterPdfPage:PdfPage
	{
		NSImage PdfImage { get; set;} 
		public string PageHeader{get; set;}
		public string PageFooter{get; set;}
		[Export ("initWithImage:")]
		public FooterPdfPage (NSImage image) : base (image)
		{
			PdfImage = image;
		}

		public override string Label {
			get {
				return PageHeader;
			}
		}

		public override CGRect GetBoundsForBox (PdfDisplayBox box)
		{
			return base.GetBoundsForBox (box);
		}

		public override void Draw (PdfDisplayBox box)
		{
			//base.Draw (box);


			CGRect sourceRect = new CGRect (0,0,0,0);
			CGRect topHalf;
			CGRect destRect;
			// Drag image.
			// ...........
			// Source rectangle.
			sourceRect.Size = PdfImage.Size;

			// Represent the top half of the page.
			topHalf = GetBoundsForBox(box);

			Utility.ColorWithHexColorValue ("#ffffff",1.0f).Set ();
			NSGraphics.RectFill (topHalf);

			// Scale and center image within top half of page.
			destRect = sourceRect;
			destRect.Height -= 120;
			destRect.Y += 60;

			// Draw.
			//Console.WriteLine("left:{0},top:{1},width:{2},height:{3}",destRect.X,destRect.Y,destRect.Width,destRect.Height);
			PdfImage.DrawInRect(destRect,new CGRect(0,0,0,0),NSCompositingOperation.SourceOver,1.0f);


			// Draw name.
			// ...........
			destRect = GetBoundsForBox(box);
			destRect.Y = destRect.Height-44.0f;
			destRect.Height = 34.0f;
			destRect.X = 30.0f;
			destRect.Width -= 60.0f;

			// Draw label.
			NSStringAttributes attributes = new NSStringAttributes ();
			attributes.Font = NSFont.SystemFontOfSize(11);
			Label.DrawInRect (destRect, attributes);

			// Draw name.
			// ...........
			destRect = GetBoundsForBox(box);
			destRect.Y = 10.0f;
			destRect.Height = 17.0f;
			destRect.X = 30.0f;
			destRect.Width -= 60.0f;

			PageFooter.DrawInRect (destRect,attributes);
		}

		static CGRect FitRectInRect (CGRect srcRect, CGRect destRect)
		{
			CGRect fitRect = new CGRect(0,0,0,0);

			// Assign.
			fitRect = srcRect;

			// Only scale down.
			if (fitRect.Width > destRect.Width) {
				nfloat		scaleFactor;

				// Try to scale for width first.
				scaleFactor = destRect.Width / fitRect.Width;
				fitRect.Width *= scaleFactor;
				fitRect.Height *= scaleFactor;

				// Did it pass the bounding test?
				if (fitRect.Height > destRect.Height)
				{
					// Failed above test -- try to scale the height instead.
					fitRect = srcRect;
					scaleFactor = destRect.Height / fitRect.Height;
					fitRect.Width *= scaleFactor;
					fitRect.Width *= scaleFactor;
				}
			} else if (fitRect.Height > destRect.Height) {
				nfloat		scaleFactor;

				// Scale based on height requirements.
				scaleFactor = destRect.Height / fitRect.Height;
				fitRect.Height *= scaleFactor;
				fitRect.Width *= scaleFactor;
			}

			// Center.
			fitRect.X = destRect.X + ((destRect.Width - fitRect.Width) / 2);
			fitRect.Y = destRect.Y + ((destRect.Height - fitRect.Height) / 2);

			// Assign back.
			return fitRect;
		}


	}
}

