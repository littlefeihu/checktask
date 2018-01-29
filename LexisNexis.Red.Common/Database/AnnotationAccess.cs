using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.DomainService;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public class AnnotationAccess : DbHelper, IAnnotationAccess
    {
        static string allFields = "RowId,Email,ServiceCode,AnnotationCode,BookID,DocumentID as DocID,AnnotationType,Status,IsSynced,AnnotationContent,CreatedDate,LastModifiedDate,LastSyncDate";
        static string deleteAnnotationSql = "DELETE FROM Annotations WHERE annotationCode=?";
        static string deleteAnnotationByBookIdSql = "DELETE FROM Annotations WHERE email=? AND serviceCode=? AND bookId=?";
        static string getAnnotationSql = String.Format("SELECT {0} FROM Annotations WHERE annotationCode=?", allFields);
        static string queryAllAnnotationsSql = String.Format("SELECT {0}  FROM Annotations WHERE Status !=? AND email=? AND serviceCode=?", allFields);
        static string queryAnnotationsByAnnotationTypeSql = String.Format("SELECT {0}  FROM Annotations WHERE Status !=? AND email=? AND serviceCode=? AND AnnotationType==? ", allFields);
        static string queryAnnotationsByAnnotationTypeAndBookIdSql = String.Format("SELECT {0}  FROM Annotations WHERE Status !=? AND email=? AND serviceCode=? AND AnnotationType==? AND bookId=? ", allFields);
        static string queryAllAnnotationsByBookIdSql = String.Format("SELECT {0} FROM Annotations WHERE Status !=? AND email=? AND serviceCode=? AND bookid=? ", allFields);
        static string queryAllAnnotationsByDocIdSql = String.Format("SELECT {0} FROM Annotations WHERE Status !=? AND email=? AND serviceCode=? AND bookid=? AND DocumentID=?", allFields);
        static string queryUnSyncedAllAnnotationsSql = String.Format("SELECT {0} FROM Annotations WHERE IsSynced=0 AND email=? AND serviceCode=?", allFields);
        static string queryUnSyncedAllAnnotationsByBookIdSql = String.Format("SELECT {0} FROM Annotations WHERE IsSynced=0 AND email=? AND serviceCode=? AND bookid=? ", allFields);

        //static string queryAnnotationFTS = String.Format("SELECT {0} FROM Annotations WHERE Annotations match ? and Status !=? AND email=? AND countryCode=? ", allFields);
        static string deleteAnnotationStatusSql = "UPDATE Annotations SET status=? ,LastModifiedDate=? ,IsSynced=? WHERE annotationCode=?";
        static string updateAnnotationSql = "UPDATE Annotations SET DocumentID=?,AnnotationType=?, status=? ,AnnotationContent=?,LastModifiedDate=? ,IsSynced=? WHERE annotationCode=?";
        static string queryAnnotationFTS
            = "SELECT RowId,Email,ServiceCode,AnnotationCode,BookID,DocumentID as DocID,AnnotationType,Status,IsSynced,AnnotationContent,CreatedDate,LastModifiedDate,LastSyncDate,NoteText,HighlightText FROM Annotations WHERE Annotations match '{0}' and Status != '{1}' AND email= '{2}' AND serviceCode= '{3}' ";

        static string initialNoteText
            = "UPDATE Annotations SET NoteText = ? where AnnotationCode = ?";

        static string initialHighLightText
            = "UPDATE Annotations SET HighlightText = ? where AnnotationCode = ?";

        static bool isAnnotationInitialized = false;

        public List<Annotations> GetAnnotations(string email, string serviceCode, int? bookId = null, string docId = null)
        {
            if (docId == null && bookId == null)
                return base.GetEntityList<Annotations>(base.MainDbPath, queryAllAnnotationsSql, (int)AnnotationStatusEnum.Deleted, email, serviceCode);
            else if (bookId != null && docId == null)
                return base.GetEntityList<Annotations>(base.MainDbPath, queryAllAnnotationsByBookIdSql, (int)AnnotationStatusEnum.Deleted, email, serviceCode, bookId);
            else if (bookId != null && docId != null)
                return base.GetEntityList<Annotations>(base.MainDbPath, queryAllAnnotationsByDocIdSql, (int)AnnotationStatusEnum.Deleted, email, serviceCode, bookId, docId);

            return null;
        }

        public List<Annotations> GetAnnotationsByType(string email, string serviceCode, int bookId, AnnotationType annotationType)
        {
            return base.GetEntityList<Annotations>(base.MainDbPath, queryAnnotationsByAnnotationTypeAndBookIdSql, (int)AnnotationStatusEnum.Deleted, email, serviceCode, (int)annotationType,bookId);
        }
        public List<Annotations> GetAnnotationsByType(string email, string serviceCode, AnnotationType annotationType)
        {
            return base.GetEntityList<Annotations>(base.MainDbPath, queryAnnotationsByAnnotationTypeSql, (int)AnnotationStatusEnum.Deleted, email, serviceCode, (int)annotationType);
        }
        public List<Annotations> GetUnSyncedAnnotations(string email, string serviceCode, int? bookId = null)
        {
            if (bookId == null)
                return base.GetEntityList<Annotations>(base.MainDbPath, queryUnSyncedAllAnnotationsSql, email, serviceCode);
            else
                return base.GetEntityList<Annotations>(base.MainDbPath, queryUnSyncedAllAnnotationsByBookIdSql, email, serviceCode, bookId);
        }

        public int DeleteAnnotation(Guid annotationCode)
        {
            return base.Execute(base.MainDbPath, deleteAnnotationSql, annotationCode);
        }

        public int AddAnnotation(Annotations annotation)
        {
            return base.Insert<Annotations>(base.MainDbPath, annotation);
        }

        public int UpdateAnnotation(Annotations annotation)
        {
            return base.Update<Annotations>(base.MainDbPath, annotation);
        }

        public int DeleteAnnotation(AnnotationStatusEnum status, DateTime lastModifiedDate, bool isSynced, Guid annoCode)
        {
            return base.Execute(base.MainDbPath, deleteAnnotationStatusSql, (int)status, lastModifiedDate, isSynced, annoCode);
        }

        public int UpdateAnnotation(string documentID, AnnotationType annotationType, AnnotationStatusEnum status, string content, DateTime lastModifiedDate, bool isSynced, Guid annoCode)
        {
            return base.Execute(base.MainDbPath, updateAnnotationSql, documentID, (int)annotationType, (int)status, content, lastModifiedDate, isSynced, annoCode);
        }
        public Annotations getAnnotation(Guid annotationCode)
        {
            return base.GetEntity<Annotations>(base.MainDbPath, getAnnotationSql, annotationCode);
        }


        public int DeleteAnnotationByBookId(int bookId, UserCredential userCredential)
        {
            return base.Execute(base.MainDbPath, deleteAnnotationByBookIdSql, userCredential.Email, userCredential.ServiceCode, bookId);
        }

        public List<Annotations> SearchAnnotations(string searchText, string email, string serviceCode, AnnotationType annotationType)
        {
            InitialNoteHighlightText(email, serviceCode);
            string queryNote = String.Format(queryAnnotationFTS, String.Format("NoteText: {0}", searchText), (int)AnnotationStatusEnum.Deleted, email, serviceCode);
            string queryHighlight = String.Format(queryAnnotationFTS, String.Format("HighlightText: {0}", searchText), (int)AnnotationStatusEnum.Deleted, email, serviceCode);
            List<Annotations> noteResult = new List<Annotations>();
            List<Annotations> highlightResult = new List<Annotations>();

            noteResult = base.GetEntityList<Annotations>(base.MainDbPath, queryNote);
            highlightResult = base.GetEntityList<Annotations>(base.MainDbPath, queryHighlight);

            noteResult.AddRange(highlightResult);

            if (annotationType == AnnotationType.Highlight)
                noteResult = noteResult.Where(r => r.AnnotationType == (int)AnnotationType.Highlight).ToList();
            else if (annotationType == AnnotationType.StickyNote)
                noteResult = noteResult.Where(r => r.AnnotationType == (int)AnnotationType.StickyNote).ToList();
            else if (annotationType == AnnotationType.Orphan)
                noteResult = noteResult.Where(r => r.Status == (int)AnnotationStatusEnum.Orphaned).ToList();

            return noteResult;
        }

        private void InitialNoteHighlightText(string email, string serviceCode)
        {
            if (!isAnnotationInitialized)
            {
                foreach (Annotations element in base.GetEntityList<Annotations>(base.MainDbPath, queryAllAnnotationsSql, (int)AnnotationStatusEnum.Deleted, email, serviceCode))
                {
                    Annotation annotation = AnnotationFactory.CreateAnnotation(element.AnnotationContent);
                    if (String.IsNullOrEmpty(element.NoteText) && !String.IsNullOrEmpty(annotation.NoteText))
                    {
                        base.Execute(base.MainDbPath, initialNoteText, annotation.NoteText, element.AnnotationCode);
                    }

                    if (String.IsNullOrEmpty(element.HighlightText) && !String.IsNullOrEmpty(annotation.HighlightText))
                    {
                        base.Execute(base.MainDbPath, initialHighLightText, annotation.HighlightText, element.AnnotationCode);
                    }
                }

                isAnnotationInitialized = true;
            }
        }
    }
}
