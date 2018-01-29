using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService
{
    public interface IOrphanItemsService
    {
        void ProcessOrphanedData(DlBook dlBook);
    }
}
