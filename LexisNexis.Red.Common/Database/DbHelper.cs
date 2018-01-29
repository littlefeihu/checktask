using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.HelpClass;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace LexisNexis.Red.Common.Database
{
    public class DbHelper : IDbHelper
    {
        private string mainDbPath = null;
        public string MainDbPath
        {
            get
            {
                return mainDbPath;
            }
        }
        public DbHelper()
        {
            mainDbPath = Path.Combine(GlobalAccess.DirectoryService.GetAppRootPath(), Constants.DATABASE_FILE_NAME);
        }
        public T GetEntity<T>(string DbPath, string query, params object[] args) where T : new()
        {
            T t = default(T);
            using (var db = SqliteCon.CreateConnection(DbPath))
            {
                t = db.Query<T>(query, args).FirstOrDefault();
            }
            return t;
        }

        public int DeleteEntity(string dbPath, object objectToDelete)
        {
            using (var db = SqliteCon.CreateConnection(dbPath))
            {
                return db.Delete(objectToDelete);
            }
        }

        public List<T> GetEntityList<T>(string dbPath, string query, params object[] args) where T : new()
        {
            using (var db = SqliteCon.CreateConnection(dbPath))
            {
                return db.Query<T>(query, args);
            }
        }
        public int Update<T>(string dbPath, T entity)
        {

            using (var db = SqliteCon.CreateConnection(dbPath))
            {
                return db.Update(entity);
            }

        }

        public int Insert<T>(string dbPath, T entity)
        {

           using (var db = SqliteCon.CreateConnection(dbPath))
           {
               return db.Insert(entity);
           }
        }

        public int Execute(string dbPath, string query, params object[] args)
        {

            using (var db = SqliteCon.CreateConnection(dbPath))
            {
                return db.Execute(query, args);
            }

        }
        public int Execute(string query, params object[] args)
        {

       	    using (var db = SqliteCon.CreateConnection(mainDbPath))
            {
                return db.Execute(query, args);
            }
        }
        public void ExecuteWithTransaction(List<string> sqls)
        {
            using (var db = SqliteCon.CreateConnection(mainDbPath))
            {
                try
                {
                    db.BeginTransaction();
                    foreach (var sql in sqls)
                    {
                        db.Execute(sql);
                    }
                    db.Commit();
                }
                catch (System.Exception)
                {
                    db.Rollback();
                }
            }

        }
        public int ExecuteScalar(string dbPath, string query, params object[] args)
        {
            using (var db = SqliteCon.CreateConnection(dbPath))
            {
                return db.ExecuteScalar<int>(query, args);
            }
        }

    }
}
