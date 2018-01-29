using System;
using Android.App;
using Android.Print;
using Com.Lexisnexis.Printext;
using Android.Widget;

namespace LexisNexis.Red.Droid.PrintUtility
{
	public class PrintWriteCallback: Java.Lang.Object, IPrintWriteResultCallback
	{
		private readonly string fullPdfFilePath;
		private readonly PrintLayoutCallback printLayoutCallback;

		public PrintWriteCallback(
			string fullPdfFilePath,
			PrintLayoutCallback printLayoutCallback)
		{
			this.fullPdfFilePath = fullPdfFilePath;
			this.printLayoutCallback = printLayoutCallback;
		}

		public void OnWriteCancelled()
		{
			throw new NotImplementedException();
		}

		public void OnWriteFailed(Java.Lang.ICharSequence p0)
		{
			throw new NotImplementedException();
		}

		public void OnWriteFinished(PageRange[] p0)
		{
			if(printLayoutCallback != null
				&& printLayoutCallback.PrintFinished != null)
			{
				printLayoutCallback.PrintFinished(fullPdfFilePath);
			}
		}
	}
}

