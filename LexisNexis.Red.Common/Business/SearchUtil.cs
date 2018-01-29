using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using SQLite;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Common;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Database;
using LexisNexis.Red.Common.DominService;

namespace LexisNexis.Red.Common.Business
{
    /// <summary>
    /// ContentCategory
    /// </summary>
    public enum ContentCategory
    {
        All = 1,
        CommentaryType = 2,
        LegislationType = 3,
        FormsPrecedentsType = 4,
        CaseType = 5
    }

    public enum HeadType
    {
        H1 = 1,
        H2 = 2,
        H3 = 3,
        H4 = 4,
        TopOfDocument = 5
    }

    /// <summary>
    /// Search Service
    /// </summary>
    public class SearchUtil
    {
        private const string HEAD1MARK = "H_MARK_1";
        private const string HEAD2MARK = "H_MARK_2";
        private const string HEAD3MARK = "H_MARK_3";
        private const string HEAD4MARK = "H_MARK_4";

        private const string HEADNO = "H_NO_";

        private static string AllPublicationQuery
            = "SELECT title AS DocId, contentType AS ContentType, SNIPPET(IndexTable, '{{|', '|}}','...', -1, {1}) AS SnippetContent FROM IndexTable WHERE IndexTable match '{0}' ORDER BY DocId";
        private static string WithOneContentTypePublicationQuery
            = "SELECT title AS DocId, contentType AS ContentType, SNIPPET(IndexTable, '{{|', '|}}','...', -1, {2}) AS SnippetContent FROM IndexTable WHERE IndexTable match '{0}' AND contentType = '{1}' ORDER BY DocId";
        private static string WithTwoContentTypePublicationQuery
            = "SELECT title AS DocId, contentType AS ContentType, SNIPPET(IndexTable, '{{|', '|}}','...', -1, {3}) AS SnippetContent FROM IndexTable WHERE IndexTable match '{0}' AND contentType IN ('{1}', '{2}') ORDER BY DocId";
        private static string WithThreeContentTypePublicationQuery
            = "SELECT title AS DocId, contentType AS ContentType, SNIPPET(IndexTable, '{{|', '|}}','...', -1, {4}) AS SnippetContent FROM IndexTable WHERE IndexTable match '{0}' AND contentType IN ('{1}', '{2}', '{3}') ORDER BY DocId";

        private static string AllDocumentQuery
            = "SELECT title AS DocId, head AS Head, contentType as ContentType, SNIPPET(IndexTable, '{{|', '|}}','...', -1, {2}) AS SnippetContent FROM IndexTable WHERE IndexTable match '{0}' AND title = '{1}' ORDER BY DocId";
        private static string WithOneContentTypeDocumentQuery
            = "SELECT title AS DocId, head AS Head, contentType as ContentType, SNIPPET(IndexTable, '{{|', '|}}','...', -1, {3}) AS SnippetContent FROM IndexTable WHERE IndexTable match '{0}' AND contentType = '{1}' AND title = '{2}' ORDER BY DocId";
        private static string WithTwoContentTypeDocumentQuery
            = "SELECT title AS DocId, head AS Head, contentType as ContentType, SNIPPET(IndexTable, '{{|', '|}}','...', -1, {4}) AS SnippetContent FROM IndexTable WHERE IndexTable match '{0}' AND contentType IN ('{1}', '{2}') AND title = '{3}' ORDER BY DocId";
        private static string WithThreeContentTypeDocumentQuery
            = "SELECT title AS DocId, head AS Head, contentType as ContentType, SNIPPET(IndexTable, '{{|', '|}}','...', -1, {5}) AS SnippetContent FROM IndexTable WHERE IndexTable match '{0}' AND contentType IN ('{1}', '{2}', '{3}') AND title = '{4}' ORDER BY DocId";

        private static IPackageAccess packageAccess = IoCContainer.Instance.ResolveInterface<IPackageAccess>();
        private static IPublicationAccess publicationAccess = IoCContainer.Instance.ResolveInterface<IPublicationAccess>();
        private static IDictionaryAccess dictionaryAccess = IoCContainer.Instance.ResolveInterface<IDictionaryAccess>();
        private static IDictionaryDomainService dictionaryDomainService = IoCContainer.Instance.ResolveInterface<IDictionaryDomainService>();

