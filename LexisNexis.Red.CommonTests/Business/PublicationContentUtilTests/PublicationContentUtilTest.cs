using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DominService;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telerik.JustMock.AutoMock;
using System.Xml.Linq;
using LexisNexis.Red.CommonTests.Impl;

namespace LexisNexis.Red.CommonTests.Business
{
    [Category("PublicationContentUtilTest")]
    [TestFixture]
    public partial class PublicationContentUtilTest
    {
        [Test, Category("Hyperlink")]
        public void BuildExternalHyperlinkTest1()
        {
            DlBook tempDlBook = new DlBook
            {
                BookId = 2,
                DpsiCode = "0JVV",
                CurrentVersion = 2
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(tempDlBook);

            var link = container.Instance.BuildHyperLink(tempDlBook.BookId, "looseleaf://...citeref?ctype=case&element=citefrag_span&decisiondate_year=2001&caseref_ID=cr000118&caseref_spanref=cr000118-001&reporter_value=nswlr&volume_num=53&page_num=198&LinkName=(2001) 53 NSWLR 198", "");
            Assert.IsTrue(link is ExternalHyperlink);
        }
        [Test, Category("Hyperlink")]
        public void BuildExternalHyperlinkTest2()
        {
            DlBook tempDlBook = new DlBook
            {
                BookId = 13,
                DpsiCode = "007S",
                CurrentVersion = 4,
                LastDownloadedVersion = 4,
                Email = GlobalAccess.Instance.CurrentUserInfo.Email,
                ServiceCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode
            };
            // container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(13, null)).IgnoreArguments().Returns(tempDlBook);
            var link = PublicationContentUtil.Instance.BuildHyperLink(tempDlBook.BookId, "looseleaf://opendocument?dpsi=0I09&amp;refpt=MC.DIV_II.GC001.GROUP1&remotekey1=REFPTID&service=DOC-ID&LinkName=[I 64.01.600]", "");
            var link1 = PublicationContentUtil.Instance.BuildHyperLink(tempDlBook.BookId, "looseleaf://opendocument?dpsi=05CC&amp;refpt=UCPN.CPA.CPAN05.S2.1&remotekey1=REFPTID&service=DOC-ID&LinkName=[I 64.01.600]", "");
            var link2 = PublicationContentUtil.Instance.BuildHyperLink(tempDlBook.BookId, "looseleaf://opendocument?dpsi=05CC&amp;refpt=UCPN.CPA.CPAN05.S2.1&remotekey1=REFPTID&service=DOC-ID&LinkName=[I 64.01.600]", "");
            var link3 = PublicationContentUtil.Instance.BuildHyperLink(tempDlBook.BookId, "looseleaf://opendocument?dpsi=05CC&amp;refpt=UCPN.CPA.CPAN05.S2.1&remotekey1=REFPTID&service=DOC-ID&LinkName=[I 64.01.600]", "");
            var link4 = PublicationContentUtil.Instance.BuildHyperLink(tempDlBook.BookId, "looseleaf://opendocument?dpsi=0I09&amp;refpt=MC.DIV_II.GC001.GROUP1&remotekey1=REFPTID&service=DOC-ID&LinkName=[I 64.01.600]", "");
            Assert.IsTrue(link is ExternalHyperlink);
        }
        [Test, Category("Hyperlink")]
        public void BuildAnchorHyperlinkTest()
        {
            var link = container.Instance.BuildHyperLink(2, "#footnote", "");
            Assert.IsTrue(link is AnchorHyperlink);
        }
        [Test, Category("Hyperlink")]
        public void BuildAttachmentHyperlinkTest()
        {
            DlBook tempDlBook = new DlBook
            {
                BookId = 2,
                DpsiCode = "P-00988",
                CurrentVersion = 2,
                Email = GlobalAccess.Instance.CurrentUserInfo.Email,
                ServiceCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode,
                DlStatus = (int)DlStatusEnum.Downloaded
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(tempDlBook);
            var link = container.Instance.BuildHyperLink(2, "looseleaf://opendocument?linktype=urjpdf&citation=BC201109206&filename=1109206.pdf", "");
            Assert.IsTrue(link is ExternalHyperlink);
        }
        [Test, Category("Hyperlink")]
        public void BuildIntraHyperlinkTest()
        {
            DlBook tempDlBook = new DlBook
            {
                BookId = 2,
                DpsiCode = "P-0098",
                CurrentVersion = 2,
                Email = GlobalAccess.Instance.CurrentUserInfo.Email,
                ServiceCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(tempDlBook);
            var link = container.Instance.BuildHyperLink(tempDlBook.BookId, "looseleaf://opendocument?dpsi=P-0098&refpt=CPV.RC1GD.64-01-600&remotekey1=REFPTID&service=DOC-ID&LinkName=[I 64.01.600]", "");
            Assert.IsTrue(link is IntraHyperlink);
        }

        [Test, Category("Hyperlink")]
        public void BuildInternalHyperlinkTest()
        {
            DlBook tempDlBook = new DlBook
            {
                BookId = 2,
                DpsiCode = "P-00988",
                CurrentVersion = 2,
                Email = GlobalAccess.Instance.CurrentUserInfo.Email,
                ServiceCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode,
                DlStatus = (int)DlStatusEnum.Downloaded
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(tempDlBook);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByDpsiCode(null, null)).IgnoreArguments().Returns(tempDlBook);
            var link = container.Instance.BuildHyperLink(tempDlBook.BookId, "looseleaf://opendocument?dpsi=P-0098&refpt=CPV.RC1GD.64-01-600&remotekey1=REFPTID&service=DOC-ID&LinkName=[I 64.01.600]", "");
            Assert.IsTrue(link is InternalHyperlink);
        }
        [Test, Category("Hyperlink")]
        public void BuildExternalHyperlinkTest3()
        {
            DlBook tempDlBook = new DlBook
            {
                BookId = 2,
                DpsiCode = "P-00989",
                CurrentVersion = 2,
                Email = GlobalAccess.Instance.CurrentUserInfo.Email,
                ServiceCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode,
                DlStatus = (int)DlStatusEnum.NotDownloaded
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(tempDlBook);
            var link = container.Instance.BuildHyperLink(tempDlBook.BookId, "looseleaf://opendocument?dpsi=P-0098&refpt=CPV.RC1GD.64-01-600&remotekey1=REFPTID&service=DOC-ID&LinkName=[I 64.01.600]", "");
            Assert.IsTrue(link is ExternalHyperlink);
        }

        [Test, Category("Hyperlink")]
        public void BuildHyperlinkTest()
        {
            var link = PublicationContentUtil.Instance.BuildHyperLink(13, "looseleaf://opendocument?dpsi=007S&refpt=CE.LATUP.C1&remotekey1=REFPTID&service=DOC-ID&LinkName=[I 64.01.600]", "");

            Assert.IsTrue(link is IntraHyperlink);
        }
        public async void SwitchUserTest()
        {
            //var loginResult = await LoginUtil.Instance.ValidateUserLogin("torres@lexisred.com", "loop", "AU");
            var loginResult = await LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode);
            var currentUser = GlobalAccess.Instance.CurrentUserInfo;
            //var loginResult = await LoginUtil.Instance.ValidateUserLogin("torres@lexisred.com", "loop", "AU");
            if (loginResult == LoginStatusEnum.LoginSuccess)
            {
                string u = currentUser.FirstName;
                var newLogin = GlobalAccess.Instance.CurrentUserInfo;
                CancellationToken token = new CancellationToken();
                var d = await PublicationUtil.Instance.GetPublicationOnline();
                bookId = 35;
                var downloadResult = await PublicationUtil.Instance.DownloadPublicationByBookId(bookId, token, (t, y) => { }, false);
                if (downloadResult.DownloadStatus == DownLoadEnum.Success)
                {
                    var tocs = await PublicationContentUtil.Instance.GetTOCByBookId(bookId);
                    Assert.IsTrue(tocs.ChildNodes.Count > 0);
                }
            }
            else
            {
                Assert.Fail();
            }

        }
        [Test, Ignore]
        public async void InstallCancelTest()
        {
            try
            {
                for (int i = 50; i < 100; i++)
                {
                    try
                    {
                        CancellationTokenSource source = new CancellationTokenSource();
                        var d = await PublicationUtil.Instance.GetPublicationOnline();
                        bookId = 36;
                        var b = d.Publications;
                        source.CancelAfter(1000 * i);
                        var downloadResult = await PublicationUtil.Instance.DownloadPublicationByBookId(bookId, source.Token, (t, y) => { }, false);
                        if (downloadResult.DownloadStatus == DownLoadEnum.Canceled)
                        {

                        }
                        else
                        {
                            var result = downloadResult.DownloadStatus;
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [Test, Ignore]
        public async void GetNextPerformenceTest()
        {
            string timerstring = "";
            bookId = 36;
            var tocs = await PublicationContentUtil.Instance.GetTOCByBookId(bookId);
            var testTOC = PublicationContentUtil.Instance.GetTOCByTOCId(0, tocs);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var nextPageTreeNode = PublicationContentUtil.Instance.GetNextPageByTreeNode(testTOC);
            timer.Stop();
            timerstring = timer.ElapsedMilliseconds.ToString();
            timer.Reset();
            timer.Start();
            var previousPageTreeNode = PublicationContentUtil.Instance.GetPreviousPageByTreeNode(testTOC);
            timer.Stop();
            timerstring = timer.ElapsedMilliseconds.ToString();
            timer.Reset();
        }

        [Test]
        public async void InstallTest()
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                var d = await PublicationUtil.Instance.GetPublicationOnline();
                bookId = 19;
                var b = d.Publications;
                var downloadResult = await PublicationUtil.Instance.DownloadPublicationByBookId(bookId, source.Token, (t, y) => { }, false);
                if (downloadResult.DownloadStatus == DownLoadEnum.Success)
                {
                    var tocs = await PublicationContentUtil.Instance.GetTOCByBookId(bookId);
                    Assert.IsTrue(tocs.ChildNodes.Count > 0);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Test()]
        public async void GetTOCByBookIdTest()
        {
            List<TOCNode> tocNodes = new List<TOCNode>
            {
                new TOCNode{ ID=6, ParentId=5, Title="Judicature Modernisation Bill",  Role="ancestor",  GuideCardTitle="Current Developments" },
                new TOCNode{ ID=7, ParentId=6, Title="Commentary",  Role="me",GuideCardTitle="Current Developments" },
                new TOCNode{ ID=8, ParentId=5, Title="JUDICATURE MODERNISATION BILL",  Role="ancestor", GuideCardTitle="Current Developments" },
                new TOCNode{ ID=2, ParentId=1, Title="Update",  Role="me", GuideCardTitle="Update" },
                new TOCNode{ ID=4, ParentId=3, Title="About this Publication",  Role="me",GuideCardTitle="About this Publication" },
                new TOCNode{ ID=1, ParentId=0, Title="Update",  Role="ancestor", GuideCardTitle="Update" },
                new TOCNode{ ID=3, ParentId=0, Title="About this Publication",  Role="ancestor", GuideCardTitle="About this Publication" },
                new TOCNode{ ID=5, ParentId=55, Title="Current Developments",  Role="ancestor", GuideCardTitle="Current Developments" },
                new TOCNode{ ID=55, ParentId=0, Title="Current Developments1",  Role="ancestor", GuideCardTitle="Current Developments" }
            };
            container.Arrange<IPackageAccess>(packageAccess => packageAccess.GetTOC(null)).IgnoreArguments().Returns(tocNodes);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(dlBook);
            var root = await container.Instance.GetTOCByBookId(bookId);
            Assert.IsTrue(root.ChildNodes.Count == 3);
            Assert.IsTrue(root.ChildNodes.FirstOrDefault(o => o.ID == 1).ChildNodes.Count == 1);
            Assert.IsTrue(root.ChildNodes.FirstOrDefault(o => o.ID == 3).ChildNodes.Count == 1);
            Assert.IsTrue(root.ChildNodes.FirstOrDefault(o => o.ID == 55).ChildNodes.Count == 1);
        }

        [Test()]
        public async void GetNextTOCNodeTest()
        {
            List<TOCNode> tocNodes = new List<TOCNode>
            {
                new TOCNode{ ID=6, ParentId=5, Title="Judicature Modernisation Bill",  Role="ancestor",GuideCardTitle="Current Developments" },
                new TOCNode{ ID=7, ParentId=6, Title="Commentary",  Role="me",GuideCardTitle="Current Developments" },
                new TOCNode{ ID=8, ParentId=5, Title="JUDICATURE MODERNISATION BILL",  Role="me", GuideCardTitle="Current Developments" },
                new TOCNode{ ID=2, ParentId=1, Title="Update",  Role="me",GuideCardTitle="Update" },
                new TOCNode{ ID=4, ParentId=3, Title="About this Publication",  Role="me", GuideCardTitle="About this Publication" },
                new TOCNode{ ID=9, ParentId=3, Title="About this Publication1",  Role="me", GuideCardTitle="About this Publication" },
                new TOCNode{ ID=1, ParentId=0, Title="Update",  Role="ancestor", GuideCardTitle="Update" },
                new TOCNode{ ID=3, ParentId=0, Title="About this Publication",  Role="ancestor", GuideCardTitle="About this Publication" },
                new TOCNode{ ID=5, ParentId=55, Title="Current Developments",  Role="ancestor", GuideCardTitle="Current Developments" },
                new TOCNode{ ID=55, ParentId=0, Title="Current Developments1",  Role="ancestor",GuideCardTitle="Current Developments" }
            };
            container.Arrange<IPackageAccess>(packageAccess => packageAccess.GetTOC(null)).IgnoreArguments().Returns(tocNodes);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(dlBook);
            var root = await container.Instance.GetTOCByBookId(bookId);
            PublicationContentDomainService contentService = new PublicationContentDomainService(null, null, null);

            var nextNode = contentService.GetNextTOCNode(4, 4, root, true);
            Assert.IsTrue(nextNode.ID == 9);
            var nextNode1 = contentService.GetNextTOCNode(2, 2, root, true);
            Assert.IsTrue(nextNode1.ID == 4);
            var nextNode2 = contentService.GetNextTOCNode(9, 9, root, true);
            Assert.IsTrue(nextNode2.ID == 7);
            var nextNode3 = contentService.GetNextTOCNode(7, 7, root, true);
            Assert.IsTrue(nextNode3.ID == 8);
            var nextNode4 = contentService.GetNextTOCNode(8, 8, root, true);
            Assert.IsTrue(nextNode4 == null);
        }

        [Test()]
        public async void GetFirsTAndLastTOCNodeTest()
        {
            List<TOCNode> tocNodes = new List<TOCNode>
            {
                new TOCNode{ ID=1, ParentId=0, Title="Update",  Role="ancestor",  GuideCardTitle="Update" },
                new TOCNode{ ID=2, ParentId=1, Title="Update",  Role="me",  GuideCardTitle="Update" },
                new TOCNode{ ID=3, ParentId=0, Title="About this Publication",  Role="ancestor",  GuideCardTitle="About this Publication" },
                new TOCNode{ ID=4, ParentId=3, Title="About this Publication",  Role="me",  GuideCardTitle="About this Publication" },
                new TOCNode{ ID=9, ParentId=3, Title="About this Publication1",  Role="me",  GuideCardTitle="About this Publication" },
                new TOCNode{ ID=55, ParentId=0, Title="Current Developments1",  Role="ancestor",  GuideCardTitle="Current Developments" },
                new TOCNode{ ID=5, ParentId=55, Title="Current Developments",  Role="ancestor",  GuideCardTitle="Current Developments" },
                new TOCNode{ ID=6, ParentId=5, Title="Judicature Modernisation Bill",  Role="ancestor",GuideCardTitle="Current Developments" },
                new TOCNode{ ID=8, ParentId=5, Title="JUDICATURE MODERNISATION BILL",  Role="me",  GuideCardTitle="Current Developments" },
                new TOCNode{ ID=7, ParentId=6, Title="Commentary",  Role="me", GuideCardTitle="Current Developments" }
            };

            container.Arrange<IPackageAccess>(packageAccess => packageAccess.GetTOC(null)).IgnoreArguments().Returns(tocNodes);
            dlBook.ServiceCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode;
            dlBook.Email = GlobalAccess.Instance.CurrentUserInfo.Email;
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(dlBook);
            var rootNode = await container.Instance.GetTOCByBookId(bookId);
            var node1 = container.Instance.GetFirstAndLastTOCNode(rootNode);
        }

        [Test()]
        public async void GetPreviousTOCNodeTest()
        {
            List<TOCNode> tocNodes = new List<TOCNode>
            {
                new TOCNode{ ID=6, ParentId=5, Title="Judicature Modernisation Bill",  Role="ancestor", GuideCardTitle="Current Developments" },
                new TOCNode{ ID=7, ParentId=6, Title="Commentary",  Role="me",GuideCardTitle="Current Developments" },
                new TOCNode{ ID=8, ParentId=5, Title="JUDICATURE MODERNISATION BILL",  Role="me", GuideCardTitle="Current Developments" },
                new TOCNode{ ID=2, ParentId=1, Title="Update",  Role="me",  GuideCardTitle="Update" },
                new TOCNode{ ID=4, ParentId=3, Title="About this Publication",  Role="me",  GuideCardTitle="About this Publication" },
                new TOCNode{ ID=9, ParentId=3, Title="About this Publication1",  Role="me",  GuideCardTitle="About this Publication" },
                new TOCNode{ ID=1, ParentId=0, Title="Update",  Role="ancestor",  GuideCardTitle="Update" },
                new TOCNode{ ID=3, ParentId=0, Title="About this Publication",  Role="ancestor",  GuideCardTitle="About this Publication" },
                new TOCNode{ ID=5, ParentId=55, Title="Current Developments",  Role="ancestor",  GuideCardTitle="Current Developments" },
                new TOCNode{ ID=55, ParentId=0, Title="Current Developments1",  Role="ancestor",  GuideCardTitle="Current Developments" }
            };
            container.Arrange<IPackageAccess>(packageAccess => packageAccess.GetTOC(null)).IgnoreArguments().Returns(tocNodes);
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(dlBook);
            var root = await container.Instance.GetTOCByBookId(bookId);

            PublicationContentDomainService contentService = new PublicationContentDomainService(null, null, null);

            var nextNode = contentService.GetNextTOCNode(8, 8, root, false);
            Assert.IsTrue(nextNode.ID == 7);
            var nextNode1 = contentService.GetNextTOCNode(7, 7, root, false);
            Assert.IsTrue(nextNode1.ID == 9);
            var nextNode2 = contentService.GetNextTOCNode(9, 9, root, false);
            Assert.IsTrue(nextNode2.ID == 4);
            var nextNode3 = contentService.GetNextTOCNode(4, 4, root, false);
            Assert.IsTrue(nextNode3.ID == 2);
            var nextNode4 = contentService.GetNextTOCNode(2, 2, root, false);
            Assert.IsTrue(nextNode4 == null);


            var nextNode11 = contentService.GetNextTOCNode(8, 8, root, true);
            Assert.IsTrue(nextNode11 == null);
            var nextNode12 = contentService.GetNextTOCNode(7, 7, root, true);
            Assert.IsTrue(nextNode12.ID == 8);
            var nextNode21 = contentService.GetNextTOCNode(9, 9, root, true);
            Assert.IsTrue(nextNode21.ID == 7);
            var nextNode31 = contentService.GetNextTOCNode(4, 4, root, true);
            Assert.IsTrue(nextNode31.ID == 9);
            var nextNode41 = contentService.GetNextTOCNode(2, 2, root, true);
            Assert.IsTrue(nextNode41.ID == 4);
        }

        [Test()]
        public async void GetIndexsByBookIdTest()
        {
            List<Index> list = new List<Index> { 
              new Index{ Title="AB",FileName="AB1.xml"},
              new Index{ Title="AC",FileName="AC1.xml"},
              new Index{ Title="AD",FileName="AD1.xml"},
              new Index{ Title="BA",FileName="BA1.xml"},
              new Index{ Title="BC",FileName="BC1.xml"},
              new Index{ Title="BE",FileName="BE1.xml"},
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(dlBook);
            container.Arrange<IPackageAccess>(packageAccess => packageAccess.GetIndexs(null)).IgnoreArguments().Returns(list);
            var indexdic = await container.Instance.GetIndexsByBookId(bookId);
            Assert.IsTrue(indexdic.Count == 2);
            Assert.IsTrue(indexdic["A"].Count == 3);
            Assert.IsTrue(indexdic["B"].Count == 3);
        }

        [Test()]
        public async void GetIndexListTest()
        {
            List<Index> list = new List<Index> { 
              new Index{ Title="AB",FileName="AB1.xml"},
              new Index{ Title="AC",FileName="AC1.xml"},
              new Index{ Title="AD",FileName="AD1.xml"},
              new Index{ Title="BA",FileName="BA1.xml"},
              new Index{ Title="BC",FileName="BC1.xml"},
              new Index{ Title="BE",FileName="BE1.xml"},
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(dlBook);
            container.Arrange<IPackageAccess>(packageAccess => packageAccess.GetIndexs(null)).IgnoreArguments().Returns(list);
            var indexList = await container.Instance.GetIndexList(bookId);
            Assert.IsTrue(indexList.Count == 6);
            Assert.IsTrue(indexList.FirstOrDefault(o => o.Title == "Ab") != null);
        }
        [Test]
        public void GetContentFromTOCTest()
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                string content = File.ReadAllText(Path.Combine(path, "XpathTest.txt"));
                HtmlHelper helper = new HtmlHelper();
                helper.LoadHtml(content);
                helper.LoadHtml(content);
                var spanid = helper.GetSpanIdByXpath("div/div[3]/div[3]/section[1]/div/h3/span");
                var xpath = helper.GetXpathBySpanId(spanid);
                Assert.IsTrue(content.Length > 0);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Test, Ignore]
        public void GetContentFromIndexTest()
        {
            try
            {
                Index index = new Index { Title = "BB" };
                container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(dlBook);
                var content = PublicationContentUtil.Instance.GetContentFromIndex(bookId, index).Result;
                Assert.IsTrue(content.Length > 0);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Test]
        public void FilePathReplace()
        {
            var dd = Regex.Replace("<a href=\"file:\\DL_FOLDER\\dd.pdf\" />dsadas<a href=\"file:\\DL_FOLDER\\dd.pdf\" />", @"file:\\DL_FOLDER", "file:\\" + dlBook.GetDirectory());
            Assert.IsTrue(!dd.Contains("DL_FOLDER"));
        }

        [Test]
        public void GetRecentHistoryTest()
        {
            TOCNode tocNode = new TOCNode
            {
                GuideCardTitle = "GuideCardTitle",
                Title = "",
                ID = 3
            };
            container.Arrange<IPublicationAccess>(publicationAccess => publicationAccess.GetDlBookByBookId(0, null)).IgnoreArguments().Returns(dlBook);
            var content = PublicationContentUtil.Instance.GetContentFromTOC(bookId, tocNode).Result;
            var history = PublicationContentUtil.Instance.GetRecentHistory();
            Assert.IsTrue(history.Count > 0);
        }
        [Test]
        public async void GetTOCListTest()
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            var tocs = await PublicationContentUtil.Instance.GetTOCByBookId(bookId);
            // timer.Stop();
            //    await Logger.Log(timer.ElapsedMilliseconds.ToString());
            //    await Logger.Log("-------------------------end---------------------------------");
        }
        [Test]
        public async void GetTOCNodeByTOCIdTest()
        {
            var tocNode = await PublicationContentUtil.Instance.GetTOCByBookId(bookId);

            var specialNode = PublicationContentUtil.Instance.GetTOCByTOCId(2, tocNode);

            Assert.IsTrue(specialNode.ID == 2);
        }

        [Test]
        public void RenderHyperLinkTest()
        {
            string str = File.ReadAllText(@"C:\TempFiles_can_delete\RenderHyperLinkTest.htm");
            string result = PublicationContentUtil.Instance.RenderHyperLink(str, 13);
            Assert.AreNotEqual(str, result);
        }
    }
}
