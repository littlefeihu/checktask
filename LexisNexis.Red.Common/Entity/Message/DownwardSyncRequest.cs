using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    public class DownwardSyncRequest
    {
        [JsonProperty("userid")]
        public string UserID { get; set; }
        [JsonProperty("dlid")]
        public int DLID { get; set; }
        [JsonProperty("dlversionid")]
        public int DLVersionID { get; set; }
        [JsonProperty("deviceid")]
        public string DeviceID { get; set; }
    }
}
