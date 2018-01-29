using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

using Foundation;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Interface;


namespace LexisNexis.Red.Mac.Common.Implementation
{
	public class MacDevices:IDevice
	{
		#region IDevice implementation
		public string GetDeviceID()
		{
			string serialNumber;
			using (var p = new Process())
			{
				var serialRegex = new Regex("\"IOPlatformSerialNumber\" = \"(\\S+)\"");
				p.StartInfo = new ProcessStartInfo("/usr/sbin/ioreg",
					"-c IOPlatformExpertDevice");
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.Start();
				serialNumber = serialRegex.Match(p.StandardOutput.ReadToEnd()).Groups[1].Captures[0].Value;
				p.WaitForExit();
			}
				
			return serialNumber;
		}

		public string GetDeviceOS()
		{
			var system = NSProcessInfo.ProcessInfo.OperatingSystemName;
			var version = NSProcessInfo.ProcessInfo.OperatingSystemVersionString;

			return system + version;
		}

		public DeviceTypeEnum GetDeviceType()
		{
			return DeviceTypeEnum.MAC;
		}

		public string GetEreaderVersion()
		{
			return "3.0";
		}
			


		#endregion
	}
}

