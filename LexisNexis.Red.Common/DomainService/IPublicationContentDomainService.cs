using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DominService
{
    public interface IPublicationContentDomainService
    {
        void BuildTocNode(TOCNode newNode, TOCNode parentTOC, List<TOCNode> nodeSource);
      
        void SaveRecentHistory(int tocId, string tocTitle, string email, string countryCode);
      
        TOCNode GetTOCByTOCId(int tocId, TOCNode sourceTOCNode);
        TOCNode GetNextTOCNode(int currentTOCID, int tocId, TOCNode sourceTOCNode, bool isNext = true);
    
        IEnumerable<TOCNodeDetail> GetTOCNodeDetails(int tocID, string decryptedDbFullName);
    }
}
