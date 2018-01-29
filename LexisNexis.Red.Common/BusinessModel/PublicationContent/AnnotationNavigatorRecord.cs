using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class AnnotationNavigatorRecord : ContentBrowserRecord
    {

        [JsonConstructor]
        public AnnotationNavigatorRecord(int bookID, int tocID, float webViewScrollPosition, int pageNum,
                                         Guid annotationID, List<AnnotationTag> selectedTags, AnnotationType? selectedAnnotationType,
                                         string refptId = null)
            : base(bookID, tocID, pageNum, webViewScrollPosition, refptId)
        {
            this.SelectedTags = selectedTags;
            this.SelectedAnnotationType = selectedAnnotationType;
            this.AnnotationID = annotationID;
            this.RecordType = RecordType.AnnotationNavigator;
        }
        public Guid AnnotationID { get; private set; }
        public List<AnnotationTag> SelectedTags { get; private set; }
        public AnnotationType? SelectedAnnotationType { get; private set; }
        public override bool Equals(BrowserRecord targetRecord)
        {
            var record = targetRecord as AnnotationNavigatorRecord;
            if (record == null)
            {
                return false;
            }
            return base.Equals(record)
                && (this.SelectedTags == record.SelectedTags)
                && (this.SelectedAnnotationType == record.SelectedAnnotationType);
        }
    }

}
