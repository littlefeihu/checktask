using System;
using AppKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class ColorTagButton : NSButton
	{
		public bool IsSelected { get; set; }
		public float OuterRadius { get; set; }
		public float InnerRadius { get; set; }
		public string InnerCircleColor { get; set; }
		public string OuterCircleColor  { get; set; }

		public ColorTagButton(float outerRadius, float innerRadius, 
			string outerCircleColor, string innerCircleColor, 
			CGRect frame, bool isSelected) : base (frame)
		{
			this.OuterRadius = outerRadius;
			this.InnerRadius = innerRadius;
			this.InnerCircleColor = innerCircleColor;
			this.OuterCircleColor = outerCircleColor;
				
			this.IsSelected = isSelected;
			this.Title = "";
			this.Cell.BezelStyle = NSBezelStyle.RegularSquare;
			this.Cell.SetButtonType (NSButtonType.MomentaryChange);
			//this.Cell.Bordered = true;

			var left = (Frame.Width - 2 * outerRadius) / 2;
			var top = (Frame.Height - 2 * outerRadius) / 2;

			NSView outerCircleView = new NSView (new CGRect(left, top, outerRadius*2, outerRadius*2));
			outerCircleView.WantsLayer = true;
			outerCircleView.Layer.BackgroundColor = Utility.ColorWithHexColorValue(outerCircleColor,1.0f).CGColor;
			outerCircleView.Layer.CornerRadius = outerRadius ;
			AddSubview (outerCircleView);

			NSView middleCircleView = new NSView (new CGRect (left + 1, top + 1, 2 * (outerRadius - 1), 2 * (outerRadius - 1)));
			middleCircleView.WantsLayer = true;
			middleCircleView.Layer.BackgroundColor = NSColor.White.CGColor;
			middleCircleView.Layer.CornerRadius = outerRadius - 1;
			AddSubview (middleCircleView);

			middleCircleView.Hidden = !isSelected;

			//create inner circle
			NSView innerCircleView = new NSView (new CGRect(left+outerRadius-innerRadius, top+outerRadius-innerRadius, innerRadius*2, innerRadius*2 ));
			innerCircleView.WantsLayer = true;
			innerCircleView.Layer.BackgroundColor = Utility.ColorWithHexColorValue(innerCircleColor,1.0f).CGColor;
			innerCircleView.Layer.CornerRadius = innerRadius;
			AddSubview (innerCircleView);

			innerCircleView.Hidden = !isSelected;
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			NSGraphicsContext.GlobalSaveGraphicsState();
			NSColor.White.Set ();
			NSGraphics.RectFill (dirtyRect);

			Subviews[1].Hidden = !this.IsSelected;
			Subviews [2].Hidden = !this.IsSelected;
			NSGraphicsContext.GlobalRestoreGraphicsState();
		}
	}
}

