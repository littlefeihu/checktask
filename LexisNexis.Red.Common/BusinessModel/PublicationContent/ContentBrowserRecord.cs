using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class ContentBrowserRecord : BrowserRecord
    {
        [JsonConstructor]
        public ContentBrowserRecord(int bookId, int tocId, int pageNum, float webViewScrollPosition, string refptId = null)
            : this(bookId, tocId, webViewScrollPosition)
        {
            this.PageNum = pageNum;
            this.RefptID = refptId;
        }

        public ContentBrowserRecord(int bookId, int tocId, float webViewScrollPosition)
        {
            this.BookID = bookId;
            this.TOCID = tocId;
            this.WebViewScrollPosition = webViewScrollPosition;
            this.RecordID = Guid.NewGuid();
            this.RecordType = RecordType.ContentRecord;
        }

        public string RefptID { get; private set; }
        public int TOCID { get; private set; }
        public float WebViewScrollPosition { get; set; }
        public int PageNum { get; private set; }

        public override bool Equals(BrowserRecord targetRecord)
        {
            var record = targetRecord as ContentBrowserRecord;
            if (record == null)
            {
                return false;
            }
            return (this.BookID == record.BookID)
                && (this.TOCID == record.TOCID)
                && (this.WebViewScrollPosition == record.WebViewScrollPosition)
                && (this.PageNum == record.PageNum)
                && (this.RefptID == record.RefptID);
        }

    }

}
