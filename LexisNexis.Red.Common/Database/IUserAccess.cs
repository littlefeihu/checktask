using LexisNexis.Red.Common.Entity;
namespace LexisNexis.Red.Common.Database
{
    public interface IUserAccess
    {

        User LoginOffline(string email, string password, string countryCode);
        /// <summary>
        /// get userinfo by email and countryCode
        /// </summary>
        /// <param name="email"></param>
        /// <param name="countryCode"></param>
        /// <returns>User</returns>
        User GetUserByEmail(string email, string countryCode);
        /// <summary>
        /// get last login userinfo for binding country
        /// </summary>
        /// <returns>User</returns>
        User GetLastUserLoginInfo();
        /// <summary>
        /// get last logininfo for automatic login
        /// </summary>
        /// <returns>User</returns>
        User GetLastIsAliveLoginUser();
        int UpdateSymmetricKey(string symmetricKey, string email, string countrycode);
        int UpdateLastLoginDate(string email, string countrycode);
        /// <summary>
        /// insert userinfo to localdb
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        int InsertUser(User user);
        /// <summary>
        /// logout
        /// </summary>
        int Logout(string email, string countryCode);
        /// <summary>
        /// update userinfo
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Update(User entity);
        int UpdateUserLastSyncDate(string email, string countryCode);
        int UpdateUserStatus(User user);
        int UpdatePassword(string email, string countryCode, string pwd);
    }
}
