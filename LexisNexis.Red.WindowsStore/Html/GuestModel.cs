using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LexisNexis.Red.WindowsStore.Html
{
    public class NotifytModel
    {
        [JsonProperty("type")]
        public string Type { set; get; }
        [JsonProperty("value1")]
        public string Value1 { set; get; }
        [JsonProperty("value2")]
        public string Value2 { set; get; }
    }
}
