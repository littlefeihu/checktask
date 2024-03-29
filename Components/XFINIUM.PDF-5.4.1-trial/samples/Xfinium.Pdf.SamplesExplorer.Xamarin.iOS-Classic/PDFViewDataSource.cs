using System;
using MonoTouch.QuickLook;
using MonoTouch.Foundation;
using System.IO;

namespace Xfinium.Pdf.SamplesExplorer.Xamarin.iOS
{
	public class PDFViewDataSource : QLPreviewControllerDataSource
	{
		public PDFViewDataSource (string fileName)
		{
			this.fileName = fileName;
		}

		private string fileName;

		public override int PreviewItemCount(QLPreviewController controller)
		{
			return 1;
		}

		public override QLPreviewItem GetPreviewItem(QLPreviewController controller, int index)
		{
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			NSUrl url = NSUrl.FromFilename (Path.Combine (documentsPath, fileName));

			return new PDFViewItem (fileName, url);
		}
	}

	public class PDFViewItem : QLPreviewItem
	{
		public PDFViewItem(string title, NSUrl url)
		{
			this.title = title.Substring(19, title.Length - 23);
			this.url = url;
		}

		private string title;
		private NSUrl url;

		public override string ItemTitle
		{
			get { return title; }
		}

		public override NSUrl ItemUrl
		{
			get { return url; }
		}
	}
}

