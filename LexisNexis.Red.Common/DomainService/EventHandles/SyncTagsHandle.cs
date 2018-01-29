using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.EventHandles
{
    public class SyncTagsHandle : IEventHandler<SyncTagsEvent>
    {
        IAnnotationSyncService annotationSyncService;
        public SyncTagsHandle(IAnnotationSyncService annotationSyncService)
        {
            this.annotationSyncService = annotationSyncService;
        }

        public Task Handle(SyncTagsEvent evt)
        {
            return Task.Run(() =>
            {
                this.annotationSyncService.RequestTagsOnly();
            });
        }
    }
}
