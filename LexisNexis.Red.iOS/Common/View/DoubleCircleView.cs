 using System;

using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	/// <summary>
	/// Circle Tag view.
	/// </summary>
	public class DoubleCircleView : UIView
	{
		public float InnerRadius{ get; set;}
		public float OuterRadius{ get; set;}
		public string TagColor{ get; set;}

		public DoubleCircleView(float innerRadius, float outerRadius, UIColor innerCircleColor, UIColor outerCircleColor, float frameX = 0, float frameY = 0) : base (new CGRect(frameX, frameY, 2 * outerRadius, 2 * outerRadius))
		{
			Layer.CornerRadius = outerRadius;
			BackgroundColor = outerCircleColor;

			//create inner circle
			UIView innerCircleView = new UIView (new CGRect(outerRadius - innerRadius, outerRadius - innerRadius, innerRadius * 2, innerRadius * 2 ));
			innerCircleView.BackgroundColor = innerCircleColor;
			innerCircleView.Layer.CornerRadius = innerRadius;
			AddSubview (innerCircleView);
		}
	}
}

