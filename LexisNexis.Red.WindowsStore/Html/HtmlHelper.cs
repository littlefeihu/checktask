using HtmlAgilityPack;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.WindowsStore.Implementation;
using LexisNexis.Red.WindowsStore.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using System.Linq;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.WindowsStore.Html
{
    public static class HtmlHelper
    {
        private static string TemplateString { set; get; }
        private static string DictionaryTemplate { set; get; }
        private static string IndexTemplate { set; get; }

        public async static Task<string> HtmlTemplateToString()
        {
            if (string.IsNullOrEmpty(TemplateString))
            {
                StorageFile htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Html/htmlDoc.html"));
                using (var htmlStream = await htmlFile.OpenAsync(FileAccessMode.Read))
                using (StreamReader sr = new StreamReader(htmlStream.GetInputStreamAt(0).AsStreamForRead(), Encoding.UTF8))
                {
                    TemplateString = await sr.ReadToEndAsync();
                }
            }
            return TemplateString;
        }

        public async static Task<string> IndexTemplateToString()
        {
            if (string.IsNullOrEmpty(IndexTemplate))
            {
                StorageFile htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Html/HtmlIndex.html"));
                using (var htmlStream = await htmlFile.OpenAsync(FileAccessMode.Read))
                using (StreamReader sr = new StreamReader(htmlStream.GetInputStreamAt(0).AsStreamForRead(), Encoding.UTF8))
                {
                    IndexTemplate = await sr.ReadToEndAsync();
                }
            }
            return IndexTemplate;
        }

        public async static Task<string> DictionaryTemplateToString()
        {
            if (string.IsNullOrEmpty(DictionaryTemplate))
            {
                StorageFile htmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Html/Dictionary.html"));
                using (var htmlStream = await htmlFile.OpenAsync(FileAccessMode.Read))
                using (StreamReader sr = new StreamReader(htmlStream.GetInputStreamAt(0).AsStreamForRead(), Encoding.UTF8))
                {
                    DictionaryTemplate = await sr.ReadToEndAsync();
                }
            }
            return DictionaryTemplate;
        }

        public static Point ConvertToPoint(string x, string y)
        {
            try
            {
                double px = double.Parse(x);
                double py = double.Parse(y);
                return new Point(px, py);
            }
            catch
            {
                return new Point(0d, 0d);
            }
        }

        public static readonly List<FontMenuItem> FONTSIZE_LIST = new List<FontMenuItem> 
        {
            new FontMenuItem{ID=0,Size=14.4,Description="Small"},
            new FontMenuItem{ID=1,Size=15,Description="Normal"},
            new FontMenuItem{ID=2,Size=18,Description="Large"},
            new FontMenuItem{ID=3,Size=27,Description="Extra Large"}
        };

        public static async Task<string> PageSplit(string html, NavigationRecord record)
        {
            string localPath = "ms-appx-web:///Assets/";
            string cb = "cb.png";
            string cbcc = "cbcc.gif";
            string nz = "nzcitator.gif";
            string cautionary = "cautionary.png";
            string citation = "citation.png";
            string negative = "negative.png";
            string neutral = "neutral.png";
            string positive = "positive.png";

            html = html.Replace(cb, localPath + cb);
            html = html.Replace(cbcc, localPath + cbcc);
            html = html.Replace(nz, localPath + nz);
            html = html.Replace(cautionary, localPath + cautionary);
            html = html.Replace(citation, localPath + citation);
            html = html.Replace(negative, localPath + negative);
            html = html.Replace(neutral, localPath + neutral);
            html = html.Replace(positive, localPath + positive);

            html = string.Format("<div id=\"tocId{0}\" class=\"layouttoc\">", record.TOCId) + html + "</div>";
            html = await ImageToBase64(html, localPath);
            return html;
        }

        public static void Copy(string content)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(content);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
        }

        private static async Task<string> ImageToBase64(string strHtml, string localPath)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(strHtml);
            var elements = doc.DocumentNode.DescendantsAndSelf();
            foreach (HtmlNode element in elements)
            {
                if (element.Name.ToLower() == "img")
                {
                    string absolutePath = element.GetAttributeValue("src", "");
                    if (!absolutePath.StartsWith(localPath))
                    {
                        FileDirectory fileTool = new FileDirectory();
                        string relativePath = absolutePath.Replace(fileTool.GetAppRootPath() + "\\", string.Empty);
                        try
                        {
                            var imgFile = await ApplicationData.Current.LocalFolder.GetFileAsync(relativePath);
                            using (var randomAccessStream = await imgFile.OpenAsync(FileAccessMode.Read))
                            using (Stream stream = randomAccessStream.AsStream())
                            {
                                byte[] bytes = new byte[stream.Length];
                                await stream.ReadAsync(bytes, 0, bytes.Length);
                                string base64String = Convert.ToBase64String(bytes);
                                string fullBase64 = "data:" + imgFile.ContentType + ";base64," + base64String;
                                strHtml = strHtml.Replace(absolutePath, fullBase64);
                            }
                        }
                        catch
                        {
                            //TODO:File Not Exit.
                        }
                    }
                }
            }
            return strHtml;
        }

        public static string GetTOCCountryCode(string strHtml)
        {
            string CountryCode = GlobalAccess.Instance.CountryCode;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(strHtml);
            var elements = doc.DocumentNode.DescendantsAndSelf();
            var node = elements.FirstOrDefault(n => !string.IsNullOrEmpty(n.GetAttributeValue("data-doc.country", "")));
            if (node != null)
            {
                CountryCode = node.GetAttributeValue("data-doc.country", "");
            }
            return CountryCode;
        }
    }

}
