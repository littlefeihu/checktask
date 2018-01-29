
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class HistoryTableViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("HistoryTableViewCell");

		public HistoryTableViewCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

