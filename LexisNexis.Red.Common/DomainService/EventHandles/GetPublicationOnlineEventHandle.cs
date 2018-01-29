using LexisNexis.Red.Common.Business;
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
    public class GetPublicationOnlineEventHandle : IEventHandler<GetPublicationOnlineEvent>
    {
        public Task Handle(GetPublicationOnlineEvent evt)
        {
            return Task.Run(() =>
            {
                DictionaryUtil.UpdateDictionary("AU").ContinueWith((t) =>
                {
                    DictionaryUtil.UpdateDictionary("NZ");
                });
            });
        }

    }
}
