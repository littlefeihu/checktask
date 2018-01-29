using LexisNexis.Red.Common.BusinessModel;
namespace LexisNexis.Red.Common.Interface
{
    public interface INetwork
    {
        /// <summary>
        /// get current network type
        /// </summary>
        /// <returns></returns>
        NetworkTypeEnum GetNetworkType();

        /// <summary>
        /// byte length
        /// </summary>
        /// <returns></returns>
        long GetFlowLimitation();

    }
}
