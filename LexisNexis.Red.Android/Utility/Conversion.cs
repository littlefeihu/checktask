using System;
using Android.Util;
using LexisNexis.Red.Droid.App;
using System.Text;
using System.Security.Cryptography;

namespace LexisNexis.Red.Droid.Utility
{
	public static class Conversion
	{
		private const long MB = 1024 * 1024;
		private const long KB = 1024;

		public static int Dp2Px(int dp)
		{
			return (int)Math.Round(dp * MainApp.ThisApp.Resources.DisplayMetrics.Density);
		}

		public static int Px2Dp(int px)
		{
			return (int)Math.Round(px / MainApp.ThisApp.Resources.DisplayMetrics.Density);
		}

		public static string Byte2ReadableString(long bytes)
		{
			/*
			if(bytes > MB)
			{
				return Math.Round(((double)bytes) / ((double)MB), 1).ToString() + "MB";
			}
			else if(bytes > KB)
			{
				return Math.Round(((double)bytes) / ((double)KB), 1).ToString() + "KB";
			}
			else
			{
				return bytes + "B";
			}
			*/

			return Math.Round(((double)bytes) / ((double)MB), 1) + "MB";
		}

		public static long String2MD5Long(string text)
		{
			var result = Encoding.Default.GetBytes(text);
			MD5 md5 = new MD5CryptoServiceProvider();
			var output = md5.ComputeHash(result);
			return BitConverter.ToInt64(output, 0);
		}
	}
}

