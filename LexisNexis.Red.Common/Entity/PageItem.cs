using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{
    public class PageItem
    {
        public int TOCID { get; set; }
        public String ItemType { get; set; }
        public Int32 Identifier { get; set; }
        public String FileName { get; set; }
        public String DocId { get; set; }
        public String Description { get; set; }
        public String Heading { get; set; }
        public Int32 StartPageNo { get; set; }
        public Int32 EndPageNo { get; set; }
    }
}
