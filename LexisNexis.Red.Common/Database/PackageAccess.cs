using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Database
{
    public class PackageAccess : DbHelper, IPackageAccess
    {
        private const string queryTOC = @"SELECT ID, ParentId, Title, Role, GuideCardTitle,SortOrder FROM tblTOC order by SortOrder";
        private const string queryTOCById = @"SELECT ID, ParentId, Title, Role, GuideCardTitle,SortOrder FROM tblTOC where ID=? ";
        private const string queryTOCByRole = @"SELECT ID, ParentId, Title, Role, GuideCardTitle,SortOrder FROM tblTOC WHERE role=?  order by SortOrder";
        private const string queryRemoteLink = @"SELECT id as ID,remotelinkid as RemoteLinkID,offset as Offset,refpttargetfile as TargetFile FROM tblremotelinktracer AS RemoteLink";
        private const string queryTOCDetail = @"SELECT ID , Filename AS FileName, DocID FROM  tblTocDetails AS TOCNodeDetail WHERE ID=?";
        private const string queryIndexs = @"SELECT  title as Title,filename AS FileName  FROM  tblindexfiles";
        private const string queryTOCDetailByFileName = @"SELECT ID , Filename AS FileName, DocID FROM  tblTocDetails AS TOCNodeDetail WHERE Filename=?  LIMIT 0,1";
        private const string queryAllTOCDetails = @"SELECT  ID , Filename AS FileName, DocID FROM  tblTocDetails AS TOCNodeDetail ";
        private const string hasTableTblSearchLookup = "SELECT COUNT(*) FROM sqlite_master WHERE name = 'tblPageSearch' AND type = 'table'";
        private const string queryPageData = @"SELECT a.*,b.id as TOCID from tblPageSearch as a left join tblTocDetails as b on a.[DocID]=b.[DocID] ";
        private const string queryTOCDetailByDocId = @"SELECT ID , Filename AS FileName, DocID FROM  tblTocDetails AS TOCNodeDetail WHERE DocID=?  LIMIT 0,1";
        private const string queryXpathBySpanId = @"SELECT spanId as SpanId,xmlXPath as Xpath FROM  tblTempXmlXpath  WHERE DocID=? AND spanId=? LIMIT 0,1";
        private const string querySpanIdByXpath = @"SELECT spanId as SpanId,xmlXPath as Xpath FROM  tblTempXmlXpath  WHERE DocID=? AND xpath=? LIMIT 0,1";
        private const string queryFirstTOCDetail = @"SELECT ID , Filename AS FileName, DocID FROM  tblTocDetails AS TOCNodeDetail LIMIT 0,1";
        private const string getRemoteLinkByLinkId = "SELECT id as ID,remotelinkid as RemoteLinkID,offset as Offset,refpttargetfile as TargetFile FROM tblremotelinktracer WHERE remotelinkid=?   LIMIT 0,1";
        private const string getTOCDetailByRemoteLinkID = "SELECT b.* FROM tblremotelinktracer a inner join tblTocDetails b on a.refpttargetfile = b.Filename WHERE a.remotelinkid=?   LIMIT 0,1";
        private const string getAllTOCDetail = "SELECT a.remotelinkid, b.* FROM tblremotelinktracer a inner join tblTocDetails b on a.refpttargetfile = b.Filename";
        public List<TOCNode> GetTOC(string dbFileName)
        {
            return base.GetEntityList<TOCNode>(dbFileName, queryTOC);
        }
        public List<TOCNode> GetTOCNodeByRole(string dbFileName, bool IsAncestor)
        {
            return base.GetEntityList<TOCNode>(dbFileName, queryTOCByRole, (IsAncestor ? "ancestor" : "me"));
        }

        public RemoteLink GetRemoteLinkByLinkID(string dbFileName, string refptLink)
        {
            return base.GetEntity<RemoteLink>(dbFileName, getRemoteLinkByLinkId, refptLink);
        }

        public List<RemoteLink> GetRemoteLink(string dbFileName)
        {
            return base.GetEntityList<RemoteLink>(dbFileName, queryRemoteLink);
        }
        public TOCNodeDetail GetTOCDetailByRemoteLinkID(string dbFileName, string remoteLinkID)
        {
            return base.GetEntity<TOCNodeDetail>(dbFileName, getTOCDetailByRemoteLinkID, remoteLinkID);
        }
        public TOCNodeDetail GetTOCDetailByFilename(string dbFileName, string fileName)
        {
            return base.GetEntity<TOCNodeDetail>(dbFileName, queryTOCDetailByFileName, fileName);
        }
        public TOCNodeDetail GetTOCDetailByDocId(string dbFileName, string docId)
        {
            return base.GetEntityList<TOCNodeDetail>(dbFileName, queryTOCDetailByDocId, docId).FirstOrDefault();
        }
        public List<TOCNodeDetail> GetTOCDetailByTOCId(string dbFileName, int tocId)
        {
            return base.GetEntityList<TOCNodeDetail>(dbFileName, queryTOCDetail, tocId);
        }
        public TOCNodeDetail GetFirstTOCDetail(string dbFileName)
        {
            return base.GetEntityList<TOCNodeDetail>(dbFileName, queryFirstTOCDetail).FirstOrDefault();
        }
        public List<TOCNodeDetail> GetAllTOCNodeDetails(string dbFileName)
        {
            return base.GetEntityList<TOCNodeDetail>(dbFileName, queryAllTOCDetails);
        }
        public List<Index> GetIndexs(string dbFileName)
        {
            return base.GetEntityList<Index>(dbFileName, queryIndexs);
        }


        public TOCNode GetTOCByTocId(string dbFileName, int tocId)
        {
            return base.GetEntity<TOCNode>(dbFileName, queryTOCById, tocId);
        }
        public bool HasPage(string dbFileName)
        {
            return base.ExecuteScalar(dbFileName, hasTableTblSearchLookup) > 0;
        }
        public List<PageItem> GetPageList(string dbFileName)
        {
            return base.GetEntityList<PageItem>(dbFileName, queryPageData);
        }

        public List<ContentLinkDetail> GetAllTOCDetail(string dbFileName)
        {
            return base.GetEntityList<ContentLinkDetail>(dbFileName, getAllTOCDetail);
        }

        public string GetXpathBySpanId(int spanId, string docId, string dbFilename)
        {
            var xpathItem = base.GetEntity<XpathItem>(dbFilename, queryXpathBySpanId, docId, spanId);
            if (xpathItem != null)
            {
                return xpathItem.Xpath;
            }
            return null;
        }

        public int GetSpanIdByXpath(string xpath, string docId, string dbFileName)
        {
            var xpathItem = base.GetEntity<XpathItem>(dbFileName, querySpanIdByXpath, docId, xpath);
            if (xpathItem != null)
            {
                return xpathItem.SpanId;
            }
            return -1;
        }
    }
}
