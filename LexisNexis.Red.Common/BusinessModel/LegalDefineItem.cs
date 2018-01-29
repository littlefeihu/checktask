using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class LegalDefinitionItem
    {
        public String SearchTerm { get; set; }

        public String DictionaryVersionText { get; set; }

        public List<LegalContextualDefinitionItem> ContextualDefinitions { get; set; }

        public LegalDefinitionItem()
        {
            ContextualDefinitions = new List<LegalContextualDefinitionItem>();
        }

        public LegalDefinitionItem(List<LegalContextualDefinitionItem> contextualDefinitions)
        {
            ContextualDefinitions = contextualDefinitions;
        }

        public Int32 RelatedKeywordsCount
        {
            get
            {
                return ContextualDefinitions.Sum(item => item.AllRelatedKeywords.Count);
            }
        }
    }
}
