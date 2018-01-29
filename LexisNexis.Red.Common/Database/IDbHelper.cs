using System.Collections.Generic;
namespace LexisNexis.Red.Common.Database
{
    public interface IDbHelper
    {
        string MainDbPath { get; }
        T GetEntity<T>(string DbPath, string query, params object[] args) where T : new();
        int DeleteEntity(string DbPath, object objectToDelete);
        List<T> GetEntityList<T>(string DbPath, string query, params object[] args) where T : new();
        int Update<T>(string DbPath, T entity);
        int Insert<T>(string dbPath, T entity);
        int Execute(string DbPath, string query, params object[] args);
        int ExecuteScalar(string dbPath, string query, params object[] args);
    }
}
