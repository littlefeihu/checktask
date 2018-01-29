using LexisNexis.Red.Common.BusinessModel;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace LexisNexis.Red.Common.Entity
{
    public class DlFileList
    {
        [JsonProperty("dlfilelist")]
        public List<DlBook> DlBooks
        {
            get;
            set;
        }
    }
}
