using LexisNexis.Red.Common.Business;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class SearchResult
    {
        public List<SearchDisplayResult> SearchDisplayResultList { get; set; }

        public List<string> FoundWordList { get; set; }

        public List<string> KeyWordList { get; set; }
    }
}
