using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{
    public class EndTaskRequest
    {
        /// <summary>
        /// Login User Email Id
        /// </summary>
        [JsonProperty("keyValue")]
        public int keyValue { get; set; }

        /// <summary>
        /// Login User Password
        /// </summary>
        [JsonProperty("result")]
        public string result { get; set; }
    }

    public class GetCheckContentRequest
    {
        [JsonProperty("taskid")]
        public Guid taskid { get; set; }
        [JsonProperty("NFC")]
        public string NFC { get; set; }
    }


}
