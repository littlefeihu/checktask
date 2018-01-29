using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    public class PingStatus
    {
        [JsonProperty("state")]
        public bool Status { get; set; }
    }
}
