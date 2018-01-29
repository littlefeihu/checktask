using System;
using Android.Text;

namespace LexisNexis.Red.Droid.TextStyle
{
	public static class SpannableStringBuilderHelper
	{
		public static void Appand(SpannableStringBuilder ssb, string text, Java.Lang.Object what, SpanTypes flags)
		{
			var start = ssb.Length();
			ssb.Append(text);
			ssb.SetSpan(what, start, ssb.Length(), flags);
		}
	}
}

