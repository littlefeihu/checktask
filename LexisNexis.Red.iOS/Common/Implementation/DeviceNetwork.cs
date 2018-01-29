using System;
using System.Collections.Generic;
using SystemConfiguration;

using Connectivity.Plugin;
using Connectivity.Plugin.Abstractions;

using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	public class DeviceNetwork : INetwork
	{
		public DeviceNetwork ()
		{
		}

		#region INetwork implementation
		/// <summary>
		/// get current network type
		/// </summary>
		/// <returns></returns>
		public NetworkTypeEnum GetNetworkType ()
		{
			NetworkStatus remoteHostStatus = Reachability.RemoteHostStatus ();

			if (remoteHostStatus == NetworkStatus.NotReachable) {
				return NetworkTypeEnum.None;
			} else if (remoteHostStatus == NetworkStatus.ReachableViaWiFiNetwork) {
				return NetworkTypeEnum.Normal;
			} else {
				return NetworkTypeEnum.Mobile;
			}
		}

		/// <summary>
		/// Get max size user could download when user are using cellular network
		/// </summary>
		/// <returns></returns>
		public long GetFlowLimitation ()
		{
			return 20971520;//20 MB, calculated with 20 * 1024 * 1024
		}

		#endregion
	}
}

