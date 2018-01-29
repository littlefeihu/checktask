using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Entity
{
    public class Settings
    {
        [PrimaryKey, AutoIncrement]
        public int RowId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Email { get; set; }
        public string ServiceCode { get; set; }
    }
}
