using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService.AnnotationSync;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService
{
    public class AnnotationSyncService : IAnnotationSyncService
    {
        private ISyncService syncService;
        private IConnectionMonitor connectionMonitor;
        private List<ISyncAction> actions;
        private IAnnotationAccess annotationAccess;
        private IPublicationAccess publicationAccess;
        private readonly Queue<IAnnotationSyncTask> annotationSyncQueue;
        private RedLock syncLock = new RedLock(1);
        private bool isSyncRunning;

        public AnnotationSyncService(ISyncService syncService, IAnnotationAccess annotationAccess, IPublicationAccess publicationAccess, IConnectionMonitor connectionMonitor)
        {
            annotationSyncQueue = new Queue<IAnnotationSyncTask>();
            this.syncService = syncService;
            this.annotationAccess = annotationAccess;
            this.publicationAccess = publicationAccess;
            this.connectionMonitor = connectionMonitor;
            actions = new List<ISyncAction>
            {
                //SyncActionFactory.CreateSyncActionBy(SyncActionName.AnnCategoryTagSyncAction)
                //,
                //SyncActionFactory.CreateSyncActionBy(SyncActionName.AnnotationDownloadAction),
                //SyncActionFactory.CreateSyncActionBy(SyncActionName.AnnotationUploadAction)
            };
        }

        private async Task PerformSync()
        {
            try
            {
                var task = annotationSyncQueue.Dequeue();
                while (task != null)
                {
                    var pingResult = await connectionMonitor.PingService(task.CountryCode);
                    if (pingResult)
                    {
                        AnnotationTaskContext context = new AnnotationTaskContext(task, syncService, annotationAccess);
                        foreach (var action in actions)
                        {
                            //if (task.IsSyncTagsOnly)
                            //{
                            //    if (action.SyncActionName == SyncActionName.AnnCategoryTagSyncAction)
                            //        await action.PerformAction(context);
                            //    break;
                            //}
                            //else
                            //{
                            //    await action.PerformAction(context);
                            //}

                        }
                    }
                    else
                    {
                        break;
                    }
                    task = annotationSyncQueue.Count > 0 ? annotationSyncQueue.Dequeue() : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
            finally
            {
                //End Queue Processor....
                isSyncRunning = false;
            }
        }

        public async void RequestAnnotationSync(IAnnotationSyncTask task)
        {
            if (GlobalAccess.Instance.CurrentUserInfo == null)
                return;
            if (GlobalAccess.Instance.CurrentUserInfo.SyncAnnotations)
            {
                annotationSyncQueue.Enqueue(task);
                try
                {
                    await syncLock.Enter();
                    if (!isSyncRunning && annotationSyncQueue.Count > 0)
                    {
                        //Start Queue Processor....
                        isSyncRunning = true;
                        PerformSync().WithNoWarning();
                    }
                }
                finally
                {
                    syncLock.Release();
                }
            }
        }

        public void RequestAllDlBooksSync()
        {
            if (GlobalAccess.Instance.UserCredential != null)
            {
                var dlBooks = publicationAccess.GetAllDlBooks(GlobalAccess.Instance.UserCredential);
                IAnnotationSyncTask task = new AnnotationSyncTask(dlBooks);
                RequestAnnotationSync(task);
            }
        }

        public void RequestTagsOnly()
        {
            if (GlobalAccess.Instance.CurrentUserInfo == null)
                return;
            RequestAnnotationSync(new AnnotationSyncTask(GlobalAccess.DeviceId,
                                                         GlobalAccess.Instance.CurrentUserInfo));
        }
    }
}
