using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexisNexis.Red.Common.Database;
using NUnit.Framework;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.CommonTests.Interface;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.CommonTests.Business;
using LexisNexis.Red.Common.Business;
namespace LexisNexis.Red.Common.Database.Tests
{
    [Category("DataBaseParallelTest")]
    [TestFixture()]
    public class DataBaseParallelTest
    {

        public DataBaseParallelTest()
        {
            TestInit.Init().Wait();
        }
        [Test]
        public async void UpdateSettingTest()
        {
            try
            {
                int parallelnum = 1000;
                SettingsAccess settingAccess = new SettingsAccess();
                RecentHistoryAccess recenthistory = new RecentHistoryAccess();
                List<Task> tasks = new List<Task>(parallelnum);
                await LoginUtil.Instance.ValidateUserLogin("allen@lexisred.com", "1234", "AU");
                for (int i = 0; i < parallelnum; i++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        //settingAccess.GetSetting(BusinessModel.SettingsEnum.FontSize, "allen.xie@lexisnexis.com", "AU")
                        settingAccess.UpdateSetting(i.ToString(), BusinessModel.SettingsEnum.FontSize, "allen.xie@lexisnexis.com", "AU");
                        recenthistory.UpdateRecentHistory(new RecentHistory
                        {
                            BookId = i,
                            TOCTitle = "",
                            Email = "allen.xie@lexisnexis.com",
                            ServiceCode = "AU"
                        });
                        PublicationUtil.Instance.GetPublicationOnline();
                        PublicationContentUtil.Instance.GetRecentHistory();
                    }));
                }
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
