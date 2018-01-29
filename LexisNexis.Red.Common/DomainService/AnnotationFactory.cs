using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LexisNexis.Red.Common.DomainService
{
    public class AnnotationFactory
    {
        public static List<Annotation> CreateAnnotations(List<Annotations> annotations, List<AnnotationTag> tags = null, List<DlBook> dlBooks = null)
        {
            var q = (from m in annotations
                     select CreateAnnotation(m.AnnotationContent, tags, dlBooks)).ToList();
            return q;
        }
        public static Annotation CreateAnnotation(string xmlString, List<AnnotationTag> tags = null, List<DlBook> dlBooks = null)
        {
            Annotation resultAnnotation = null;
            var xDocument = XDocument.Parse(xmlString);
            XElement annotation = xDocument.Descendants(Constants.Annotation).First();
            if (annotation != null)
            {
                var startLevel = annotation.Element(Constants.SAnnotationData).Elements().FirstOrDefault(o => o.Name == Constants.SmallStartLevel);
                var endLevel = annotation.Element(Constants.SAnnotationData).Elements().FirstOrDefault(o => o.Name == Constants.SmallEndLevel);
                AnnotationDataItem annotDataStart = GetAnnotationDataItem(startLevel, true);
                AnnotationDataItem annotDataEnd = GetAnnotationDataItem(endLevel, false);

                var BookId = int.Parse(annotation.Attribute(Constants.SyncSDlid).Value);
                var currentVersion = int.Parse(annotation.Attribute(Constants.SDlversionid).Value);
                var updatedOn = annotation.Attribute(Constants.SUpdatedOn).Value;
                var type = (AnnotationType)int.Parse(annotation.Attribute(Constants.SType).Value);
                var annotationCode = Guid.Parse(annotation.Attribute(Constants.SId).Value);
                var isUpdated = annotation.Element(Constants.SIsupdated).Value;
                var guideCardName = annotation.Element(Constants.SGuideCardName).Value;
                var noteText = annotation.Element(Constants.SNote).Value;
                var tocTitle = annotation.Element(Constants.SdlBookTitle).Value;
                var highlightText = annotation.Element(Constants.SHighlight).Value;
                var statusString = annotation.Attribute(Constants.Status).Value;
                AnnotationStatusEnum annoStatus;

                annoStatus = (AnnotationStatusEnum)Enum.Parse(typeof(AnnotationStatusEnum), statusString[0].ToString().ToUpper() + statusString.Substring(1));

                XElement tagIDs = annotation.Element(Constants.CategoryTagIDs);
                var categoryTagIDs = new List<Guid>();
                if (tagIDs != null && !string.IsNullOrEmpty(tagIDs.Value))
                {
                    categoryTagIDs = tagIDs.Value.Split(',').Select(id => Guid.Parse(id)).ToList();
                }
                resultAnnotation = new Annotation(annotationCode, type, annoStatus, BookId, currentVersion, annotDataStart.DocId, guideCardName, noteText, highlightText, tocTitle, categoryTagIDs, annotDataStart, annotDataEnd);
                resultAnnotation.UpdatedTime = DateTime.Parse(updatedOn);

                if (dlBooks != null)
                {
                    //var dlBook = dlBooks.FirstOrDefault(o => o.BookId == BookId);
                    //resultAnnotation.BookTitle = dlBook != null ? dlBook.Name : string.Empty;
                }
                resultAnnotation.CategoryTags = new List<AnnotationTag>();
                if (tags != null)
                {
                    foreach (var tagId in categoryTagIDs)
                    {
                        var tag = tags.FirstOrDefault(o => o.TagId == tagId);
                        if (tag != null)
                            resultAnnotation.CategoryTags.Add(tag);
                    }
                }
            }
            return resultAnnotation;
        }
        private static AnnotationDataItem GetAnnotationDataItem(XElement xelement, bool isStartLevel = true)
        {
            var xpath = xelement.Attribute(Constants.SmallXPath).Value;
            var levelId = xelement.Attribute(Constants.SmallLevelId).Value;
            var offset = String.IsNullOrEmpty(xelement.Attribute(Constants.SOffset).Value.Trim()) ? -1 : Convert.ToInt32(xelement.Attribute(Constants.SOffset).Value, CultureInfo.CurrentCulture);
            var docId = xelement.Attribute(isStartLevel ? Constants.StartDocId : Constants.EndDocId).Value;
            var filename = xelement.Attribute(Constants.SId) != null ? xelement.Attribute(Constants.SId).Value : null;
            AnnotationDataItem anotDataStart = new AnnotationDataItem(offset, filename, levelId, xpath, docId);
            return anotDataStart;
        }

    }
}
