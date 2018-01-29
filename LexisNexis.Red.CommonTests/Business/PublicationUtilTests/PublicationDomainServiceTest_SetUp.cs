using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService;
using LexisNexis.Red.Common.DominService;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telerik.JustMock.AutoMock;

namespace LexisNexis.Red.CommonTests.Business
{
    public partial class PublicationDomainServiceTest
    {
        AutoMockSettings autoMockSettings;
        MockingContainer<PublicationUtil> container;
        MockingContainer<PublicationDomainService> domainContainer;
        CancellationTokenSource source;
        Task<bool> pingTaskFailure, pingTaskSuccess;
        Task<HttpResponse> validateResult;
        DlBook dlBook = null;
        LoginInfo TestUser;
        public PublicationDomainServiceTest()
        {

            TestUser = TestHelper.TestUsers[0];
            LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Wait();
        }

        [SetUp()]
        public void Init()
        {
          
            autoMockSettings = new AutoMockSettings
            {
                ConstructorArgTypes = new Type[] {  typeof(IPublicationAccess), 
                                                    typeof(IConnectionMonitor), 
                                                    typeof(IDeliveryService), 
                                                    typeof(IAnnotationAccess), 
                                                    typeof(INetwork), 
                                                    typeof(IPackageAccess) }
            };
            source = new CancellationTokenSource();
            pingTaskFailure = new Task<bool>(() => { return false; });
            pingTaskSuccess = new Task<bool>(() => { return true; });
            pingTaskFailure.Start();
            pingTaskSuccess.Start();
            validateResult = new Task<HttpResponse>(() =>
            {
                return new HttpResponse
                {
                    IsSuccess = true,
                    Content = JsonConvert.SerializeObject(new LoginUserValidationResponse { ValidUser = true })
                };
            });
            validateResult.Start();
            domainContainer = new MockingContainer<PublicationDomainService>(autoMockSettings);
            dlBook = new DlBook
            {
                Size = 100,
                FileUrl = "",
                CurrentVersion = 1,
                BookId = 1,
                InitVector = "QDFCMmMzRDRlNUY2ZzdIOA==",
                K2Key = "cldADc+tjskTH1G5H9R19IFNOLRaVBaQYSL9QZe/FB4=",
                HmacKey = "N2IhLPP0CO58AXHy4QGfbFHZ+Yw="
            };
        }
        [TearDown]
        public void Cleanup()
        {

        }
    }
}
