using System;
using Android.Widget;

namespace LexisNexis.Red.Droid.Widget.ExpandableTextView
{
	public interface ISimpleExpandableTextViewListerner
	{
		void LineCountDetected(TextView tv);
	}
}

