using Newtonsoft.Json;
using System.Collections.Generic;

namespace LexisNexis.Red.Common.Entity
{
    public class DownwardSyncRequests
    {
        [JsonProperty("downwardsyncrequest")]
        public List<DownwardSyncRequest> DownwardSyncRequest { get; set; }
    }
}
