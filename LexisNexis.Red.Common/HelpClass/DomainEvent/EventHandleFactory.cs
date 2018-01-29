using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass.DomainEvent
{
    public class EventHandleFactory : IEventhandleFactory
    {
        public IEnumerable<IEventHandler<T>> GetDomainEventHandlersFor<T>(T domainEvent) where T : IEvent
        {
            return IoCContainer.Instance.ResolveAll<IEventHandler<T>>();
        }
    }
}
