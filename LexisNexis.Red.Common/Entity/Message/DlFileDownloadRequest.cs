using Newtonsoft.Json;
namespace LexisNexis.Red.Common.Entity
{
    public class DlFileDownloadRequest
    {
        /// <summary>
        /// Login User Email Id
        /// </summary>
        [JsonProperty("email")]
        public string UserEmailId { get; set; }

        /// <summary>
        /// Login User Device Id
        /// </summary>
        [JsonProperty("deviceid")]
        public string DeviceId { get; set; }

        /// <summary>
        /// DL Id
        /// </summary>
        [JsonProperty("dlid")]
        public int DlId { get; set; }
    }
}
