using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    public class DlUpdateStatus
    {
        [JsonProperty("status")]
        public bool Status { get; set; }
    }
}
