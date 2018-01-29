using SQLite;
using System;
using System.IO;
using System.Runtime.Serialization;
namespace LexisNexis.Red.Common.BusinessModel
{

    public class LoginUserDetails
    {
        public LoginUserDetails(string email, bool locked, string pwd, Country country,
                                bool needChangePWD, bool syncAnnotations, string symmetricKey,
                                string firstName, string lastName, string lastSyncDate)
        {
            Email = email;
            Locked = locked;
            Password = pwd;
            Country = country;
            NeedChangePassword = needChangePWD;
            SyncAnnotations = syncAnnotations;
            SymmetricKey = symmetricKey;
            FirstName = firstName;
            LastName = lastName;
            DateTime syncDate;
            if (DateTime.TryParse(lastSyncDate, out syncDate))
            {
                LastSyncDate = syncDate;
            }
            else
            {
                LastSyncDate = null;
            }
        }

        public string Email { get; internal set; }

        public string Password { get; internal set; }

        public bool NeedChangePassword { get; internal set; }

        public bool Locked { get; internal set; }

        public string FullName
        {
            get
            {
                return (FirstName ?? string.Empty) + " " + (LastName ?? string.Empty);
            }
        }

        public bool SyncAnnotations { get; internal set; }

        public Country Country { get; internal set; }


        public string SymmetricKey { get; internal set; }

        public int FontSize { get; internal set; }

        public string FirstName { get; internal set; }

        public string LastName { get; internal set; }
        public DateTime? LastSyncDate { get; internal set; }
        /// <summary>
        /// Current User folder Directory
        /// </summary>
        public string UserFolder
        {
            get
            {
                return Path.Combine(Country.ServiceCode, Email);
            }
        }

    }
}
