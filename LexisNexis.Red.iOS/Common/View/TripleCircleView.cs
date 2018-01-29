using System;

using UIKit;
using CoreGraphics;
using Foundation;
namespace LexisNexis.Red.iOS
{
	public class TripleCircleView:UIView
	{

		public float InnerRadius{ get; set;}
		public float OuterRadius{ get; set;}
		public string TagColor{ get; set;}

		public TripleCircleView(float innerRadius, float outerRadius, UIColor innerCircleColor, UIColor outerCircleColor, float frameX = 0, float frameY = 0) : base (new CGRect(frameX, frameY, 2 * outerRadius, 2 * outerRadius))
		{
			Layer.CornerRadius = outerRadius;
			BackgroundColor = innerCircleColor;

			UIView middleCircleView = new UIView (new CGRect(1, 1, 2 *(outerRadius - 1),  2 * (outerRadius  -1 )));
			middleCircleView.BackgroundColor = UIColor.White;
			middleCircleView.Layer.CornerRadius = outerRadius -1 ;
			AddSubview (middleCircleView);

			//create inner circle
			UIView innerCircleView = new UIView (new CGRect(outerRadius - innerRadius, outerRadius - innerRadius, innerRadius * 2, innerRadius * 2 ));
			innerCircleView.BackgroundColor = innerCircleColor;
			innerCircleView.Layer.CornerRadius = innerRadius;
			AddSubview (innerCircleView);
		}
	}
}

