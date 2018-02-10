using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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


        public HttpResponse GetCheckContentByTaskID(GetCheckContentRequest request)
        {
            return Task.Run(async () =>
            {
                return await ServiceAgent.RestFullServiceJsonRequest(base.GetTargetUri(), ServiceConfig.GetCheckContentByTaskID, request);
            }).Result;


            //var id1 = Guid.Parse("9CD8A54E-8EBD-403E-AD14-7DFC200CA038");
            //var id2 = Guid.Parse("9CD8A54E-8EBD-403E-AD14-7DFC200CA039");

            //return new HttpResponse
            //{

            //    IsSuccess = true,
            //    Content = JsonConvert.SerializeObject(new List<CheckContentGroup> {
            //        new CheckContentGroup { Id = id1, CheckName = "消防栓", Data = new List<CheckContentItem> { new CheckContentItem {
            //             ParentId=id1,
            //             CheckContent="消防栓水压是否正常？",
            //             CheckName="消防栓",
            //             CheckPointId=Guid.NewGuid(),
            //             NFCID="111",
            //             CheckPointName="楼道",
            //             CheckTaskId=Guid.NewGuid(),
            //             CheckContentId=Guid.NewGuid()
            //        } } },
            //                  new CheckContentGroup { Id = id2, CheckName = "通道", Data = new List<CheckContentItem> { new CheckContentItem {
            //             ParentId=id2,
            //             CheckContent="消防通道是否畅通？",
            //             CheckName="通道",
            //             CheckPointId=Guid.NewGuid(),
            //             NFCID="111",
            //             CheckPointName="通道",
            //             CheckTaskId=Guid.NewGuid(),
            //             CheckContentId=Guid.NewGuid()
            //        } } }

            //    })
            //};
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
