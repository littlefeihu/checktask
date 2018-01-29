using System;
using Android.Text.Style;
using Android.Graphics;
using Android.Widget;
using Android.Text;

namespace LexisNexis.Red.Droid.TextStyle
{
	public class NoUnderlineURLSpan: URLSpan
	{
		private readonly Action<string> clickHandler;
		private readonly Color? color;

		public NoUnderlineURLSpan(string url, Color? color = null, Action<string> clickHandler = null) : base(url)
		{
			this.clickHandler = clickHandler;
			this.color = color;
		}

		public override void UpdateDrawState(TextPaint ds)
		{
			ds.Color = color ?? new Color(ds.LinkColor);
			ds.UnderlineText = false;
		}

		public override void OnClick(Android.Views.View widget)
		{
			if(clickHandler != null)
			{
				clickHandler(this.URL);
			}
			else
			{
				base.OnClick(widget);
			}
		}
	}
}

