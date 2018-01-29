using LexisNexis.Red.Common;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.CommonTests.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.CommonTests.Business
{
    public static class TestInit
    {
        public static async Task Init()
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                IoCContainer.Instance.RegisterInterface<IDevice, Device>();
                IoCContainer.Instance.RegisterInterface<IDirectory, MyDirectory>();
                IoCContainer.Instance.RegisterInterface<ICryptogram, Cryptogram>();
                IoCContainer.Instance.RegisterInterface<INetwork, Network>();
                IoCContainer.Instance.RegisterInterface<IPackageFile, PackageFile>();
                await GlobalAccess.Instance.Init();
                
                TestHelper.TestUsers = new List<LoginInfo>();
                LoginInfo loginInfo = new LoginInfo
                {
                    UserName = System.Configuration.ConfigurationManager.AppSettings["username"].ToString(),
                    Password = System.Configuration.ConfigurationManager.AppSettings["password"].ToString(),
                    CountryCode = System.Configuration.ConfigurationManager.AppSettings["countrycode"].ToString()
                };
                TestHelper.TestUsers.Add(loginInfo);
                watch.Stop();
                Console.WriteLine(watch.Elapsed);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
