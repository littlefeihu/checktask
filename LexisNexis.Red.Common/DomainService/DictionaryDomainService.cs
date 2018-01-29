using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DominService
{
    public class DictionaryDomainService : IDictionaryDomainService
    {
        IDeliveryService deliveryService;
        public DictionaryDomainService(IDeliveryService deliveryService)
        {
            this.deliveryService = deliveryService;
        }
        public async Task DownloadDictionary(string fileUrl, string dictionaryPath)
        {
            await deliveryService.DictionaryFileDownload(fileUrl, dictionaryPath);
        }

        public async Task<DictionaryVersionResponse> GetDictionaryVersion()
        {
            DictionaryVersionResponse version = new DictionaryVersionResponse();
            var dictionaryResponse = await deliveryService.GetDictionaryVersion();
            if (dictionaryResponse.IsSuccess)
            {
                version = JsonConvert.DeserializeObject<DictionaryVersionResponse>(dictionaryResponse.Content);
            }
            return version;
        }
    }
}
