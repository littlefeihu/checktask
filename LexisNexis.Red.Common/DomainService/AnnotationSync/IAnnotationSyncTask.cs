using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.AnnotationSync
{
    public interface IAnnotationSyncTask
    {
        IList<int> BookIds { get; }
        string UserSpecificFolder { get; }
        string ServiceCode { get; }
        string CountryCode { get; }
        string Email { get; }
        string DeviceId { get; }
        IDictionary<int, int> CurrentVersion { get; }
        bool IsSyncTagsOnly { get; }
    }
}
