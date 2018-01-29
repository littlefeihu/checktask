using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.AnnotationSync
{
    public class AnnotationTaskContext
    {
        public AnnotationTaskContext(IAnnotationSyncTask annotationSyncTask, ISyncService syncService, IAnnotationAccess annotationAccess)
        {
            this.AnnotationSyncTask = annotationSyncTask;
            this.SyncService = syncService;
            this.AnnotationAccess = annotationAccess;
            var uerFolder = GlobalAccess.Instance.CurrentUserInfo.UserFolder;
            this.AnnotationDownloadFile = Path.Combine(uerFolder, Constants.AnnotationDownZip);
            this.AnnotationDownloadTempFolder = Path.Combine(uerFolder, Constants.AnnotationDown);
        }

        public IAnnotationSyncTask AnnotationSyncTask
        {
            get;
            private set;
        }

        public ISyncService SyncService
        {
            get;
            private set;
        }
        public IAnnotationAccess AnnotationAccess
        {
            get;
            private set;
        }
        public string AnnotationDownloadTempFolder { get; private set; }

        public string AnnotationDownloadFile { get; private set; }
    }
}
