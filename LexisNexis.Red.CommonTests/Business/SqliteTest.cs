using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.CommonTests.Interface;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Business
{
    public class SqliteTest
    {
        IPublicationAccess publicationAccess = new PublicationAccess();
        IUserAccess userAccess = new UserAccess();
        [Test]
        public async void Log()
        {
            try
            {
                var dlBooks = publicationAccess.GetDlBooksOffline(GlobalAccess.Instance.UserCredential);
                foreach (var item in dlBooks)
                {
                    item.InstalledDate = DateTime.Now;
                    publicationAccess.Update(item);
                }
                var loginResult = await LoginUtil.Instance.ValidateUserLogin("allen@lexisred.com", "1234", "AU");
                var r = SettingsUtil.Instance.GetLastSyncedTime();

                 Logger.Log("LSSt:" + r.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
