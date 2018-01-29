using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Impl
{
    public class Network : INetwork
    {
        public Common.BusinessModel.NetworkTypeEnum GetNetworkType()
        {
            return NetworkTypeEnum.Normal;
        }

        public long GetFlowLimitation()
        {
            return 1000000;
        }
    }
}
