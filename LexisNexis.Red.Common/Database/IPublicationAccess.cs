using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace LexisNexis.Red.Common.Database
{
    public interface IPublicationAccess
    {
        /// <summary>
        /// insert dlbook to localdb
        /// </summary>
        /// <param name="newBook"></param>
        void InsertDlBook(DlBook newBook);
        void InsertMetadata(BookMetadata metadata);
        List<DlBook> GetAllDlBooks(UserCredential user);

        List<DlBook> GetDeletedDlBooks(UserCredential user);

        List<DlBook> GetDlBooksOffline(UserCredential user);
        int UpdateDlBookStatus(int bookId, DlStatusEnum dlStatus, UserCredential userCredential);
        /// <summary>
        /// get dlbooks in offline mode
        /// </summary>
        /// <param name="IsDeleted">IsDeleted</param>
        /// <returns>List<DlBook></returns>
        List<DlBook> GetDlBooksOffline(UserCredential userCredential, bool? isDeleted = false);
        /// <summary>
        /// get dlbook by bookid
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns>DlBook</returns>
        DlBook GetDlBookByBookId(int bookId, UserCredential userCredential);

        BookMetadata GetDlBookMetedata(int bookId, UserCredential userCredential);

        DlBook GetDlBookByDpsiCode(UserCredential userCredential, string dpsiCode);
        /// <summary>
        /// delete dlbook by bookid
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        int DeleteDlBookByBookId(int bookId, UserCredential userCredential);
        /// <summary>
        /// update dlbook
        /// </summary>
        /// <param name="dlBook"></param>
        /// <returns></returns>
        int Update(DlBook dlBook);

        int Update(BookMetadata metadata);

        int DeleteRecentHistory(int bookId, UserCredential userCredential);
        Task UpdateDlBookOrder(Dictionary<int, int> orders, UserCredential userCredential);
    }
}
