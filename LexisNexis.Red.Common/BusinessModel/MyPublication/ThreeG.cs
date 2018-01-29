using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    internal class ThreeG
    {
        long localSize;
        INetwork networkService;
        internal ThreeG(INetwork networkService, long localSize)
        {
            this.localSize = localSize;
            this.networkService = networkService;
        }

        internal bool VerifyNetwork(ref DownloadResult result)
        {
            bool canContinue = false;
            var networkType = networkService.GetNetworkType();
            if (networkType == NetworkTypeEnum.Mobile)
            {
                var flowLimitations = networkService.GetFlowLimitation();
                if (localSize > flowLimitations)
                {
                    result.DownloadStatus = DownLoadEnum.OverLimitation;
                }
                else
                {
                    canContinue = true;
                }
            }
            else if (networkType == NetworkTypeEnum.None)
            {
                result.DownloadStatus = DownLoadEnum.NetDisconnected;
            }
            else
            {
                canContinue = true;
            }
            return canContinue;
        }
    }
}
