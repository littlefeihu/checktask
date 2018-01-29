using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LexisNexis.Red.Common.Database
{
    public class UserAccess : DbHelper, IUserAccess
    {
        /// <summary>
        /// get local dlbook and check whether the user can open mypublication screen without logining
        /// </summary>
        private const string loginOfflineQuery = "SELECT * FROM User WHERE LockedStatus=0 AND NeedChangePassword=0  AND LOWER(Email)=? AND Password=?  AND CountryCode=? LIMIT 0,1";
        /// <summary>
        /// get user by email 
        /// </summary>
        private const string getUserByEmailQuery = "SELECT * FROM User WHERE LOWER(Email)=?  AND CountryCode=? LIMIT 0,1";
        /// <summary>
        /// get last login information
        /// </summary>
        private const string getLastUserLoginInfoQuery = "SELECT  * FROM User  ORDER BY LastSuccessfulLogin DESC LIMIT 0,1";
        /// <summary>
        /// get last login and isalive user 
        /// </summary>
        private const string getLastIsAliveLoginUserQuery = "SELECT  * FROM User WHERE LockedStatus=0 AND NeedChangePassword=0 AND isalive=1  AND SymmetricKey IS NOT NULL ORDER BY LastSuccessfulLogin DESC LIMIT 0,1";
        /// <summary>
        /// get current login user
        /// </summary>
        private const string getLoginUserQuery = "SELECT  * FROM User WHERE email=? AND Password=? AND countrycode=? LIMIT 0,1";

        private const string updateSyncDateSql = "UPDATE User SET LastSyncDate=? WHERE email=? AND countryCode=?";
        private const string updateIsAliveSql = "UPDATE User SET IsAlive=0 WHERE email=? AND countryCode=?";
        private const string updateLastSuccessfulLoginSql = "UPDATE User SET LastSuccessfulLogin=?,IsAlive=1 WHERE email=? AND countryCode=?";
        private const string updateSymmetricKeySql = "UPDATE User SET SymmetricKey=? WHERE email=? AND countryCode=?";
        private const string updateUserStatusSql = "UPDATE User SET IsAlive=?,LockedStatus=?,NeedChangePassword=?,Password=?,SyncAnnotations=? WHERE email=? AND countryCode=?";
        private const string updateUserPassword = "UPDATE User SET NeedChangePassword=0,Password=? WHERE email=? AND countryCode=?";
        public User LoginOffline(string email, string password, string countryCode)
        {
            //notlocked not need to change pwd and devicehas register
            return base.GetEntity<User>(base.MainDbPath, loginOfflineQuery, email.ToLower(), password, countryCode);
        }

        public User GetUserByEmail(string email, string countrycode)
        {
            User user = base.GetEntity<User>(base.MainDbPath, getUserByEmailQuery, email.ToLower(), countrycode);
            return user;
        }

        public int UpdateSymmetricKey(string symmetricKey, string email, string countrycode)
        {
            return base.Execute(base.MainDbPath, updateSymmetricKeySql, symmetricKey, email, countrycode);
        }
        public int UpdateLastLoginDate(string email, string countrycode)
        {
            return base.Execute(base.MainDbPath, updateLastSuccessfulLoginSql, DateTime.Now, email, countrycode);
        }

        public User GetLastUserLoginInfo()
        {
            User user = base.GetEntity<User>(base.MainDbPath, getLastUserLoginInfoQuery);
            return user;
        }

        public User GetLastIsAliveLoginUser()
        {
            return base.GetEntity<User>(base.MainDbPath, getLastIsAliveLoginUserQuery);
        }


        public int Logout(string email, string countryCode)
        {
            return base.Execute(base.MainDbPath, updateIsAliveSql, email, countryCode);
        }

        public int InsertUser(User user)
        {
            if (user == null) return -1;
            return base.Insert<User>(base.MainDbPath, user);
        }

        public int UpdateUserStatus(User user)
        {
            return base.Execute(base.MainDbPath, updateUserStatusSql,   user.IsAlive,
                                                                        user.LockedStatus,
                                                                        user.NeedChangePassword,
                                                                        user.Password,
                                                                        user.SyncAnnotations,
                                                                        user.Email,
                                                                        user.CountryCode);
        }
        public int Update(User entity)
        {
            return base.Update<User>(base.MainDbPath, entity);
        }
        public int UpdateUserLastSyncDate(string email, string countryCode)
        {
            return base.Execute(base.MainDbPath, updateSyncDateSql, DateTime.Now.ToString(), email, countryCode);
        }
        public int UpdatePassword(string email, string countryCode, string pwd)
        {
            return Execute(MainDbPath, updateUserPassword, pwd, email, countryCode);
        }
    }
}
