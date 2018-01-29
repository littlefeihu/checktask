using System;
using Android.Widget;
using Android.Views;

namespace LexisNexis.Red.Droid.Widget.ExpandableTextView
{
	public class SimpleExpandableTextViewUtilty: Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
	{
		private readonly TextView tv;
		private int lineCount;
		private readonly int maxLines;
		private readonly ISimpleExpandableTextViewListerner listerner;
		private bool isPartial;
		//private readonly Android.Text.TextUtils.TruncateAt ellipsize;

		public SimpleExpandableTextViewUtilty(
			TextView tv, int maxLines, bool isPartial, ISimpleExpandableTextViewListerner listerner)
		{
			this.tv = tv;
			this.maxLines = maxLines;
			this.listerner = listerner;
			this.isPartial = isPartial;
			//ellipsize = Android.Text.TextUtils.TruncateAt.End;

			tv.Tag = this;
		}

		public static void MakeTextViewResizable(
			TextView tv, int maxLines, bool isPartial, ISimpleExpandableTextViewListerner listerner)
		{
			tv.ViewTreeObserver.AddOnGlobalLayoutListener(
				new SimpleExpandableTextViewUtilty(
					tv, maxLines, isPartial, listerner));
		}

		public static void Expand(TextView tv)
		{
			SimpleExpandableTextViewUtilty tag = tv.Tag as SimpleExpandableTextViewUtilty;
			if(tag == null)
			{
				throw new InvalidOperationException("Unable to handle a TextView without Tag.");
			}

			tag.isPartial = false;
			//tv.Ellipsize = null;
			tv.SetMaxLines(Int32.MaxValue);
		}

		public static void Collapse(TextView tv)
		{
			SimpleExpandableTextViewUtilty tag = tv.Tag as SimpleExpandableTextViewUtilty;
			if(tag == null)
			{
				throw new InvalidOperationException("Unable to handle a TextView without Tag.");
			}

			tag.isPartial = true;
			//tv.Ellipsize = tag.ellipsize;
			tv.SetMaxLines(tag.maxLines);
		}

		public static bool ExceedMaxLines(TextView tv)
		{
			SimpleExpandableTextViewUtilty tag = tv.Tag as SimpleExpandableTextViewUtilty;
			if(tag == null)
			{
				throw new InvalidOperationException("Unable to handle a TextView without Tag.");
			}

			return tag.lineCount > tag.maxLines;
		}

		public static bool IsPartial(TextView tv, bool defaultValue)
		{
			SimpleExpandableTextViewUtilty tag = tv.Tag as SimpleExpandableTextViewUtilty;
			if(tag == null)
			{
				return defaultValue;
			}

			return tag.isPartial;
		}

		public static bool IsPartial(TextView tv)
		{
			SimpleExpandableTextViewUtilty tag = tv.Tag as SimpleExpandableTextViewUtilty;
			if(tag == null)
			{
				throw new InvalidOperationException("Unable to handle a TextView without Tag.");
			}

			return tag.isPartial;
		}

		public static bool IsExpandable(TextView tv)
		{
			SimpleExpandableTextViewUtilty tag = tv.Tag as SimpleExpandableTextViewUtilty;
			return tag != null;
		}

		public void OnGlobalLayout()
		{
			ViewTreeObserver obs = tv.ViewTreeObserver;
			obs.RemoveOnGlobalLayoutListener(this);

			lineCount = tv.LineCount;
			if(isPartial && lineCount > maxLines)
			{
				//tv.Ellipsize = ellipsize;
				tv.SetMaxLines(maxLines);
				tv.RequestLayout();
			}

			listerner.LineCountDetected(tv);
		}
	}
}

