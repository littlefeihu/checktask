using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class AttachmentHyperlink : Hyperlink
    {
        public AttachmentHyperlink(string fileName, FileType fileType)
        {
            base.LinkType = HyperLinkType.AttachmentHyperlink;
            this.FileType = fileType;
            this.TargetFileName = fileName;
        }
        public FileType FileType { get; private set; }
        public string TargetFileName { get; private set; }
    }
}
