using System;

using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public class AllTagView : UIView
	{
		public AllTagView (float height, float frameX, float frameY ) : base(new CGRect(frameX, frameY, height * 2, height))
		{
			float outerRadius = height / 2;
			float innerRadius = outerRadius - 1;

			DoubleCircleView greenCircle  = new DoubleCircleView(innerRadius, outerRadius, UIColor.Green, UIColor.White, 0, 0);
			DoubleCircleView yellowCircle  = new DoubleCircleView(innerRadius, outerRadius, ColorUtil.ConvertFromHexColorCode("#EEC900"), UIColor.White, outerRadius, 0);
			DoubleCircleView redCircle  = new DoubleCircleView(innerRadius, outerRadius, UIColor.Red, UIColor.White, 2 * outerRadius, 0);

			AddSubview (redCircle);
			AddSubview (yellowCircle);
			AddSubview (greenCircle);
		}
	}
}

