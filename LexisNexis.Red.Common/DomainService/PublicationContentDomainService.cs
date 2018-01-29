using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.DominService
{
    public class PublicationContentDomainService : IPublicationContentDomainService
    {
        IPublicationAccess publicationAccess;
        IRecentHistoryAccess recentHistoryAccess;
        IPackageAccess packageAccess;
        static RedLock nextNodeLock = new RedLock(1);
        List<int> list = new List<int>();
        public PublicationContentDomainService(IPublicationAccess publicationAccess, IRecentHistoryAccess recentHistoryAccess, IPackageAccess packageAccess)
        {
            this.publicationAccess = publicationAccess;
            this.recentHistoryAccess = recentHistoryAccess;
            this.packageAccess = packageAccess;
        }
        public void BuildTocNode(TOCNode newNode, TOCNode parentTOC, List<TOCNode> nodeSource)
        {
            newNode.ParentNode = parentTOC;
            newNode.ChildNodes = nodeSource.FindAll(o => o.ParentId == newNode.ID);
            if (newNode.HasChildNodes())
            {
                nodeSource.RemoveAll(o => o.ParentId == newNode.ID);
                foreach (var item in newNode.ChildNodes)
                {
                    BuildTocNode(item, newNode, nodeSource);
                }
            }
        }

        public void SaveRecentHistory(int tocId, string tocTitle, string email, string serviceCode)
        {
            var publication = GlobalAccess.Instance.CurrentPublication.DlBook;
            var tocDetail = packageAccess.GetTOCDetailByTOCId(publication.GetDecryptedDbFullName(), tocId);
            //RecentHistory history = new RecentHistory
            //{
            //    BookId = publication.BookId,
            //    DocID = tocDetail != null ? string.Join(",", tocDetail.Select(o => o.DocID)) : string.Empty,
            //    LastReadDate = DateTime.Now,
            //    TOCTitle = tocTitle,
            //    ColorPrimary = publication.ColorPrimary,
            //    ServiceCode = serviceCode,
            //    Email = email,
            //    DlBookTitle = publication.Name
            //};
            //recentHistoryAccess.UpdateRecentHistory(history);
        }

        public TOCNode GetTOCByTOCId(int tocId, TOCNode rootTOCNode)
        {
            if (rootTOCNode != null)
            {
                var isRootNode = (tocId == 0);
                if (isRootNode)
                {
                    return rootTOCNode;
                }
                if (rootTOCNode.HasChildNodes())
                {
                    foreach (var item in rootTOCNode.ChildNodes)
                    {
                        if (item.ID == tocId)
                            return item;
                        else
                        {
                            rootTOCNode = GetTOCByTOCId(tocId, item);
                            if (rootTOCNode != null)
                                return rootTOCNode;
                        }
                    }
                }
            }
            return null;
        }

        public TOCNode GetNextTOCNode(int currentTocId, int targetTocId, TOCNode sourceTOCNode, bool isNext = true)
        {
            try
            {
                nextNodeLock.Enter().Wait();
                list.Clear();
                return GetTargetTOCNode(currentTocId, targetTocId, sourceTOCNode, isNext);
            }
            finally
            {
                list.Clear();
                nextNodeLock.Release();
            }
        }

        private TOCNode GetTargetTOCNode(int currentTocId, int targetTocId, TOCNode sourceTOCNode, bool isNext = true)
        {
            var tocNode = GetTOCByTOCId(targetTocId, sourceTOCNode);
            if (tocNode != null)
            {
                var isLeafNode = (currentTocId != targetTocId && tocNode.IsLeafNode());
                if (isLeafNode)
                {
                    return tocNode;
                }
                var noSearchFromChildNodes = (list.FirstOrDefault(o => o == tocNode.ID) == 0);
                if (tocNode.HasChildNodes() && noSearchFromChildNodes)
                {
                    list.Add(tocNode.ID);
                    var child = isNext ? tocNode.ChildNodes[0] : tocNode.ChildNodes[tocNode.ChildNodes.Count - 1];
                    return GetTargetTOCNode(currentTocId, child.ID, sourceTOCNode, isNext);
                }
                else
                {
                    TOCNode sibling;
                    if (tocNode.HasSibling(isNext, out sibling))
                    {
                        return GetTargetTOCNode(currentTocId, sibling.ID, sourceTOCNode, isNext);
                    }
                    else
                    {
                        if (tocNode.IsTopLevel())
                        {// when come to last or first node with node level equal to 1 ,return null

                            return null;
                        }
                        if (tocNode.HasParentNode())
                        {
                            list.Add(tocNode.ParentNode.ID);
                            return GetTargetTOCNode(currentTocId, tocNode.ParentNode.ID, sourceTOCNode, isNext);
                        }
                    }
                }
            }
            return null;
        }
        public IEnumerable<TOCNodeDetail> GetTOCNodeDetails(int tocID, string decryptedDbFullName)
        {
            var tocDetails = packageAccess.GetTOCDetailByTOCId(decryptedDbFullName, tocID);
            return tocDetails;
        }


    }
}
