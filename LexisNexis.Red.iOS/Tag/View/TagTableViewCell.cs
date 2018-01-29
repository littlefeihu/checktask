
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class TagTableViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("TagTableViewCell");

		public TagTableViewCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

