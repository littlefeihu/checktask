
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class RegionsTableViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("RegionsTableViewCell");

		public RegionsTableViewCell () : base (UITableViewCellStyle.Default, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			//TextLabel.Text = "TextLabel";

		}
	}
}

