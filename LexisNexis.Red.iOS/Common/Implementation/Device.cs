using System;
using UIKit;

using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS.Common.Implementation
{
	public class Device:IDevice
	{
		#region IDevice implementation
		public string GetDeviceID ()
		{
			var nsUid = UIDevice.CurrentDevice.IdentifierForVendor;
			var udid = nsUid.AsString ();
			return udid;
		}

		/// <summary>
		/// GetDevice OS information
		/// </summary>
		/// <returns>"iPhone OS 8.2.1 e.g."</returns>
		public string GetDeviceOS ()
		{
			return UIDevice.CurrentDevice.SystemName + UIDevice.CurrentDevice.SystemVersion;
		}

		//TODO should not defined so
		//The return type should be a string
		public DeviceTypeEnum GetDeviceType()
		{
			return DeviceTypeEnum.iPad;
		}

		//This function should be in common project
		public string GetEreaderVersion()
		{
			return "3.0";
		}

		#endregion
	}
}

