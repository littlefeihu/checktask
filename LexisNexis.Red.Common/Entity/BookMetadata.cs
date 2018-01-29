using SQLite;
using System;
namespace LexisNexis.Red.Common.Entity
{
    public class BookMetadata
    {
        [PrimaryKey, AutoIncrement]
        public int RowId { set; get; }
        public int BookId { set; get; }
        public string ServiceCode { get; set; }
        public string Email { set; get; }
        public string IncludedGuideCard { set; get; }

    }
}
