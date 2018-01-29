using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Services
{
    public interface ISyncService
    {
        /// <summary>
        /// download annotations from server
        /// </summary>
        /// <param name="downwardSyncRequest">downwardSyncRequest</param>
        /// <returns></returns>
        Task<bool> DownwardSyncRequest(DownwardSyncRequests downwardSyncRequest, string fileName);
        /// <summary>
        /// upward annotations to server
        /// </summary>
        /// <param name="strFile">annotation file</param>
        /// <returns></returns>
        Task<HttpResponse> UpwardSyncRequest(string strFile);

        /// <summary>
        /// sync category tags data to server
        /// </summary>
        /// <param name="data">categorytags data</param>
        /// <returns></returns>
        Task<AnnCategoryTagsSyncData> AnnCategoryTagsSync(AnnCategoryTagsSyncData data);

        Task<HttpResponse> UpwardSyncRequestV2(UpwardRequestsInput upwardRequestsInput);

    }
}
