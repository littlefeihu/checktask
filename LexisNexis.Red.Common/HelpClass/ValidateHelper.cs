using LexisNexis.Red.Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LexisNexis.Red.Common.HelpClass
{
    public class ValidateHelper
    {
        public static bool IsEmail(string email)
        {
            bool isEmail = false;
            if (!string.IsNullOrEmpty(email))
            {
                Regex r = new Regex(Constants.EMAIL_REGEX);
                Match m = r.Match(email);
                isEmail = m.Success;
            }
            return isEmail;
        }
        public static Dictionary<string, string> ParseUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            int questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex == -1)
                return null;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (questionMarkIndex == url.Length - 1)
                return null;
            string ps = url.Substring(questionMarkIndex + 1);

            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.IgnoreCase);
            MatchCollection mc = re.Matches(ps);
            foreach (Match m in mc)
            {
                if (!dic.ContainsKey(m.Result("$2").ToLower()))
                    dic.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
            return dic;
        }
        public static string ConstructQueryString(Dictionary<string, string> dic)
        {
            string queryString = string.Empty;
            foreach (string key in dic.Keys)
            {
                if (queryString.Length > 0)
                    queryString += "&";
                queryString += key + "=" + dic[key];
            }
            return queryString;
        }
        public static bool IsValidWebUrl(string href)
        {
            if (!string.IsNullOrEmpty(href))
            {
                const string pattern = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
                Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
                return reg.IsMatch(href);
            }

            return false;
        }
    }
}
