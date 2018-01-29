using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LexisNexis.Red.Common.Entity
{
    public class AnnCategoryTags
    {
        [PrimaryKey, AutoIncrement]
        public int RowId { get; set; }

        public string Email { get; set; }

        public string ServiceCode { get; set; }

        public DateTime? LastServerSyncTime { get; set; }

        public DateTime? LastUpdateTimeClient { get; set; }

        public bool IsModified { get; set; }

        public string CategoryTagXMLData { get; set; }
    }
}
