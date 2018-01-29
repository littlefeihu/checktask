using LexisNexis.Red.Common.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LexisNexis.Red.Common.Entity
{
    /// <summary>
    /// This class is used to send & receive data to & from web service.
    /// </summary>
    public class AnnCategoryTagsSyncData
    {
        [JsonProperty("annotationtagsxml")]
        public string AnnotationTagsXml { get; set; }
        [JsonProperty("lsst")]
        public System.DateTime? LSST { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }

    }

}
