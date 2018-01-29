using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.Events
{
    public class PublicationOpeningEvent : IEvent
    {
        public PublicationOpeningEvent(int? bookId, bool enforceUpdate)
        {
            BookId = bookId;
            EnforceUpdate = enforceUpdate;
        }
        public PublicationOpeningEvent(DlBook dlBook, bool enforceUpdate)
        {
            DlBook = dlBook;
            EnforceUpdate = enforceUpdate;
        }
        public bool EnforceUpdate
        {
            get;
            set;
        }
        public bool NeedSearchDb()
        {
            return DlBook == null;
        }
        public int? BookId { get; private set; }

        public DlBook DlBook { get; private set; }
    }
}
