using System;
using System.Collections.Generic;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.Async;
using System.Threading.Tasks;
using System.Threading;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Droid.Business
{
	public class PublicationManager
	{
		private List<PublicationExtInfo> publicationExtInfoList;

		private List<ObjHolder<Publication>> publicationList;
		public List<ObjHolder<Publication>> PublicationList
		{
			get
			{
				if(publicationList == null)
				{
					if(GlobalAccess.Instance.CurrentUserInfo == null)
					{
						SetPublicationList(new List<Publication>());
					}
					else
					{
						SetPublicationList(PublicationUtil.Instance.GetPublicationOffline());
					}
				}

				return publicationList;
			}

			private set
			{
				publicationList = value;
			}
		}

		// private List<ObjHolder<Publication>> publicationList
		// only can be used in this method.
		public void SetPublicationList(List<Publication> list, bool logout = false)
		{
			var oldExtInfoList = publicationExtInfoList;
			publicationExtInfoList = new List<PublicationExtInfo>();

			if(list == null || list.Count == 0)
			{
				if(logout)
				{
					publicationList = null;
					return;
				}

				if(publicationList != null)
				{
					publicationList.Clear();
				}
				else
				{
					publicationList = new List<ObjHolder<Publication>>();
				}

				publicationExtInfoList.Clear();
				return;
			}

			publicationList = new List<ObjHolder<Publication>>(list.Count);
			foreach(var pub in list)
			{
				publicationList.Add(new ObjHolder<Publication>(pub));

				PublicationExtInfo found = null;
				if(oldExtInfoList != null)
				{
					found = oldExtInfoList.Find((p) => pub.BookId == p.BookId);
				}

				if(found == null)
				{
					found = new PublicationExtInfo()
					{
						BookId = pub.BookId,
						DownloadingProgress = 0,
					};
				}

				publicationExtInfoList.Add(found);
			}
		}

		public bool IsPublicationListEmpty()
		{
			return PublicationList.Count == 0;
		}

		public bool IsDownloading(Publication pub)
		{
			return IsDownloading(pub.BookId);
		}

		public bool IsDownloading(int requiredBookId)
		{
			var task = AsyncTaskManager.INSTATNCE.FindTask((t, tag1, tag2) =>
			{
				string taskType = tag1 as string;
				int? bookId = tag2 as int?;

				return (!string.IsNullOrEmpty(taskType))
					&& taskType == PublicationExtInfo.DOWNLOAD_PUBLICATION_TASK
					&& bookId != null
					&& bookId == requiredBookId;
			});

			return task != null;
		}

		public bool IsGettingDetail(Publication pub)
		{
			var task = AsyncTaskManager.INSTATNCE.FindTask((t, tag1, tag2) =>
			{
				string taskType = tag1 as string;
				int? bookId = tag2 as int?;

				return (!string.IsNullOrEmpty(taskType))
					&& taskType == PublicationExtInfo.GET_PUBLICATION_DETAIL_TASK
					&& bookId != null
					&& bookId == pub.BookId;
			});

			return task != null;
		}

		public PublicationExtInfo GetPublicationExtInfo(int bookId)
		{
			var extInfo = publicationExtInfoList.Find((p) => bookId == p.BookId);
			if(extInfo == null)
			{
				throw new ArgumentOutOfRangeException("bookId", "Unable to find the extend information of the publication.");
			}

			return extInfo;
		}

		public void RemovePublication(int bookId)
		{
			int index = PublicationList.FindIndex((p) => bookId == p.Value.BookId);
			if(index >= 0)
			{
				PublicationList.RemoveAt(index);
			}

			index  = publicationExtInfoList.FindIndex((p) => bookId == p.BookId);
			if(index >= 0)
			{
				publicationExtInfoList.RemoveAt(index);
			}
		}

		public ObjHolder<Publication> GetPublication(int bookId)
		{
			return PublicationList.Find((p) => bookId == p.Value.BookId);
		}

		public int GetDownloadProgress(Publication pub)
		{
			var extInfo = publicationExtInfoList.Find((p) => pub.BookId == p.BookId);
			if(extInfo == null)
			{
				throw new ArgumentOutOfRangeException("pub", "Unable to find the extend information of the publication.");
			}

			return extInfo.DownloadingProgress;
		}

		public void SetDownloadProgress(int bookId, int progress)
		{
			var extInfo = publicationExtInfoList.Find((p) => bookId == p.BookId);
			if(extInfo == null)
			{
				throw new ArgumentOutOfRangeException("pub", "Unable to find the extend information of the publication.");
			}

			extInfo.DownloadingProgress = progress;
		}

		public bool IsDownloadFailed(Publication pub)
		{
			var extInfo = publicationExtInfoList.Find((p) => pub.BookId == p.BookId);
			if(extInfo == null)
			{
				throw new ArgumentOutOfRangeException("pub", "Unable to find the extend information of the publication.");
			}

			return extInfo.DownloadFailed;
		}

		public void SetDownloadFailed(int bookId)
		{
			var extInfo = publicationExtInfoList.Find((p) => bookId == p.BookId);
			if(extInfo == null)
			{
				return;
			}

			extInfo.DownloadFailed = true;
		}

		public void UpdatePublication(Publication pub)
		{
			var found = PublicationList.FindIndex((p) => pub.BookId == p.Value.BookId);
			if(found < 0)
			{
				throw new ArgumentOutOfRangeException("pub", "Unable to find the extend information of the publication.");
			}

			PublicationList[found].Value = pub;
		}

		public async void DownloadPublication(IAsyncTaskActivity activity, int bookId, bool force)
		{
			if(IsDownloading(bookId))
			{
				throw new ArgumentException("The required book is downloading.", "bookId");
			}

			var extInfo = publicationExtInfoList.Find(p => bookId == p.BookId);
			extInfo.CancellationSource = new CancellationTokenSource();
			extInfo.DownloadingProgress = 0;

			var downloadTask = PublicationUtil.Instance.DownloadPublicationByBookId(
				bookId,
				extInfo.CancellationSource.Token,
				(int progress, long downloadSize) =>
					AsyncUIOperationRepeater.INSTATNCE.SubmitAsyncUIOperation(
						activity,
						currentActivity => OnPublicationDownloadProgress(currentActivity, bookId, progress),
						true,
						currentActivity => currentActivity is IDownloadPublicationTaskListener),
				!force);

			AsyncTaskManager.INSTATNCE.RegisterTask(downloadTask, PublicationExtInfo.DOWNLOAD_PUBLICATION_TASK, bookId);

			((IDownloadPublicationTaskListener)activity).UpdatePublicationDownloadingProgress(bookId);

			var result = await downloadTask;

			if(!AsyncTaskManager.INSTATNCE.IsBelongToCurrentUser(downloadTask))
			{
				// The task owner has logged off
				return;
			}

			// The CancellationSource is useless now.
			extInfo.CancellationSource.Dispose();
			extInfo.CancellationSource = null;

			// All status about download publication should be update here;
			// Because the AsyncUIOperation will be set as discardable;
			// If the download result is coming and the Activity is not
			// showing on the screen, the all status about the result will be lost;
			// Like the bug: http://wiki.lexiscn.com/issues/13206.
			if(result.DownloadStatus == DownLoadEnum.Success)
			{
				DataCache.INSTATNCE.PublicationManager.UpdatePublication(result.Publication);
				DataCache.INSTATNCE.PublicationManager.SetDownloadProgress(result.Publication.BookId, 100);
			}
			else if(result.DownloadStatus == DownLoadEnum.Canceled
				|| result.DownloadStatus == DownLoadEnum.Failure
				|| result.DownloadStatus == DownLoadEnum.NetDisconnected)
			{
				DataCache.INSTATNCE.PublicationManager.SetDownloadFailed(bookId);
			}
			/*
			else if(result.DownloadStatus == DownLoadEnum.NetDisconnected)
			{
				DataCache.INSTATNCE.PublicationManager.SetDownloadProgress(bookId, 0);
			}
			*/

			AsyncTaskManager.INSTATNCE.UnregisterTask(downloadTask);

			AsyncUIOperationRepeater.INSTATNCE.SubmitAsyncUIOperation(
				activity,
				currentActivity=>
			{
				var pub = DataCache.INSTATNCE.PublicationManager.GetPublication(bookId);
				if(pub == null)
				{
					return;
				}

				if(pub.Value.PublicationStatus == PublicationStatusEnum.Downloaded)
				{
					// to fix the download status due to unknown reason.
					// releated bug: http://wiki.lexiscn.com/issues/12947
					result.DownloadStatus = DownLoadEnum.Success;
				}

				((IDownloadPublicationTaskListener)currentActivity).ProcessDownloadResult(result, bookId);
			},
				true,
				currentActivity => currentActivity is IDownloadPublicationTaskListener
			);
		}

		public void CancelAllDownloadingTask()
		{
			foreach(var extInfo in publicationExtInfoList)
			{
				if(extInfo.CancellationSource != null)
				{
					extInfo.CancellationSource.Cancel();
				}
			}
		}

		/*
		public async Task<DownloadResult> DownloadPublicationByBookId(int bookId, CancellationToken cancelToken, DownloadProgressEventHandler downloadHandler, bool checkNetLimitation = true)
		{
			return await Task.Run(delegate{
				for(int i = 0; i < 100; ++i)
				{
					if(cancelToken.IsCancellationRequested)
					{
						return new DownloadResult(){
							DownloadStatus = DownLoadEnum.Canceled,
							Publication = GetPublication(bookId).Value,
						};
					}

					Thread.Sleep(100);
					if(cancelToken.IsCancellationRequested)
					{
						return new DownloadResult(){
							DownloadStatus = DownLoadEnum.Canceled,
							Publication = GetPublication(bookId).Value,
						};
					}

					downloadHandler(i, 100);
				}

				return new DownloadResult(){
					DownloadStatus = DownLoadEnum.Success,
					Publication = GetPublication(bookId).Value,
				};
			});
		}
		*/

		private void OnPublicationDownloadProgress(IAsyncTaskActivity currentActivity, int bookId, int progress)
		{
			var foundPub = PublicationList.Find(p => p.Value.BookId == bookId);
			if(foundPub == null)
			{
				return;
			}

			var foundExtInfo = publicationExtInfoList.Find(p => bookId == p.BookId);
			if(foundExtInfo == null)
			{
				return;
			}

			if(progress != 100 && progress - foundExtInfo.DownloadingProgress < 2)
			{
				return;
			}

			foundExtInfo.DownloadingProgress = progress;

			((IDownloadPublicationTaskListener)currentActivity).UpdatePublicationDownloadingProgress(bookId);
		}
	}
}

