using LexisNexis.Red.Common.BusinessModel;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace LexisNexis.Red.Common.Entity
{
    public class DictionaryVersionResponse
    {
        /// <summary>
        /// AU
        /// </summary>
        [JsonProperty("AU")]
        public string AU { get; set; }

        /// <summary>
        /// NZ
        /// </summary>
        [JsonProperty("NZ")]
        public string NZ { get; set; }

        /// <summary>
        /// SG
        /// </summary>
        [JsonProperty("SG")]
        public string SG { get; set; }

        /// <summary>
        /// HK
        /// </summary>
        [JsonProperty("HK")]
        public string HK { get; set; }

        /// <summary>
        /// JP
        /// </summary>
        [JsonProperty("JP")]
        public string JP { get; set; }

        /// <summary>
        /// MY
        /// </summary>
        [JsonProperty("MY")]
        public string MY { get; set; }
    }
}
