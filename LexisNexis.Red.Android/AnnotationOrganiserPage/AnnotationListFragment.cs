using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Fragment=Android.Support.V4.App.Fragment;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common.BusinessModel;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.AnnotationUtility;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.Async;

namespace LexisNexis.Red.Droid.AnnotationOrganiserPage
{
	public class AnnotationListFragment : Fragment, ViewTreeObserver.IOnGlobalLayoutListener
	{
		public const string FragmentTag = "AnnotationListFragment";

		public static readonly string Title = MainApp.ThisApp.Resources.GetString(Resource.String.MyPub_TitleBarTab_Annotations);

		private static readonly int TagContainerTotalWidthMargins;

		public static AnnotationListFragment NewInstance()
		{
			return new AnnotationListFragment();
		}

		private static readonly List<Annotation> FakeAnnotationList;
		static AnnotationListFragment()
		{
			TagContainerTotalWidthMargins = (int)(MainApp.ThisApp.Resources.GetDimension(
				Resource.Dimension.contentpage_annotationpanel_item_leftpadding));

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
			for(int i = 0; i < FakeAnnotationList.Count; ++i)
			{
				FakeAnnotationList[i].BookTitle = "Fake book title " + (i + 1);
			}
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		private RadioGroup rgAnnotationTypeFilter;
		private RecyclerView rvTagFilter;
		private RecyclerView rvAnnotationList;

		private LinearLayoutManager tagFilterLayoutManager;
		private static Tuple<string, TagFilterAdaptor> cachedAdaptor;
		private LinearLayoutManager annotationListLayoutManager;
		private AnnotationListAdaptor alrvAdaptor;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.annotationlist_fragment, container, false);

			rgAnnotationTypeFilter = v.FindViewById<RadioGroup>(Resource.Id.rgAnnotationTypeFilter);
			rvTagFilter = v.FindViewById<RecyclerView>(Resource.Id.rvTagFilter);
			rvAnnotationList = v.FindViewById<RecyclerView>(Resource.Id.rvAnnotationList);

			tagFilterLayoutManager = new LinearLayoutManager (Activity);
			tagFilterLayoutManager.Orientation = LinearLayoutManager.Vertical;
			rvTagFilter.SetLayoutManager (tagFilterLayoutManager);

			if(cachedAdaptor == null || cachedAdaptor.Item1 != ((IAsyncTaskActivity)Activity).AsyncTaskActivityGUID)
			{
				cachedAdaptor = new Tuple<string, TagFilterAdaptor>(
					((IAsyncTaskActivity)Activity).AsyncTaskActivityGUID,
					new TagFilterAdaptor(this));
			}

			cachedAdaptor.Item2.SetTagList(AnnCategoryTagUtil.Instance.GetTags());
			rvTagFilter.SetAdapter(cachedAdaptor.Item2);

			annotationListLayoutManager = new LinearLayoutManager (Activity);
			annotationListLayoutManager.Orientation = LinearLayoutManager.Vertical;
			rvAnnotationList.SetLayoutManager (annotationListLayoutManager);
			alrvAdaptor = new AnnotationListAdaptor(
				this,
				Resource.Layout.annotationlist_annotationitem,
				TagContainerTotalWidthMargins);
			alrvAdaptor.SetAnnotationList(FakeAnnotationList);
			rvAnnotationList.SetAdapter(alrvAdaptor);

			rvAnnotationList.ViewTreeObserver.AddOnGlobalLayoutListener(this);

			rgAnnotationTypeFilter.CheckedChange += AnnotationTypeFilterCheckedChange;

			return v;
		}

		private void AnnotationTypeFilterCheckedChange (object sender, RadioGroup.CheckedChangeEventArgs e)
		{
			UpdateAnnotationList();
		}

		public void UpdateAnnotationList()
		{
		}

		public void OnGlobalLayout()
		{
			if(rvAnnotationList == null || alrvAdaptor == null)
			{
				return;
			}

			alrvAdaptor.ItemTotalWidth = rvAnnotationList.Width;
		}

		public void Refresh()
		{
			cachedAdaptor.Item2.SetTagList(AnnCategoryTagUtil.Instance.GetTags());
		}
	}
}

