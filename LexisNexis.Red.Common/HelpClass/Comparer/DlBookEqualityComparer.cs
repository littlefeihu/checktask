using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class DlBookEqualityComparer : IEqualityComparer<DlBook>
    {
        public bool Equals(DlBook dlBook1, DlBook dlBook2)
        {
            return dlBook1.BookId == dlBook2.BookId;
        }
        public int GetHashCode(DlBook dlBook)
        {
            return dlBook.BookId.GetHashCode();
        }
    }
}
