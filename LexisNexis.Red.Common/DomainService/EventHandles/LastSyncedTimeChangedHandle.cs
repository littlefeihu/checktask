using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.EventHandles
{
    public class LastSyncedTimeChangedHandle : IEventHandler<LastSyncedTimeChangedEvent>
    {
        public Task Handle(LastSyncedTimeChangedEvent evt)
        {
            return Task.Run(() =>
            {
                SettingsUtil.Instance.UpdateLastSyncedTime();
            });
        }
    }
}
