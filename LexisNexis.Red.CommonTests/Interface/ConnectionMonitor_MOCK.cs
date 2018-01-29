using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.CommonTests.HelpClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Interface
{
    public class ConnectionMonitor_MOCK : IConnectionMonitor
    {
        public async Task<bool> PingService(string countrycode)
        {
            if (GlobalApp.isNetworkAvailable == true)
                return await Task.Run(() => { return true; });

            else if (GlobalApp.isNetworkAvailable == false)
                return await Task.Run(() => { return false; });
            else
                return await Task.Run(() => { return false; });
        }


        public Task<bool> PingService(string countryCode, System.Threading.CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
