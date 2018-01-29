
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class PublicationSortingCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("PublicationSortingCell");

		public int PublicationId{ get; set;}

		public PublicationSortingCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