        private static List<TOCNodeDetail> tocDetailList = new List<TOCNodeDetail>();
        private static List<TOCNode> tocList = new List<TOCNode>();

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="bookID"></param>
        /// <param name="TocID"></param>
        /// <param name="keywords"></param>
        /// <param name="contentTypeList"></param>
        /// <returns></returns>
        public static SearchResult Search(int bookID, int TocID, string keywords, List<ContentCategory> contentTypeList = null)
        {
            if(contentTypeList == null)
            {
                contentTypeList = new List<ContentCategory>();
                contentTypeList.Add(ContentCategory.All);
            }

            SearchResult sr = new SearchResult();
            sr.SearchDisplayResultList = new List<SearchDisplayResult>();
            sr.FoundWordList = new List<string>();
            sr.KeyWordList = new List<string>();
            List<SearchDisplayResult> beforeRemoveDuplicateResult = new List<SearchDisplayResult>();
            List<SearchDisplayResult> publicationTempResult = new List<SearchDisplayResult>();
            List<SearchDisplayResult> documentTempResult = new List<SearchDisplayResult>();
            List<IndexData> indexDataPubList = new List<IndexData>();
            List<IndexData> indexDataDocList = new List<IndexData>();
            List<String> keyWordList = new List<string>();
            List<String> foundWordList = new List<string>();
            Dictionary<String, int> ExistedDocIdDictionary = new Dictionary<String, int>();
            String indexPath = publicationAccess.GetDlBookByBookId(bookID, GlobalAccess.Instance.UserCredential).GetIndexDbFullName();
            tocDetailList = packageAccess.GetAllTOCNodeDetails(GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName);
            tocList = packageAccess.GetTOC(GlobalAccess.Instance.CurrentPublication.DecryptedDbFullName);

            try
            {
                List<string> critieriaStringList = SegmentUtil.Instance.PhraseSegment(keywords);

                if (critieriaStringList.Count > 0)
                {
                    int SearchRound = 0;
                    foreach (String critieriaString in critieriaStringList)
                    {
                        SearchRound++;
                        keyWordList.AddRange(critieriaString.Split(new String[] { Constants.NEAR_SEPERATOR }, StringSplitOptions.RemoveEmptyEntries).ToList());
                        using (var db = new SQLiteConnection(indexPath))
                        {
                            //For Publication query
                            string queryStringPub = String.Empty;

                            //For Document query
                            string queryStringDoc = String.Empty;

                            string DocId = String.Empty;
                            DocId = (from tocD in tocDetailList where tocD.ID == TocID select tocD.DocID).ToList().FirstOrDefault();

                            int snippetNumber = (critieriaStringList.Count) * Constants.MAX_WORD_NUMBER;
                            BuildQueryString(contentTypeList, critieriaString, ref queryStringPub, ref queryStringDoc, DocId, snippetNumber);

                            // Get publication Index Data
                            var tempPublicationDataList = db.Query<IndexData>(queryStringPub);

                            foreach (IndexData element in tempPublicationDataList.ToList<IndexData>())
                            {
                                // get found word llist
                                string foundWordText = element.SnippetContent;
                                foundWordText = foundWordText.Replace("{|", "TargetSeperator|||").Replace("|}", "|||TargetSeperator");

                                var tempfoundWordList = foundWordText.Split(new String[] { "TargetSeperator" }, StringSplitOptions.None).ToList();

                                foreach (string foundWord in tempfoundWordList)
                                {
                                    if (foundWord.Contains("|||") && !foundWordList.Contains(foundWord.Replace("|||", "")))
                                        foundWordList.Add(foundWord.Replace("|||", "").Trim());
                                }

                                if (ExistedDocIdDictionary.ContainsKey(element.DocId))
                                {
                                    if (ExistedDocIdDictionary[element.DocId] < SearchRound)
                                        ExistedDocIdDictionary[element.DocId]++;
                                }
                                else
                                {
                                    ExistedDocIdDictionary.Add(element.DocId, 1);
                                }
                            }
                            indexDataPubList = tempPublicationDataList.ToList<IndexData>();
                            // Get document Index Data
                            indexDataDocList = db.Query<IndexData>(queryStringDoc);
                        }

                        if (indexDataPubList.Count > 0)
                        {
                            var result = from r in indexDataPubList
                                         join tocD in tocDetailList
                                           on r.DocId equals tocD.DocID
                                         join toc in tocList
                                           on tocD.ID equals toc.ID
                                         orderby toc.ID
                                         select new SearchDisplayResult
                                         {
                                             TocId = tocD.ID,
                                             TocTitle = toc.Title,
                                             GuideCardTitle = toc.GuideCardTitle,
                                             SnippetContent = r.SnippetContent.Replace("{|", "").Replace("|}", "").Replace("HEADEND", ""),
                                             ContentType = (ContentCategory)Enum.Parse(typeof(ContentCategory), r.ContentType),
                                             isDocument = false,
                                             DocId = r.DocId
                                         };
                            publicationTempResult = result.ToList();
                        }

                        if (indexDataDocList.Count > 0 && documentTempResult.Count == 0)
                        {
                            var result = from r in indexDataDocList
                                         select new SearchDisplayResult
                                         {
                                             Head = r.Head,
                                             SnippetContent = r.SnippetContent.Replace("{|", "").Replace("|}", "").Replace("HEADEND", ""),
                                             ContentType = (ContentCategory)Enum.Parse(typeof(ContentCategory), r.ContentType),
                                             TocId = TocID,
                                             isDocument = true
                                         };
                            documentTempResult = result.ToList();
                            List<SearchDisplayResult> beforeGetHeadNoResult = GetHeadSequence(documentTempResult);

                            documentTempResult.Clear();
                            documentTempResult = beforeGetHeadNoResult;
                        }
                        beforeRemoveDuplicateResult.AddRange(publicationTempResult);
                    }
                }

                foreach (string keyword in keyWordList)
                {
                    if (!sr.KeyWordList.Contains(keyword))
                        sr.KeyWordList.Add(keyword);

                    if (!foundWordList.Contains(keyword))
                        foundWordList.Add(keyword);
                }

                foreach (string foundWord in foundWordList)
                {
                    if (!sr.FoundWordList.Contains(foundWord))
                        sr.FoundWordList.Add(foundWord);
                }

                HashSet<string> duplicateDocIdList = new HashSet<string>();
                HashSet<int> duplicateTocIdList = new HashSet<int>();
                bool inCurrentDocument = false;
                foreach (SearchDisplayResult e in beforeRemoveDuplicateResult.OrderBy(b=>b.TocId))
                {
                    if (e.TocId == TocID)
                        inCurrentDocument = true;

                    if (ExistedDocIdDictionary[e.DocId] == critieriaStringList.Count &&
                           !duplicateDocIdList.Contains(e.DocId) && !duplicateTocIdList.Contains(e.TocId))
                        sr.SearchDisplayResultList.Add(e);

                    duplicateTocIdList.Add(e.TocId);
                    duplicateDocIdList.Add(e.DocId);
                }

                if (inCurrentDocument)
                    sr.SearchDisplayResultList.AddRange(documentTempResult);

                sr.SearchDisplayResultList = sr.SearchDisplayResultList.OrderByDescending(e => e.isDocument).ThenBy(e => e.TocId).ToList();

                if (sr.SearchDisplayResultList.Count == 0)
                    sr.FoundWordList.Clear();

                Regex replaceSpanId = new Regex("SPANMARK([0-9]+)");
                Regex replaceSpace = new Regex(@"\s{2,}", RegexOptions.IgnoreCase);
                foreach (SearchDisplayResult e in sr.SearchDisplayResultList)
                {
                    var match = replaceSpanId.Matches(e.SnippetContent);
                    if (match.Count > 0)
                    {
                        string highlightStartSpanId = match[0].Groups[1].Value;
                        string highlightEndSpanId = match[match.Count - 1].Groups[1].Value;
                        int spanId = -1;
                        if (Int32.TryParse(highlightStartSpanId, out spanId))
                        {
                            e.HighlightStartSpanId = spanId - 1;
                        }
                        if (Int32.TryParse(highlightEndSpanId, out spanId))
                        {
                            e.HighlightEndSpanId = spanId;
                        }
                    }

                    if (e.HighlightStartSpanId == e.HighlightEndSpanId && e.HighlightEndSpanId == 0)
                    {
                        e.HighlightStartSpanId = -1;
                        e.HighlightEndSpanId = -1;
                    }

                    if (!String.IsNullOrEmpty(e.Head))
                        e.Head = replaceSpace.Replace(replaceSpanId.Replace(e.Head, ""), " ").Trim();
                    if (!String.IsNullOrEmpty(e.SnippetContent))
                        e.SnippetContent = replaceSpace.Replace(replaceSpanId.Replace(e.SnippetContent, ""), " ").Trim();
                }

                return sr;
            }
            catch (Exception ex)
            {
                Logger.Log("Search Failed : " + ex.Message);
                return null;
            }
        }

