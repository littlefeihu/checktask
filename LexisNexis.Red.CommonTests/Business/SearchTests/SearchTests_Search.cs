using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexisNexis.Red.Common;
using NUnit.Framework;
using Telerik.JustMock.AutoMock;
using LexisNexis.Red.Common.Interface;
using Telerik.JustMock;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.Common.Entity;
using Newtonsoft.Json;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.CommonTests.Interface;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Business;
using System.Threading;
using LexisNexis.Red.CommonTests.Impl;
using LexisNexis.Red.CommonTests.Business;
using LexisNexis.Red.Common.Segment.Framework;
using System.IO;
using System.Diagnostics;
namespace LexisNexis.Red.CommonTests.Business
{

    [TestFixture()]
    public partial class SearchTests
    {
        [SetUp()]
        public void Test_Init()
        {
            Stopwatch time = new Stopwatch();
            time.Start();
            TestInit.Init().Wait();
            LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Wait();
            DlBook dlBook = new DlBook
            {
                BookId = 26,
                InitVector = "QDFCMmMzRDRlNUY2ZzdIOA==",
                K2Key = "Kd7qQHCj9qT05gDAAAgXsj74kxQpHA2r2Flf3FH7XLQ=",
                HmacKey = "7PPPQbvLEVsMRwpXJJqQv49fU7c=",
                LastDownloadedVersion =18,
                ServiceCode = GlobalAccess.Instance.CurrentUserInfo.Country.ServiceCode,
               // CountryCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode,
                Email = GlobalAccess.Instance.CurrentUserInfo.Email
            };
            var contentKey = dlBook.GetContentKey(GlobalAccess.Instance.CurrentUserInfo.SymmetricKey).Result;
            GlobalAccess.Instance.CurrentPublication = new PublicationContent(dlBook, contentKey);
            time.Stop();
            Console.Out.WriteLine("Time : " + time.ElapsedMilliseconds);
        }

