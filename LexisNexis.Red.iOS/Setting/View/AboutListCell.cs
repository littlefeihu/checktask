
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class AboutListCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("AboutListCell");

		public AboutListCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
		}
	}
}

