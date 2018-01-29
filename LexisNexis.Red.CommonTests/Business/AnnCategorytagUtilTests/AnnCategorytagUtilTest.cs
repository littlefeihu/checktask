using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
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
    [Category("AnnCategorytagUtilTests")]
    [TestFixture]
    public partial class AnnCategorytagUtilTests
    {
        [Test()]
        public void SortTest()
        {
            var tags = AnnCategoryTagUtil.Instance.GetTags();
            for (int i = 0; i < tags.Count; i++)
            {
                tags[i].SortOrder = tags.Count - i;
            }
            AnnCategoryTagUtil.Instance.Sort(tags.OrderBy(o => o.SortOrder).ToList());
            tags = AnnCategoryTagUtil.Instance.GetTags();
            Assert.IsTrue(tags.Count > 0);
        }
        [Test()]
        public void GetTagsTest()
        {
            var tags = AnnCategoryTagUtil.Instance.GetTags();
            Assert.IsTrue(tags.Count > 0);
        }
        [Test]
        public void AddTagTest()
        {
            var tagColors = AnnCategoryTagUtil.Instance.GetTagColors();
            for (int i = 1; i <= 1; i++)
            {
                var rows = AnnCategoryTagUtil.Instance.AddTag("tag" + i, tagColors[0].ColorValue);
                Assert.IsTrue(rows != Guid.Empty);
            }
        }
        [Test()]
        public void DeleteTagTest()
        {
            var tags = AnnCategoryTagUtil.Instance.GetTags();
            if (tags.Count > 10)
            {
                var deletedTags = tags.Skip(10).Take(tags.Count - 10);
                foreach (var deletedTag in deletedTags)
                {

                    var rows = AnnCategoryTagUtil.Instance.DeleteTag(deletedTag.TagId);

                    Assert.IsTrue(rows > 0);
                }
            }
        }
        [Test()]
        public void UpdateTagTest()
        {
            var tags = AnnCategoryTagUtil.Instance.GetTags();
            if (tags.Count > 0)
            {

                var rows = AnnCategoryTagUtil.Instance.UpdateTag(tags[0].TagId, "Tag1Updated", "#FAD788");
                Assert.IsTrue(rows > 0);
            }
        }
    }
}
