using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class AnchorHyperlink : Hyperlink
    {
        public AnchorHyperlink(string url)
        {
            base.LinkType = HyperLinkType.AnchorHyperlink;
            this.AnchorName = url.TrimStart('#');
        }
        public string AnchorName { get; private set; }
    }
}
