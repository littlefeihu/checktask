using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.EventHandles
{
    public class PublicationOpeningEventHandle : IEventHandler<PublicationOpeningEvent>
    {
        private IConnectionMonitor connectionMonitor;
        private IPackageAccess packageAccess;
        private IPublicationAccess publicationAccess;
        public PublicationOpeningEventHandle(IConnectionMonitor connection, IPackageAccess packageAccess, IPublicationAccess publicationAccess)
        {
            this.connectionMonitor = connection;
            this.packageAccess = packageAccess;
            this.publicationAccess = publicationAccess;
        }
        public async Task Handle(PublicationOpeningEvent evt)
        {
            if (evt.EnforceUpdate)
            {
                if (evt.NeedSearchDb())
                {
                    await EnforceRefreshCurrentPublicationByBookId(evt.BookId.Value);
                }
                else
                {
                    await EnforceRefreshCurrentPublicationByDlBook(evt.DlBook);
                }
            }
            else
            {
                await RefreshCurrentPublication(evt.BookId.Value);
            }
        }
        private async Task EnforceRefreshCurrentPublicationByBookId(int bookId)
        {
            var dlBook = publicationAccess.GetDlBookByBookId(bookId, GlobalAccess.Instance.UserCredential);
            await EnforceRefreshCurrentPublicationByDlBook(dlBook);
        }
        private async Task EnforceRefreshCurrentPublicationByDlBook(DlBook dlBook)
        {
            //dlBook = new DlBook { BookId = 1, Author = "Allen", Email = "378917466@qq.com", ServiceCode = "AUNZ", DlStatus = (short)DlStatusEnum.Downloaded, CurrencyDate = DateTime.Now, Description = "在程序开发的世界里，各路前辈们为了提高所谓的编码速度，搞出了各式各样的代码生成器，来避免所谓的重复的人为机械地粘贴和复制", Name = "枢纽", ColorPrimary = "#00FF00", ColorSecondary = "#C0C0C0", FontColor = "#000000", DpsiCode = "0299" };

            //if (dlBook != null)
            //{
            //    var contentKey = await dlBook.GetContentKey(GlobalAccess.Instance.CurrentUserInfo.SymmetricKey);
            //    var publicationContent = new PublicationContent(dlBook, contentKey);
            //    publicationContent.HasPage = packageAccess.HasPage(publicationContent.DecryptedDbFullName);
            //    if (publicationContent.HasPage)
            //    {
            //        publicationContent.Pages = packageAccess.GetPageList(publicationContent.DecryptedDbFullName);
            //    }
            //    GlobalAccess.Instance.CurrentPublication = publicationContent;
            //}
            //else
            //{
            //    Logger.Log("RefreshCurrentPublication DlBookNoneExistsException");
            //    //throw new DlBookNoneExistsException();
            //}
        }
        private async Task RefreshCurrentPublication(int bookId)
        {
            //var isSamePublication = (GlobalAccess.Instance.CurrentPublication != null) && (GlobalAccess.Instance.CurrentPublication.DlBook.BookId == bookId);
            //if (isSamePublication)
            //{
            //    return;
            //}
            await EnforceRefreshCurrentPublicationByBookId(bookId);
        }
    }
}
