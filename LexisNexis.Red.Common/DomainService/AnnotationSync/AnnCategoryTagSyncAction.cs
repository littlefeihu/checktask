using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DomainService.AnnotationSync
{
    public class AnnCategoryTagSyncAction : ISyncAction
    {
        ITagDomainService tagDomainService;
        public AnnCategoryTagSyncAction(ITagDomainService tagDomainService)
        {
            this.tagDomainService = tagDomainService;
        }
        public async Task PerformAction(AnnotationTaskContext context)
        {
            var syncResult = await SyncFromRemote(context);
            if (!syncResult)
            {
                syncResult = await SyncFromRemote(context);
            }
        }

        private async Task<bool> SyncFromRemote(AnnotationTaskContext context)
        {
            bool syncResult = false;
            try
            {
                List<AnnotationTag> tags;
                AnnCategoryTags annCategorytag = new AnnCategoryTags();
                string email = context.AnnotationSyncTask.Email;
                string serviceCode = context.AnnotationSyncTask.ServiceCode;
                tagDomainService.LoadTagsFromDb(email, serviceCode, out  annCategorytag, out tags);

                AnnCategoryTagsSyncData syncData = new AnnCategoryTagsSyncData();
                syncData.UserName = email;
                syncData.LSST = tagDomainService.AnnCategorytag.LastServerSyncTime.HasValue ? tagDomainService.AnnCategorytag.LastServerSyncTime.Value : DateTime.Now.AddYears(-20);
                syncData.AnnotationTagsXml = tagDomainService.AnnCategorytag.IsModified ? tagDomainService.ToXmlForSync(tags) : null;
                syncData = await context.SyncService.AnnCategoryTagsSync(syncData);
                if (syncData != null)
                {
                    if (syncData.LSST.HasValue)
                    {
                        tagDomainService.AnnCategorytag.LastServerSyncTime = syncData.LSST.Value;
                    }
                    if (tagDomainService.AnnCategorytag.IsModified)
                    {
                        if (string.IsNullOrEmpty(syncData.AnnotationTagsXml))
                        {//sync success
                            tagDomainService.AnnCategorytag.IsModified = false;
                            syncResult = true;
                        }
                        else
                        { //sync failure since server tags have been updated by other device
                            syncResult = false;
                        }
                    }
                    else
                    {
                        syncResult = true;
                    }
                    if (!string.IsNullOrEmpty(syncData.AnnotationTagsXml))
                    {
                        var serverTags = tagDomainService.GenerateTagsFromTagXML(syncData.AnnotationTagsXml);
                        if (!syncResult)
                        {
                            var exceptTags = serverTags.Except(tags, new TagEqualityComparer());
                            tags.AddRange(exceptTags);
                        }
                        else
                        {
                            tags = serverTags;
                        }
                        tagDomainService.SaveToSqlite(tags, email, serviceCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("PerformSyncTags error:" + ex.ToString());
            }
            return syncResult;
        }


        public SyncActionName SyncActionName
        {
            get
            {
                return SyncActionName.AnnCategoryTagSyncAction;
            }
        }
    }
}
