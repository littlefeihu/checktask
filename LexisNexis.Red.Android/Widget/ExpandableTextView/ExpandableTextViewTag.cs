using System;
using Android.Text;

namespace LexisNexis.Red.Droid.Widget.ExpandableTextView
{
	public class ExpandableTextViewTag: Java.Lang.Object
	{
		public string Text
		{
			get;
			set;
		}

		public int LineCount
		{
			get;
			set;
		}

		public int MaxLines
		{
			get;
			set;
		}

		public ISpannable FormattedText
		{
			get;
			set;
		}

		public bool ClickSwitch
		{
			get;
			set;
		}

		public bool IsPartial
		{
			get;
			set;
		}

		public ISimpleExpandableTextViewListerner Listerner
		{
			get;
			set;
		}

		public AutoLinkOptions AutoLinkOptions
		{
			get;
			set;
		}


		public ViewMoreLessOptions ViewMoreLessOptions
		{
			get;
			set;
		}
	}
}

