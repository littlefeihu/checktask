using LexisNexis.Red.Common.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LexisNexis.Red.WindowsStore.Html
{
    public class WebBrowserTOCHelper
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
              "Html", typeof(string), typeof(WebBrowserTOCHelper), new PropertyMetadata(null, OnHtmlChanged));

        public static string GetHtml(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(HtmlProperty, value);
        }

        private async static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebView;
            if (browser == null|| string.IsNullOrEmpty(e.NewValue.ToString()))
                return;
            var body = e.NewValue.ToString();
            string template = await HtmlHelper.HtmlTemplateToString();
            int id = (int)SettingsUtil.Instance.GetFontSize();
            var font = HtmlHelper.FONTSIZE_LIST[id];
            template = template.Replace("#FONTSIZE#", font.Size.ToString());
            template = template.Replace("#BODY#", body);
            browser.NavigateToString(template);
        }
    }

    public class WebBrowserIndexHelper
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
              "Html", typeof(string), typeof(WebBrowserIndexHelper), new PropertyMetadata(null, OnHtmlChanged));

        public static string GetHtml(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(HtmlProperty);
        }

        public static void SetHtml(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(HtmlProperty, value);
        }

        private async static void OnHtmlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var browser = d as WebView;

            if (browser == null || string.IsNullOrEmpty(e.NewValue.ToString()))
                return;
            //data-tag="title" data-value="A"
            var body = e.NewValue.ToString();
            string template = await HtmlHelper.IndexTemplateToString();
            int id = (int)SettingsUtil.Instance.GetFontSize();
            var font = HtmlHelper.FONTSIZE_LIST[id];
            template = template.Replace("#FONTSIZE#", font.Size.ToString());
            template = template.Replace("#BODY#", body);
            browser.NavigateToString(template);
        }
    }
}
