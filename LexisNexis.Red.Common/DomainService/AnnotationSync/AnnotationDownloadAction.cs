using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LexisNexis.Red.Common.DomainService.AnnotationSync
{
    public class AnnotationDownloadAction : ISyncAction
    {
        public SyncActionName SyncActionName
        {
            get
            {
                return SyncActionName.AnnotationDownloadAction;
            }
        }
        public async Task PerformAction(AnnotationTaskContext context)
        {
            if (context.AnnotationSyncTask.BookIds == null || !context.AnnotationSyncTask.BookIds.Any())
            {
                throw new InvalidOperationException("Book Id Shouldn't be empty.");
            }

            DownwardSyncRequests bookSpecificDownwardSyncRequest = new DownwardSyncRequests
            {
                DownwardSyncRequest = new List<DownwardSyncRequest>()
            };

            foreach (var bookId in context.AnnotationSyncTask.BookIds)
            {
                bookSpecificDownwardSyncRequest.DownwardSyncRequest.Add(new DownwardSyncRequest
                {
                    UserID = context.AnnotationSyncTask.Email,
                    DLID = bookId,
                    DLVersionID = context.AnnotationSyncTask.CurrentVersion[bookId],
                    DeviceID = context.AnnotationSyncTask.DeviceId
                });
            }

            await CleanTempFiles(context);
            await context.SyncService.DownwardSyncRequest(bookSpecificDownwardSyncRequest, context.AnnotationDownloadFile);
            await IoCContainer.Instance.ResolveInterface<IPackageFile>().DepressFile(context.AnnotationDownloadFile, context.AnnotationDownloadTempFolder, default(CancellationToken));
            foreach (var bookId in context.AnnotationSyncTask.BookIds)
            {
                string targetDirectory = Path.Combine(context.AnnotationDownloadTempFolder, bookId.ToString());
                if (!await GlobalAccess.DirectoryService.DirectoryExists(targetDirectory))
                    await GlobalAccess.DirectoryService.CreateDirectory(targetDirectory);

                string sourceFile = Path.Combine(context.AnnotationDownloadTempFolder, string.Format("DL_{0}.zip", bookId));

                await IoCContainer.Instance.ResolveInterface<IPackageFile>().DepressFile(sourceFile, targetDirectory, default(CancellationToken));

                if (await GlobalAccess.DirectoryService.FileExists(Path.Combine(targetDirectory, Constants.EmptyStreamXml)))
                {
                    continue;
                }
                foreach (var fileName in await GlobalAccess.DirectoryService.GetFiles(targetDirectory))
                {
                    if (fileName.EndsWith(Constants.SDeletedAnnotationsXml)) //if the annotation had been deleted then delete from local
                    {
                        await DeleteAnnotation(context, fileName);
                    }
                    else if (!fileName.EndsWith(Constants.SRequestxml) && !fileName.EndsWith(Constants.SErrorxml)) //if it is in the request file, then merge with local file.
                    {
                        await MergeAnnotation(context, fileName, bookId);
                    }
                }
            }
            await CleanTempFiles(context);
        }

        private static async Task MergeAnnotation(AnnotationTaskContext context, string strFileName, int bookID)
        {
            XDocument xDocument = null;
            using (var stream = await GlobalAccess.DirectoryService.OpenFile(strFileName, BusinessModel.FileModeEnum.Open))
            {
                xDocument = XDocument.Load(stream);
            }

            if (xDocument != null)
            {
                var annotationCode = Guid.Parse(xDocument.Root.Attribute(Constants.SId).Value);
                var localAnnotations = context.AnnotationAccess.getAnnotation(annotationCode);
                if (localAnnotations != null)
                {
                    var annotation = AnnotationFactory.CreateAnnotation(localAnnotations.AnnotationContent);
                    var localDateTime = annotation.UpdatedTime;
                    var serverDateTime = DateTime.Parse(xDocument.Root.Attribute(XName.Get(Constants.SUpdatedOn)).Value);

                    var dateDiff = DateTime.Compare(localDateTime, serverDateTime);
                    if (dateDiff > 0) //local version is newer than the server version
                    {
                        return;
                    }
                    localAnnotations.AnnotationContent = xDocument.Root.ToString();
                    localAnnotations.IsSynced = true;
                    localAnnotations.AnnotationType = int.Parse(xDocument.Root.Attribute(Constants.SType).Value);
                    localAnnotations.LastSyncDate = DateTime.Now;
                    var statusString = xDocument.Root.Attribute(Constants.Status).Value;
                    AnnotationStatusEnum annoStatus = (AnnotationStatusEnum)Enum.Parse(typeof(AnnotationStatusEnum), statusString[0].ToString().ToUpper() + statusString.Substring(1));

                    localAnnotations.Status = (int)annoStatus;
                    localAnnotations.DocumentID = xDocument.Root.Attribute(Constants.DocId).Value;

                    context.AnnotationAccess.UpdateAnnotation(localAnnotations);
                }
                else
                {
                    Annotations annotations = new Annotations
                    {
                        Email = context.AnnotationSyncTask.Email,
                        ServiceCode = context.AnnotationSyncTask.ServiceCode,
                        AnnotationCode = annotationCode,
                        BookID = bookID,
                        IsSynced = true,
                        CreatedDate = DateTime.Now,
                        LastModifiedDate = DateTime.Now,
                        LastSyncDate = DateTime.Now
                    };
                    annotations.DocumentID = xDocument.Root.Attribute(Constants.DocId).Value;
                    annotations.AnnotationType = int.Parse(xDocument.Root.Attribute(Constants.SType).Value);

                    var statusString = xDocument.Root.Attribute(Constants.Status).Value;
                    AnnotationStatusEnum annoStatus = (AnnotationStatusEnum)Enum.Parse(typeof(AnnotationStatusEnum), statusString[0].ToString().ToUpper() + statusString.Substring(1));

                    annotations.Status = (int)annoStatus;
                    annotations.AnnotationContent = xDocument.Root.ToString();
                    context.AnnotationAccess.AddAnnotation(annotations);
                }
            }
        }
        private static async Task DeleteAnnotation(AnnotationTaskContext context, string strFileName)
        {
            using (var stream = await GlobalAccess.DirectoryService.OpenFile(strFileName, BusinessModel.FileModeEnum.Open))
            {
                var responseElement = XDocument.Load(strFileName).Root;

                foreach (var idElement in responseElement.Descendants(Constants.SId))
                {
                    var annotationCode = Guid.Parse(idElement.Value);
                    context.AnnotationAccess.DeleteAnnotation(annotationCode);
                }
            }
        }
        private static async Task CleanTempFiles(AnnotationTaskContext context)
        {
            try
            {
                if (await GlobalAccess.DirectoryService.FileExists(context.AnnotationDownloadFile))
                {
                    await GlobalAccess.DirectoryService.DeleteFile(context.AnnotationDownloadFile);
                }
                if (await GlobalAccess.DirectoryService.DirectoryExists(context.AnnotationDownloadTempFolder))
                {
                    await GlobalAccess.DirectoryService.DeleteDirectory(context.AnnotationDownloadTempFolder);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }
    }
}
