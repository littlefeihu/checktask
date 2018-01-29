using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexisNexis.Red.Common.HelpClass;
using NUnit.Framework;
namespace LexisNexis.Red.Common.HelpClass.Tests
{
    [TestFixture()]
    public class RedLockTests
    {
        [Test()]
        public async void RestFullServiceRequestForFileDownloadTest()
        {
            RedLock redLock = new RedLock(1);

            List<Task> taskList = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                taskList.Add(Task.Run(async () =>
                {
                    await redLock.Enter();
                    for (int j = 0; j < 100; j++)
                    {
                        await Task.Delay(1000);
                        Console.WriteLine("count:" + j);
                    }
                    redLock.Release();
                }));
            }
            await Task.WhenAll(taskList);
            Console.WriteLine("ds");
        }


    }
}
