using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class AnnotationOrganiserRecord : BrowserRecord
    {
        [JsonConstructor]
        public AnnotationOrganiserRecord(Guid annotationID, List<AnnotationTag> selectedTags, AnnotationType? selectedAnnotationType, string keyWords, List<string> spliteKeywords = null)
        {
            this.SelectedTags = selectedTags;
            this.SelectedAnnotationType = selectedAnnotationType;
            this.AnnotationID = annotationID;
            this.KeyWords = keyWords;
            this.RecordID = Guid.NewGuid();
            this.SpliteKeywords = spliteKeywords;
            this.RecordType = RecordType.AnnotationOrganiser;
            this.BookID = -1;
        }
        public List<string> SpliteKeywords { get; private set; }
        public string KeyWords { get; private set; }
        public Guid AnnotationID { get; private set; }
        public List<AnnotationTag> SelectedTags { get; private set; }
        public AnnotationType? SelectedAnnotationType { get; private set; }
        public override bool Equals(BrowserRecord targetRecord)
        {
            var record2 = targetRecord as AnnotationOrganiserRecord;
            if (record2 == null)
            {
                return false;
            }
            return (this.AnnotationID == record2.AnnotationID) && (this.SelectedAnnotationType == record2.SelectedAnnotationType);
        }
    }
   
}
