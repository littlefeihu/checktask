
using CoreGraphics;
using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public partial class GotoCollectionViewCell : UICollectionViewCell
	{
		public static readonly NSString Key = new NSString ("GotoCollectionViewCell");

		public UILabel titleLabe =new UILabel();
   
		[Export ("initWithFrame:")]
		public GotoCollectionViewCell (CGRect frame) : base (frame)
		{
			UIColor lineColor = ColorUtil.ConvertFromHexColorCode ("#C3C4C2");
			ContentView.Layer.BorderColor = lineColor.CGColor;
			ContentView.Layer.BorderWidth = 0.5f;
			ContentView.SetNeedsDisplay ();
  
			titleLabe.Frame = new CGRect (0, 0,106,44);
			titleLabe.Font = UIFont.SystemFontOfSize(24);
			titleLabe.TextAlignment = UITextAlignment.Center;
     		this.AddSubview (titleLabe);

 		}
	}
}

