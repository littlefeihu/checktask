using System;
using System.Collections.Generic;
using Android.Graphics;
using Android.Content;
using LexisNexis.Red.Droid.App;

namespace LexisNexis.Red.Droid.Utility
{
	public static class CustomFontsCache
	{
		private static readonly Dictionary<string, Typeface> fontList = new Dictionary<string, Typeface>();

		public static Typeface GetTypeface(string fontName)
		{
			Typeface result = null;
			if(!fontList.TryGetValue(fontName, out result))
			{
				result = Typeface.CreateFromAsset(MainApp.ThisApp.Assets, "fonts/" + fontName);
				fontList[fontName] = result;
			}

			return result;
		}
	}
}

