using HtmlAgilityPack;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
namespace LexisNexis.Red.Common.Business
{
    public class AnnotationUtil : AppServiceBase<AnnotationUtil>
    {
        private IAnnotationAccess annotationAccess;
        private IPackageAccess packageAccess;
        private IPublicationAccess publicationAccess;
        private ITagDomainService tagDomainService;
        public AnnotationUtil(IAnnotationAccess annotationAccess, IPackageAccess packageAccess, IPublicationAccess publicationAccess, ITagDomainService tagDomainService)
        {
            this.annotationAccess = annotationAccess;
            this.packageAccess = packageAccess;
            this.publicationAccess = publicationAccess;
            this.tagDomainService = tagDomainService;
        }
        /// <summary>
        /// Add new Annotation
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        public async Task AddAnnotation(Annotation annotation)
        {
            await DomainEvents.Publish(new PublicationOpeningEvent(annotation.BookId, false));

            annotation.UpdatedTime = DateTime.UtcNow;
            annotation.Status = AnnotationStatusEnum.Created;
            annotation.IsUpdated = false;

            Annotations annotations = new Annotations();
            annotations.Email = GlobalAccess.Instance.Email;
            annotations.ServiceCode = GlobalAccess.Instance.ServiceCode;
            annotations.DocumentID = annotation.DocId;
            annotations.AnnotationType = (int)annotation.Type;
            annotations.BookID = annotation.BookId;
            annotations.AnnotationCode = annotation.AnnotationCode;
            annotations.AnnotationContent = annotation.ToXmlString();
            annotations.CreatedDate = DateTime.Now;
            annotations.LastModifiedDate = DateTime.Now;
            annotations.Status = (int)AnnotationStatusEnum.Created;
            annotations.IsSynced = false;
            annotations.NoteText = annotation.NoteText;
            annotations.HighlightText = annotation.HighlightText;
            annotationAccess.AddAnnotation(annotations);
        }
        /// <summary>
        /// delete Annotation By AnnotationCode
        /// </summary>
        /// <param name="annotationCode"></param>
        public void DeleteAnnotation(Guid annotationCode)
        {
            annotationAccess.DeleteAnnotation(AnnotationStatusEnum.Deleted, DateTime.Now, false, annotationCode);
        }
        /// <summary>
        /// Update a existing Annotation
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        public async Task UpdateAnnotation(Annotation annotation)
        {
            await DomainEvents.Publish(new PublicationOpeningEvent(annotation.BookId, false));

            annotation.UpdatedTime = DateTime.UtcNow;
            annotation.Status = AnnotationStatusEnum.Updated;
            annotation.IsUpdated = true;

            annotationAccess.UpdateAnnotation(annotation.DocId, annotation.Type, AnnotationStatusEnum.Updated, annotation.ToXmlString(), DateTime.Now, false, annotation.AnnotationCode);
        }

        /// <summary>
        /// get All Annotations
        /// </summary>
        /// <returns></returns>
        public List<Annotation> GetAnnotations()
        {
            var dlBooks = publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential);

            var annotations = annotationAccess.GetAnnotations(GlobalAccess.Instance.Email, GlobalAccess.Instance.ServiceCode);

            return AnnotationFactory.CreateAnnotations(annotations, this.tagDomainService.Tags, dlBooks);
        }
        /// <summary>
        /// get Annotations By special BookId
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public List<Annotation> GetAnnotationsByBookId(int bookId)
        {
            var annotations = annotationAccess.GetAnnotations(GlobalAccess.Instance.Email, GlobalAccess.Instance.ServiceCode, bookId);
            var dlBook = publicationAccess.GetDlBookByBookId(bookId, GlobalAccess.Instance.UserCredential);

            return AnnotationFactory.CreateAnnotations(annotations, this.tagDomainService.Tags, new List<DlBook> { dlBook });
        }
        /// <summary>
        /// Convert List to Dictionary by GuidCardName
        /// </summary>
        /// <param name="annotations"></param>
        /// <returns></returns>
        public Dictionary<string, List<Annotation>> ConvertToDictionary(List<Annotation> annotations)
        {
            var guideCardGroups = from annotation in annotations
                                  group annotation by annotation.GuideCardName into guideCardGroup
                                  select guideCardGroup.Key;

            Dictionary<string, List<Annotation>> dic = new Dictionary<string, List<Annotation>>();
            foreach (var guideCardGroup in guideCardGroups)
            {
                dic.Add(guideCardGroup, annotations.FindAll(o => o.GuideCardName == guideCardGroup));
            }
            return dic;
        }

