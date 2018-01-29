using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Fragment=Android.Support.V4.App.Fragment;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.AnnotationUtility;
using LexisNexis.Red.Droid.App;
using Newtonsoft.Json;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class AnnotationsPanelFragment :
		Fragment,
		IRefreshableFragment,
		ViewTreeObserver.IOnGlobalLayoutListener
	{
		public const string FragmentTag = "AnnotationsPanelFragment";
		private const string AnnotationsPanelStatusCacheFile = "AnnotationsPanelStatus.{0}.cache";

		private static readonly int TagContainerTotalWidthMargins;

		private static readonly List<Annotation> FakeAnnotationList;
		static AnnotationsPanelFragment()
		{
			TagContainerTotalWidthMargins = (int)(MainApp.ThisApp.Resources.GetDimension(Resource.Dimension.contentpage_annotationpanel_item_leftpadding)
				+ MainApp.ThisApp.Resources.GetDimension(Resource.Dimension.contentpage_annotationpanel_item_rightpadding));

			FakeAnnotationList = new List<Annotation>();
			FakeAnnotationList.Add(new Annotation(
				Guid.NewGuid(),
				AnnotationType.Highlight,
				AnnotationStatusEnum.Updated,
				0,
				0,
				"Fake doc id 1",
				"Fake guide card name 1",
				"Fake note text 1",
				"Fake highlight text 1",
				"Fake toc title 1",
				null, null, null));
			FakeAnnotationList.Add(new Annotation(
				Guid.NewGuid(),
				AnnotationType.Highlight,
				AnnotationStatusEnum.Updated,
				0,
				0,
				"Fake doc id 2",
				"Fake guide card name 2",
				"Fake note text 2",
				"Fake highlight text 2",
				"Fake toc title 2",
				null, null, null));
			FakeAnnotationList.Add(new Annotation(
				Guid.NewGuid(),
				AnnotationType.Highlight,
				AnnotationStatusEnum.Updated,
				0,
				0,
				"Fake doc id 3",
				"Fake guide card name 3",
				"Fake note text 3",
				"Fake highlight text 3",
				"Fake toc title 3",
				null, null, null));
			FakeAnnotationList.Add(new Annotation(
				Guid.NewGuid(),
				AnnotationType.Highlight,
				AnnotationStatusEnum.Updated,
				0,
				0,
				"Fake doc id 4",
				"Fake guide card name 4",
				"Fake note text 4",
				"Fake highlight text 4",
				"Fake toc title 4",
				null, null, null));
			FakeAnnotationList.Add(new Annotation(
				Guid.NewGuid(),
				AnnotationType.Highlight,
				AnnotationStatusEnum.Updated,
				0,
				0,
				"Fake doc id 5",
				"Fake guide card name 5",
				"Fake note text 5",
				"Fake highlight text 5",
				"Fake toc title 5",
				null, null, null));
		}

		private ImageView ivTagFilter;
		private RecyclerView rvAnnotationList;

		private LinearLayoutManager annotationListLayoutManager;
		private AnnotationListAdaptor alrvAdaptor;
		private RadioGroup rgAnnotationTypeFilter;

		private AnnotationsPanelFragmentStatus status = new AnnotationsPanelFragmentStatus();

		public AnnotationsPanelFragmentStatus Status
		{
			get
			{
				return status;
			}
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if(savedInstanceState != null)
			{
				var annotationsPanelStatusCache = FileCacheHelper.ReadCacheFile(
					ContentActivity.CacheCatagory, string.Format(
						AnnotationsPanelStatusCacheFile,
						((ContentActivity)Activity).AsyncTaskActivityGUID));
				
				if(annotationsPanelStatusCache != null)
				{
					status = JsonConvert.DeserializeObject<AnnotationsPanelFragmentStatus>(annotationsPanelStatusCache);
				}
			}

			var v = inflater.Inflate(Resource.Layout.contentpage_annotationspanel_fragment, container, false);

			ivTagFilter = v.FindViewById<ImageView>(Resource.Id.ivTagFilter);
			rvAnnotationList = v.FindViewById<RecyclerView>(Resource.Id.rvAnnotationList);
			rgAnnotationTypeFilter = v.FindViewById<RadioGroup>(Resource.Id.rgAnnotationTypeFilter);

			annotationListLayoutManager = new LinearLayoutManager (Activity);
			annotationListLayoutManager.Orientation = LinearLayoutManager.Vertical;
			rvAnnotationList.SetLayoutManager (annotationListLayoutManager);
			alrvAdaptor = new AnnotationListAdaptor(
				this,
				Resource.Layout.contentpage_annotationlist_annotationitem,
				TagContainerTotalWidthMargins);
			alrvAdaptor.SetAnnotationList(FakeAnnotationList);
			rvAnnotationList.SetAdapter(alrvAdaptor);

			rvAnnotationList.ViewTreeObserver.AddOnGlobalLayoutListener(this);

			ivTagFilter.Click += delegate
			{
				var tagFilterDialogFragment = new TagFilterDialogFragment();
				tagFilterDialogFragment.Show(FragmentManager.BeginTransaction(), TagFilterDialogFragment.FragmentTag);
			};

			rgAnnotationTypeFilter.Check(status.AnnotationTypeFilter);
			rgAnnotationTypeFilter.CheckedChange += AnnotationTypeFilterCheckedChange;

			return v;
		}

		private void AnnotationTypeFilterCheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
		{
			status.AnnotationTypeFilter = rgAnnotationTypeFilter.CheckedRadioButtonId;
			Refresh();
		}

		public override void OnHiddenChanged(bool hidden)
		{
			base.OnHiddenChanged(hidden);
			if(!hidden)
			{
				Refresh();
			}
		}

		public override void OnResume()
		{
			base.OnResume();
			Refresh();
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			FileCacheHelper.SaveCacheFile(
				ContentActivity.CacheCatagory,
				string.Format(
					AnnotationsPanelStatusCacheFile,
					((ContentActivity)Activity).AsyncTaskActivityGUID),
				JsonConvert.SerializeObject(Status));

			base.OnSaveInstanceState(outState);
		}

		public void Refresh()
		{
			LoadTagList();
		}

		public void LoadTagList()
		{
			LoadTocList();
			if(DataCache.INSTATNCE.Toc == null)
			{
				return;
			}

			((ContentActivity)Activity).ClosePleaseWaitPageLoadDialog();

			// LoadTagList Now

		}

		public void LoadTocList()
		{
			var publication = ((ContentActivity)Activity).Publication;

			if(DataCache.INSTATNCE.Toc !=null
				&& DataCache.INSTATNCE.Toc.IsCurrentPublication(publication.Value.BookId))
			{
				//SyncStatus(publication.Value);
				return;
			}

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
					LoadTagList();
				});
			}, null);
		}

		public void OnGlobalLayout()
		{
			if(rvAnnotationList == null || alrvAdaptor == null)
			{
				return;
			}

			alrvAdaptor.ItemTotalWidth = rvAnnotationList.Width;
		}

		public void UpdateSelectedTagFilter(List<string> selectedTagFilter)
		{
			status.SelectedTagFilter = selectedTagFilter;
			Refresh();
		}
	}
}

