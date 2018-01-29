
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	public class AnnotationTableViewCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString ("AnnotationTableViewCell");

		public AnnotationTableViewCell () : base (UITableViewCellStyle.Value1, Key)
		{
			// TODO: add subviews to the ContentView, set various colors, etc.
			TextLabel.Text = "TextLabel";
		}
	}
}

