using System;

namespace LexisNexis.Red.Droid.Utility
{
	public static class DateTimeHelper
	{
		public static long GetTimeStamp()
		{
			return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		}
	}
}

