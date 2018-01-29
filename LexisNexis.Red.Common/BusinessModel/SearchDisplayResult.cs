using LexisNexis.Red.Common.Business;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class SearchDisplayResult
    {
        public int TocId { get; set; }

        public string TocTitle { get; set; }

        public string GuideCardTitle { get; set; }

        public string Head { get; set; }

        public ContentCategory ContentType { get; set; }

        public string SnippetContent { get; set; }

        public bool isDocument { get; set; }

        public string DocId { get; set; }

        public HeadType HeadType { get; set; }

        public int HeadSequence { get; set; }

        public int HighlightStartSpanId { get; set; }

        public int HighlightEndSpanId { get; set; }

    }
}
