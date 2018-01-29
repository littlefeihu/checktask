using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Common.DomainService.EventHandles
{
    public class TagDeletedHandle : IEventHandler<TagDeletedEvent>
    {
        private IAnnotationAccess annotationAccess;
        public TagDeletedHandle(IAnnotationAccess annotationAccess)
        {
            this.annotationAccess = annotationAccess;
        }

        public Task Handle(TagDeletedEvent evt)
        {
            return Task.Run(() =>
            {
                var annotations = annotationAccess.GetAnnotations(evt.Email, evt.ServiceCode);
                if (annotations.Count > 0)
                {
                    var ConvertedAnnotations = AnnotationFactory.CreateAnnotations(annotations);
                    foreach (var annotation in ConvertedAnnotations)
                    {
                        if (annotation.CategoryTagIDs.Remove(evt.TagId))
                            annotationAccess.UpdateAnnotation(annotation.DocId, annotation.Type, AnnotationStatusEnum.Updated, annotation.ToXmlString(), DateTime.Now, false, annotation.AnnotationCode);
                    }
                }
            });
        }
    }
}
