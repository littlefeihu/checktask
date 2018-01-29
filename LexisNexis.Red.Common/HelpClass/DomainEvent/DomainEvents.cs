using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass.DomainEvent
{
    public class DomainEvents
    {
        public static IEventhandleFactory DomainEventHandlerFactory { get; set; }

        public static Task Publish<T>(T domainEvent) where T : IEvent
        {
            return Task.Run(async () =>
            {
                await DomainEventHandlerFactory.GetDomainEventHandlersFor(domainEvent).ForEach(async (o) =>
                      {
                          await o.Handle(domainEvent);
                      });
            });
        }

    }
}
