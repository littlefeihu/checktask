
using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public class GotoTableViewControllerCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("GotoTableViewControllerCell");

 		public UILabel contentLabel = new UILabel();
		public UILabel detailLabel = new UILabel();

 
		public GotoTableViewControllerCell () : base (UITableViewCellStyle.Value1, Key)
		{
 			contentLabel.Frame = new CGRect (20,10,280,15);
			contentLabel.Font = UIFont.BoldSystemFontOfSize(14);
			ContentView.AddSubview (contentLabel);
	
			detailLabel.Frame = new CGRect (20,35,280,15);
			detailLabel.Font = UIFont.SystemFontOfSize(14);
			ContentView.AddSubview (detailLabel);


  		}
	}
}

