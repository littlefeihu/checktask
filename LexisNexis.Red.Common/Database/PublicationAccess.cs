using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public class PublicationAccess : DbHelper, IPublicationAccess
    {
        /// <summary>
        /// get local dlbook which is deleted by user
        /// </summary>
        private string getDlBooksOfflineIsDeletedQuery = "SELECT * FROM DLBook WHERE Email=? AND ServiceCode=? AND DlStatus=3";
        /// <summary>
        /// get local dlbook which is not deleted by user
        /// </summary>
        private string getDlBooksOfflineNotDeletedQuery = "SELECT * FROM DLBook WHERE Email=? AND ServiceCode=? AND DlStatus IN (1,2)";
        /// <summary>
        /// get local dlbook which is not deleted by user
        /// </summary>
        private string getAllDlBooksOfflineQuery = "SELECT * FROM DLBook WHERE Email=? AND ServiceCode=? ";
        /// <summary>
        /// get local dlbook by the bookid 
        /// </summary>
        private string getDlBookByBookIdQuery = "SELECT * FROM DLBook WHERE Email=? AND ServiceCode=? AND BookId=?";
        /// <summary>
        /// get local dlbook metadata by bookid
        /// </summary>
        private string getDlBookMetedataQuery = "SELECT * FROM BookMetadata WHERE Email=? AND ServiceCode=? AND BookId=?";
        /// <summary>
        /// delete book history by bookid
        /// </summary>
        private string deleteRecentHistorySql = "DELETE FROM RecentHistory WHERE BookId=? AND Email=? AND ServiceCode=?";
        /// <summary>
        /// update local dlbook orderby by bookid
        /// </summary>
        private string resetDlBookOrderSql = "UPDATE DLBook SET OrderBy={0} WHERE Email='{1}' AND ServiceCode='{2}' AND BookId={3}";

        private string getDlBookByDpsiCode = "SELECT * FROM DLBook WHERE Email=? AND ServiceCode=? AND DpsiCode=?";

        private string updateDlBookStatus = "UPDATE DLBook SET DlStatus=? WHERE Email=? AND ServiceCode=? AND BookId=?";

        public void InsertDlBook(DlBook newBook)
        {
            Insert<DlBook>(MainDbPath, newBook);
        }

        public void InsertMetadata(BookMetadata metadata)
        {
            Insert<BookMetadata>(MainDbPath, metadata);
        }

        public List<DlBook> GetAllDlBooks(UserCredential user)
        {
            return GetEntityList<DlBook>(MainDbPath, getAllDlBooksOfflineQuery, user.Email, user.ServiceCode);
        }
        public List<DlBook> GetDeletedDlBooks(UserCredential user)
        {
            return GetEntityList<DlBook>(MainDbPath, getDlBooksOfflineIsDeletedQuery, user.Email, user.ServiceCode);
        }
        public List<DlBook> GetDlBooksOffline(UserCredential user)
        {
            return GetEntityList<DlBook>(MainDbPath, getDlBooksOfflineNotDeletedQuery, user.Email, user.ServiceCode);
        }

        public List<DlBook> GetDlBooksOffline(UserCredential userCredential, bool? isDeleted = false)
        {
            string querySql = null;
            if (isDeleted.HasValue)
            {
                if (isDeleted.Value)
                {//get dlbook which Removed by user
                    querySql = getDlBooksOfflineIsDeletedQuery;
                }
                else
                {//get downloaded and Not downloaded status's local dlbook
                    querySql = getDlBooksOfflineNotDeletedQuery;
                }
            }
            else
            {
                //get all local dlbook
                querySql = getAllDlBooksOfflineQuery;
            }
            return GetEntityList<DlBook>(MainDbPath, querySql, userCredential.Email, userCredential.ServiceCode);
        }

        public DlBook GetDlBookByBookId(int bookId, UserCredential userCredential)
        {
            return GetEntity<DlBook>(MainDbPath, getDlBookByBookIdQuery, userCredential.Email, userCredential.ServiceCode, bookId);
        }

        public BookMetadata GetDlBookMetedata(int bookId, UserCredential userCredential)
        {
            return GetEntity<BookMetadata>(MainDbPath, getDlBookMetedataQuery, userCredential.Email, userCredential.ServiceCode, bookId);
        }
        public DlBook GetDlBookByDpsiCode(UserCredential userCredential, string dpsiCode)
        {
            return GetEntity<DlBook>(MainDbPath, getDlBookByDpsiCode, userCredential.Email, userCredential.ServiceCode, dpsiCode);
        }
        public int DeleteDlBookByBookId(int bookId, UserCredential userCredential)
        {
            int deleteResult = 0;
            var dlBook = GetDlBookByBookId(bookId, userCredential);
            if (dlBook != null)
            {
                deleteResult = DeleteEntity(MainDbPath, dlBook);
            }
            return deleteResult;
        }
        public int Update(DlBook entity)
        {
            return Update<DlBook>(MainDbPath, entity);
        }

        public int Update(BookMetadata entity)
        {
            return Update<BookMetadata>(MainDbPath, entity);
        }

        public int DeleteRecentHistory(int bookId, UserCredential userCredential)
        {
            return Execute(deleteRecentHistorySql, bookId, userCredential.Email, userCredential.ServiceCode);
        }

        public int UpdateDlBookStatus(int bookId, DlStatusEnum dlStatus, UserCredential userCredential)
        {
            return Execute(updateDlBookStatus, (short)dlStatus, userCredential.Email, userCredential.ServiceCode, bookId);

        }

        public Task UpdateDlBookOrder(Dictionary<int, int> orders, UserCredential userCredential)
        {
            return Task.Run(() =>
            {
                try
                {
                    var q = from order in orders
                            select string.Format(resetDlBookOrderSql, order.Value, userCredential.Email, userCredential.ServiceCode, order.Key);
                    ExecuteWithTransaction(q.ToList());
                }
                catch (System.Exception ex)
                {
                    Logger.Log("UpdateDlBookOrder" + ex.ToString());
                }
            });
        }

    }
}
