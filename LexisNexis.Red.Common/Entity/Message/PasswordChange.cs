using LexisNexis.Red.Common.HelpClass;
using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    public class PasswordChange
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonIgnore]
        public string Password2 { get; set; }

        [JsonProperty("deviceid")]
        public string DeviceId { get; set; }
        [JsonIgnore]
        public string CountryCode { get; set; }
    }
}