        [Test, Category("SearchContentTest_Search")]
        public void SearchContentTest_Regular()
        {
            string keywords = "act";
            Stopwatch time = new Stopwatch();
            time.Start();
            int bookID = 26;
            int tocID = 2;
            List<ContentCategory> contentTypeList = new List<ContentCategory>();
            var result = SearchUtil.Search(bookID, tocID, keywords);
            Assert.LessOrEqual(0, result.SearchDisplayResultList.Count);

            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.FormsPrecedentsType);
            var resultFormsPrecedentsType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultFormsPrecedentsType.SearchDisplayResultList.Count);

            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.CommentaryType);
            var resultCommentaryType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultCommentaryType.SearchDisplayResultList.Count);

            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.LegislationType);
            var resultLegislationType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultLegislationType.SearchDisplayResultList.Count);

            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.CommentaryType);
            contentTypeList.Add(ContentCategory.FormsPrecedentsType);
            var resultCommentaryType_FormsPrecedentsType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultCommentaryType_FormsPrecedentsType.SearchDisplayResultList.Count);

            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.LegislationType);
            contentTypeList.Add(ContentCategory.FormsPrecedentsType);
            var resultLegislationType_FormsPrecedentsType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultLegislationType_FormsPrecedentsType.SearchDisplayResultList.Count);

            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.LegislationType);
            contentTypeList.Add(ContentCategory.CommentaryType);
            var resultLegislationType_CommentaryType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultLegislationType_CommentaryType.SearchDisplayResultList.Count);

            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.FormsPrecedentsType);
            contentTypeList.Add(ContentCategory.CommentaryType);
            contentTypeList.Add(ContentCategory.LegislationType);
            var result_ALL = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, result_ALL.SearchDisplayResultList.Count);

            time.Stop();
            Console.Out.WriteLine("Time : " + time.ElapsedMilliseconds);
            var tempText = keywords;
        }

        [Test, Category("SearchContentTest_Search")]
        public void SearchContentTest_FormsPrecedentsType()
        {
            string keywords = "Supreme Court";
            Stopwatch time = new Stopwatch();
            time.Start();
            int bookID = 41;
            int tocID = 8;
            List<ContentCategory> contentTypeList = new List<ContentCategory>();
            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.FormsPrecedentsType);
            var resultFormsPrecedentsType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultFormsPrecedentsType.SearchDisplayResultList.Count);

            time.Stop();
            Console.Out.WriteLine("Time : " + time.ElapsedMilliseconds);
            var tempText = keywords;
        }

        [Test, Category("SearchContentTest_Search")]
        public void SearchContentTest_CommentaryType()
        {
            string keywords = "Supreme Court";
            Stopwatch time = new Stopwatch();
            time.Start();
            int bookID = 41;
            int tocID = 8;
            List<ContentCategory> contentTypeList = new List<ContentCategory>();
            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.CommentaryType);
            var resultCommentaryType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultCommentaryType.SearchDisplayResultList.Count);

            time.Stop();
            Console.Out.WriteLine("Time : " + time.ElapsedMilliseconds);
            var tempText = keywords;
        }

        [Test, Category("SearchContentTest_Search")]
        public void SearchContentTest_LegislationType()
        {
            string keywords = "Supreme Court";
            Stopwatch time = new Stopwatch();
            time.Start();
            int bookID = 41;
            int tocID = 8;
            List<ContentCategory> contentTypeList = new List<ContentCategory>();
            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.LegislationType);
            var resultLegislationType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultLegislationType.SearchDisplayResultList.Count);

            time.Stop();
            Console.Out.WriteLine("Time : " + time.ElapsedMilliseconds);
            var tempText = keywords;
        }

        [Test, Category("SearchContentTest_Search")]
        public void SearchContentTest_MixType()
        {
            string keywords = "Supreme Court";
            Stopwatch time = new Stopwatch();
            time.Start();
            int bookID = 41;
            int tocID = 8;
            List<ContentCategory> contentTypeList = new List<ContentCategory>();
            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.CommentaryType);
            contentTypeList.Add(ContentCategory.FormsPrecedentsType);
            var resultCommentaryType_FormsPrecedentsType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultCommentaryType_FormsPrecedentsType.SearchDisplayResultList.Count);

            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.LegislationType);
            contentTypeList.Add(ContentCategory.FormsPrecedentsType);
            var resultLegislationType_FormsPrecedentsType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultLegislationType_FormsPrecedentsType.SearchDisplayResultList.Count);

            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.LegislationType);
            contentTypeList.Add(ContentCategory.CommentaryType);
            var resultLegislationType_CommentaryType = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, resultLegislationType_CommentaryType.SearchDisplayResultList.Count);

            contentTypeList.Clear();
            contentTypeList.Add(ContentCategory.FormsPrecedentsType);
            contentTypeList.Add(ContentCategory.CommentaryType);
            contentTypeList.Add(ContentCategory.LegislationType);
            var result_ALL = SearchUtil.Search(bookID, tocID, keywords, contentTypeList);
            Assert.LessOrEqual(0, result_ALL.SearchDisplayResultList.Count);
            time.Stop();
            Console.Out.WriteLine("Time : " + time.ElapsedMilliseconds);
            var tempText = keywords;
        }

        [Test, Category("SearchContentTest_Search")]
        public void SearchContentTest_SPECIAL()
        {
            string keywords = "!@#$%^&*()_+}{|\\][\":?></.,;\'law";
            Stopwatch time = new Stopwatch();
            time.Start();
            int bookID = 41;
            int tocID = 1074;
            List<ContentCategory> contentTypeList = new List<ContentCategory>();
            var result = SearchUtil.Search(bookID, tocID, keywords);
            Assert.LessOrEqual(0, result.SearchDisplayResultList.Count);
        }

        [Test, Category("SearchAnnoationTest_Search")]
        public void SearchAnnoationTest_Regular()
        {
            string keywords = "A1";
            Stopwatch time = new Stopwatch();
            time.Start();
            var result0 = AnnotationUtil.SearchAnnotation(keywords);
            Assert.LessOrEqual(0, result0.Count);
            var result1 = AnnotationUtil.SearchAnnotation(keywords, AnnotationType.Highlight);
            Assert.LessOrEqual(0, result1.Count);
            var result2 = AnnotationUtil.SearchAnnotation(keywords, AnnotationType.StickyNote);
            Assert.LessOrEqual(0, result2.Count);
            var result3 = AnnotationUtil.SearchAnnotation(keywords, AnnotationType.Orphan);
            Assert.LessOrEqual(0, result3.Count);
            var tempText = keywords;
        }

        [Test, Category("SearchAnnoationTest_Search")]
        public void SearchAnnoationTest_NoResult()
        {
            string keywords = "ABCDEFGHIJKLMNOOPQRSTUVWXYZ";
            Stopwatch time = new Stopwatch();
            time.Start();
            var result0 = AnnotationUtil.SearchAnnotation(keywords);
            Assert.AreEqual(0, result0.Count);
            var result1 = AnnotationUtil.SearchAnnotation(keywords, AnnotationType.Highlight);
            Assert.AreEqual(0, result1.Count);
            var result2 = AnnotationUtil.SearchAnnotation(keywords, AnnotationType.StickyNote);
            Assert.AreEqual(0, result2.Count);
            var result3 = AnnotationUtil.SearchAnnotation(keywords, AnnotationType.Orphan);
            Assert.AreEqual(0, result3.Count);
            var tempText = keywords;
        }
    }
}
