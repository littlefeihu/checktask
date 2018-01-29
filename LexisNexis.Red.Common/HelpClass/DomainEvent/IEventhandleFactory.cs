using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass.DomainEvent
{
    public interface IEventhandleFactory
    {
        IEnumerable<IEventHandler<T>> GetDomainEventHandlersFor<T>(T domainEvent) where T : IEvent;
    }
}
