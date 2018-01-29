using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace LexisNexis.Red.Common.BusinessModel
{
    public class LegalRelatedKeywordItem
    {
        public String Term { get; set; }

        public String HyperlinkTermKeyword
        {
            get { return "<a href='looseleaf://legaldefine?term=" + Term + "'>" + Term + "</a>"; }
        }

        public LegalRelatedKeywordItem(String term)
        {
            String correctString = term.Replace("’", "'");

            Term = correctString;
        }
    }
}
