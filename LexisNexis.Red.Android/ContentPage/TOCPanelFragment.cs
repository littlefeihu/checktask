
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Fragment=Android.Support.V4.App.Fragment;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.Async;
using System.Threading.Tasks;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Common.BusinessModel;
using Android.Support.V4.App;
using LexisNexis.Red.Droid.WebViewUtility;
using System.Threading;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.DetailInfoModal;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class TOCPanelFragment : Fragment, IRefreshableFragment
	{
		public const string FragmentTag = "TOCPanelFragment";

		private LinearLayout llExpiredInfoContainer;
		private TextView tvExpiredCurrencyData;

		private RecyclerView rcTOCList;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		LinearLayoutManager layoutManager;

		TOCListAdaptor tocListAdaptor;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.contentpage_tocpanel_fragment, container, false);

			if(((ContentActivity)Activity).Publication == null)
			{
				return v;
			}

			rcTOCList = v.FindViewById<RecyclerView>(Resource.Id.rcTOCList);

			llExpiredInfoContainer = v.FindViewById<LinearLayout>(Resource.Id.llExpiredInfoContainer);
			tvExpiredCurrencyData = v.FindViewById<TextView>(Resource.Id.tvExpiredCurrencyData);

			//llExpiredInfoContainer.Click +=
			//	(object sender, EventArgs e) =>
			//	PublicationDetailInfoFragment.NewInstance(
			//		((ContentActivity)Activity).Publication.Value.BookId, true)
			//		.Show(Activity.SupportFragmentManager);

			layoutManager = new LinearLayoutManager (Activity);
			layoutManager.Orientation = LinearLayoutManager.Vertical;
			rcTOCList.SetLayoutManager (layoutManager);
			tocListAdaptor = new TOCListAdaptor ((ContentActivity)Activity);
			rcTOCList.SetAdapter (tocListAdaptor);

			UpdateExpiredInfo();

			return v;
		}

		public override void OnResume()
		{
			base.OnResume();

			var publication = ((ContentActivity)Activity).Publication;

			if(publication == null)
			{
				return;
			}

			if(DataCache.INSTATNCE.Toc == null
			   || !DataCache.INSTATNCE.Toc.IsCurrentPublication(publication.Value.BookId))
			{
				GetTocList();
			}
			else
			{
				tocListAdaptor.RefreshNodeList();
			}
		}

		public override void OnHiddenChanged(bool hidden)
		{
			base.OnHiddenChanged(hidden);

			if(!hidden
			   &&
			   (DataCache.INSTATNCE.Toc == null || !DataCache.INSTATNCE.Toc.IsCurrentNavigationItem()))
			{
				GetTocList();
			}
			else
			{
				tocListAdaptor.RefreshNodeList();
			}
		}

		private void UpdateExpiredInfo()
		{
			var pub = ((ContentActivity)Activity).Publication;

			var tag = llExpiredInfoContainer.Tag as JavaObjWrapper<int>;
			if(tag == null || tag.Value != pub.Value.BookId)
			{
				if(pub.Value.DaysRemaining < 0)
				{
					llExpiredInfoContainer.Visibility = ViewStates.Visible;
					tvExpiredCurrencyData.Text = String.Format(
						MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_CurrencyDate),
						pub.Value.CurrencyDate.Value.ToString("dd MMM yyyy"));
				}
				else
				{
					llExpiredInfoContainer.Visibility = ViewStates.Gone;
				}

				llExpiredInfoContainer.Tag = new JavaObjWrapper<int>(pub.Value.BookId);
			}
		}

		private void GetTocList()
		{
			var publication = ((ContentActivity)Activity).Publication;
			UpdateExpiredInfo();

			if(DataCache.INSTATNCE.Toc !=null
				&& DataCache.INSTATNCE.Toc.IsCurrentPublication(publication.Value.BookId))
			{
				SyncStatus(publication.Value);
				return;
			}

			tocListAdaptor.RefreshNodeList();

			var runningTask = AsyncTaskManager.INSTATNCE.FindTask((t, tag1, tag2) =>
			{
				string taskType = tag1 as string;
				var bookId = tag2 as int?;

				return (!string.IsNullOrEmpty(taskType))
					&& taskType == TOCBreadcrumbs.GetTOCListTASK
					&& bookId != null
					&& bookId == publication.Value.BookId;
			});

			if(runningTask != null)
			{
				return;
			}

			var task = PublicationUtil.Instance.GetDlBookTOC(publication.Value.BookId);
			AsyncTaskManager.INSTATNCE.RegisterTask(task, TOCBreadcrumbs.GetTOCListTASK, publication.Value.BookId);

			LogHelper.Debug("dbg", "RotateWait::ShowWait");
			((ContentActivity)Activity).ShowPleaseWaitDialog();

			Application.SynchronizationContext.Post(async delegate
			{
				var result = await task;
				if(!AsyncTaskManager.INSTATNCE.UnregisterTask(task))
				{
					// The task owner has been logged off.
					return;
				}

				DataCache.INSTATNCE.Toc = new TOCBreadcrumbs(((ContentActivity)Activity).Publication, result);

				AsyncUIOperationRepeater.INSTATNCE.SubmitAsyncUIOperation(
					(IAsyncTaskActivity)Activity,
					a =>
				{
					SyncStatus(publication.Value);
				});
			}, null);
		}

		private void SyncStatus(Publication pub)
		{
			if(!DataCache.INSTATNCE.Toc.IsCurrentNavigationItem())
			{
				var tocid = NavigationManagerHelper.ContentsTabGetTocId(NavigationManager.Instance.CurrentRecord);
				if(!NavigationManagerHelper.IsShowContentRecord(tocid))
				{
					BrowserRecord record = DataCache.INSTATNCE.Toc.GetNavigationItem();
					if(record == null)
					{
						record = NavigationManagerHelper.GetLastReasonableRecord(
							new List<RecordType>{ RecordType.ContentRecord, RecordType.SearchResultRecord });
					}

					if(record == null
						|| (tocid = NavigationManagerHelper.ContentsTabGetTocId(record)) == NavigationManagerHelper.TocIdDefaultPage)
					{
						DataCache.INSTATNCE.Toc.CurrentTOCNode = DataCache.INSTATNCE.Toc.GetFirstPage();
					}
					else
					{
						DataCache.INSTATNCE.Toc.SetCurrentTOCNodeById(tocid);
					}

					tocListAdaptor.RefreshNodeList();
					((ContentActivity)Activity).GetMainFragment().OpenContentPage();
					return;
				}

				if(tocid == NavigationManagerHelper.TocIdDefaultPage
				   && NavigationManager.Instance.Records.Count > 1)
				{
					DataCache.INSTATNCE.Toc.CurrentTOCNode = DataCache.INSTATNCE.Toc.GetFirstPage();
				}
				else
				{
					DataCache.INSTATNCE.Toc.SetCurrentTOCNodeById(tocid);
				}

				DataCache.INSTATNCE.Toc.BindNavigationItem();
			}

			tocListAdaptor.RefreshNodeList();
			((ContentActivity)Activity).GetMainFragment().OpenContentPage();
			return;
		}

		public void Refresh()
		{
			GetTocList();
		}
	}
}

