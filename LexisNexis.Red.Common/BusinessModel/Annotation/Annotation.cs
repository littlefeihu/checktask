using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class Annotation
    {

        public Annotation(Guid annotationCode, AnnotationType annType, AnnotationStatusEnum annStatus, int bookId, int bookCurrentVersion, string docId, string guideCardName, string noteText, string highlightText, string tocTitle, List<Guid> categoryTagIDs, AnnotationDataItem startLevel, AnnotationDataItem endLevel)
        {
            this.BookId = bookId;
            this.BookCurrentVersion = bookCurrentVersion;
            this.UpdatedTime = DateTime.UtcNow;
            this.DocId = docId;
            this.Type = annType;
            this.Status = annStatus;
            this.AnnotationCode = annotationCode;
            this.GuideCardName = guideCardName;
            this.NoteText = noteText;
            this.HighlightText = highlightText;
            this.CategoryTagIDs = categoryTagIDs;
            this.StartLevel = startLevel;
            this.EndLevel = endLevel;
            this.TOCTitle = tocTitle;
        }

        public Annotation(AnnotationType annType, int bookId, int bookCurrentVersion, string docId, string guideCardName, string noteText, string highlightText, string tocTitle, List<Guid> categoryTagIDs, AnnotationDataItem startLevel, AnnotationDataItem endLevel)
            : this(Guid.NewGuid(), annType, AnnotationStatusEnum.Created, bookId, bookCurrentVersion, docId, guideCardName, noteText, highlightText, tocTitle, categoryTagIDs, startLevel, endLevel)
        {

        }
        public int BookId { get; set; }
        public int BookCurrentVersion { get; set; }
        public DateTime UpdatedTime { get; internal set; }
        /// <summary>
        /// current document document id
        /// </summary>
        public string DocId { get; set; }
        public AnnotationType Type { get; set; }
        public AnnotationStatusEnum Status { get; set; }
        public bool IsUpdated { get; set; }
        /// <summary>
        /// indicate unique annotation
        /// </summary>
        public Guid AnnotationCode { get; private set; }
        /// <summary>
        /// fist level TOC Title
        /// </summary>
        public string GuideCardName { get; set; }
        public string NoteText { get; set; }

        public string HighlightText { get; set; }
        /// <summary>
        /// low level TOC Title
        /// </summary>
        public string TOCTitle { get; set; }
        public string BookTitle { get; set; }
        /// <summary>
        /// CategoryTag IDs
        /// </summary>
        public List<Guid> CategoryTagIDs { get; set; }

        public List<AnnotationTag> CategoryTags { get; set; }

        public AnnotationDataItem StartLevel { get; set; }
        public AnnotationDataItem EndLevel { get; set; }
    }

    public class AnnotationDataItem
    {
        private AnnotationDataItem(int offset, string filename, string levelId, string docId)
        {
            this.Offset = offset;
            if (filename != null)
                this.FileName = filename.TrimEnd(Constants.Xml.ToCharArray());
            this.LevelId = levelId;
            this.DocId = docId;
        }
        public AnnotationDataItem(int offset, string filename, string levelId, string xpath, string docId)
            : this(offset, filename, levelId, docId)
        {
            this.Xpath = xpath;
        }
        public int Offset { get; set; }
        public string FileName { get; set; }
        public string LevelId { get; set; }
        public string Xpath { get; set; }
        public string DocId { get; set; }
        public int SpanId { get; set; }

    }

}

