using LexisNexis.Red.Common.HelpClass;
using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    public class LoginUser
    {
        /// <summary>
        /// Login User Email Id
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Login User Password
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Login User Device Id
        /// </summary>
        [JsonProperty("deviceid")]
        public string DeviceId { get; set; }
        [JsonIgnore]
        public string CountryCode { get; set; }
    }
}
