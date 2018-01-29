using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.AnnotationSync
{
    public class AnnotationUploadAction : ISyncAction
    {

        List<Annotations> allUnSyncedAnnotations = new List<Annotations>();
        public SyncActionName SyncActionName
        {
            get
            {
                return SyncActionName.AnnotationUploadAction;
            }
        }
        public async Task PerformAction(AnnotationTaskContext context)
        {
            if (context.AnnotationSyncTask.BookIds == null || !context.AnnotationSyncTask.BookIds.Any())
            {
                throw new InvalidOperationException("Book Id Shouldn't be empty.");
            }
            List<UpwardRequests> requests = new List<UpwardRequests>();
            foreach (var bookId in context.AnnotationSyncTask.BookIds)
            {
                var unSyncedAnnotations = context.AnnotationAccess.GetUnSyncedAnnotations(context.AnnotationSyncTask.Email, context.AnnotationSyncTask.ServiceCode, bookId);
                if (unSyncedAnnotations.Count == 0)
                    continue;
                allUnSyncedAnnotations.AddRange(unSyncedAnnotations);
                requests.Add(BuildRequest(context, bookId, unSyncedAnnotations));
            }
            if (requests.Count > 0)
            {
                UpwardRequestsInput upwardRequestsInput = new UpwardRequestsInput();
                upwardRequestsInput.UpwardRequests = requests;
                var upwardResult = await context.SyncService.UpwardSyncRequestV2(upwardRequestsInput);
                if (upwardResult.IsSuccess)
                {
                    UpwardProcess(context, upwardResult.Content);
                }
            }
        }
        private void UpwardProcess(AnnotationTaskContext context, string content)
        {
            var upwardReponse = JsonConvert.DeserializeObject<UpwardSyncResponse>(content);
            foreach (var item in upwardReponse.SyncStatus)
            {
                if (!item.Status)
                {
                    continue;
                }
                var unSyncedAnnotations = allUnSyncedAnnotations.Where(o => o.BookID == item.DlId);
                foreach (var unSyncedAnnotation in unSyncedAnnotations)
                {
                    if (unSyncedAnnotation.Status == (int)AnnotationStatusEnum.Deleted)
                    {
                        context.AnnotationAccess.DeleteAnnotation(unSyncedAnnotation.AnnotationCode);
                    }
                    else
                    {
                        unSyncedAnnotation.IsSynced = true;
                        unSyncedAnnotation.LastSyncDate = DateTime.Now;
                        context.AnnotationAccess.UpdateAnnotation(unSyncedAnnotation);
                    }
                }
            }
        }
        private UpwardRequests BuildRequest(AnnotationTaskContext context, int bookId, List<Annotations> unSyncedAnnotations)
        {
            UpwardRequests upwardRequest = new UpwardRequests
            {
                DeviceID = context.AnnotationSyncTask.DeviceId,
                DlCurrentVersion = context.AnnotationSyncTask.CurrentVersion[bookId],
                Email = context.AnnotationSyncTask.Email,
                DlId = bookId
            };
            upwardRequest.UpwardSyncRequests = new List<UpwardSyncRequestEntity>();

            foreach (var unSyncedAnnotation in unSyncedAnnotations)
            {
                if (unSyncedAnnotation == null)
                    continue;
                UpwardSyncRequestEntity entity = new UpwardSyncRequestEntity
                {
                    Annotation = unSyncedAnnotation.AnnotationContent,
                    AnnotationID = unSyncedAnnotation.AnnotationCode.ToString(),
                    Status = ((AnnotationStatusEnum)unSyncedAnnotation.Status).ToString().ToLower()
                };
                upwardRequest.UpwardSyncRequests.Add(entity);
            }
            return upwardRequest;
        }
    }
}
