
using System;

using Foundation;
using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public partial class GotoCollectionReusableView : UICollectionReusableView
	{
 
		[Export ("initWithFrame:")]
		public GotoCollectionReusableView (CGRect frame) : base (frame)
		{
 			BackgroundColor = UIColor.Red;
		}

	}
 
}

