using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public interface IAnnCategoryTagsAccess
    {
        int InsertAnnCategoryTags(AnnCategoryTags tags);
        int UpdateAnnCategoryTags(AnnCategoryTags tags);
        AnnCategoryTags GetAnnCategoryTag(string email, string serviceCode);
    }
}
