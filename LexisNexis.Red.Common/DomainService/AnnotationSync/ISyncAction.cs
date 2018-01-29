using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.AnnotationSync
{
    public interface ISyncAction
    {
        SyncActionName SyncActionName { get; }
        Task PerformAction(AnnotationTaskContext context);
    }
}
