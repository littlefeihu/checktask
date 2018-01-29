using System;

using Foundation;
using UIKit;
using CoreGraphics;
 
namespace LexisNexis.Red.iOS
{
	public class TagColorCollectionViewCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("TagColorCollectionViewCell");

		public UIView OptionalColorViewContainer;

		[Export ("initWithFrame:")]
		public TagColorCollectionViewCell (CGRect frame) : base (frame)
		{

			UIColor lineColor = ColorUtil.ConvertFromHexColorCode ("#E1E3E0");

			ContentView.Layer.BorderColor = lineColor.CGColor;
			ContentView.Layer.BorderWidth = 0.5f;
			ContentView.SetNeedsDisplay ();

			OptionalColorViewContainer = new UIView (new CGRect(0, 0, ViewConstant.TAG_OUTER_CIRCLE_RADIUS * 2, ViewConstant.TAG_OUTER_CIRCLE_RADIUS * 2));
			OptionalColorViewContainer.BackgroundColor = UIColor.Clear;
			OptionalColorViewContainer.Center = ContentView.Center;
			ContentView.AddSubview (OptionalColorViewContainer);
		}

		public void SetOptionColorView (UIView view)
		{
			if (OptionalColorViewContainer.Subviews != null && OptionalColorViewContainer.Subviews.Length > 0) {
				foreach (var subview in OptionalColorViewContainer.Subviews) {
					subview.RemoveFromSuperview ();
				}
			}
			OptionalColorViewContainer.AddSubview (view);
		}



	}
}