        private static List<SearchDisplayResult> GetHeadSequence(List<SearchDisplayResult> documentTempResult)
        {
            List<SearchDisplayResult> beforeGetHeadNoResult = new List<SearchDisplayResult>();
            string pattern = @"\bH_NO_(\w*)\b";
            foreach (SearchDisplayResult e in documentTempResult)
            {
                if (e.Head.Contains(HEAD1MARK) && e.Head.Contains(HEADNO))
                {
                    e.HeadType = HeadType.H1;
                    var matches = Regex.Matches(e.Head, pattern);
                    e.HeadSequence = int.Parse(matches[0].Groups[1].Value);
                }
                else if (e.Head.Contains(HEAD2MARK) && e.Head.Contains(HEADNO))
                {
                    e.HeadType = HeadType.H2;
                    var matches = Regex.Matches(e.Head, pattern);
                    e.HeadSequence = int.Parse(matches[0].Groups[1].Value);
                }
                else if (e.Head.Contains(HEAD3MARK) && e.Head.Contains(HEADNO))
                {
                    e.HeadType = HeadType.H3;
                    var matches = Regex.Matches(e.Head, pattern);
                    e.HeadSequence = int.Parse(matches[0].Groups[1].Value);
                }
                else if (e.Head.Contains(HEAD4MARK) && e.Head.Contains(HEADNO))
                {
                    e.HeadType = HeadType.H4;
                    var matches = Regex.Matches(e.Head, pattern);
                    e.HeadSequence = int.Parse(matches[0].Groups[1].Value);
                }
                else
                {
                    e.HeadType = HeadType.TopOfDocument;
                    e.HeadSequence = 0;
                }

                e.Head = Regex.Replace(Regex.Replace(e.Head, @"\bH_NO_\w*\b", ""), @"\bH_MARK_[0-9]+", "").Trim();
                e.SnippetContent = Regex.Replace(Regex.Replace(e.SnippetContent, @"\bH_NO_\w*\b", ""), @"\bH_MARK_[0-9]+", "").Trim();
                beforeGetHeadNoResult.Add(e);
            }
            return beforeGetHeadNoResult;
        }

