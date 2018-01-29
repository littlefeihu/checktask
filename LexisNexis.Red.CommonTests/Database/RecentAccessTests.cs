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
namespace LexisNexis.Red.Common.Database.Tests
{
    [Category("RecentAccessTests")]
    [TestFixture()]
    public class RecentAccessTests
    {

        public RecentAccessTests()
        {
            IoCContainer.Instance.RegisterInterface<IDevice, Device>();
            IoCContainer.Instance.RegisterInterface<IDirectory, MyDirectory>();
            IoCContainer.Instance.RegisterInterface<INetwork, Network>();
            GlobalAccess.Instance.Init().Wait();
        }
        [Test()]
        public async void UpdateRecentHistoryTest()
        {
            IRecentHistoryAccess recentHistoryAccess = new RecentHistoryAccess();
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < 500; i++)
            {
                Task<int> task = new Task<int>(() =>
                {
                    RecentHistory recentHistory = new RecentHistory
                    {
                        BookId = 1000,
                        ColorPrimary = "ColorPrimary",
                        Email = "allen@lexisred.com",
                        DocID = "DocID",
                        ServiceCode = "AU",
                        DlBookTitle = "Family Law",
                        LastReadDate = DateTime.Now,
                        TOCTitle = "Judicature Modernisation Bill"
                    };
                    return recentHistoryAccess.UpdateRecentHistory(recentHistory);
                });
                task.Start();
                taskList.Add(task);
            }
            try
            {
                await Task.WhenAll(taskList);
                Console.WriteLine("ds");
            }
            catch (Exception)
            {

                throw;
            }
            //recentHistory.DlBookTitle = "DlBookTitle";
            //recentHistoryAccess.UpdateRecentHistory(recentHistory);
            //var recentHistoryUpdated = recentHistoryAccess.GetRecentHistory(recentHistory.Email, recentHistory.CountryCode, 1000);
            //Assert.IsTrue(recentHistoryUpdated.FirstOrDefault().DlBookTitle == recentHistory.DlBookTitle);

        }
    }
}
