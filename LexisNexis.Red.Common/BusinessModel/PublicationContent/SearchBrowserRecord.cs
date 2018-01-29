using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class SearchBrowserRecord : ContentBrowserRecord
    {
        [JsonConstructor]
        public SearchBrowserRecord(int bookID, int tocID, float webViewScrollPosition, int pageNum,
                                  string keyWords, string headType, int headSequence, List<string> spliteKeywords = null, string refptId = null)
            : base(bookID, tocID, pageNum, webViewScrollPosition, refptId)
        {
            this.KeyWords = keyWords;
            this.SpliteKeywords = spliteKeywords;
            this.RecordType = RecordType.SearchResultRecord;
            this.HeadType = headType;
            this.HeadSequence = headSequence;
        }
        public string HeadType { get; private set; }
        public int HeadSequence { get; private set; }
        public string KeyWords { get; private set; }
        public List<string> SpliteKeywords { get; private set; }
        public override bool Equals(BrowserRecord targetRecord)
        {
            var record = targetRecord as SearchBrowserRecord;
            if (record == null)
            {
                return false;
            }
            return base.Equals(record)
                   && (this.KeyWords == record.KeyWords)
                   && this.HeadSequence == record.HeadSequence
                   && this.HeadType == record.HeadType;
        }

    }

}
