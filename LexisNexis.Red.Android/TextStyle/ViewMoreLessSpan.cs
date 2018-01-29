using System;
using Android.Text.Style;
using Android.Views;
using Android.Text;

namespace LexisNexis.Red.Droid.TextStyle
{
	public class ViewMoreLessSpan: ClickableSpan
	{
		private readonly Action<View> click;

		public ViewMoreLessSpan(Action<View> click)
		{
			this.click = click;
		}

		public override void OnClick(View widget)
		{
			if (click != null)
				click(widget);
		}

		public override void UpdateDrawState(TextPaint ds)
		{
			ds.UnderlineText = false;
		}
	}
}

