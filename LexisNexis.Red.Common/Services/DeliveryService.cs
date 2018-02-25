using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Services
{
    public class DeliveryService : ServiceBase, IDeliveryService
    {
        public DeliveryService()
            : base(ServiceConfig.DELIVERY_SERVICE)
        {

        }

        public async Task<HttpResponse> ListFileDetails(UserDetails userDetails)
        {
            return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DL_FILE_DETAILS, userDetails);
        }
        public HttpResponse StartTask(StartTaskRequest request)
        {
            return Task.Run(async () =>
            {
                return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DL_StartTask, request);
            }).Result;
        }

        public HttpResponse EndTask(EndTaskRequest request)
        {
            return Task.Run(async () =>
            {
                return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DL_EndTask, request);
            }).Result;
        }

        public HttpResponse CreateCheckRercord(List<CreateCheckRercordRequest> requests)
        {
            return Task.Run(async () =>
            {
                return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.CreateCheckRercord, requests);
            }).Result;
        }
        public HttpResponse UploadRepair(UploadRepairRequest request)
        {
            return Task.Run(async () =>
            {
                request.ContentStream = await IoCContainer.Instance.Resolve<IDirectory>().OpenFile(request.xmlName, FileModeEnum.Open);

                return await ServiceAgent.RestFullServiceJsonRequest1(base.GetTargetUri(), ServiceConfig.UploadRepair, request.ContentStream);

            }).Result;
        }

        public HttpResponse GetCheckContentByTaskID(GetCheckContentRequest request)
        {
            return Task.Run(async () =>
            {
                return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.GetCheckContentByTaskID, request);
            }).Result;
        }

        public async Task<bool> DlFileDownload(DlBook dlBook, CancellationToken cancellationToken, DownloadProgressEventHandler downloadHandler)
        {
            return await ServiceAgent.RestFullServiceRequestForFileDownload(dlBook, cancellationToken, downloadHandler);
        }

        public async Task DictionaryFileDownload(string fileUrl, string dictionaryPath)
        {
            await ServiceAgent.RestFullServiceRequestForFileDownload(new Uri(fileUrl), dictionaryPath);
        }

        public async Task<HttpResponse> GetDictionaryVersion()
        {
            return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DICTIONARY_VERSION, new Object());
        }

        public async Task<byte[]> DlFileDownload(string fileUrl)
        {
            return await ServiceAgent.RestFullServiceRequestForFileDownload(new Uri(fileUrl));
        }
        public async Task<HttpResponse> DlFileDifferenceVersion(DlVersionDifference dlVersionDifference)
        {
            return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DL_VERSION_DIFFERENCE, dlVersionDifference);
        }

        public async Task<bool> DlFileStatusUpdate(DlFileStatusUpdate dlFileStatusUpdate, CancellationToken token = default(CancellationToken))
        {
            bool updateResult = false;
            try
            {
                var requestResult = await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DL_FILE_STATUS_UPDATE, dlFileStatusUpdate, token);
                if (requestResult.IsSuccess)
                {
                    var item = JsonConvert.DeserializeObject<DlUpdateStatus>(requestResult.Content);
                    updateResult = item.Status;
                }
            }
            catch (Exception)
            {
            }
            return updateResult;
        }

        public async Task<HttpResponse> DlMetadata(DlMetadata dlFileMetadata)
        {
            return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DL_METADATA, dlFileMetadata);
        }

        public async Task<HttpResponse> DlFileCurrentVersion(DlFileCurrentVersionRequest dlFileCurrentVersionRequest)
        {
            return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DL_FILE_CURRENT_VERSION, dlFileCurrentVersionRequest);
        }


        public async Task<HttpResponse> DlVersionChangeHistory(DlVersionChangeHistoryRequest dlVersionChangeHistoryRequest)
        {
            return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DL_VERSION_CHANGE_HISTORY, dlVersionChangeHistoryRequest);

        }

        public async Task<HttpResponse> DlFileDownloadRequestValidation(string email, string deviceId, int dlId)
        {
            DlFileDownloadRequest dlFileDownloadRequest = new DlFileDownloadRequest { UserEmailId = email, DeviceId = deviceId, DlId = dlId };
            return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DL_FILE_DOWNLOAD_VALIDATION, dlFileDownloadRequest);
        }


        public async Task<HttpResponse> DlFileDetailByDlId(string email, string deviceId, int dlId, CancellationToken token)
        {
            DlFileDownloadRequest dlFileDownloadRequest = new DlFileDownloadRequest { UserEmailId = email, DeviceId = deviceId, DlId = dlId };
            return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.DL_FILE_DETAIL_BY_ID, dlFileDownloadRequest, token);
        }
    }
}
