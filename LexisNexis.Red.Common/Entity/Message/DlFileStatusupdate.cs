using Newtonsoft.Json;
using System;

namespace LexisNexis.Red.Common.Entity
{
    public class DlFileStatusUpdate
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("deviceid")]
        public string DeviceId { get; set; }
        [JsonProperty("dlid")]
        public int DLId { get; set; }
        [JsonProperty("ver")]

        public string Ver { get; set; }
        [JsonProperty("statuscode")]
        public string StatusCode { get; set; }
    }
}
