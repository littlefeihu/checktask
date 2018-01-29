using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    public class DlMetadata
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("deviceid")]
        public string DeviceId { get; set; }
        [JsonProperty("dlid")]
        public int DlId { get; set; }
    }
}
