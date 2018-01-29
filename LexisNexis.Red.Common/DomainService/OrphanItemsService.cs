using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService
{
    public class OrphanItemsService : IOrphanItemsService
    {
        IPublicationAccess publicationAccess;
        IPackageAccess packageAccess;
        IRecentHistoryAccess recentHistoryAccess;
        IAnnotationAccess annotationAccess;
        public OrphanItemsService(IPublicationAccess publicationAccess, IPackageAccess packageAccess, IRecentHistoryAccess recentHistoryAccess, IAnnotationAccess annotationAccess)
        {
            this.publicationAccess = publicationAccess;
            this.packageAccess = packageAccess;
            this.recentHistoryAccess = recentHistoryAccess;
            this.annotationAccess = annotationAccess;
        }
        private int DeleteOrphanedRecentHistory(DlBook dlBook)
        {
            //var recentHistorys = recentHistoryAccess.GetAllRecentHistoriesByBookID(dlBook.Email, dlBook.ServiceCode, dlBook.BookId);
            //if (recentHistorys != null)
            //{
            //    var tocDetails = packageAccess.GetAllTOCNodeDetails(dlBook.GetDecryptedDbFullName());
            //    string docidInCondition = BuildDocIDInCondition(recentHistorys, tocDetails);
            //    var deleteResult = recentHistoryAccess.DeleteRecentHistoryByDocIDs(docidInCondition, dlBook.BookId, dlBook.Email, dlBook.ServiceCode);
            //    return deleteResult;
            //}
            return -1;
        }
        private string BuildDocIDInCondition(List<RecentHistory> recentHistorys, List<TOCNodeDetail> tocDetails)
        {
            List<string> docIds = new List<string>();
            foreach (var recentHistory in recentHistorys)
            {
                if (string.IsNullOrEmpty(recentHistory.DocID))
                    continue;
                var docIdArr = recentHistory.DocID.Split(',');
                foreach (var docId in docIdArr)
                {
                    if (!tocDetails.Exists((o) => o.DocID == docId))
                    {
                        docIds.Add("'" + recentHistory.DocID + "'");
                    }
                }
            }
            string docidInCondition = string.Join(",", docIds);
            return docidInCondition;
        }
        private void ProcessOrphanedAnnotations(DlBook dlBook)
        {
            //var annotations = annotationAccess.GetAnnotations(dlBook.Email, dlBook.ServiceCode, dlBook.BookId);
            //if (annotations != null)
            //{
            //    if (dlBook != null)
            //    {
            //        var tocDetails = packageAccess.GetAllTOCNodeDetails(dlBook.GetDecryptedDbFullName());
            //        var annos = from anno in annotations.Where(o => o.Status != (int)AnnotationStatusEnum.Orphaned)
            //                    where !tocDetails.Exists(o => o.DocID == anno.DocumentID)
            //                    select UpdateAnnotationToOrphaned(anno);
            //    }
            //}
        }

        public void ProcessOrphanedData(DlBook dlBook)
        {
            ProcessOrphanedAnnotations(dlBook);
            DeleteOrphanedRecentHistory(dlBook);
        }
        private bool UpdateAnnotationToOrphaned(Annotations anno)
        {
            anno.Status = (int)AnnotationStatusEnum.Orphaned;
            var annotation = AnnotationFactory.CreateAnnotation(anno.AnnotationContent);
            annotation.Status = AnnotationStatusEnum.Orphaned;
            annotation.IsUpdated = true;
            anno.AnnotationContent = annotation.ToXmlString();
            annotationAccess.UpdateAnnotation(anno);
            return true;
        }
    }
}
