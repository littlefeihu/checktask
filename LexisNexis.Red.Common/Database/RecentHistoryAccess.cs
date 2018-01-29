using LexisNexis.Red.Common.Entity;
using System.Collections.Generic;
using System;
namespace LexisNexis.Red.Common.Database
{
    public class RecentHistoryAccess : DbHelper, IRecentHistoryAccess
    {
        private string getRecentHistoryListByBookIdSql = "SELECT * FROM RecentHistory WHERE Email=? AND serviceCode=? AND BookId=?  ORDER BY LastReadDate DESC  LIMIT 0,? ";
        private string getRecentHistoryListSql = "SELECT * FROM RecentHistory WHERE Email=? AND serviceCode=?  ORDER BY LastReadDate DESC  LIMIT 0,? ";
        private string getrecentHistoryByDOCId = "SELECT * FROM RecentHistory WHERE Email=? AND serviceCode=? AND BookId=? AND DocID=? LIMIT 0,1 ";
        private string deleteRecentHistoryByDocIDs = "DELETE FROM RecentHistory WHERE DocID in ({0}) AND bookId=? AND Email=? AND serviceCode=?";
        private string deleteRecentHistoryByBookID = "DELETE FROM RecentHistory WHERE  bookId=? AND Email=? AND serviceCode=?";
        private string getAllRecentHistoryListByBookIdSql = "SELECT * FROM RecentHistory WHERE Email=? AND serviceCode=? AND  BookId=?";
        private string deleteRecentHistorySql = "DELETE FROM RecentHistory WHERE RowId IN (SELECT RowId FROM RecentHistory  ORDER BY LastReadDate DESC LIMIT 10,5000)";
        public List<RecentHistory> GetRecentHistory(string email, string serviceCode, int? bookId = null, int topNum = 10)
        {
            if (bookId != null)
                return base.GetEntityList<RecentHistory>(base.MainDbPath, getRecentHistoryListByBookIdSql, email, serviceCode, bookId, topNum);
            else
                return base.GetEntityList<RecentHistory>(base.MainDbPath, getRecentHistoryListSql, email, serviceCode, topNum);
        }
        public void DeleteOlderRecentHistory()
        {
            Execute(deleteRecentHistorySql);
        }
        public List<RecentHistory> GetAllRecentHistoriesByBookID(string email, string serviceCode, int bookId)
        {
            return GetEntityList<RecentHistory>(MainDbPath, getAllRecentHistoryListByBookIdSql, email, serviceCode, bookId);
        }
     
        public int UpdateRecentHistory(RecentHistory recentHistory)
        {
            var history = base.GetEntity<RecentHistory>(MainDbPath, getrecentHistoryByDOCId, recentHistory.Email,
                                                                                            recentHistory.ServiceCode,
                                                                                            recentHistory.BookId,
                                                                                            recentHistory.DocID);
            if (history != null)
            {//update lastReadDate
                recentHistory.RowId = history.RowId;
                return base.Update<RecentHistory>(base.MainDbPath, recentHistory);
            }
            return base.Insert<RecentHistory>(base.MainDbPath, recentHistory);
        }
        public int DeleteRecentHistory(RecentHistory recentHistory)
        {
            return base.DeleteEntity(base.MainDbPath, recentHistory);
        }
        public int DeleteRecentHistoryByDocIDs(string docIDs, int bookId, string email, string serviceCode)
        {
            return base.Execute(base.MainDbPath, string.Format(deleteRecentHistoryByDocIDs, docIDs), bookId, email, serviceCode);
        }
        public int DeleteRecentHistoryByBookID(int bookId, string email, string serviceCode)
        {
            return base.Execute(base.MainDbPath, deleteRecentHistoryByBookID, bookId, email, serviceCode);
        }
    }
}
