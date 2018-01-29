using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.EventHandles
{
    public class DownloadCompetedHandle : IEventHandler<DownloadCompletedEvent>
    {
        IOrphanItemsService orphanItemsService;
        IPackageAccess packageAccess;
        public DownloadCompetedHandle(IOrphanItemsService orphanItemsService, IPackageAccess packageAccess)
        {
            this.orphanItemsService = orphanItemsService;
            this.packageAccess = packageAccess;
        }
        public Task Handle(DownloadCompletedEvent evt)
        {
            return Task.Run(async () =>
            {
                orphanItemsService.ProcessOrphanedData(evt.DlBook);
                if (evt.RecenthistoryChanged != null)
                    evt.RecenthistoryChanged();
                //if (NavigationManager.Instance.Records.Exists(o => o.BookID == evt.DlBook.BookId))
                //{
                //    var tocDetail = packageAccess.GetFirstTOCDetail(evt.DlBook.GetDecryptedDbFullName());
                //    if (tocDetail != null)
                //        NavigationManager.Instance.CalculateCurrentIndex(evt.DlBook.BookId, tocDetail.ID);
                //}
                await UpdateCurrentPublication(evt.DlBook);
            });
        }

        private async Task UpdateCurrentPublication(DlBook dlBook)
        {
            var isSamePublication = (GlobalAccess.Instance.CurrentPublication != null
                               && GlobalAccess.Instance.CurrentPublication.DlBook.BookId == dlBook.BookId);
            //update currentPublication infomation when current publication is the downloaded publication
            if (isSamePublication)
            {
                await DomainEvents.Publish(new PublicationOpeningEvent(dlBook, true));
            }
        }
    }
}
