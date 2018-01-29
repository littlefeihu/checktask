using System;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.BusinessModel;
using Android.Net;
using LexisNexis.Red.Droid.App;

namespace LexisNexis.Red.Droid.Implementation
{
	public class AndroidNetwork: INetwork
	{
		public long GetFlowLimitation()
		{
			return 20 * 1024 * 1024;
		}

		public NetworkTypeEnum GetNetworkType()
		{
			var connectivityManager =
				(ConnectivityManager)MainApp.ThisApp.GetSystemService(Android.Content.Context.ConnectivityService);
			var activeConnection = connectivityManager.ActiveNetworkInfo;
			if(activeConnection == null || (!activeConnection.IsConnected))
			{
				return NetworkTypeEnum.None;
			}

			var wifiState = connectivityManager.GetNetworkInfo (ConnectivityType.Wifi).GetState ();
			if(wifiState == NetworkInfo.State.Connected)
			{
				return NetworkTypeEnum.Normal;
			}

			var mobileState = connectivityManager.GetNetworkInfo (ConnectivityType.Mobile).GetState ();
			if(mobileState == NetworkInfo.State.Connected)
			{
				return NetworkTypeEnum.Mobile;
			}

			return NetworkTypeEnum.None;
		}
	}
}

