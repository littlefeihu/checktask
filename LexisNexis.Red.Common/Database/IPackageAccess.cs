using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public interface IPackageAccess
    {
        List<TOCNode> GetTOC(string dbFileName);
        List<TOCNode> GetTOCNodeByRole(string dbFileName, bool IsAncestor);
        TOCNode GetTOCByTocId(string dbFileName, int tocId);
        List<RemoteLink> GetRemoteLink(string dbFileName);
        RemoteLink GetRemoteLinkByLinkID(string dbFileName, string refptLink);
        TOCNodeDetail GetTOCDetailByRemoteLinkID(string dbFileName, string remoteLinkID);
        TOCNodeDetail GetTOCDetailByFilename(string dbFileName, string fileName);
        TOCNodeDetail GetTOCDetailByDocId(string dbFileName, string docId);
        List<TOCNodeDetail> GetTOCDetailByTOCId(string dbFileName, int tocId);
        TOCNodeDetail GetFirstTOCDetail(string dbFileName);
        List<Index> GetIndexs(string dbFileName);
        List<TOCNodeDetail> GetAllTOCNodeDetails(string dbFileName);
        bool HasPage(string dbFileName);
        List<PageItem> GetPageList(string dbFileName);
        List<ContentLinkDetail> GetAllTOCDetail(string dbFileName);
        string GetXpathBySpanId(int spanId, string docId, string dbFilename);
        int GetSpanIdByXpath(string xpath, string docId, string dbFileName);
    }
}
