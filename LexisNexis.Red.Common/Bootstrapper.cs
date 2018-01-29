using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService;
using LexisNexis.Red.Common.DomainService.AnnotationSync;
using LexisNexis.Red.Common.DominService;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common
{
    public class Bootstrapper
    {
        public static void Init()
        {
            #region DatabaseAccess
            IoCContainer.Instance.RegisterInterface<IPublicationAccess, PublicationAccess>();
            IoCContainer.Instance.RegisterInterface<IPackageAccess, PackageAccess>();
            IoCContainer.Instance.RegisterInterface<IRecentHistoryAccess, RecentHistoryAccess>();
            IoCContainer.Instance.RegisterInterface<IAnnotationAccess, AnnotationAccess>();
            IoCContainer.Instance.RegisterInterface<IAnnCategoryTagsAccess, AnnCategoryTagsAccess>();
            IoCContainer.Instance.RegisterInterface<IDictionaryAccess, DictionaryAccess>();
            IoCContainer.Instance.RegisterInterface<IUserAccess, UserAccess>();
            IoCContainer.Instance.RegisterInterface<ISettingsAccess, SettingsAccess>();

            #endregion

            #region Network
            IoCContainer.Instance.RegisterInterface<IConnectionMonitor, ConnectionMonitor>();
            #endregion

            #region Adapter
            IoCContainer.Instance.RegisterInterface<IAuthenticationService, AuthenticationService>();
            IoCContainer.Instance.RegisterInterface<IDeliveryService, DeliveryService>();
            IoCContainer.Instance.RegisterInterface<ISyncService, SyncService>();
            #endregion

            #region EventHandles
            EventHandlesMapping.HandlesMapping();
            #endregion

            #region DomainService
            IoCContainer.Instance.RegisterInterface<IOrphanItemsService, OrphanItemsService>();
            IoCContainer.Instance.RegisterInterface<ILoginDomainService, LoginDomainService>();
            IoCContainer.Instance.RegisterInterface<IPublicationContentDomainService, PublicationContentDomainService>();
            IoCContainer.Instance.RegisterInterface<IPublicationDomainService, PublicationDomainService>();
            IoCContainer.Instance.RegisterInterface<IDictionaryDomainService, DictionaryDomainService>();
            IoCContainer.Instance.RegisterInterface<ITagDomainService, TagDomainService>();

            IoCContainer.Instance.RegisterInterface<ISyncAction, AnnCategoryTagSyncAction>(SyncActionName.AnnCategoryTagSyncAction.ToString());
            IoCContainer.Instance.RegisterInterface<ISyncAction, AnnotationDownloadAction>(SyncActionName.AnnotationDownloadAction.ToString());
            IoCContainer.Instance.RegisterInterface<ISyncAction, AnnotationUploadAction>(SyncActionName.AnnotationUploadAction.ToString());
            IoCContainer.Instance.RegisterInterface<IAnnotationSyncService, AnnotationSyncService>();

            #endregion

            #region AppService
            IoCContainer.Instance.RegisterInstance<LoginUtil>(LoginUtil.Instance);
            IoCContainer.Instance.RegisterInstance<AnnotationUtil>(AnnotationUtil.Instance);
            IoCContainer.Instance.RegisterInstance<PageSearchUtil>(PageSearchUtil.Instance);
            IoCContainer.Instance.RegisterInstance<PublicationContentUtil>(PublicationContentUtil.Instance);
            IoCContainer.Instance.RegisterInstance<PublicationUtil>(PublicationUtil.Instance);
            IoCContainer.Instance.RegisterInstance<SettingsUtil>(SettingsUtil.Instance);
            IoCContainer.Instance.RegisterInstance<AnnCategoryTagUtil>(AnnCategoryTagUtil.Instance);

            #endregion
        }
    }
}
