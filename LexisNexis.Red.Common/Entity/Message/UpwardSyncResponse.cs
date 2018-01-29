using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{

    public class UpwardSyncResponse
    {
        [JsonProperty("syncstatus")]
        public List<DLSyncStatus> SyncStatus { get; set; }
    }

    public class DLSyncStatus
    {
        [JsonProperty("dlid")]
        public int DlId { get; set; }
        [JsonProperty("status")]
        public bool Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }

}
