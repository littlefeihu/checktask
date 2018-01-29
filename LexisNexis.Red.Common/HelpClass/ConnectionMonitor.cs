using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using System.Threading;
using System.Threading.Tasks;
namespace LexisNexis.Red.Common.HelpClass
{
    public class ConnectionMonitor : ServiceBase, IConnectionMonitor
    {
        public ConnectionMonitor()
            : base(ServiceConfig.AUTHENTICATION_SERVICE)
        {

        }
        public async Task<bool> PingService(string countryCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            bool pingResult = false;
            var isIsNetworkAvailable = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            if (isIsNetworkAvailable)
            {
                pingResult = await ServiceAgent.RestFullPingServiceRequest(base.GetTargetUri(countryCode), ServiceConfig.IS_ALIVE, cancellationToken);
            }
            return pingResult;
        }
    }
}
