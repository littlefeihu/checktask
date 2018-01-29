using System;

using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public class TagFilterView : UIView
	{
		public TagFilterView (float radius, float frameX, float frameY ) : base(new CGRect(frameX, frameY, radius * 2, radius * 2))
		{
			Layer.CornerRadius = radius;
			BackgroundColor = UIColor.Red;

			float outerRadius = 6;
			float innerRadius = outerRadius - 1;

			DoubleCircleView first  = new DoubleCircleView(innerRadius, outerRadius, UIColor.White, UIColor.Red, (float)(outerRadius / 2), radius - outerRadius);
			DoubleCircleView second  = new DoubleCircleView(innerRadius, outerRadius, UIColor.White, UIColor.Red, (float)(outerRadius * 1.5), radius - outerRadius);
			DoubleCircleView third  = new DoubleCircleView(innerRadius, outerRadius, UIColor.White, UIColor.Red, (float)(2.5 * outerRadius), radius - outerRadius);

			AddSubview (third);
			AddSubview (second);
			AddSubview (first);
		}
	}
}

