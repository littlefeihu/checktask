using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class IntraHyperlink : Hyperlink
    {
        public IntraHyperlink(int tocID, string refpt)
        {
            base.LinkType = HyperLinkType.IntraHyperlink;
            this.TOCID = tocID;
            this.Refpt = refpt;
        }
        public int TOCID { get; private set; }
        public string Refpt { get; private set; }
    }
}