        private static void BuildQueryString(List<ContentCategory> contentTypeList, String critieriaString, ref string queryStringPub, ref string queryStringDoc, string DocId, int snippetNumber)
        {

            if (contentTypeList.Contains(ContentCategory.All) || contentTypeList.Count > 3)
            {
                queryStringPub = String.Format(AllPublicationQuery, critieriaString, snippetNumber);

                queryStringDoc = String.Format(AllDocumentQuery, critieriaString, DocId, snippetNumber);
            }
            else if (contentTypeList.Count == 1)
            {
                queryStringPub = String.Format(WithOneContentTypePublicationQuery, critieriaString,
                     contentTypeList[0].ToString(), snippetNumber);

                queryStringDoc = String.Format(WithOneContentTypeDocumentQuery, critieriaString,
                     contentTypeList[0].ToString(), DocId, snippetNumber);
            }
            else if (contentTypeList.Count == 2)
            {
                queryStringPub = String.Format(WithTwoContentTypePublicationQuery, critieriaString,
                     contentTypeList[0].ToString(), contentTypeList[1].ToString(), snippetNumber);

                queryStringDoc = String.Format(WithTwoContentTypeDocumentQuery, critieriaString,
                     contentTypeList[0].ToString(), contentTypeList[1].ToString(), DocId, snippetNumber);
            }
            else if (contentTypeList.Count == 3)
            {
                queryStringPub = String.Format(WithThreeContentTypePublicationQuery, critieriaString,
                     contentTypeList[0].ToString(), contentTypeList[1].ToString(), contentTypeList[2].ToString(), snippetNumber);

                queryStringDoc = String.Format(WithThreeContentTypeDocumentQuery, critieriaString,
                     contentTypeList[0].ToString(), contentTypeList[1].ToString(), contentTypeList[2].ToString(), DocId, snippetNumber);
            }
        }
    }

    public static class StringUtil
    {
        /// <summary>
        /// ToSingular
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string ToSingular(string word)
        {
            Regex plural1 = new Regex("(?<keep>[^aeiou])ies$");
            Regex plural2 = new Regex("(?<keep>[aeiou]y)s$");
            Regex plural3 = new Regex("(?<keep>[sxzh])es$");
            Regex plural4 = new Regex("(?<keep>[^sxzhyu])s$");

            if (plural1.IsMatch(word))
                return plural1.Replace(word, "${keep}y");
            else if (plural2.IsMatch(word))
                return plural2.Replace(word, "${keep}");
            else if (plural3.IsMatch(word))
                return plural3.Replace(word, "${keep}");
            else if (plural4.IsMatch(word))
                return plural4.Replace(word, "${keep}");

            return word;
        }

        /// <summary>
        /// ToPlural
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string RemoveApostrophes(string word)
        {
            Regex replaceSpace = new Regex(@"\s{2,}", RegexOptions.IgnoreCase);
            word = replaceSpace.Replace(word.Replace("\'s", " ").Replace("\'", " ").Trim(), " ");
            return word;
        }
    }
} 