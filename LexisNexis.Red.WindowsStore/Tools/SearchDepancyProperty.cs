using LexisNexis.Red.WindowsStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LexisNexis.Red.WindowsStore.Tools
{
    public class SearchDepancyProperty
    {
        public static readonly DependencyProperty SearchProperty = DependencyProperty.RegisterAttached(
              "Search", typeof(string), typeof(SearchDepancyProperty), new PropertyMetadata(null, OnSearchChanged));

        public static string GetSearch(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(SearchProperty);
        }

        public static void SetSearch(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(SearchProperty, value);
        }

        private static void OnSearchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var block = d as TextBlock;
            if (e.NewValue!=null)
            {
                SearchTools.HighLightKeywords(block, e.NewValue.ToString());
            }
            else
            {
                block.Text = string.Empty;
            }
        }
    }
}
