using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.HelpClass;
using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
namespace LexisNexis.Red.Common.Entity
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }
        public DateTime LastSuccessfulLogin { get; set; }
        public bool LockedStatus { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string SymmetricKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CountryCode { get; set; }
        public bool SyncAnnotations { get; set; }
        public string LastSyncDate { get; set; }
        public bool IsAlive { get; set; }
        public bool NeedChangePassword { get; set; }

        static internal LoginStatusEnum ValidateUserInfo(string email, string password, string countryCode)
        {

            LoginStatusEnum loginStatus = LoginStatusEnum.LoginFailure;
            bool isValidEmail = false;
            bool isValidPassword = false;

            bool isEmptyPassword = string.IsNullOrEmpty(password);
            bool isEmptyEmail = string.IsNullOrEmpty(email);

            // isValidEmail = ValidateHelper.IsEmail(email);

            if (!string.IsNullOrEmpty(password))
                isValidPassword = (password.Length > Constants.VALID_PWD_LENGTH);

            if (string.IsNullOrEmpty(countryCode) && !isEmptyEmail && !isEmptyPassword)
            {
                loginStatus = LoginStatusEnum.SelectCountry;
                return loginStatus;
            }

            if (isEmptyEmail && isEmptyPassword)
            {
                loginStatus = LoginStatusEnum.EmptyEmailAndEmptyPwd;
            }
            else if (!isValidEmail && !isEmptyEmail && isValidPassword)
            {
                loginStatus = LoginStatusEnum.InvalidemailAndValidPwd;
            }
            else if (isValidEmail && !isValidPassword && !isEmptyPassword)
            {
                loginStatus = LoginStatusEnum.ValidemailAndInvalidPwd;
            }
            else if (!isEmptyEmail && !isValidEmail && !isValidPassword && !isEmptyPassword)
            {
                loginStatus = LoginStatusEnum.InvalidemailAndInvalidPwd;
            }
            else if (isValidEmail && isEmptyPassword)
            {
                loginStatus = LoginStatusEnum.ValidemailAndEmptyPwd;
            }
            else if (isEmptyEmail && isValidPassword)
            {
                loginStatus = LoginStatusEnum.EmptyemailAndValidPwd;
            }
            else if (isEmptyEmail && !isValidPassword && !isEmptyPassword)
            {
                loginStatus = LoginStatusEnum.EmptyemailAndInvalidPwd;
            }
            else if (!isValidEmail && !isEmptyEmail && isEmptyPassword)
            {
                loginStatus = LoginStatusEnum.InvalidemailAndEmptyPwd;
            }
            return loginStatus;
        }

    }
}
