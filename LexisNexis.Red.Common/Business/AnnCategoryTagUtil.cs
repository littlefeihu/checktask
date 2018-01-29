using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LexisNexis.Red.Common.Business
{
    public class AnnCategoryTagUtil : AppServiceBase<AnnCategoryTagUtil>
    {

        private List<TagColor> tagColors;
        private ITagDomainService tagDomainService;

        public AnnCategoryTagUtil(ITagDomainService tagDomainService)
        {
            this.tagDomainService = tagDomainService;
        }

        public void Clear()
        {
            this.tagDomainService.Tags = new List<AnnotationTag>();
            this.tagDomainService.AnnCategorytag = new AnnCategoryTags();
        }

        public List<TagColor> GetTagColors()
        {
            if (tagColors == null)
                tagColors = TagColorFactory.InitTagColors();

            return tagColors;
        }

        public AnnotationTag AddAndReturnTag(string title, string color)
        {
            return tagDomainService.AddAndReturnTag(title, color);
        }
        public Guid AddTag(string title, string color)
        {
            var tag = AddAndReturnTag(title, color);
            return tag == null ? Guid.Empty : tag.TagId;
        }
        public int DeleteTag(Guid tagId)
        {
            return tagDomainService.DeleteTag(tagId);
        }
        public int UpdateTag(Guid tagId, string title, string color)
        {
            var tag = UpdateAndReturnTag(tagId, title, color);
            return tag == null ? 0 : 1;
        }

        public AnnotationTag UpdateAndReturnTag(Guid tagId, string title, string color)
        {
            return tagDomainService.UpdateAndReturnTag(tagId, title, color);
        }
        public List<AnnotationTag> GetTags()
        {
            return tagDomainService.GetTags();
        }
        public void Sort(IEnumerable<AnnotationTag> orderedTags)
        {
            tagDomainService.Sort(orderedTags);
        }
        public void Sort(IEnumerable<Guid> orderedTagIds)
        {
            tagDomainService.Sort(orderedTagIds);
        }
    }
}
