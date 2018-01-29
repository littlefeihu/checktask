using LexisNexis.Red.Common.DomainService.EventHandles;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common
{
    public class EventHandlesMapping
    {
        public static void HandlesMapping()
        {
            IoCContainer.Instance.RegisterInterface<IEventhandleFactory, EventHandleFactory>();
            IoCContainer.Instance.RegisterInterface<IEventHandler<LastSyncedTimeChangedEvent>, LastSyncedTimeChangedHandle>("LastSyncedTimeChangedEvent");
            IoCContainer.Instance.RegisterInterface<IEventHandler<PublicationOpeningEvent>, PublicationOpeningEventHandle>("PublicationOpeningEvent");
            IoCContainer.Instance.RegisterInterface<IEventHandler<LoginSuccessEvent>, LoginSuccessHandle>("LoginSuccessHandle");
            IoCContainer.Instance.RegisterInterface<IEventHandler<DownloadCompletedEvent>, DownloadCompetedHandle>("DownloadCompetedHandle");
            IoCContainer.Instance.RegisterInterface<IEventHandler<SyncTagsEvent>, SyncTagsHandle>("TagChangedHandle");
            IoCContainer.Instance.RegisterInterface<IEventHandler<GetPublicationOnlineEvent>, GetPublicationOnlineEventHandle>("GetPublicationOnlineEventHandle");
            IoCContainer.Instance.RegisterInterface<IEventHandler<TagDeletedEvent>, TagDeletedHandle>("TagDeletedHandle");
            DomainEvents.DomainEventHandlerFactory = IoCContainer.Instance.Resolve<IEventhandleFactory>();

        }
    }
}
