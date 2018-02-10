using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{
    public class StartTaskRequest
    {
        /// <summary>
        /// Login User Email Id
        /// </summary>
        [JsonProperty("keyValue")]
        public string keyValue { get; set; }

        /// <summary>
        /// Login User Password
        /// </summary>
        [JsonProperty("username")]
        public string UserName { get; set; }
    }
    public class ServiceResponse
    {
      
        public bool Success { get { return state == "success"; } }

        public string state { get; set; }


        [JsonProperty("Message")]
        public string Message { get; set; }
    }

}
