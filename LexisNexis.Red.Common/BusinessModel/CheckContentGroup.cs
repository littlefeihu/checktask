using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class CheckContentGroup
    {
        public Guid Id { get; set; }
        public string CheckName { get; set; }

        public List<CheckContentItem> Data { get; set; }

    }

    //CheckPointName = b.Name,
    //CheckName = d.Name,
    //CheckContent = c.Content,
    //a.CheckPointId,
    //a.CheckContentId,
    //c.ParentId,
    //b.NFCID,
    //e.CheckTaskId

    public class CheckContentItem
    {
        public string CheckPointName { get; set; }
        public string CheckName { get; set; }
        public string CheckContent { get; set; }
        public Guid CheckPointId { get; set; }
        public Guid ParentId { get; set; }
        public string NFCID { get; set; }
        public Guid CheckTaskId { get; set; }
        public Guid CheckContentId { get; set; }
    }
}
