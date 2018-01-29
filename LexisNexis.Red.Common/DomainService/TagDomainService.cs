using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using LexisNexis.Red.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LexisNexis.Red.Common.DomainService
{
    public class TagDomainService : ITagDomainService
    {
        private IAnnCategoryTagsAccess tagAccess;
        private List<AnnotationTag> tags;
        private AnnCategoryTags annCategorytag;

        public TagDomainService(IAnnCategoryTagsAccess tagAccess)
        {
            this.tagAccess = tagAccess;
            tags = new List<AnnotationTag>();
            annCategorytag = new AnnCategoryTags();
        }

        public AnnCategoryTags AnnCategorytag
        {
            get
            {
                return annCategorytag;
            }
            set
            {
                annCategorytag = value;
            }
        }
        public List<AnnotationTag> Tags
        {
            get
            {
                return tags;
            }
            set
            {
                tags = value;
            }
        }
        public void LoadTagsFromDb(string email, string serviceCode, out AnnCategoryTags tag, out List<AnnotationTag> outTags)
        {
            tag = tagAccess.GetAnnCategoryTag(email, serviceCode);
            outTags = new List<AnnotationTag>();
            if (tag != null && !string.IsNullOrEmpty(tag.CategoryTagXMLData))
            {
                outTags = GenerateTagsFromTagXML(AnnCategorytag.CategoryTagXMLData);
            }
            if (tag == null)
                tag = new AnnCategoryTags();
        }

        public void RefreshInstance(string email, string serviceCode)
        {
            LoadTagsFromDb(GlobalAccess.Instance.Email, GlobalAccess.Instance.ServiceCode, out annCategorytag, out tags);
        }

        public List<AnnotationTag> GenerateTagsFromTagXML(string categoryTagXml)
        {
            XDocument doc = XDocument.Parse(categoryTagXml);
            XElement root = doc.Element("AnnotationTags");
            if (root != null)
            {
                var elements = root.Elements("AnnotationTag");
                if (elements != null)
                {
                    List<AnnotationTag> tagList = new List<AnnotationTag>(elements.Count());
                    foreach (XElement el in root.Elements("AnnotationTag"))
                    {
                        AnnotationTag data = new AnnotationTag
                        {
                            TagId = (Guid)el.Attribute("TagID"),
                            Color = (string)el.Attribute("Color"),
                            Title = (string)el.Element("Title"),
                            SortOrder = (int)el.Attribute("SortOrder")
                        };
                        tagList.Add(data);
                    }
                    return tagList;
                }
            }
            return null;
        }

        public AnnotationTag AddAndReturnTag(string title, string color)
        {
            AnnCategorytag.IsModified = true;
            var newTag = new AnnotationTag
            {
                Title = title,
                Color = color,
                TagId = Guid.NewGuid(),
                SortOrder = Tags.Count + 1
            };
            Tags.Add(newTag);
            if (SaveToSqliteWithSynctags() > 0)
            {
                return newTag;
            }
            else
            {
                return null;
            }
        }
        public int DeleteTag(Guid tagId)
        {
            AnnCategorytag.IsModified = true;
            var tag = Tags.FirstOrDefault(t => t.TagId == tagId);
            if (tag != null)
            {
                if (Tags.Remove(tag))
                {
                    PublishTagDeletedEvent(tagId).Wait();
                }
            }
            return SaveToSqliteWithSynctags();
        }

        private Task PublishTagDeletedEvent(Guid tagId)
        {
            return Task.Run(async () =>
            {
                await DomainEvents.Publish(new TagDeletedEvent(tagId, GlobalAccess.Instance.Email, GlobalAccess.Instance.ServiceCode));
            });
        }
        public AnnotationTag UpdateAndReturnTag(Guid tagId, string title, string color)
        {
            AnnCategorytag.IsModified = true;
            var tag = Tags.FirstOrDefault(t => t.TagId == tagId);
            if (tag != null)
            {
                tag.Title = title;
                tag.Color = color;
            }
            var saveSuccess = (SaveToSqliteWithSynctags() > 0);
            return saveSuccess ? tag : null;
        }
        public List<AnnotationTag> GetTags()
        {
            DomainEvents.Publish(new SyncTagsEvent());
            RefreshInstance(GlobalAccess.Instance.Email,GlobalAccess.Instance.ServiceCode);
            return Tags.OrderBy(o => o.SortOrder).ToList();
        }
        public void Sort(IEnumerable<Guid> orderedTagIds)
        {
            int order = 0;
            foreach (var tagId in orderedTagIds)
            {
                order++;
                Tags.FirstOrDefault(o => o.TagId == tagId).SortOrder = order;
            }
            var saveSuccess = (SaveToSqliteWithSynctags() > 0);
            if (saveSuccess)
                RefreshInstance(GlobalAccess.Instance.Email, GlobalAccess.Instance.ServiceCode);
        }
        public void Sort(IEnumerable<AnnotationTag> orderedTags)
        {
            Sort(orderedTags.Select(o => o.TagId));
        }
        public string ToXmlForSync(List<AnnotationTag> tags)
        {
            XElement root = new XElement("AnnotationTags");
            foreach (var tag in tags)
            {
                root.Add(
                    new XElement("AnnotationTag",
                        new XAttribute("TagID", tag.TagId),
                        new XAttribute("Color", tag.Color),
                         new XAttribute("SortOrder", tag.SortOrder),
                        new XElement("Title", tag.Title)));
            }
            root.Add(new XAttribute("MaxID", tags.Count));
            return root.ToString();
        }
        public int SaveToSqlite(List<AnnotationTag> tags, string email, string serviceCode)
        {
            if (email == null || serviceCode == null)
            {
                throw new NullUserException("SaveToSqlite,NullUserException");
            }
            AnnCategorytag.CategoryTagXMLData = ToXmlForSync(tags);
            AnnCategorytag.LastUpdateTimeClient = DateTime.Now;
            AnnCategorytag.Email = email;
            AnnCategorytag.ServiceCode = serviceCode;
            if (tagAccess.GetAnnCategoryTag(email, serviceCode) != null)
            {
                return tagAccess.UpdateAnnCategoryTags(AnnCategorytag);
            }
            else
            {
                return tagAccess.InsertAnnCategoryTags(AnnCategorytag);
            }

        }
        public int SaveToSqliteWithSynctags()
        {
            try
            {
                return SaveToSqlite(this.Tags, GlobalAccess.Instance.Email, GlobalAccess.Instance.ServiceCode);
            }
            finally
            {
                DomainEvents.Publish(new SyncTagsEvent());
            }
        }
    }
}
