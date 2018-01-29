using LexisNexis.Red.Common.DomainService.AnnotationSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService
{
    public interface IAnnotationSyncService
    {
        void RequestAnnotationSync(IAnnotationSyncTask task);
        void RequestAllDlBooksSync();
        void RequestTagsOnly();
    }
}
