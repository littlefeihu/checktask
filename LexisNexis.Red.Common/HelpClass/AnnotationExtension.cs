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

namespace LexisNexis.Red.Common.HelpClass
{
    public static class AnnotationExtension
    {
        public static string ToXmlString(this Annotation annotation)
        {
            XDocument annotationDoc = new XDocument(
               new XDeclaration("1.0", Constants.UTFEncoding, "yes"),
               new XElement(Constants.Annotation,
               new XAttribute(Constants.SyncSDlid, annotation.BookId),
               new XAttribute(Constants.SDlversionid, annotation.BookCurrentVersion),
               new XAttribute(Constants.Status, annotation.Status.ToString().ToLower()),
               new XAttribute(Constants.SUpdatedOn, annotation.UpdatedTime),
               new XAttribute(Constants.DocId, annotation.DocId),
               new XAttribute(Constants.SType, ((int)annotation.Type).ToString()),
               new XAttribute(Constants.SId, annotation.AnnotationCode),
               new XElement(Constants.SIsupdated, annotation.IsUpdated ? Constants.Yes : Constants.No),
               new XElement(Constants.SGuideCardName, annotation.GuideCardName),
               new XElement(Constants.SNote, annotation.NoteText),
               new XElement(Constants.SHighlight, annotation.HighlightText),
               new XElement(Constants.CategoryTagIDs, annotation.CategoryTagIDs != null ? string.Join(",", annotation.CategoryTagIDs) : string.Empty),

               new XElement(Constants.SdlBookTitle, annotation.TOCTitle),

               new XElement(Constants.SAnnotationData,
                         new XElement(Constants.SmallStartLevel,
                             new XAttribute(Constants.SmallXPath, annotation.StartLevel.Xpath),
                             new XAttribute(Constants.SmallLevelId, annotation.StartLevel.LevelId),
                             new XAttribute(Constants.SOffset, annotation.StartLevel.Offset),
                             new XAttribute(Constants.StartDocId, annotation.StartLevel.DocId),
                             new XAttribute(Constants.SId, annotation.StartLevel.FileName)),
                         new XElement(Constants.SmallEndLevel,
                             new XAttribute(Constants.SmallXPath, annotation.EndLevel.Xpath),
                             new XAttribute(Constants.SmallLevelId, annotation.EndLevel.LevelId),
                             new XAttribute(Constants.SOffset, annotation.EndLevel.Offset),
                             new XAttribute(Constants.EndDocId, annotation.EndLevel.DocId),
                             new XAttribute(Constants.SId, annotation.EndLevel.FileName)))));
            return annotationDoc.ToString();
        }

    }
}
