using System;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Droid.Utility;

namespace LexisNexis.Red.Droid
{
	public class AndroidLogger: ILogger
	{
		public void Log(string message)
		{
			LogHelper.Debug("CommonDbg", message);
		}
	}
}

