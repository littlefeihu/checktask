
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class TableOfContentTableViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("TableOfContentTableViewCell");

		public TableOfContentTableViewCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

