using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Content.Res;
using Android.Views;

namespace LexisNexis.Red.Droid.Widget.Layout
{
	public class FixedAspectRatioFrameLayout: FrameLayout
	{
		private const int DEFAULT_XRATIO = 1;
		private const int DEFAULT_YRATIO = 1;

		public int XRatio
		{
			get;
			set;
		}

		public int YRatio
		{
			get;
			set;
		}

		public FixedAspectRatioFrameLayout(Context context, IAttributeSet attrs = null, int defStyle = 0)
			:base(context, attrs, defStyle)
		{
			TypedArray a = context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.FixedAspectFrameLayout, 0, 0);
			try
			{
				XRatio = a.GetInt(Resource.Styleable.FixedAspectFrameLayout_xRatio, DEFAULT_XRATIO);
				YRatio = a.GetInt(Resource.Styleable.FixedAspectFrameLayout_yRatio, DEFAULT_YRATIO);
			}
			finally
			{
				a.Recycle();
			}
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

			var widthMode = MeasureSpec.GetMode(widthMeasureSpec);
			var heightMode = MeasureSpec.GetMode(heightMeasureSpec);

			int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
			int heightSize = MeasureSpec.GetSize(heightMeasureSpec);

			if(widthMode == MeasureSpecMode.Exactly
				&& (heightMode == MeasureSpecMode.AtMost || heightMode == MeasureSpecMode.Unspecified))
			{
				SetMeasuredDimension(widthSize, (int)((double)widthSize / XRatio * YRatio));
			}
			else if(heightMode == MeasureSpecMode.Exactly
				&& (widthMode == MeasureSpecMode.AtMost || widthMode == MeasureSpecMode.Unspecified))
			{
				SetMeasuredDimension((int)((double)heightSize / YRatio * XRatio), heightSize);
			}
			else
			{
				SetMeasuredDimension(widthMeasureSpec, heightMeasureSpec);
			}
		}
	}
}

