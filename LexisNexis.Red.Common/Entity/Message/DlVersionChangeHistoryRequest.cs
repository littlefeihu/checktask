using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    /// <summary>
    /// Used to hold dl currrent version request datacontract.
    /// </summary>
    public class DlVersionChangeHistoryRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("deviceid")]
        public string DeviceId { get; set; }
        [JsonProperty("dlid")]
        public int DlId { get; set; }
        [JsonProperty("ver")]
        public int Ver { get; set; }
    }
}
