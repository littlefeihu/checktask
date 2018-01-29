using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public class DictionaryAccess : DbHelper, IDictionaryAccess
    {
        static string queryString = "SELECT TERM AS Term, CONTEXT AS Context, DEFINITION AS DefinitionHtml, SEE AS See, SEEALSO AS SeeMore FROM LEGAL_DEFINE WHERE TERM COLLATE nocase = ?";

        public LegalDefinitionItem SearchDictionary(string term, string countryCode)
        {
            LegalDefinitionItem result = new LegalDefinitionItem();
            List<LegalContextualDefinitionItem> contextualItemList = new List<LegalContextualDefinitionItem>();
            contextualItemList = base.GetEntityList<LegalContextualDefinitionItem>(Path.Combine(GlobalAccess.DirectoryService.GetAppExternalRootPath(),
                Constants.DICITIONARY_DB_NAME + countryCode + Constants.DICITIONARY_EXTENSION_NAME), queryString, term);

            if (GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode.Equals(CountryEnum.AU.ToString()))
            {
                result.DictionaryVersionText = Constants.AU_DICTIONARY_VERSION_TEXT;
            }
            else
            {
                result.DictionaryVersionText = Constants.NZ_DICTIONARY_VERSION_TEXT;
            }

            foreach (LegalContextualDefinitionItem item in contextualItemList)
            {
                if(!String.IsNullOrEmpty(item.See))
                {
                    List<string> itemStringList = item.See.Split('|').ToList();
                    List<LegalRelatedKeywordItem> itemKeywordList = new List<LegalRelatedKeywordItem>();
                    foreach(string stringItem in itemStringList)
                    {
                        LegalRelatedKeywordItem kItem = new LegalRelatedKeywordItem(stringItem.Trim());
                        itemKeywordList.Add(kItem);
                    }
                    item.SeeKeywords = itemKeywordList;
                }

                if (!String.IsNullOrEmpty(item.SeeMore))
                {
                    List<string> itemStringList = item.SeeMore.Split('|').ToList();
                    List<LegalRelatedKeywordItem> itemKeywordList = new List<LegalRelatedKeywordItem>();
                    foreach (string stringItem in itemStringList)
                    {
                        LegalRelatedKeywordItem kItem = new LegalRelatedKeywordItem(stringItem.Trim());
                        itemKeywordList.Add(kItem);
                    }
                    item.SeeMoreKeywords = itemKeywordList;
                }
            }
            result.ContextualDefinitions = contextualItemList;
            return result;
        }
    }
}
