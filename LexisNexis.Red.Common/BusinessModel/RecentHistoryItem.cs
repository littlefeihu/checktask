using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class RecentHistoryItem
    {
        public int BookId { set; get; }
        public string DOCID { set; get; }
        public string TOCTitle { set; get; }
        public string PublicationTitle { set; get; }
        public string ColorPrimary { set; get; }
        public DateTime LastReadDate { set; get; }
    }
}
