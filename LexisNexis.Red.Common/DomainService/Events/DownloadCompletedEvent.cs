using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.Events
{
    public class DownloadCompletedEvent : IEvent
    {
        public DownloadCompletedEvent(DlBook dlBook, Action recenthistoryChanged)
        {
            this.DlBook = dlBook;
            this.RecenthistoryChanged = recenthistoryChanged;
        }
        public Action RecenthistoryChanged
        {
            get;
            private set;
        }

        public DlBook DlBook
        {
            get;
            private set;
        }
    }
}
