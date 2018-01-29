using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.Interface
{
    public interface IHtmlHelper
    {
        void LoadHtml(string htmlContent);
        IEnumerable<int> GetSpanIdsByXpaths(IEnumerable<string> xpaths);
        IEnumerable<string> GetXpathsBySpanIds(IEnumerable<int> spanIds);
        int GetSpanIdByXpath(string xpath);
        string GetXpathBySpanId(int spanId);
    }
}
