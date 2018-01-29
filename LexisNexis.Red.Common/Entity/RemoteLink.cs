using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{
    public class RemoteLink
    {
        public int ID { set; get; }
        public string RemoteLinkID { set; get; }
        public string Offset { set; get; }
        public string TargetFile { set; get; }
    }
}
