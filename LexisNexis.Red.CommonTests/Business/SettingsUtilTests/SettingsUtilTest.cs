using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Business
{
    [Category("SettingsUtil")]
    [TestFixture]
    public class SettingsUtilTest
    {
        LoginInfo TestUser = null;
        public SettingsUtilTest()
        {
            TestUser = TestHelper.TestUsers[0];

        }
        [Test()]
        public async void GetLastSyncTimeTest()
        {
            await LoginUtil.Instance.ValidateUserLogin(TestUser.UserName, TestUser.Password, TestUser.CountryCode);
            var lastSyncTime = SettingsUtil.Instance.GetLastSyncedTime();
        }
        [Test()]
        public async void GetLexisNexisInfoTest()
        {
            await LoginUtil.Instance.ValidateUserLogin(TestUser.UserName, TestUser.Password, TestUser.CountryCode);
            var LNinfo = SettingsUtil.Instance.GetLexisNexisInfo();
            Assert.IsTrue(!string.IsNullOrEmpty(LNinfo));
            var LNRedinfo = SettingsUtil.Instance.GetLexisNexisRedInfo();
            Assert.IsTrue(!string.IsNullOrEmpty(LNRedinfo));
            var Terms = SettingsUtil.Instance.GetTermsAndConditions();
            Assert.IsTrue(!string.IsNullOrEmpty(Terms));

        }

        [Test()]
        public void UpdateFontSizeTest()
        {
            var fontSize = SettingsUtil.Instance.GetFontSize();
            var updateSuccess = SettingsUtil.Instance.SaveFontSize(fontSize + 5);
            var updateSuccess1 = SettingsUtil.Instance.SaveFontSize(fontSize + 9);
            Assert.IsTrue(updateSuccess1 > 0 && updateSuccess > 0);

        }
        [Test()]
        public async void GetExpiryNotificationTest()
        {
            string expiryNotification = "";
            await LoginUtil.Instance.ValidateUserLogin(TestUser.UserName, TestUser.Password, TestUser.CountryCode);
            expiryNotification = SettingsUtil.Instance.GetExpiryNotification();
            Assert.IsNotEmpty(expiryNotification);
        }


    }
}
