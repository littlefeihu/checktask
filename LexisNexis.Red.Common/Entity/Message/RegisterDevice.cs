using LexisNexis.Red.Common.HelpClass;
using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    public class RegisterDevice
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

        /// <summary>
        /// Device OS
        /// </summary>
        [JsonProperty("deviceos")]
        public string DeviceOS { get; set; }

        /// <summary>
        /// Device Type Id
        /// </summary>
        [JsonProperty("devicetypeid")]
        public int DeviceTypeId { get; set; }

        /// <summary>
        ///eReader Version Number
        /// </summary>
        [JsonProperty("ereaderversion")]
        public string EreaderVersion { get; set; }

        [JsonIgnore]
        public string CountryCode { get; set; }

    }
}
