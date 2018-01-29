using System;

namespace LexisNexis.Red.Droid.Utility
{
	public static class LogHelper
	{
		public static void Debug(string tag, string msg)
		{
			Android.Util.Log.Debug(tag, msg);
		}
	}
}

