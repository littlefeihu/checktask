using System;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Droid
{
	public static class LastSyncedTimeHelper
	{
		public static DateTime Get()
		{
			if(GlobalAccess.Instance.CurrentUserInfo == null)
			{
				return DateTime.Now;
			}

			return SettingsUtil.Instance.GetLastSyncedTime();
		}
	}
}

