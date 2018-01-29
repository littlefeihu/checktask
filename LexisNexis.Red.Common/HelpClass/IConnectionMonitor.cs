using System.Threading;
using System.Threading.Tasks;
namespace LexisNexis.Red.Common.Interface
{
    public interface IConnectionMonitor
    {
        /// <summary>
        ///  check the net status
        /// </summary>
        /// <param name="countryCode">if countrycode is null then countrycode refer to  currentuser'countrycode </param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        Task<bool> PingService(string countryCode, CancellationToken cancellationToken = default(CancellationToken));

    }
}
