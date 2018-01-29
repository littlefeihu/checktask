using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DominService
{
    public interface ILoginDomainService
    {
        User LoginOffline(string email, string password, string countryCode);
        Task<Tuple<User, LoginStatusEnum>> LoginOnline(string email, string password, string countryCode);
        Task<PasswordResetEnum> ResetPassword(string email, string countryCode);
        Task<PasswordChangeEnum> PasswordChange(string email, string countryCode, string password1, string password2);
        LoginUserDetails GetLastUserLoginInfo();
        User GetLastIsAliveLoginUser();
        void Logout(string email, string countryCode);
        LoginUserDetails UpdateCurrentUser(User user);
    }
}
