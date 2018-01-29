using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LexisNexis.Red.Common.Services
{
    public class SyncService : ServiceBase, ISyncService
    {
        public SyncService()
            : base(ServiceConfig.SYNC_SERVICE)
        {

        }
        public async Task<bool> DownwardSyncRequest(DownwardSyncRequests downwardSyncRequest, string fileName)
        {
            return await ServiceAgent.RestFullServiceRequestForAnnDownwardSync(base.GetTargetUri(), ServiceConfig.AnnDownwardSyncV2, fileName, downwardSyncRequest);
        }

        public async Task<HttpResponse> UpwardSyncRequest(string strFile)
        {
            return await ServiceAgent.RestFullServiceRequestForAnnUpwardSync(base.GetTargetUri(), ServiceConfig.ANN_UPWARD_SYNC, strFile);
        }
        public async Task<HttpResponse> UpwardSyncRequestV2(UpwardRequestsInput upwardRequestsInput)
        {
            return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.ANN_UPWARD_SYNCV2, upwardRequestsInput);
        }
        public async Task<AnnCategoryTagsSyncData> AnnCategoryTagsSync(AnnCategoryTagsSyncData data)
        {
            var httpResponse = await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.ANN_CATEGORYTAGS_SYNC, data);
            if (httpResponse.IsSuccess)
                return JsonConvert.DeserializeObject<AnnCategoryTagsSyncData>(httpResponse.Content);
            else
                return null;
        }
    }
}
