using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class Hiddenlink : Hyperlink
    {
        public Hiddenlink()
        {
            base.LinkType = HyperLinkType.Hiddenlink;
        }
    }
}
