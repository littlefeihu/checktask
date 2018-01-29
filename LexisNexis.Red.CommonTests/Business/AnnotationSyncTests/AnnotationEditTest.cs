using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LexisNexis.Red.CommonTests.Business
{

    [Category("AnnotationEditTest")]
    [TestFixture]
    public class AnnotationEditTest
    {
        List<Annotation> annotations;
        int bookid = 41;
        public AnnotationEditTest()
        {
            var loginResut = LoginUtil.Instance.ValidateUserLogin(TestHelper.TestUsers[0].UserName, TestHelper.TestUsers[0].Password, TestHelper.TestUsers[0].CountryCode).Result;
            annotations = AnnotationUtil.Instance.GetAnnotations();
            if (annotations != null)
            {
                //foreach (var item in annotations)
                //{
                //    AnnotationUtil.Instance.DeleteAnnotation(item.AnnotationCode);
                //}
            }
            //AnnotationUtil.Instance.AddAnnotation(InitAnnotation()).Wait();
        }

        private Annotation InitAnnotation()
        {
            AnnotationDataItem startLevel = new AnnotationDataItem(1, "latup10001000.xml", "CE_LATUP.SGM_CE.LATUP.S177", "/div/div[3]/div[3]/section[1]/ul[1]/li/span[3]/span", "CE_LATUP.SGM_CE.LATUP.C1");
            AnnotationDataItem endLevel = new AnnotationDataItem(2, "latup10001000.xml", "CE_LATUP.SGM_CE.LATUP.S177", "/div/div[3]/div[3]/section[1]/ul[1]/li/span[4]", "CE_LATUP.SGM_CE.LATUP.C1");
            List<Guid> TagIds = new List<Guid>();
            TagIds.Add(Guid.Parse("c731fa5f-1db3-4142-9541-45dbbd6cccba"));

            Annotation annotation = new Annotation(Guid.NewGuid(), AnnotationType.Highlight, AnnotationStatusEnum.Created, bookid, 19, startLevel.DocId, "Introduction to the 2001 National Corporations Legislation", null, null, "Historical background: Introduction to the 2001 National Corporations Legislation", TagIds, startLevel, endLevel);
            return annotation;
        }

        private Annotation InitAnnotationWithNoTag()
        {
            AnnotationDataItem startLevel = new AnnotationDataItem(1, "latup10001000.xml", "CE_LATUP.SGM_CE.LATUP.S177", "/div/div[3]/div[3]/section[1]/ul[1]/li/span[3]/span", "CE_LATUP.SGM_CE.LATUP.C1");
            AnnotationDataItem endLevel = new AnnotationDataItem(2, "latup10001000.xml", "CE_LATUP.SGM_CE.LATUP.S177", "/div/div[3]/div[3]/section[1]/ul[1]/li/span[4]", "CE_LATUP.SGM_CE.LATUP.C1");
            List<Guid> TagIds = new List<Guid>();
            Annotation annotation = new Annotation(Guid.NewGuid(), AnnotationType.Highlight, AnnotationStatusEnum.Created, bookid, 19, startLevel.DocId, "Introduction to the 2001 National Corporations Legislation", "fdffffffffffffffffffffffffffff", "highlight Text", "Historical background: Introduction to the 2001 National Corporations Legislation", TagIds, startLevel, endLevel);
            return annotation;
        }
        [Test]
        public async void AddAnnotationTest()
        {
            var annotation = InitAnnotation();
            await AnnotationUtil.Instance.AddAnnotation(annotation);
            var annotation1 = InitAnnotationWithNoTag();
            await AnnotationUtil.Instance.AddAnnotation(annotation1);
            var annotations = AnnotationUtil.Instance.GetAnnotations();
            var anno = annotations.FirstOrDefault(o => o.AnnotationCode == annotation.AnnotationCode);
            Assert.IsTrue(anno != null);
        }
        [Test]
        public void DeleteAnnotationTest()
        {
            annotations = AnnotationUtil.Instance.GetAnnotations();
            if (annotations != null || annotations.Count > 0)
            {
                var annoCode = annotations[0].AnnotationCode;
                AnnotationUtil.Instance.DeleteAnnotation(annoCode);
                annotations = AnnotationUtil.Instance.GetAnnotations();
                Assert.IsTrue(annotations.FirstOrDefault(o => o.AnnotationCode == annoCode) == null);
            }
        }

        [Test]
        public async void UpdateAnnotationTest()
        {
            annotations = AnnotationUtil.Instance.GetAnnotationsByDocId(bookid, "CE_LATUP.SGM_CE.LATUP.C1");
            if (annotations != null || annotations.Count > 0)
            {
                var updateAnnotation = annotations[0];
                updateAnnotation.StartLevel.FileName = "11111111111111";
                updateAnnotation.EndLevel.FileName = "11111111111111";
                await AnnotationUtil.Instance.UpdateAnnotation(updateAnnotation);
                var key = updateAnnotation.AnnotationCode;
                annotations = AnnotationUtil.Instance.GetAnnotations();
                var anno = annotations.FirstOrDefault(o => o.AnnotationCode == key);
                Assert.IsTrue(anno.StartLevel.FileName == updateAnnotation.StartLevel.FileName);
            }
        }

        [Test]
        public void GetAnnotationsTest()
        {
            annotations = AnnotationUtil.Instance.GetAnnotations();
            Assert.IsTrue(annotations.Count > 0);
        }
        [Test]
        public void GetAnnotationsTest1()
        {
            var dic = AnnotationUtil.Instance.GetAnnotationsByBookId(bookid);
            Assert.IsTrue(dic.Count() > 0);
        }
        [Test]
        public void GetAnnotationsintegratedSearchTest()
        {
            var dic = AnnotationUtil.Instance.GetAnnotations(AnnotationType.Highlight, new List<Guid> { Guid.Parse("c731fa5f-1db3-4142-9541-45dbbd6cccba") }, true);
            Assert.IsTrue(dic.Count() > 0);
            var dic1 = AnnotationUtil.Instance.GetAnnotations(bookid, AnnotationType.Highlight, new List<Guid> { Guid.Parse("c731fa5f-1db3-4142-9541-45dbbd6cccba") }, true);
            Assert.IsTrue(dic1.Count() > 0);
        }
        [Test]
        public void GetAnnotationsByDocIdTest()
        {
            var dic = AnnotationUtil.Instance.GetAnnotationsByDocId(bookid, "CE_LATUP.SGM_CE.LATUP.C1");
            Assert.IsTrue(dic.Count() > 0);
            var dic1 = AnnotationUtil.Instance.GetAnnotationsByTocId(bookid, 3);
            Assert.IsTrue(dic1.Count() > 0);
        }

    }
}
