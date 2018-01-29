using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{
    public class DlFileCurrentVersionResponse
    {
        /// <summary>
        /// DL Current Version
        /// </summary>
        [JsonProperty("curver")]
        public int DlCurrentVersion { get; set; }
    }
}
