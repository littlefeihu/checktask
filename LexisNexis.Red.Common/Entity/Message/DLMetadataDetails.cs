using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace LexisNexis.Red.Common.Entity
{
    public class DlMetadataDetails
    {
        [JsonProperty("dltitle")]
        public string DLTitle { get; set; }
        [JsonProperty("version")]
        public int Version { get; set; }
        [JsonProperty("size")]
        public string DlSize { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("lastupdateddate")]
        public DateTime LastUpdatedDate { get; set; }
        [JsonProperty("practicearea")]
        public string PracticeArea { get; set; }
     
        [JsonProperty("primarycolor")]
        public string PrimaryColor { get; set; }

        [JsonProperty("secondarycolor")]
        public string SecondaryColor { get; set; }

        [JsonProperty("fontcolor")]
        public string FontColor { get; set; }
        [JsonProperty("summary")]
        public string Summary { get; set; }
        [JsonProperty("subcategory")]
        public string SubCategory { get; set; }
        [JsonProperty("guidecards")]
        public List<GuidCards> GuideCard { get; set; }

    }
}
