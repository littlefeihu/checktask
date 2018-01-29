using System;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using System.Text.RegularExpressions;
using Android.Text.Util;

namespace LexisNexis.Red.Droid.TextStyle
{
	public static class FixedTextUtils
	{
		private static readonly IntPtr AndroidTextUtils;
		private static readonly IntPtr StaticEllipsize1;

		static FixedTextUtils()
		{
			try
			{
				AndroidTextUtils = JNIEnv.FindClass(typeof(TextUtils));
				StaticEllipsize1 = JNIEnv.GetStaticMethodID(
						AndroidTextUtils,
						"ellipsize",
						"(Ljava/lang/CharSequence;Landroid/text/TextPaint;FLandroid/text/TextUtils$TruncateAt;)Ljava/lang/CharSequence;");
			}
			catch(Exception ee)
			{
				AndroidTextUtils = IntPtr.Zero;
				StaticEllipsize1 = IntPtr.Zero;
			}
		}

		public static Java.Lang.ICharSequence Ellipsize(Java.Lang.ICharSequence text, TextPaint p, float avail, TextUtils.TruncateAt where)
		{
			if(AndroidTextUtils == IntPtr.Zero || StaticEllipsize1 == IntPtr.Zero)
			{
				return null;
			}

			var methodParams = new JValue[4];
			methodParams[0] = new JValue(text);
			methodParams[1] = new JValue(p);
			methodParams[2] = new JValue(avail);
			methodParams[3] = new JValue(where);
			return Java.Lang.Object.GetObject<Java.Lang.ICharSequence>(
				JNIEnv.CallStaticObjectMethod(AndroidTextUtils, StaticEllipsize1, methodParams),
				JniHandleOwnership.TransferLocalRef);
		}

		public static ISpannable CastToSpannable(Java.Lang.ICharSequence charSequence)
		{
			try
			{
				return charSequence.JavaCast<ISpannable>();
			}
			catch(System.Reflection.TargetInvocationException)
			{
			}

			return null;
		}

		public static CharacterStyle FindSpan(ISpanned text, Java.Lang.Object[] spans, int startIndex)
		{
			foreach(var span in spans)
			{
				if(startIndex == text.GetSpanStart(span))
				{
					return span.JavaCast<CharacterStyle>();
				}
			}

			return null;
		}

		public static MatchOptions AutoLinkNone()
		{
			return (MatchOptions)0;
		}

		public static bool IsAutoLinkNone(MatchOptions autoLinkMask)
		{
			return autoLinkMask == (MatchOptions)0;
		}

		public static void RemoveLinkUnderline(SpannableStringBuilder text)
		{
			var allURLSpans = text.GetSpans(
				0, text.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));

			foreach(var span in allURLSpans)
			{
				int startIndex = text.GetSpanStart(span);
				int endIndex = text.GetSpanEnd(span);
				text.RemoveSpan(span);

				text.SetSpan(new NoUnderlineURLSpan(span.JavaCast<URLSpan>().URL), startIndex, endIndex, 0);
			}
		}

		/*
		public static ISpanned LinkEmail(ISpanned text, bool underline = true)
		{
			const string MatchEmailPattern = @"(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
				+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
				+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
				+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})";
			Regex rx = new Regex(MatchEmailPattern,  RegexOptions.Compiled | RegexOptions.IgnoreCase);
			MatchCollection matches = rx.Matches(text.ToString());

			SpannableStringBuilder ssb = new SpannableStringBuilder(text);
			foreach (Match match in matches)
			{
				if(underline)
				{
					ssb.SetSpan(new URLSpan("mailto:" + match.Value), match.Index, match.Index + match.Length, 0);
				}
				else
				{
					ssb.SetSpan(new NoUnderlineURLSpan("mailto:" + match.Value), match.Index, match.Index + match.Length, 0);
				}
			}

			return ssb;
		}

		public static ISpanned LinkWebAndEmail(ISpanned text, bool underline = true)
		{
			const string MatchEmailPattern = @"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)";
			Regex rx = new Regex(MatchEmailPattern,  RegexOptions.Compiled | RegexOptions.IgnoreCase);
			MatchCollection matches = rx.Matches(text.ToString());

			SpannableStringBuilder ssb = new SpannableStringBuilder(text);
			foreach (Match match in matches)
			{
				string link = match.Value.IndexOf('@') < 0 ? match.Value : "mailto:" + match.Value;
				if(underline)
				{
					ssb.SetSpan(new URLSpan(link), match.Index, match.Index + match.Length, 0);
				}
				else
				{
					ssb.SetSpan(new NoUnderlineURLSpan(link), match.Index, match.Index + match.Length, 0);
				}
			}

			return ssb;
		}
		*/
	}
}

