using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService.AnnotationSync;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.EventHandles
{
    public class LoginSuccessHandle : IEventHandler<LoginSuccessEvent>
    {
        IAnnotationSyncService annotationSyncService;
        IConnectionMonitor connectionMonitor;
        ITagDomainService annCategoryTagDomainService;
        public LoginSuccessHandle(IAnnotationSyncService annotationSyncService, IConnectionMonitor connectionMonitor, ITagDomainService annCategoryTagDomainService)
        {
            this.annotationSyncService = annotationSyncService;
            this.connectionMonitor = connectionMonitor;
            this.annCategoryTagDomainService = annCategoryTagDomainService;
        }
        public async Task Handle(LoginSuccessEvent evt)
        {
            await InitUserDirectory(evt.LoginUserDetails.Country.ServiceCode, evt.LoginUserDetails.Email);
            //Task.Run(async () =>
            //{
            //    if (await connectionMonitor.PingService(evt.LoginUserDetails.Country.CountryCode))
            //    {
            //        DictionaryUtil.UpdateDictionary("AU").ContinueWith((t) =>
            //        {
            //            DictionaryUtil.UpdateDictionary("NZ");
            //        }).ContinueWith((t) =>
            //        {
            //            annotationSyncService.RequestAllDlBooksSync();
            //        }).WithNoWarning();
            //    }
            //}).WithNoWarning();
            this.annCategoryTagDomainService.RefreshInstance(evt.LoginUserDetails.Email, evt.LoginUserDetails.Country.ServiceCode);
        }
        private async Task InitUserDirectory(string serviceCode, string email)
        {
            string localPath = Path.Combine(serviceCode, email);
            if (!await GlobalAccess.DirectoryService.DirectoryExists(localPath))
                await GlobalAccess.DirectoryService.CreateDirectory(localPath);
        }
    }
}
