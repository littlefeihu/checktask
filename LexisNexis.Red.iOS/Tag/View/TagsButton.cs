using System;
using UIKit;
using Foundation;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public class TagsButton:UIButton
	{
		public UIView ColorView = new UIView();

		public UILabel ColorLabel = new UILabel();

 		public TagsButton ()
		{
			this.BackgroundColor = UIColor.White;
			ColorView.Frame = new CGRect (0, 10, 10,10);
			ColorView.Layer.BorderColor = UIColor.Clear.CGColor;
			ColorView.Layer.BorderWidth = 1;
			ColorView.Layer.CornerRadius = 10/2;
			ColorView.ClipsToBounds = true;
			this.AddSubview (ColorView);//((TagHeight-96)*4)  (tableView.Frame.Size.Width/5)

			ColorLabel.Frame = new CGRect (15,5,58,20);
			ColorLabel.Font = UIFont.SystemFontOfSize(14);
			ColorLabel.BackgroundColor=UIColor.Clear;
			ColorLabel.TextColor = ColorUtil.ConvertFromHexColorCode ("#cccccc");
			ColorLabel.TextAlignment = UITextAlignment.Left;
 			this.AddSubview (ColorLabel);


		}
	}
}

