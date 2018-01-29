using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class LegalContextualDefinitionItem
    {
        public LegalContextualDefinitionItem()
        {
        }
        public String Term { get; set; }
        public String Context { get; set; }
        public String DefinitionHtml { get; set; }
        public String See { get; set; }
        public String SeeMore { get; set; }
        public List<LegalRelatedKeywordItem> SeeKeywords { get; set; }
        public List<LegalRelatedKeywordItem> SeeMoreKeywords { get; set; }

        public List<LegalRelatedKeywordItem> AllRelatedKeywords
        {
            get
            {
                List<LegalRelatedKeywordItem> allSeeList = new List<LegalRelatedKeywordItem>();

                if (SeeKeywords != null && SeeKeywords.Count > 0)
                    allSeeList.AddRange(SeeKeywords);

                if (SeeMoreKeywords != null && SeeMoreKeywords.Count > 0)
                    allSeeList.AddRange(SeeMoreKeywords);

                return allSeeList;
            }
        }

        public LegalContextualDefinitionItem(String term, String context, String definitionHtml)
        {
            Term = term;
            Context = context;
            DefinitionHtml = definitionHtml;
        }

        public LegalContextualDefinitionItem(String term, String context, String definitionHtml, List<LegalRelatedKeywordItem> seeKeywords, List<LegalRelatedKeywordItem> seeMoreKeywords)
        {
            Term = term;
            Context = context;
            DefinitionHtml = definitionHtml;
            SeeKeywords = seeKeywords;
            SeeMoreKeywords = seeMoreKeywords;
        }
    }
}
