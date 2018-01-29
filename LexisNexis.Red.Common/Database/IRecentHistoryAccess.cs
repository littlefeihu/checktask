using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public interface IRecentHistoryAccess
    {
        void DeleteOlderRecentHistory();
        List<RecentHistory> GetRecentHistory(string email, string serviceCode, int? bookId = null, int topNum = 10);
        int UpdateRecentHistory(RecentHistory recentHistory);

        List<RecentHistory> GetAllRecentHistoriesByBookID(string email, string serviceCode, int bookId);
        int DeleteRecentHistory(RecentHistory recentHistory);
        int DeleteRecentHistoryByDocIDs(string docIDs, int bookId, string email, string serviceCode);
        int DeleteRecentHistoryByBookID(int bookId, string email, string serviceCode);
    }
}
