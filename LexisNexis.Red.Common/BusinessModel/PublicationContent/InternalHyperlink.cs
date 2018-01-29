using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class InternalHyperlink : Hyperlink
    {
        public InternalHyperlink(int bookID, int tocID, string refpt)
        {
            base.LinkType = HyperLinkType.InternalHyperlink;
            this.BookID = bookID;
            this.TOCID = tocID;
            this.Refpt = refpt;
        }
        public int BookID { get; private set; }
        public int TOCID { get; private set; }
        public string Refpt { get; private set; }
    }
}
