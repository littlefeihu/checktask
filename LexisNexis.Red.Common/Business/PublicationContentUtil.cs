using HtmlAgilityPack;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DomainService.Events;
using LexisNexis.Red.Common.DominService;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.HelpClass.DomainEvent;
using LexisNexis.Red.Common.HelpClass.Tools;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Business
{
    public class PublicationContentUtil : AppServiceBase<PublicationContentUtil>
    {
        private const string TOCID = "tocId";
        private const string CSS_NONE_DISPLAY_KEY = "style";
        private const string CSS_NONE_DISPLAY_VALUE = "display:none";
        private const string CSS_CLASS_KEY = "class";
        private const string CSS_INTERBOOK_CLASS = "interbook";
        private const string CSS_EXTERNAL_CLASS = "external";

        private const string CSS_CLASS = "class";
        private const string CSS_PLAIN_TEXT = "plaintexthyperlink ";
        private const string CSS_HIDDEN_HYPER_LINK = "hiddenhyperlink ";
        IPublicationAccess publicationAccess;
        IPackageAccess packageAccess;
        IRecentHistoryAccess recentHistoryAccess;
        IPublicationContentDomainService publicationContentService;
        private static Dictionary<string, TOCNodeDetail> interBookDictionary = new Dictionary<string, TOCNodeDetail>();
        private static Dictionary<string, ContentLinkDetail> intraBookDictionary = new Dictionary<string, ContentLinkDetail>();
        private static DlBook currentBook = null;
        private static Dictionary<string, DlBook> intraBookLinkDictionary = new Dictionary<string, DlBook>();
        private static Dictionary<string, int> targetBookIdDictionary = new Dictionary<string, int>();
        private static List<ContentLinkDetail> allContentLinkDetailList = new List<ContentLinkDetail>();
        public PublicationContentUtil(IPublicationAccess publicationAccess, IPackageAccess packageAccess, IRecentHistoryAccess recentHistoryAccess)
        {
            this.publicationAccess = publicationAccess;
            this.packageAccess = packageAccess;
            this.recentHistoryAccess = recentHistoryAccess;
            this.publicationContentService = new PublicationContentDomainService(publicationAccess, recentHistoryAccess, packageAccess);
        }

        /// <summary>
        /// get root node ,getfirstpage by add namespace LexisNexis.Red.Common.HelpClass
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public Task<TOCNode> GetTOCByBookId(int bookId)
        {
            return Task.Run(async () =>
            {
                await RefreshCurrentPublication(bookId);
                var tocNodes = packageAccess.GetTOC(GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName);
                TOCNode root = new TOCNode();
                root.Role = Constants.ANCESTOR;
                root.Title = "Table of Content";
                root.ChildNodes = tocNodes.FindAll(o => o.ParentId == 0);
                tocNodes.RemoveAll(o => o.ParentId == 0);
                if (root.ChildNodes != null)
                {
                    foreach (TOCNode node in root.ChildNodes)
                    {
                        publicationContentService.BuildTocNode(node, root, tocNodes);
                    }
                }
                return root;
            });
        }

        public Task<List<TOCNode>> GetTOCListByBookId(int bookId)
        {
            return Task.Run(async () =>
           {
               await RefreshCurrentPublication(bookId);
               var tocNodes = packageAccess.GetTOC(GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName);
               foreach (var tocNode in tocNodes)
               {
                   tocNode.ParentNode = tocNodes.FirstOrDefault(o => o.ID == tocNode.ParentId);
                   tocNode.ChildNodes = tocNodes.FindAll(o => o.ParentId == tocNode.ID);
               }
               return tocNodes;
           });
        }
        public TOCNode GetTOCByTOCId(int bookId, int tocId)
        {
            RefreshCurrentPublication(bookId).Wait();
            return packageAccess.GetTOCByTocId(GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName, tocId);
        }
        /// <summary>
        /// get toc node by tocid and a full node tree from GetTOCByBookId
        /// </summary>
        /// <param name="tocId">tocId</param>
        /// <param name="rootTOCNode">root node  from GetTOCByBookId</param>
        /// <returns></returns>
        public TOCNode GetTOCByTOCId(int tocId, TOCNode rootTOCNode)
        {
            if (rootTOCNode != null)
            {
                var node = rootTOCNode.GetRootTOCNode();
                if (node != null)
                {
                    return publicationContentService.GetTOCByTOCId(tocId, node);
                }
            }
            return null;
        }
        /// <summary>
        /// Get First And Last  TOCNode
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns>item1:firstNode,item2:lastnode</returns>
        public Tuple<TOCNode, TOCNode> GetFirstAndLastTOCNode(TOCNode treeNode)
        {
            if (treeNode != null)
            {
                var node = treeNode.GetRootTOCNode();
                if (node != null)
                {
                    return new Tuple<TOCNode, TOCNode>(node.GetFirstPage(), node.GetLastPage());
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentTOCNode">full tree node</param>
        /// <returns>null:have come to the last page</returns>
        public TOCNode GetNextPageByTreeNode(TOCNode currentTOCNode)
        {
            if (currentTOCNode != null)
            {
                var rootNode = currentTOCNode.GetRootTOCNode();
                if (rootNode != null)
                {
                    return publicationContentService.GetNextTOCNode(currentTOCNode.ID, currentTOCNode.ID, rootNode, true);
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentTOCNode">full tree node</param>
        /// <returns>null:have come to the first page</returns>
        public TOCNode GetPreviousPageByTreeNode(TOCNode currentTOCNode)
        {
            if (currentTOCNode != null)
            {
                currentTOCNode = currentTOCNode.GetFirstPage();
                if (currentTOCNode != null)
                {
                    var rootNode = currentTOCNode.GetRootTOCNode();
                    if (rootNode != null)
                    {
                        return publicationContentService.GetNextTOCNode(currentTOCNode.ID, currentTOCNode.ID, rootNode, false);
                    }
                }
            }
            return null;
        }
        public async Task<List<Index>> GetIndexList(int bookId)
        {
            await RefreshCurrentPublication(bookId);
            var indexs = packageAccess.GetIndexs(GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName);
            if (indexs != null && indexs.Count > 0)
            {
                foreach (var index in indexs)
                {
                    index.TolowerCase();
                }
                return indexs;
            }
            return null;
        }

        private Task RefreshCurrentPublication(int bookId)
        {
            return DomainEvents.Publish(new PublicationOpeningEvent(bookId, false));
        }

        public async Task<Dictionary<string, List<Index>>> GetIndexsByBookId(int bookId)
        {
            await RefreshCurrentPublication(bookId);
            var indexs = packageAccess.GetIndexs(GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName);
            if (indexs != null && indexs.Count > 0)
            {
                Dictionary<string, List<Index>> indexDic = new Dictionary<string, List<Index>>();
                var queryIndexLetter = from index in indexs
                                       group index by index.Title[0] into letter
                                       select letter.Key;
                foreach (var letter in queryIndexLetter)
                {
                    var filterIndexs = indexs.FindAll(o => o.Title.StartsWith(letter.ToString()));
                    foreach (var item in filterIndexs)
                    {
                        item.TolowerCase();
                    }
                    indexDic.Add(letter.ToString(), filterIndexs);
                }
                return indexDic;
            }

            return null;
        }

        public async Task<string> GetContentFromIndex(int bookId, Index index, bool renderHyperlink = true)
        {
            await RefreshCurrentPublication(bookId);
            StringBuilder builder = new StringBuilder();
            try
            {
                if (index != null && !string.IsNullOrEmpty(index.Title))
                {
                    var letter = index.Title[0];
                    var indexs = packageAccess.GetIndexs(GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName);
                    if (indexs != null && indexs.Count > 0)
                    {
                        var filterIndexs = indexs.FindAll(o => o.Title.StartsWith(letter.ToString()));
                        if (filterIndexs != null)
                        {
                            foreach (var item in filterIndexs)
                            {
                                var indexContent = await GlobalAccess.Instance.CurrentPublication.DecryptIndexContent(item.FileName);
                                builder.AppendLine(indexContent);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("GetContentFromIndex:" + ex.ToString());
            }

            if (renderHyperlink)
                builder = new StringBuilder(RenderHyperLink(builder.ToString(), bookId));

            return builder.ToString();
        }
        public Task<Dictionary<string, string>> GetContentDataFromTOC(int bookId, TOCNode tocNode, bool performSaveHistory = true, bool renderHyperlink = true)
        {
            return Task.Run(async () =>
            {
                string content = string.Empty;
                Dictionary<string, string> contentDic = new Dictionary<string, string>();
                try
                {
                    await RefreshCurrentPublication(bookId);
                    currentBook = GlobalAccess.Instance.CurrentPublication.DlBook;
                    var dbpath = GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName;
                    var tOCNodeDetails = publicationContentService.GetTOCNodeDetails(tocNode.ID, dbpath);
                    if (tOCNodeDetails != null)
                    {
                        foreach (var tOCNodeDetail in tOCNodeDetails)
                        {
                            content = await GlobalAccess.Instance.CurrentPublication.DecryptTOCContent(tOCNodeDetail.FileName);
                            if (renderHyperlink)
                                content = RenderHyperLink(content, bookId);
                            content = GlobalAccess.Instance.CurrentPublication.UpdateContentImgSrcValue(content);
                            contentDic.Add(tOCNodeDetail.DocID, content);
                        }
                        if (performSaveHistory)
                            publicationContentService.SaveRecentHistory(tocNode.ID,
                                                                        tocNode.Title,
                                                                        GlobalAccess.Instance.Email,
                                                                        GlobalAccess.Instance.ServiceCode);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("GetContentFromTOC:" + ex.ToString());
                }
                return contentDic;
            });
        }
        public async Task<string> GetContentFromTOC(int bookId, TOCNode tocNode, bool performSaveHistory = true, bool renderHyperlink = true)
        {
            string tocContent = "dddddddddddddddddd";
            var contentDic = await GetContentDataFromTOC(bookId, tocNode, performSaveHistory, renderHyperlink);
            foreach (var content in contentDic)
            {
                tocContent += content.Value;
            }
            return tocContent;
        }


        public async Task<int> GetTOCIDByDocId(int bookId, string docId)
        {
            await RefreshCurrentPublication(bookId);
            var docids = docId.Split(',');
            TOCNodeDetail tocDetail = null;
            string sqlFullName = GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName;
            foreach (var docid in docids)
            {
                tocDetail = packageAccess.GetTOCDetailByDocId(sqlFullName, docid);
                if (tocDetail != null)
                {
                    break;
                }
            }
            if (tocDetail != null)
            {
                return tocDetail.ID;
            }
            else
            {
                throw new NullReferenceException("null tocDetail");
            }
        }

        public List<RecentHistoryItem> GetRecentHistory(int? bookId = null, int topNum = 10)
        {

            List<RecentHistoryItem> historyItemList = null;
            var historyList = recentHistoryAccess.GetRecentHistory(GlobalAccess.Instance.Email,
                                                                   GlobalAccess.Instance.ServiceCode,
                                                                   bookId,
                                                                   topNum);
            if (historyList != null && historyList.Count > 0)
            {
                historyItemList = historyList.Select((item) =>
                  {
                      return new RecentHistoryItem
                      {
                          BookId = item.BookId,
                          DOCID = item.DocID,
                          TOCTitle = item.TOCTitle,
                          PublicationTitle = item.DlBookTitle,
                          ColorPrimary = item.ColorPrimary,
                          LastReadDate = item.LastReadDate
                      };
                  }).ToList();
            }
            recentHistoryAccess.DeleteOlderRecentHistory();
            return historyItemList;
        }


        private string BuildRemoteUrl(string url, int bookId)
        {
            var currentUser = GlobalAccess.Instance.CurrentUserInfo;
            string uri = String.Format(currentUser.Country.ContentLinkRedirectorUrl.Trim(),
                                       currentUser.Country.CountryCode,
                                       currentUser.Email,
                                       (short)GlobalAccess.DeviceService.GetDeviceType(),
                                       bookId,
                                       DLMapping.SG_SOURCE);
            url = url.Replace(DLMapping.REMOTEURL_PREFIX, string.Empty);
            uri += !string.IsNullOrEmpty(url) ? "&" + url : url;
            return uri;
        }

        private bool IsAnchorHyperLink(string url)
        {
            return url.StartsWith("#");
        }

        private bool IsExternalHyperlink(string url)
        {
            return url.StartsWith(DLMapping.REMOTEURL_PREFIX);
        }

        private bool IsAttachmentHyperlink(string href, out string fileName, out FileType fileType)
        {
            bool linkToAttachment = href.ToLower().Contains(DLMapping.LINK_FILE_PREFIX);
            fileName = "";
            fileType = FileType.PDF;
            if (linkToAttachment)
            {
                fileName = href.ToLower().Replace(DLMapping.LINK_FILE_PREFIX, "");
                fileType = href.ToLower().EndsWith(".pdf") ? FileType.PDF : FileType.Word;
            }
            return linkToAttachment;
        }

        /// <summary>
        /// RenderHyperLink
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string RenderHyperLink(string strHtml, int currentBookId)
        {
            string result = strHtml;
            try
            {
                RefreshCurrentPublication(currentBookId).Wait();
                currentBook = GlobalAccess.Instance.CurrentPublication.DlBook;
                interBookDictionary.Clear();
                intraBookDictionary.Clear();
                allContentLinkDetailList.Clear();
                intraBookLinkDictionary.Clear();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(strHtml);
                var hyperlinkList = doc.DocumentNode.DescendantsAndSelf().Where(e => e.Name.Equals("a")).ToList();
                foreach (HtmlNode link in hyperlinkList)
                {
                    if(link.GetAttributeValue(CSS_CLASS, "") == CSS_HIDDEN_HYPER_LINK || link.GetAttributeValue(CSS_CLASS, "") == CSS_PLAIN_TEXT)
                    {
                        break;
                    }
                    string url = link.GetAttributeValue("href", "");
                    link.SetAttributeValue(CSS_CLASS_KEY, CSS_EXTERNAL_CLASS);
                    if (!String.IsNullOrEmpty(url))
                    {
                        //Hyperlink hyperlink = BuildHyperLink(currentBook.BookId, url, link.GetAttributeValue(TOCID, ""));
                        //if (hyperlink != null)
                        //{
                        //    if (hyperlink.LinkType == HyperLinkType.Hiddenlink)
                        //        link.SetAttributeValue(CSS_NONE_DISPLAY_KEY, CSS_NONE_DISPLAY_VALUE);
                        //    else if (hyperlink.LinkType == HyperLinkType.ExternalHyperlink)
                        //        link.SetAttributeValue(CSS_CLASS_KEY, CSS_EXTERNAL_CLASS);
                        //    else
                        //        link.SetAttributeValue(CSS_CLASS_KEY, CSS_INTERBOOK_CLASS);
                        //}
                    }
                    else
                        link.SetAttributeValue(CSS_CLASS, CSS_PLAIN_TEXT);
                }

                //RemoveCompendiumLink(doc);
                using (Stream st = new MemoryStream())
                {
                    doc.Save(st, Encoding.UTF8);
                    result = Encoding.UTF8.GetString((st as MemoryStream).ToArray(), 0, (st as MemoryStream).ToArray().Count());
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Log("Render Hyper Link Failed : " + ex.Message);
                return strHtml;
            }
        }

        /// <summary>
        /// BuildHyperLink
        /// </summary>
        /// <param name="bookID"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Hyperlink BuildHyperLink(int bookID, string url, string tocId)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                    return null;
                else if (IsExternalHyperlink(url))
                    return new ExternalHyperlink(BuildRemoteUrl(url, bookID));
                else if (IsAnchorHyperLink(url))
                    return new AnchorHyperlink(url);
                else
                {
                    var queryParams = ValidateHelper.ParseUrl(url);
                    if (queryParams == null)
                    {
                        if (currentBook != null)
                        {
                            string attachmentFileName = null;
                            FileType fileType;
                            if (IsAttachmentHyperlink(url, out attachmentFileName, out fileType))
                                return new AttachmentHyperlink(Path.Combine(currentBook.GetDirectory(), attachmentFileName), fileType);
                        }
                    }
                    else if (queryParams != null && queryParams.Count > 0)
                    {
                        if (queryParams.ContainsKey(DLMapping.LINK_TEXT))
                        {
                            if (queryParams[DLMapping.LINK_TEXT].Contains(DLMapping.SUBORDINATE_TEXT)
                                && queryParams[DLMapping.LINK_TEXT].Contains(DLMapping.LEGISLATION_TEXT))
                            {
                                return new Hiddenlink();
                            }
                        }
                        //looseleaf://opendocument?linktype=urjpdf&citation=BC201109206&filename=1109206.pdf
                        //looseleaf://...citeref?ctype=case&element=citefrag_span&decisiondate_year=2001&caseref_ID=cr000118&caseref_spanref=cr000118-001&reporter_value=nswlr&volume_num=53&page_num=198&LinkName=(2001) 53 NSWLR 198
                        //looseleaf://opendocument?dpsi=P-0098&refpt=CPV.RC1GD.64-01-600&remotekey1=REFPTID&service=DOC-ID&LinkName=[I 64.01.600]
                        if (currentBook != null)
                        {
                            string dpsiCodeLink = queryParams.ContainsKey(DLMapping.ATRRIBUTE_DPSI) ? queryParams[DLMapping.ATRRIBUTE_DPSI] : "-9999";
                            string refptLink = queryParams.ContainsKey(DLMapping.ATRRIBUTE_REF_POINTER) ? queryParams[DLMapping.ATRRIBUTE_REF_POINTER] : "-9999";
                            bool isIntraHyperlink = (currentBook.DpsiCode.Equals(dpsiCodeLink));
                            if (isIntraHyperlink && !String.IsNullOrEmpty(tocId))
                            {
                                int tocID = -1;
                                if(int.TryParse(tocId, out tocID))
                                {
                                    return new IntraHyperlink(tocID, refptLink);
                                }
                            }
                            else
                            {
                                string premiumKey = DLMapping.PREMIUM_PREFIX + dpsiCodeLink + "|" + refptLink;
                                string standardKey = dpsiCodeLink + "|" + refptLink;
                                DlBook targetDlbook = null;
                                if (!interBookDictionary.ContainsKey(premiumKey) && !interBookDictionary.ContainsKey(standardKey))
                                {
                                    DlBook targetFtcDlbook = null;
                                    DlBook targetStandardDlbook = null;

                                    if (!intraBookLinkDictionary.ContainsKey(DLMapping.PREMIUM_PREFIX + dpsiCodeLink) && !dpsiCodeLink.StartsWith(DLMapping.PREMIUM_PREFIX))
                                    {
                                        targetFtcDlbook = publicationAccess.GetDlBookByDpsiCode(GlobalAccess.Instance.UserCredential, DLMapping.PREMIUM_PREFIX + dpsiCodeLink);
                                        intraBookLinkDictionary.Add(DLMapping.PREMIUM_PREFIX + dpsiCodeLink, targetFtcDlbook);
                                    }
                                    else if (!dpsiCodeLink.StartsWith(DLMapping.PREMIUM_PREFIX))
                                        targetFtcDlbook = intraBookLinkDictionary[DLMapping.PREMIUM_PREFIX + dpsiCodeLink];

                                    if (!intraBookLinkDictionary.ContainsKey(dpsiCodeLink))
                                    {
                                        targetStandardDlbook = publicationAccess.GetDlBookByDpsiCode(GlobalAccess.Instance.UserCredential, dpsiCodeLink);
                                        intraBookLinkDictionary.Add(dpsiCodeLink, targetStandardDlbook);
                                    }
                                    else
                                        targetStandardDlbook = intraBookLinkDictionary[dpsiCodeLink];

                                    targetDlbook = GetBookByPriority(targetFtcDlbook, targetStandardDlbook, targetDlbook);
                                    if (targetDlbook != null)
                                    {
                                        if (!targetBookIdDictionary.ContainsKey(dpsiCodeLink))
                                            targetBookIdDictionary.Add(dpsiCodeLink, targetDlbook.BookId);

                                        bool hasDownloaded = (targetDlbook.DlStatus == (short)DlStatusEnum.Downloaded);
                                        if (hasDownloaded)
                                        {
                                            var contentLinkDetail = packageAccess.GetTOCDetailByRemoteLinkID(targetDlbook.GetDecryptedDbFullName(), refptLink);
                                            if (contentLinkDetail != null)
                                            {
                                                interBookDictionary.Add(targetDlbook.DpsiCode + "|" + refptLink, contentLinkDetail);
                                                return new InternalHyperlink(targetDlbook.BookId, contentLinkDetail.ID, refptLink);
                                            }
                                            else
                                            {
                                                interBookDictionary.Add(targetDlbook.DpsiCode + "|" + refptLink, null);
                                            }
                                        }
                                    }
                                }
                                else if (interBookDictionary.ContainsKey(premiumKey))
                                {
                                    if (interBookDictionary[premiumKey] != null && targetBookIdDictionary.ContainsKey(dpsiCodeLink))
                                    {
                                        return new InternalHyperlink(targetBookIdDictionary[dpsiCodeLink], interBookDictionary[premiumKey].ID, refptLink);
                                    }
                                }
                                else if (interBookDictionary.ContainsKey(standardKey))
                                {
                                    if (interBookDictionary[standardKey] != null && targetBookIdDictionary.ContainsKey(dpsiCodeLink))
                                    {
                                        return new InternalHyperlink(targetBookIdDictionary[dpsiCodeLink], interBookDictionary[standardKey].ID, refptLink);
                                    }
                                }
                            }
                        }
                        var additionalQueryString = ValidateHelper.ConstructQueryString(queryParams);
                        return new ExternalHyperlink(BuildRemoteUrl(additionalQueryString, bookID));
                    }
                    return new ExternalHyperlink(url);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
            }
        }

        /// <summary>
        /// GetBookByPriority
        /// </summary>
        /// <param name="targetFtcDlbook"></param>
        /// <param name="targetStandardDlbook"></param>
        /// <param name="targetDlbook"></param>
        /// <returns></returns>
        private static DlBook GetBookByPriority(DlBook targetFtcDlbook, DlBook targetStandardDlbook, DlBook targetDlbook)
        {
            bool isFtcExpired = true;
            bool isStatndardExpired = true;
            isFtcExpired = targetFtcDlbook == null || targetFtcDlbook.ValidTo == null || (targetFtcDlbook.ValidTo != null && targetFtcDlbook.ValidTo.Value.DaysRemaining() < 0);
            isStatndardExpired = targetStandardDlbook == null || targetStandardDlbook.ValidTo == null || (targetStandardDlbook.ValidTo != null && targetStandardDlbook.ValidTo.Value.DaysRemaining() < 0);
            if (targetFtcDlbook != null && targetStandardDlbook != null)
            {
                targetDlbook = isFtcExpired ? targetStandardDlbook : targetFtcDlbook;
            }
            else if (targetFtcDlbook != null)
            {
                targetDlbook = targetFtcDlbook;
            }
            else if (targetFtcDlbook == null && targetStandardDlbook != null)
            {
                targetDlbook = targetStandardDlbook;
            }
            return targetDlbook;
        }

        /// <summary>
        /// RemoveCompendiumLink
        /// </summary>
        /// <param name="doc"></param>
        private static void RemoveCompendiumLink(HtmlDocument doc)
        {
            var asideList = doc.DocumentNode.DescendantsAndSelf().Where(e => e.Name.Equals("aside")).ToList();
            foreach (HtmlNode link in asideList)
            {
                var hyperlinkList = link.DescendantsAndSelf().Where(e => e.Name.Equals("a")).ToList();
                var hrList = link.DescendantsAndSelf().Where(e => e.Name.Equals("hr")).ToList();
                foreach (HtmlNode alink in hyperlinkList)
                {
                    if (alink.InnerText.Contains(DLMapping.COMPENDIUM_TEXT) && alink.InnerText.Contains(DLMapping.CASES_TEXT)
                        && alink.InnerText.Contains(DLMapping.COMMENTARY_TEXT))
                    {
                        alink.SetAttributeValue(CSS_NONE_DISPLAY_KEY, CSS_NONE_DISPLAY_VALUE);
                        if (link.DescendantsAndSelf().Where(e => e.Name.Equals("h3")).ToList().Count == 0
                          && link.DescendantsAndSelf().Where(e => e.Name.Equals("h4")).ToList().Count == 0)
                        {
                            foreach (HtmlNode hrlink in hrList)
                            {
                                hrlink.SetAttributeValue(CSS_NONE_DISPLAY_KEY, CSS_NONE_DISPLAY_VALUE);
                            }
                        }
                    }
                }
                link.InnerHtml = link.InnerHtml.Replace("|", " ");
            }
        }
    }
}
