using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public interface IAnnotationAccess
    {
        List<Annotations> GetAnnotations(string email, string serviceCode, int? bookId = null, string docId = null);
        List<Annotations> GetUnSyncedAnnotations(string email, string serviceCode, int? bookId = null);
        List<Annotations> GetAnnotationsByType(string email, string serviceCode, int bookId, AnnotationType annotationType);
        List<Annotations> GetAnnotationsByType(string email, string serviceCode, AnnotationType annotationType);
        int DeleteAnnotation(Guid annoCode);
        int DeleteAnnotationByBookId(int bookId, UserCredential userCredential);

        int AddAnnotation(Annotations annotation);
        int DeleteAnnotation(AnnotationStatusEnum status, DateTime lastModifiedDate, bool isSynced, Guid annoCode);
        int UpdateAnnotation(string documentID, AnnotationType annotationType, AnnotationStatusEnum status, string content, DateTime lastModifiedDate, bool isSynced, Guid annoCode);

        int UpdateAnnotation(Annotations annotation);

        Annotations getAnnotation(Guid annotationCode);
        List<Annotations> SearchAnnotations(string searchText, string email, string serviceCode, AnnotationType annotationType);

    }
}
