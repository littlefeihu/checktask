using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.CommonTests.Interface;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telerik.JustMock;
using Telerik.JustMock.AutoMock;

namespace LexisNexis.Red.CommonTests.Business
{
    [TestFixture, Category("PublicationDomainServiceTest")]
    public partial class PublicationDomainServiceTest
    {
        [Test]
        public void PublicationDomainService_VerifyNetwork()
        {
            domainContainer.Arrange<INetwork>(network => network.GetNetworkType()).IgnoreArguments().Returns(NetworkTypeEnum.Mobile);
            domainContainer.Arrange<INetwork>(network => network.GetFlowLimitation()).IgnoreArguments().Returns(long.Parse("20"));
            try
            {
                domainContainer.Instance.VerifyNetwork(30);
            }
            catch (ExceedLimitationException)
            {
                Assert.IsTrue(1 == 1);
            }
            catch (Exception)
            {
                throw;
            }

            try
            {
                domainContainer.Instance.VerifyNetwork(10);
            }
            catch (Exception)
            {
                throw;
            }

            domainContainer.Arrange<INetwork>(network => network.GetNetworkType()).IgnoreArguments().Returns(NetworkTypeEnum.None);
            try
            {
                domainContainer.Instance.VerifyNetwork(30);
            }
            catch (CusNetDisConnectedException)
            {
                Assert.IsTrue(1 == 1);
            }
            catch (Exception)
            {
                throw;
            }

        }
        [Test]
        public void GetAdditionalDlBooks()
        {
            List<DlBook> localBooks = new List<DlBook> 
            { 
                new DlBook{BookId=1, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded, ValidTo=DateTime.Now.AddDays(10)},
                new DlBook{BookId=2, IsLoan=true, DlStatus=(short)DlStatusEnum.Downloaded,ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=3, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=4, IsLoan=false, DlStatus=(short)DlStatusEnum.Downloaded, ValidTo=DateTime.Now.AddDays(-10) },
                new DlBook{BookId=5, IsLoan=false, DlStatus=(short)DlStatusEnum.NotdDownloaded, ValidTo=DateTime.Now.AddDays(-10)},
                new DlBook{BookId=6, IsLoan=false, DlStatus=(short)DlStatusEnum.NotdDownloaded, ValidTo=DateTime.Now.AddDays(10)},
                new DlBook{BookId=7, IsLoan=true, DlStatus=(short)DlStatusEnum.RemovedByUser,ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=8, IsLoan=false, IsTrial=true, DlStatus=(short)DlStatusEnum.RemovedByUser, ValidTo=DateTime.Now.AddDays(10)},
            };

            var additionalBooks = domainContainer.Instance.GetAdditionalDlBooks(localBooks);
            Assert.IsTrue(additionalBooks.Count() == 2);
            foreach (var item in additionalBooks)
            {
                Assert.IsTrue(item.ValidTo.Value < DateTime.Now.Date);
            }
        }
        [Test]
        public async void RefreshLocalDlBook()
        {
            DlBook serverDl = new DlBook
            {
                SubCategory = "SubCategory",
                PracticeArea = "PracticeArea",
                AddedGuideCard = "AddedGuideCard",
                CurrentVersion = 20,
                Size = 2000
            };
            DlBook localDl = new DlBook
            {
                DlStatus = (short)DlStatusEnum.NotdDownloaded
            };

            domainContainer.Arrange<IDeliveryService>((deliveryService) => deliveryService.DlMetadata(null)).IgnoreArguments()
                           .Returns(GetDlMetadata());
            domainContainer.Arrange<IDeliveryService>((deliveryService) => deliveryService.DlVersionChangeHistory(null)).IgnoreArguments()
                         .Returns(GetDlBookChangeHistory());
            await domainContainer.Instance.RefreshLocalDlBook(serverDl, localDl, "");
            Assert.IsTrue(localDl.SubCategory == serverDl.SubCategory);
            Assert.IsTrue(localDl.PracticeArea == serverDl.PracticeArea);
            var localAddedGuideCard = JsonConvert.DeserializeObject<List<GuideCard>>(localDl.AddedGuideCard);
            Assert.IsTrue(localAddedGuideCard[0].Name == serverDl.AddedGuideCard);

            Assert.IsTrue(localDl.LastDownloadedVersion == serverDl.CurrentVersion);
            Assert.IsTrue(localDl.LocalSize == serverDl.Size);
        }

        private Task<HttpResponse> GetDlBookChangeHistory()
        {
            Task<HttpResponse> changeHistory = new Task<HttpResponse>(() =>
            {
                return new HttpResponse
                {
                    Content = JsonConvert.SerializeObject(new DlVersionChangeHistoryResponse
                    {
                        AddedGuideCard = new List<GuideCard> { new GuideCard { Name = "AddedGuideCard" } }
                    }),
                    IsSuccess = true
                };
            });
            changeHistory.Start();
            return changeHistory;
        }

        private Task<HttpResponse> GetDlMetadata()
        {
            Task<HttpResponse> metaDataTask = new Task<HttpResponse>(() =>
            {
                return new HttpResponse
                {
                    Content = JsonConvert.SerializeObject(new DlMetadataDetails { SubCategory = "SubCategory", PracticeArea = "PracticeArea" }),
                    IsSuccess = true
                };
            });
            metaDataTask.Start();
            return metaDataTask;
        }
        [Test]
        public void UpdateLocalDlBooks()
        {
            List<DlBook> localBooks = new List<DlBook> 
            { 
                new DlBook{  BookId=1, 
                            IsLoan=true,
                            DlStatus=(short)DlStatusEnum.Downloaded,
                            LastDownloadedVersion=1,
                            ValidTo=DateTime.Now.AddDays(10)},
           };

            List<DlBook> serverBooks = new List<DlBook> 
            { 
                new DlBook{BookId=1, IsLoan=true, CurrentVersion=20, ValidTo=DateTime.Now.AddDays(10)},
                new DlBook{BookId=2, IsLoan=true, ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=3, IsLoan=false,ValidTo=DateTime.Now.AddDays(10) },
                new DlBook{BookId=6, IsLoan=false, ValidTo=DateTime.Now.AddDays(10)},
            };
            var publications = domainContainer.Instance.UpdateLocalDlBooks(serverBooks, localBooks);
            var publication = publications.FirstOrDefault(o => o.BookId == 1);
            Assert.IsTrue(publication.PublicationStatus == PublicationStatusEnum.RequireUpdate);
            Assert.IsTrue(publications.Count() == serverBooks.Count);
        }


        [Test]
        public void GetOnlineDlBooks()
        {

            domainContainer.Arrange<IDeliveryService>((deliveryService) => deliveryService.ListFileDetails(null)).IgnoreArguments()
                          .Returns(ListFileDetails());
            var dlBooks = domainContainer.Instance.GetOnlineDlBooks("allen@lexis.com").Result;
            Assert.IsTrue(dlBooks.Count == 0);
        }
        private Task<HttpResponse> ListFileDetails()
        {
            Task<HttpResponse> metaDataTask = new Task<HttpResponse>(() =>
            {
                return new HttpResponse
                {
                    Content = JsonConvert.SerializeObject(new DlFileList { DlBooks = new List<DlBook>() }),
                    IsSuccess = true
                };
            });
            metaDataTask.Start();
            return metaDataTask;
        }
    }
}
