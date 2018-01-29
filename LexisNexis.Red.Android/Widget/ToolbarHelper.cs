using System;
using Android.Util;
using LexisNexis.Red.Droid.App;
using System.Text;
using System.Security.Cryptography;
using LexisNexis.Red.Droid.Utility;
using Newtonsoft.Json;

namespace LexisNexis.Red.Droid.Widget
{
	public static class ToolbarHelper
	{
		public const string CacheCatagory = "Globle";
		public const string CacheFile = "ToolbarParams.cache";
		private static ToolbarParams tbParams = null;

		public class ToolbarParams
		{
			public int Height;
		}

		public static int GetToolbarHeight()
		{
			if(tbParams != null && tbParams.Height > 0)
			{
				return tbParams.Height;
			}

			var toolbarParamsString = FileCacheHelper.ReadCacheFile(CacheCatagory, CacheFile);
			if(string.IsNullOrEmpty(toolbarParamsString))
			{
				return 0;
			}

			tbParams = JsonConvert.DeserializeObject<ToolbarParams>(toolbarParamsString);
			return tbParams.Height;
		}

		public static void SetToolbarHeight(int height)
		{
			if(tbParams == null)
			{
				tbParams = new ToolbarParams();
			}

			tbParams.Height = height;

			FileCacheHelper.SaveCacheFile(
				CacheCatagory,
				CacheFile,
				JsonConvert.SerializeObject(tbParams));
		}
	}
}

