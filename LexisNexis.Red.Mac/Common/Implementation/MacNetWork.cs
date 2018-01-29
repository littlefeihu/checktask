using System;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac.Common.Implementation
{
	public class MacNetWork:INetwork
	{
		#region INetwork implemention

		public NetworkTypeEnum GetNetworkType()
		{
			return NetworkTypeEnum.Normal;
		}

		public long GetFlowLimitation()
		{
		    return 1024;
		}

		#endregion
	}

}


