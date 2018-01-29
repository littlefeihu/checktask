using System;

using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public class LNBadageBarButtonItem : UIBarButtonItem
	{
		public UILabel BadageLabel{ get; set;}


		public LNBadageBarButtonItem (UIView view) : base(view)
		{
		}

		 
		public void SetBadage (int num)
		{
			if (BadageLabel == null) {
				BadageLabel = new UILabel (new CGRect(ViewConstant.BADAGE_ORIGIN_X, ViewConstant.BADAGE_ORIGIN_Y, ViewConstant.BADAGE_SIZE_DIAMETER, ViewConstant.BADAGE_SIZE_DIAMETER));	
				BadageLabel.BackgroundColor = UIColor.Red;
				BadageLabel.TextColor = UIColor.White;
				BadageLabel.TextAlignment = UITextAlignment.Center;
				BadageLabel.Font = UIFont.SystemFontOfSize (ViewConstant.BADAGE_FONT_SIZE);
				BadageLabel.Layer.CornerRadius = ViewConstant.BADAGE_SIZE_DIAMETER / 2;
				BadageLabel.Layer.MasksToBounds = true;
			}

			if (num > 0) {
				BadageLabel.Text = num.ToString ();

				CustomView.AddSubview (BadageLabel);
			} else {
				BadageLabel.RemoveFromSuperview ();
			}
		}


	}
}

