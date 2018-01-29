using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class ExternalHyperlink : Hyperlink
    {
        public ExternalHyperlink(string remoteUrl)
        {
            base.LinkType = HyperLinkType.ExternalHyperlink;
            this.Url = remoteUrl;
        }
        public string Url { get; private set; }
    }
}
