using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService
{
    public interface ITagDomainService
    {
        AnnCategoryTags AnnCategorytag { get; set; }
        List<AnnotationTag> Tags { get; set; }
        void LoadTagsFromDb(string email, string serviceCode, out AnnCategoryTags tag, out List<AnnotationTag> outTags);
        void RefreshInstance(string email, string serviceCode);
        List<AnnotationTag> GenerateTagsFromTagXML(string categoryTagXml);
        AnnotationTag AddAndReturnTag(string title, string color);
        int DeleteTag(Guid tagId);
        AnnotationTag UpdateAndReturnTag(Guid tagId, string title, string color);
        List<AnnotationTag> GetTags();
        void Sort(IEnumerable<Guid> orderedTagIds);
        void Sort(IEnumerable<AnnotationTag> orderedTags);
        string ToXmlForSync(List<AnnotationTag> tags);
        int SaveToSqlite(List<AnnotationTag> tags, string email, string serviceCode);
        int SaveToSqliteWithSynctags();
    }
}
