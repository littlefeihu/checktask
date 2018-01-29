using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.WindowsStore.ViewModels
{
    public class SearchResultModel
    {
        public int TocId { get; set; }

        public string FirstLine { get; set; }

        public string SecondLine { get; set; }

        public string Type { get; set; }

        public string ContentType { get; set; }

        public string SnippetContent { get; set; }
        public List<string> Keywords { get; set; }
        public string HeadType { get; set; }
        public int HeadIndex { get; set; }
    }

    public class SearchPageModel
    {
        public string FileTitle { get; set; }
        public string GuideCardTitle { get; set; }
        public int TOCID { get; set; }
        public int PageNum { get; set; }
    }

}
