using System;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;
using System.Threading;
using LexisNexis.Red.Mac.Data;
using Foundation;
using System.Threading.Tasks;

namespace LexisNexis.Red.Mac
{
	public class DownloadManager
	{
		public PublicationView BookView { get; set; }
		CancellationTokenSource tokenSource;
		public string BookTitle { get; set; }
		public int bytesPreDownloaded {get; set;}

		public DownloadManager (PublicationView aView)
		{
			BookView = aView;
		}

		public async Task StartDownloadWithoutLimitation(int bookID, bool isUpdate)
		{
			bytesPreDownloaded = 0;
			tokenSource = new CancellationTokenSource ();

			DownloadResult downloadResult = await PublicationUtil.Instance.DownloadPublicationByBookId (bookID, tokenSource.Token, UpdateDownloadProgress);

			switch (downloadResult.DownloadStatus) {
			case DownLoadEnum.Canceled:
			case DownLoadEnum.Failure:
			case DownLoadEnum.NetDisconnected:
			case DownLoadEnum.Success:
				BookView.UpdateDownloadStatus (downloadResult, isUpdate);
				break;
			}
		}

		void UpdateDownloadProgress(int bytesDownloaded, long downloadSize)
		{
			if ((bytesDownloaded - bytesPreDownloaded) < 2) {
				return;
			}

			bytesPreDownloaded = bytesDownloaded;

			BookView.UpdateDownloadProgress (bytesDownloaded, downloadSize);
		}

		public void CancelDownload ()
		{
			if (tokenSource != null) {
				tokenSource.Cancel ();
			}
			//RemovePublicationDownloadingProgress ();
		}

		private void SavePublicationDownladProgress (int bytesDownloaded)
		{
			NSUserDefaults userDefault = NSUserDefaults.StandardUserDefaults;
			userDefault.SetInt (bytesDownloaded, BookTitle);
			userDefault.Synchronize ();
		}

		private void RemovePublicationDownloadingProgress()
		{
			if (BookTitle == null) {
				return;
			}

			NSUserDefaults userDefault = NSUserDefaults.StandardUserDefaults;
			userDefault.RemoveObject (BookTitle);
			userDefault.Synchronize ();
		}

		public int PublicationDownloadingProgress ()
		{
			NSUserDefaults userDefault = NSUserDefaults.StandardUserDefaults;
			nint value = userDefault.IntForKey (BookTitle);
			return Convert.ToInt32(value);
		}

	}
}

