using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DominService
{
    public interface IDictionaryDomainService
    {
        Task DownloadDictionary(string fileUrl, string dictionaryPath);

        Task<DictionaryVersionResponse> GetDictionaryVersion();
    }
}
