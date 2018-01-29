using Windows.Networking.Connectivity;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Interface;

namespace LexisNexis.Red.WindowsStore.Implementation
{
    public class Network : INetwork
    {

        public NetworkTypeEnum GetNetworkType()
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile != null)
            {
                var ianaInterfaceType = profile.NetworkAdapter.IanaInterfaceType;
                if (ianaInterfaceType == 243 || ianaInterfaceType == 244)
                {
                    return NetworkTypeEnum.Mobile;
                }
                return NetworkTypeEnum.Normal;
            }
            return NetworkTypeEnum.None;
            //return NetworkTypeEnum.Mobile;
        }

        public long GetFlowLimitation()
        {
            return 20971520L;
        }
    }
}
