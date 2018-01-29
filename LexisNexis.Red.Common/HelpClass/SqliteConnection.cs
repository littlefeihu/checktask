using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class SqliteCon
    {
        public static SQLiteConnection CreateConnection(string databasePath)
        {
            var con = new SQLiteConnection(databasePath);
            con.BusyTimeout = new TimeSpan(0, 0, 5);
            return con;
        }
    }
}
