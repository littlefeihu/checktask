using LexisNexis.Red.Common.DomainService.AnnotationSync;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService
{
    public class SyncActionFactory
    {
        public static ISyncAction CreateSyncActionBy(SyncActionName syncActionName)
        {
            return IoCContainer.Instance.ResolveInterface<ISyncAction>(syncActionName.ToString());
        }
    }
}
