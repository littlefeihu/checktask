using System;
using Android.Util;

namespace LexisNexis.Red.Droid.Utility
{
	public static class PreventMultipleUIOperation
	{
		private const long Threshold = 500;
		private static long lastUIOperation = 0;
		private static long additionalThreshold = 0;

		public static bool IsValid(long additional = 0)
		{
			var now = DateTimeHelper.GetTimeStamp();
			var result = Math.Abs(now - lastUIOperation) > (Threshold + additionalThreshold);
			if(result)
			{
				lastUIOperation = now;
				additionalThreshold = additional;
			}

			return result;
		}
	}
}

