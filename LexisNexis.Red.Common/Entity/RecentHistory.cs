using SQLite;
using System;
namespace LexisNexis.Red.Common.Entity
{
    public class RecentHistory
    {
        [PrimaryKey, AutoIncrement]
        public int RowId { set; get; }
        public string Email { set; get; }
        public string ServiceCode { set; get; }
        public int BookId { set; get; }
        public string DocID { get; set; }
        public string TOCTitle { set; get; }
        public string DlBookTitle { set; get; }
        public string ColorPrimary { set; get; }
        public DateTime LastReadDate { set; get; }

    }
}