        /// <summary>
        /// Get Annotations for Special DlBook by DocId
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="docId"></param>
        /// <returns></returns>
        public List<Annotation> GetAnnotationsByDocId(int bookId, string docId)
        {
            DomainEvents.Publish(new PublicationOpeningEvent(bookId, false)).Wait();

            return GetAnnotationsByDocId(docId, GlobalAccess.Instance.CurrentPublication.DlBook);
        }
        private List<Annotation> GetAnnotationsByDocId(string docId, DlBook dlBook)
        {
            //var annotations = annotationAccess.GetAnnotations(GlobalAccess.Instance.Email, GlobalAccess.Instance.ServiceCode, dlBook.BookId, docId);
            //if (annotations == null)
            //    return null;

            //  return AnnotationFactory.CreateAnnotations(annotations, this.tagDomainService.Tags, new List<DlBook> { dlBook });
            return null;
        }
        /// <summary>
        /// get Annotations By TocId
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="tocId"></param>
        /// <returns></returns>
        public List<Annotation> GetAnnotationsByTocId(int bookId, int tocId)
        {
            DomainEvents.Publish(new PublicationOpeningEvent(bookId, false)).Wait();
            var tocdDetails = packageAccess.GetTOCDetailByTOCId(GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName, tocId);
            if (tocdDetails != null)
            {
                List<Annotation> annotations = new List<Annotation>();
                foreach (var tocdDetail in tocdDetails)
                {
                    annotations.AddRange(GetAnnotationsByDocId(tocdDetail.DocID, GlobalAccess.Instance.CurrentPublication.DlBook));
                }
                return annotations;
            }
            else
            {
                return null;
            }
        }

        public List<Annotation> GetAnnotations(int bookId, AnnotationType annotationType, List<Guid> selectedTagIds, bool includeNoTags)
        {
            List<Annotation> annotationsFilteredByAnnotationType = new List<Annotation>();
            if (annotationType == AnnotationType.All)
            {
                annotationsFilteredByAnnotationType = GetAnnotationsByBookId(bookId);
            }
            else
            {
                var annotations = annotationAccess.GetAnnotationsByType(GlobalAccess.Instance.Email, GlobalAccess.Instance.ServiceCode, bookId, annotationType);
                var dlBook = publicationAccess.GetDlBookByBookId(bookId, GlobalAccess.Instance.UserCredential);
                annotationsFilteredByAnnotationType = AnnotationFactory.CreateAnnotations(annotations, this.tagDomainService.Tags, new List<DlBook> { dlBook });
            }
            if (annotationsFilteredByAnnotationType != null)
            {
                List<Annotation> noTags = null;
                if (includeNoTags)
                    noTags = annotationsFilteredByAnnotationType.FindAll(o => o.CategoryTagIDs.Count == 0);

                if (selectedTagIds != null && selectedTagIds.Count > 0)
                    annotationsFilteredByAnnotationType = annotationsFilteredByAnnotationType.FindAll(o => selectedTagIds.Intersect(o.CategoryTagIDs).Count() > 0);

                if (noTags != null)
                    annotationsFilteredByAnnotationType.AddRange(noTags);

            }
            return annotationsFilteredByAnnotationType;
        }

