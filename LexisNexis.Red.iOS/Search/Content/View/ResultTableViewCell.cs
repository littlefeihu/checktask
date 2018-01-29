
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class ResultTableViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("ResultTableViewCell");

		public ResultTableViewCell () : base (UITableViewCellStyle.Subtitle, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

