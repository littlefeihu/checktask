using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public class AnnCategoryTagsAccess : DbHelper, IAnnCategoryTagsAccess
    {

        string getAnnCategoryTag = "SELECT * FROM AnnCategoryTags WHERE Email=? AND ServiceCode=? limit 0,1 ";
        public int InsertAnnCategoryTags(AnnCategoryTags tags)
        {
            return base.Insert<AnnCategoryTags>(base.MainDbPath, tags);
        }

        public int UpdateAnnCategoryTags(AnnCategoryTags tags)
        {
            return base.Update<AnnCategoryTags>(base.MainDbPath, tags);
        }

        public AnnCategoryTags GetAnnCategoryTag(string email, string serviceCode)
        {
            return base.GetEntity<AnnCategoryTags>(base.MainDbPath, getAnnCategoryTag, email, serviceCode);
        }
    }
}
