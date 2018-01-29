using System;
using System.Threading;

namespace LexisNexis.Red.Droid.Business
{
	public class PublicationExtInfo
	{
		public const string DOWNLOAD_PUBLICATION_TASK = "DOWNLOAD_PUBLICATION_TASK";
		public const string GET_PUBLICATION_DETAIL_TASK = "DOWNLOAD_PUBLICATION_TASK";
		public const string CANCELDOWNLOAD_CONFIRM_DIALOG_EXTRATAG = "CANCELDOWNLOAD_CONFIRM_DIALOG_EXTRATAG:";

		private const int DOWNLOAD_FAILED = -1;

		public PublicationExtInfo()
		{
		}

		public int BookId
		{
			get;
			set;
		}

		private int downloadingProgress;
		public int DownloadingProgress
		{
			get
			{
				return downloadingProgress < 0 ? 0 : downloadingProgress;
			}

			set
			{
				if(value < 0 || value > 100)
				{
					throw new ArgumentOutOfRangeException("value", "DownloadingProgress should be in [0, 100]");
				}

				downloadingProgress = value;
			}
		}

		public bool DownloadFailed
		{
			get
			{
				return downloadingProgress == DOWNLOAD_FAILED;
			}

			set
			{
				if(value)
				{
					downloadingProgress = DOWNLOAD_FAILED;
				}
				else
				{
					throw new ArgumentOutOfRangeException("value", "Unable to set the DownloadFailed to false. Please set the DownloadingProgress to 0.");
				}
			}
		}

		public CancellationTokenSource CancellationSource
		{
			get;
			set;
		}
	}
}

