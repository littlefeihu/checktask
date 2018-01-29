using Newtonsoft.Json;
using System;
namespace LexisNexis.Red.Common.Entity
{
    public class UserDetails
    {
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("deviceid")]
        public string DeviceId { get; set; }
    }
}
