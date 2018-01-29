using HtmlAgilityPack;
using LexisNexis.Red.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LexisNexis.Red.CommonTests.Impl
{
    public class HtmlHelper : IHtmlHelper
    {
        HtmlDocument doc = new HtmlDocument();
        public void LoadHtml(string htmlContent)
        {
            doc.LoadHtml(htmlContent);
        }
        public IEnumerable<int> GetSpanIdsByXpaths(IEnumerable<string> xpaths)
        {
            foreach (var xpath in xpaths)
            {
                var node = doc.DocumentNode.SelectSingleNode(xpath);
                var id = node != null ? node.GetAttributeValue("id", 0) : 0;
                yield return id;
            }
        }
        public IEnumerable<string> GetXpathsBySpanIds(IEnumerable<int> spanIds)
        {
            foreach (var spanId in spanIds)
            {
                var node = doc.DocumentNode.SelectSingleNode(@"//span[@id='" + spanId + "']");
                yield return node != null ? node.XPath : string.Empty;
            }
        }
        public int GetSpanIdByXpath(string xpath)
        {
            var node = doc.DocumentNode.SelectSingleNode(xpath);
            var id = node != null ? node.GetAttributeValue("id", 0) : 0;
            return id;
        }

        public string GetXpathBySpanId(int spanId)
        {
            var node = doc.DocumentNode.SelectSingleNode(@"//span[@id='" + spanId + "']");
            return node != null ? node.XPath : string.Empty;
        }
    }
}
