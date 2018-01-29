using System;
using Android.Widget;
using Android.Util;
using Android.Content.Res;
using Android.Graphics;
using System.Collections.Generic;
using LexisNexis.Red.Droid.Utility;

namespace LexisNexis.Red.Droid.EnhancedTextView
{
	public class CustomFontTextView: TextView
	{
		public CustomFontTextView(Android.Content.Context context) : base(context)
		{
			init(null);
		}

		public CustomFontTextView(Android.Content.Context context, IAttributeSet attrs) : base(context, attrs)
		{
			init(attrs);
		}

		public CustomFontTextView(Android.Content.Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			init(attrs);
		}
		

		public CustomFontTextView(Android.Content.Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
		{
			init(attrs);
		}

		private void init(IAttributeSet attrs)
		{
			if (attrs!=null)
			{
				TypedArray a = Context.ObtainStyledAttributes(attrs, Resource.Styleable.CustomFontTextView);
				String fontName = a.GetString(Resource.Styleable.CustomFontTextView_fontName);
				if(fontName!=null)
				{
					this.Typeface = CustomFontsCache.GetTypeface(fontName);
				}

				a.Recycle();
			}
		}
	}
}

