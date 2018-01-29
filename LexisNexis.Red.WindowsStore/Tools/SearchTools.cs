using LexisNexis.Red.WindowsStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace LexisNexis.Red.WindowsStore.Tools
{
    public static class SearchTools
    {
        private static List<string> Keywords { set; get; }
        private static string Pattern { set; get; }
        private static readonly ResourceLoader RESOURCE_LOADER = new ResourceLoader();
        public static void KeepKeyword(List<string> keyword)
        {
            Keywords = keyword;
            Pattern = @"\b(" + string.Join("|", Keywords) + @")\b";
        }
        public static void HighLightKeywords(TextBlock block, string inputString)
        {
            var model = block.DataContext as SearchResultModel;
            if (model != null && model.Type == RESOURCE_LOADER.GetString("Publication"))
            {
                block.Text = inputString;
            }
            else
            {
                var stringSplit = Regex.Split(inputString, Pattern);
                if (block.Inlines.Count > 0)
                    block.Inlines.Clear();
                foreach (var subString in stringSplit)
                {
                    if (string.IsNullOrEmpty(subString))
                        continue;
                    Run value = new Run();
                    if (Keywords.Contains(subString))
                    {
                        value.FontWeight = FontWeights.Bold;
                    }
                    value.Text = subString;
                    block.Inlines.Add(value);
                }
            }
        }

        public static string RemoveBlank(string str)
        {
            var array = str.Split(new string[] { "\n", "\t", "\r"," "}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            return string.Join(" ", array);
        }
    }
}
