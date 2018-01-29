using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Foundation;
using CoreGraphics;

using LexisNexis.Red.Common;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.iOS
{
	public static class PdfUtil
	{
		public static void SaveContentInWebViewAsPDF ()
		{
			ContentPrintPageRender render = new ContentPrintPageRender (AppDataUtil.Instance.GetCurrentPublication().Name + " / " + AppDataUtil.Instance.GetOpendTOC().Title);
			render.AddPrintFormatter (AppDisplayUtil.Instance.contentVC.ContentWebView.ViewPrintFormatter, 0);

			CGSize kPaperSizeA4 = new CGSize (595.2, 841.8);

			float topPadding = 20, bottomPadding = 20, leftPadding = 10, rightPadding=10;
			CGRect printableRect = new CGRect (leftPadding, topPadding, kPaperSizeA4.Width - leftPadding - rightPadding, kPaperSizeA4.Height - topPadding -bottomPadding);
			CGRect paperRect = new CGRect (0, 0, kPaperSizeA4.Width, kPaperSizeA4.Height);
			render.SetValueForKey (NSValue.FromCGRect(paperRect), new NSString("paperRect"));
			render.SetValueForKey (NSValue.FromCGRect(printableRect), new NSString("printableRect"));
			render.PrintToPDF ();
		}

	}
}

