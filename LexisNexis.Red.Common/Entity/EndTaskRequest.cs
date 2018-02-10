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
        public string keyValue { get; set; }

        /// <summary>
        /// Login User Password
        /// </summary>
        [JsonProperty("result")]
        public string result { get; set; }
    }

    public class GetCheckContentRequest
    {
        [JsonProperty("taskid")]
        public string taskid { get; set; }
        [JsonProperty("NFC")]
        public string NFC { get; set; }
    }


    public class CreateCheckRercordRequest
    {
        public bool Passed { get; set; }
        public string CheckContentId { get; set; }
        public string CheckPointId { get; set; }
        public string CheckTaskId { get; set; }
        public string Result { get; set; }


    }

}
