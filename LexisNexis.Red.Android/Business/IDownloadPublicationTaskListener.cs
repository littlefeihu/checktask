using System;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.Business
{
	public interface IDownloadPublicationTaskListener
	{
		void UpdatePublicationDownloadingProgress(int bookId);
		void ProcessDownloadResult(DownloadResult result, int bookId);
	}
}

