using LexisNexis.Red.Common.HelpClass.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.Events
{
    public class TagDeletedEvent : IEvent
    {
        public TagDeletedEvent(Guid tagId, string email, string serviceCode)
        {
            TagId = tagId;
            Email = email;
            ServiceCode = serviceCode;
        }
        public Guid TagId
        {
            get;
            private set;
        }
        public string Email
        {
            get;
            private set;
        }
        public string ServiceCode
        {
            get;
            private set;
        }
    }
}
