using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;

namespace LexisNexis.Red.Droid.Widget.Layout
{
	public class InterceptTouchLinearLayout: LinearLayout
	{
		private Func<MotionEvent, bool> handler;

		public InterceptTouchLinearLayout(Context context, IAttributeSet attrs = null, int defStyle = 0)
			:base(context, attrs, defStyle)
		{

		}

		public InterceptTouchLinearLayout(Context context, IAttributeSet attrs = null)
			:base(context, attrs)
		{

		}

		public InterceptTouchLinearLayout(Context context)
			:base(context)
		{
		}

		public void SetOnInterceptTouchHanlder(Func<MotionEvent, bool> handler)
		{
			this.handler = handler;
		}

		public override bool OnInterceptTouchEvent(MotionEvent ev)
		{
			return handler(ev) || base.OnInterceptTouchEvent(ev);
		}
	}
}

