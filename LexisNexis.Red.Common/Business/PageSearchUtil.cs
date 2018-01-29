using LexisNexis.Red.Common.BusinessModel;
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

namespace LexisNexis.Red.Common.Business
{
    public class PageSearchUtil : AppServiceBase<PageSearchUtil>
    {
        IPackageAccess packageAccess;
        public PageSearchUtil(IPackageAccess packageAccess)
        {
            this.packageAccess = packageAccess;
        }

        public async Task<bool> IsPBO(int bookId)
        {
            await DomainEvents.Publish(new PublicationOpeningEvent(bookId, false));
            return GlobalAccess.Instance.CurrentPublication.HasPage;
        }

        public async Task<List<PageItem>> GetPagesByTOCID(int bookId, int tocId)
        {
            await DomainEvents.Publish(new PublicationOpeningEvent(bookId, false));
            var currentPublication = GlobalAccess.Instance.CurrentPublication;
            if (currentPublication.HasPage)
            {
                return currentPublication.Pages.Where(page => page.TOCID == tocId).ToList();
            }
            return null;
        }

        public async Task<PageItem> GetFirstPageItem(int bookId, int tocId)
        {
            await DomainEvents.Publish(new PublicationOpeningEvent(bookId, false));
            var currentPublication = GlobalAccess.Instance.CurrentPublication;
            if (currentPublication.HasPage)
            {
                var pages = await GetPagesByTOCID(bookId, tocId);
                return pages.FirstOrDefault();
            }
            return null;
        }

        public async Task<int> GetMaxPageNum(int bookId)
        {
            await DomainEvents.Publish(new PublicationOpeningEvent(bookId, false));
            var currentPublication = GlobalAccess.Instance.CurrentPublication;
            if (currentPublication.HasPage)
            {
                return currentPublication.Pages.Max((o) => o.Identifier);
            }
            else
            {
                return 0;
            }
        }
        public async Task<List<PageSearchItem>> SeachByPageNum(int bookId, int pageNum)
        {
            await DomainEvents.Publish(new PublicationOpeningEvent(bookId, false));
            var currentPublication = GlobalAccess.Instance.CurrentPublication;
            if (currentPublication.HasPage)
            {
                return (
                       from d in currentPublication.Pages
                       where d.Identifier == pageNum
                       select new PageSearchItem
                       {
                           FileTitle = d.Heading,
                           GuideCardTitle = d.Description + (d.StartPageNo != pageNum ? " (from page " + d.StartPageNo + ")" : ""),
                           TOCID = d.TOCID
                       }
                   ).ToList();
            }
            return null;
        }
    }
}
