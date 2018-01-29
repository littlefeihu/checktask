using LexisNexis.Red.Common.BusinessModel;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LexisNexis.Red.Common.Entity
{
    public class DlVersionChangeHistoryResponse
    {
        /// <summary>
        /// Delete Guide Card List
        /// </summary>
        [JsonProperty("deletedguidecard")]
        public List<GuideCard> DeletedGuideCard { get; set; }

        /// <summary>
        /// Newly Included Guide Card
        /// </summary>
        [JsonProperty("includedguidecard")]
        public List<GuideCard> AddedGuideCard { get; set; }

        /// <summary>
        /// Updated Guide Card
        /// </summary>
        [JsonProperty("updatedguidecard")]
        public List<GuideCard> UpdatedGuideCard { get; set; }
    }
}
