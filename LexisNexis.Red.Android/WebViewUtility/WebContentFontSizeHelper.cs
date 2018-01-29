using System;

namespace LexisNexis.Red.Droid.WebViewUtility
{
	public static class WebContentFontSizeHelper
	{
		private const int ZoomSmall = 88;
		private const int ZoomNormal = 100;
		private const int ZoomLarge = 122;

		public static WebContentFontSize ParseSize(int sizeFromApi)
		{
			return sizeFromApi <= 0 ? WebContentFontSize.Normal : (WebContentFontSize)sizeFromApi;
		}

		public static int ParseZoom(WebContentFontSize size)
		{
			switch(size)
			{
			case WebContentFontSize.Small:
				return ZoomSmall;
			case WebContentFontSize.Normal:
				return ZoomNormal;
			case WebContentFontSize.Large:
				return ZoomLarge;
			}

			return ZoomNormal;
		}

		public static int ParseZoom(int sizeFromApi)
		{
			return ParseZoom(ParseSize(sizeFromApi));
		}
	}
}

