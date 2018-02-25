using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class UploadRepairRequest
    {
        [JsonProperty("Content")]
        public byte[] Content { get; set; }

        public Stream ContentStream { get; set; }

        public string username { get; set; }
        public string deviceid { get; set; }
        public string faultDesc { get; set; }
        public string userid { get; set; }
        public List<string> imgs { get; set; }

        public string xmlName { get; set; }

    }
}
