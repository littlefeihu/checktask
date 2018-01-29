using System;
using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.Implementation
{
	public class AndroidDevice: IDevice
	{
		#region IDevice implementation

		public string GetDeviceID()
		{
			return MainApp.DeviceId;
		}

		public string GetDeviceOS()
		{
			return Android.OS.Build.Model
				+ " - " + Android.OS.Build.VERSION.Sdk
				+ " - " + Android.OS.Build.VERSION.Release;
		}

		public DeviceTypeEnum GetDeviceType()
		{
			return DeviceTypeEnum.Android;
		}

		public string GetEreaderVersion()
		{
			return "2.1";
		}
		#endregion
	}
}

