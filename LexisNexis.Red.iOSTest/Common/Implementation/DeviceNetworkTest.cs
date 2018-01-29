using System;

using LexisNexis.Red.iOS;
namespace LexisNexis.Red.iOSTest
{
	public class DeviceNetworkTest
	{
		public DeviceNetwork iOSNetwork;
		public DeviceNetworkTest ()
		{
			iOSNetwork = new DeviceNetwork ();
		}

		public void GetNetworkTypeTest()
		{
			iOSNetwork.GetNetworkType ();
			
		}

		static void Main (string[] args)
		{
			
			DeviceNetworkTest test = new DeviceNetworkTest ();
			test.GetNetworkTypeTest (); 
		}
	}
}

