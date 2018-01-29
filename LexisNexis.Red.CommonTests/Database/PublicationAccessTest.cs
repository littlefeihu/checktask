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
    [Category("PublicationAccessTest")]
    [TestFixture()]
    public class PublicationAccessTest
    {

        public PublicationAccessTest()
        {
            IoCContainer.Instance.RegisterInterface<IDevice, Device>();
            IoCContainer.Instance.RegisterInterface<IDirectory, MyDirectory>();
            IoCContainer.Instance.RegisterInterface<INetwork, Network>();
            GlobalAccess.Instance.Init().Wait();
        }
        [Test, Ignore]
        public void UpdateDlBookOrder()
        {
            PublicationAccess publicationAccess = new PublicationAccess();
            Dictionary<int, int> orderDic = new Dictionary<int, int>();
            orderDic.Add(41, 1);
            orderDic.Add(23, 2);
            orderDic.Add(35, 3);
            publicationAccess.UpdateDlBookOrder(orderDic, new UserCredential("allen@lexisred.com", "AU")).Wait();
        }
    }
}
