using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexisNexis.Red.Common.Services;
using NUnit.Framework;
using LexisNexis.Red.CommonTests.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using System.IO;
namespace LexisNexis.Red.Common.Services.Tests
{
    [TestFixture()]
    public class SyncServiceTests
    {
        [SetUp()]
        public void Test_Init()
        {
            TestInit.Init().Wait();
        }
        [Test()]
        public async void DownwardSyncRequestTest()
        {

            //var loginResult = await LoginUtil.Instance.ValidateUserLogin("allen@lexisred.com", "O6GJBTqB", "HK");
            //SyncService ss = new SyncService();
            //DownwardSyncRequests requests = new DownwardSyncRequests
            //{
            //    DownwardSyncRequest = new List<DownwardSyncRequest> {
            //            new DownwardSyncRequest{ DeviceID=GlobalAccess.DeviceId, DLID=21, DLVersionID=1, UserID=GlobalAccess.Instance.CurrentUserInfo.Email  },
            //             new DownwardSyncRequest{ DeviceID=GlobalAccess.DeviceId, DLID=26, DLVersionID=1, UserID=GlobalAccess.Instance.CurrentUserInfo.Email  }
            //        }
            //};
            //await ss.DownwardSyncRequest(requests);
            //var directory = System.AppDomain.CurrentDomain.BaseDirectory;
            //string fullfilename = Path.Combine(directory, "aa.zip");
            //if (File.Exists(fullfilename))
            //{
            //    File.Delete(fullfilename);
            //}
            //using (var fileStream = File.Open(fullfilename, FileMode.Create))
            //using (StreamWriter streamWriter = new StreamWriter(fileStream))
            //{
            //    streamWriter.Write(bytes);
            //}
        }
    }
}
