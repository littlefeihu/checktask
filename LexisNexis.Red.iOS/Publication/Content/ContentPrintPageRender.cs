using System;

using Foundation;
using UIKit;
using CoreGraphics;
using LexisNexis.Red.Common;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.iOS
{
	public class ContentPrintPageRender : UIPrintPageRenderer
	{
		const int CONTENT_LEFT_RIGHT_MARGIN = 50;
		const int HEADER_FONT_SIZE = 8;
		const int HEADER_TOP_PADDING = 20;

		const int FOOTER_FONT_SIZE = 8;
		const int FOOTER_BOTTOM_PADDING = 20;

		const string PAGE_FOOTER_COPYRIGHT = "© 2015 LexisNexis";
		const string PAGE_FOOTER_PAGE_INFO = "Printed page ";
		const string PAGE_FOOTER_DATE_STR = "Currency date:";

		const int DEFAULT_HEADER_HEIGHT  = 50;
		const int DEFAULT_FOOTER_HEIGHT = 50;

		public NSString HeaderText{ get; set;}

		public ContentPrintPageRender(string headerText = ""):base()
		{
			FooterHeight = DEFAULT_HEADER_HEIGHT;
			HeaderHeight = DEFAULT_FOOTER_HEIGHT;

			HeaderText = new NSString(headerText);
		}

		public override void DrawHeaderForPage (nint index, CGRect headerRect)
		{
			UIFont headerFont = UIFont.SystemFontOfSize (FOOTER_FONT_SIZE);

			CGPoint point = new CGPoint (headerRect.X + CONTENT_LEFT_RIGHT_MARGIN, headerRect.Y + HEADER_TOP_PADDING);
			HeaderText.DrawString (point, headerFont); 
		}

		public override void DrawFooterForPage (nint index, CGRect footerRect)
		{
			base.DrawFooterForPage (index, footerRect);

			UIFont footerFont = UIFont.SystemFontOfSize (FOOTER_FONT_SIZE);

			//Draw Print date
			NSString dateStr = new NSString (PAGE_FOOTER_DATE_STR + DateTime.Now.ToString("dd MMM yyyy "));
			CGSize dateSize = dateStr.StringSize (footerFont);
			CGPoint point = new CGPoint (CONTENT_LEFT_RIGHT_MARGIN, footerRect.GetMaxY() - dateSize.Height - FOOTER_BOTTOM_PADDING);
			dateStr.DrawString (point, footerFont);

			//Draw "© 2015 LexisNexis" in the horizontal center of footer
			NSString copyrightStr = new NSString (PAGE_FOOTER_COPYRIGHT);
			CGSize copyrightSize = copyrightStr.StringSize (footerFont);
			point = new CGPoint (footerRect.GetMaxX() / 2 - copyrightSize.Width / 2, footerRect.GetMaxY() - copyrightSize.Height - FOOTER_BOTTOM_PADDING);
			copyrightStr.DrawString (point, footerFont);

			//Draw page num in the footer
			NSString pageNumStr = new NSString (PAGE_FOOTER_PAGE_INFO + (index + 1));
			CGSize pageNumSize = pageNumStr.StringSize (footerFont);
			point = new CGPoint (footerRect.GetMaxX() - pageNumSize.Width - CONTENT_LEFT_RIGHT_MARGIN, footerRect.GetMaxY() - pageNumSize.Height - FOOTER_BOTTOM_PADDING);
			pageNumStr.DrawString (point, footerFont);

		}

		public void PrintToPDF ()
		{
			IDirectory dirInstance = IoCContainer.Instance.ResolveInterface<IDirectory> ();
			string fileName = dirInstance.GetAppRootPath () + ViewConstant.SHARE_TMP_PDF_NAME;
			
			UIGraphics.BeginPDFContext (fileName, PaperRect, new CGPDFInfo {Author ="LexisNexis Red", Title = "LexisNexis Red"});

			PrepareForDrawingPages (new NSRange (0, NumberOfPages));
			CGRect bounds = UIGraphics.PDFContextBounds;
			for (int i = 0; i < NumberOfPages; i++) {
				UIGraphics.BeginPDFPage ();
				DrawPage (i, bounds);
			}

			UIGraphics.EndPDFContent ();
		}
	}
}

