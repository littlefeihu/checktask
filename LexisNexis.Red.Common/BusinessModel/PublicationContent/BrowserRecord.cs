using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public abstract class BrowserRecord
    {
        public int BookID { get; protected set; }
        public Guid RecordID { get; set; }
        public RecordType RecordType { get; protected set; }
        public abstract bool Equals(BrowserRecord targetRecord);
    }

}
