using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class DocumentBrowserRecord : ContentBrowserRecord
    {
        [JsonConstructor]
        public DocumentBrowserRecord(int bookID, int tocID, float webViewScrollPosition, int pageNum, AttachmentHyperlink link)
            : base(bookID, tocID, pageNum, webViewScrollPosition)
        {
            this.Attachmentlink = link;
            this.RecordID = Guid.NewGuid();
            this.RecordType = RecordType.Document;
        }
        public AttachmentHyperlink Attachmentlink { get; private set; }
        public override bool Equals(BrowserRecord targetRecord)
        {
            var record = targetRecord as DocumentBrowserRecord;
            if (record == null)
            {
                return false;
            }
            return base.Equals(record)
                   && (this.Attachmentlink.TargetFileName == record.Attachmentlink.TargetFileName);
        }
    }
}
