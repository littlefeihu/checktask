using LexisNexis.Red.Common.Business;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class IndexData
    {
        public string DocId { get; set; }

        public string SnippetContent { get; set; }

        public string ContentType { get; set; }

        public string Head { get; set; }
    }
}
