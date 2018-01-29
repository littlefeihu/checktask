using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Content.Res;

namespace LexisNexis.Red.Droid.Widget.Layout
{
	public class BoundedFrameLayout: FrameLayout
	{
		private readonly int boundedWidth;
		private readonly int boundedHeight;

		public BoundedFrameLayout(Context context, IAttributeSet attrs = null, int defStyle = 0)
			:base(context, attrs, defStyle)
		{
			TypedArray a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.BoundedFrameLayout, 0, 0);
			try
			{
				boundedWidth = a.GetInt(Resource.Styleable.BoundedFrameLayout_bounded_width, 0);
				boundedHeight = a.GetInt(Resource.Styleable.BoundedFrameLayout_bounded_height, 0);
			}
			finally
			{
				a.Recycle();
			}
		}

		public BoundedFrameLayout(Context context, IAttributeSet attrs = null)
			:base(context, attrs)
		{
			TypedArray a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.BoundedFrameLayout, 0, 0);
			try
			{
				boundedWidth = a.GetDimensionPixelSize(Resource.Styleable.BoundedFrameLayout_bounded_width, 0);
				boundedHeight = a.GetDimensionPixelSize(Resource.Styleable.BoundedFrameLayout_bounded_height, 0);
			}
			finally
			{
				a.Recycle();
			}
		}

		public BoundedFrameLayout(Context context)
			:base(context)
		{
			boundedWidth = 0;
			boundedHeight = 0;
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			// Adjust width as necessary
			int measuredWidth = MeasureSpec.GetSize(widthMeasureSpec);
			if(boundedWidth > 0 && boundedWidth < measuredWidth) {
				var measureMode = MeasureSpec.GetMode(widthMeasureSpec);
				widthMeasureSpec = MeasureSpec.MakeMeasureSpec(boundedWidth, measureMode);
			}

			// Adjust height as necessary
			int measuredHeight = MeasureSpec.GetSize(heightMeasureSpec);
			if(boundedHeight > 0 && boundedHeight < measuredHeight) {
				var measureMode = MeasureSpec.GetMode(heightMeasureSpec);
				heightMeasureSpec = MeasureSpec.MakeMeasureSpec(boundedHeight, measureMode);
			}

			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
		}
	}
}

