using System;
using System.IO;
using Android.Print;
using Com.Lexisnexis.Printext;
using Android.OS;

namespace LexisNexis.Red.Droid.PrintUtility
{
	public class PrintLayoutCallback: Java.Lang.Object, IPrintLayoutResultCallback
	{
		private readonly string pdfFileName;
		private readonly PrintDocumentAdapter printDocumentAdapter;
		private readonly Action<string> printFinished;

		public Action<string> PrintFinished
		{
			get
			{
				return printFinished;
			}
		}

		public PrintLayoutCallback(
			string pdfFileName,
			PrintDocumentAdapter printDocumentAdapter,
			Action<string> printFinished)
		{
			this.pdfFileName = pdfFileName;
			this.printDocumentAdapter = printDocumentAdapter;
			this.printFinished = printFinished;
		}

		public void OnLayoutCancelled()
		{
			throw new NotImplementedException();
		}

		public void OnLayoutFailed(Java.Lang.ICharSequence p0)
		{
			throw new NotImplementedException();
		}

		public void OnLayoutFinished(PrintDocumentInfo p0, bool p1)
		{
			var cacheFolder = PdfPrintCacheHelper.PrepareCacheFolder();
			var pdfFile = new FileInfo(Path.Combine(cacheFolder.FullName, pdfFileName));
			pdfFile.Delete();

			PrintHelper.Write(
				printDocumentAdapter,
				new PageRange[]{ PageRange.AllPages },
				pdfFile.FullName,
				new CancellationSignal(),
				new PrintWriteCallback(pdfFile.FullName, this));
		}
	}
}

