using System;

namespace LexisNexis.Red.Droid.Widget.ExpandableTextView
{
	public class AutoLinkOptions
	{
		public Android.Text.Util.MatchOptions AutoLinkMask
		{
			get;
			set;
		}

		public bool IsLinkUnderline
		{
			get;
			set;
		}

		public AutoLinkOptions(Android.Text.Util.MatchOptions autoLinkMask, bool isLinkUnderline)
		{
			AutoLinkMask = autoLinkMask;
			IsLinkUnderline = isLinkUnderline;
		}
	}
}