        public List<Annotation> GetAnnotations(AnnotationType annotationType, List<Guid> selectedTagIds, bool includeNoTags)
        {
            List<Annotation> annotationsFilteredByAnnotationType = new List<Annotation>();
            if (annotationType == AnnotationType.All)
            {
                annotationsFilteredByAnnotationType = GetAnnotations();
            }
            else
            {
                var annotations = annotationAccess.GetAnnotationsByType(GlobalAccess.Instance.Email, GlobalAccess.Instance.ServiceCode, annotationType);
                var dlBooks = publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential);
                annotationsFilteredByAnnotationType = AnnotationFactory.CreateAnnotations(annotations, this.tagDomainService.Tags, dlBooks);
            }
            if (annotationsFilteredByAnnotationType != null)
            {
                List<Annotation> noTags = null;
                if (includeNoTags)
                    noTags = annotationsFilteredByAnnotationType.FindAll(o => o.CategoryTagIDs.Count == 0);

                if (selectedTagIds != null && selectedTagIds.Count > 0)
                    annotationsFilteredByAnnotationType = annotationsFilteredByAnnotationType.FindAll(o => selectedTagIds.Intersect(o.CategoryTagIDs).Count() > 0);

                if (noTags != null)
                    annotationsFilteredByAnnotationType.AddRange(noTags);

            }
            return annotationsFilteredByAnnotationType;
        }

        public List<Annotation> GetAnnotationsByAnnotationType(List<Annotation> annotations, AnnotationType annotationType)
        {
            if (annotationType == AnnotationType.All)
            {
                return annotations;
            }
            else
            {
                return annotations.FindAll(o => o.Type == annotationType);
            }
        }

        public List<Annotation> GetAnnotationsbyNoTag(List<Annotation> annotations)
        {
            return annotations.FindAll(o => o.CategoryTagIDs.Count == 0);
        }

        public List<Annotation> GetAnnotationsByTagIds(List<Annotation> annotations, List<Guid> tagIds)
        {
            return annotations.FindAll(o => tagIds.Intersect(o.CategoryTagIDs).Count() > 0);
        }


        /// <summary>
        /// get tocid by annotation ,and used for get TOC By TOCId;
        /// </summary>
        /// <param name="annotation"></param>
        /// <returns></returns>
        public int GetTOCIdByAnnotation(Annotation annotation)
        {
            DomainEvents.Publish(new PublicationOpeningEvent(annotation.BookId, false)).Wait();
            var tocDetail = packageAccess.GetTOCDetailByDocId(GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName, annotation.DocId);

            return tocDetail.ID;

        }



        /// <summary>
        /// only for search annotationlist
        /// </summary>
        /// <returns></returns>
        public List<Annotation> SearchAnnotations(string searchText, AnnotationType annotationType)
        {
            var dlBooks = publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential);
            var annotations = annotationAccess.SearchAnnotations(searchText, GlobalAccess.Instance.Email, GlobalAccess.Instance.ServiceCode, annotationType);
            if (annotations == null)
                return null;

            var q = from anno in annotations
                    select AnnotationFactory.CreateAnnotation(anno.AnnotationContent, this.tagDomainService.Tags, dlBooks);
            return q.ToList();
        }



        /// <summary>
        /// SearchAnnotation
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="annotationType"></param>
        /// <returns></returns>
        public static List<Annotation> SearchAnnotation(string keywords, AnnotationType annotationType = AnnotationType.All)
        {
            List<Annotation> result = new List<Annotation>();

            List<Annotation> beforeRemoveDuplicateResult = new List<Annotation>();
            Dictionary<Guid, int> ExistedAnnotationIdDictionary = new Dictionary<Guid, int>();
            try
            {
                List<string> critieriaStringList = SegmentUtil.Instance.PhraseSegment(keywords);

                if (critieriaStringList.Count > 0)
                {
                    int SearchRound = 0;
                    foreach (String critieriaString in critieriaStringList)
                    {
                        SearchRound++;
                        foreach (Annotation element in AnnotationUtil.Instance.SearchAnnotations(critieriaString, annotationType))
                        {
                            beforeRemoveDuplicateResult.Add(element);
                            if (ExistedAnnotationIdDictionary.ContainsKey(element.AnnotationCode))
                            {
                                if (ExistedAnnotationIdDictionary[element.AnnotationCode] < SearchRound)
                                    ExistedAnnotationIdDictionary[element.AnnotationCode]++;
                            }
                            else
                            {
                                ExistedAnnotationIdDictionary.Add(element.AnnotationCode, 1);
                            }
                        }
                    }

                    HashSet<Guid> duplicateList = new HashSet<Guid>();
                    foreach (Annotation e in beforeRemoveDuplicateResult)
                    {
                        if (ExistedAnnotationIdDictionary[e.AnnotationCode] == critieriaStringList.Count &&
                               !duplicateList.Contains(e.AnnotationCode))
                            result.Add(e);

                        duplicateList.Add(e.AnnotationCode);
                    }

                }

                return result;
            }
            catch (Exception ex)
            {
                Logger.Log("Search Annotation Failed : " + ex.Message);
                return result;
            }
        }
    }
}
