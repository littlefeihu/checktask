
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
using System.Threading;
using LexisNexis.Red.Droid.Widget.ListView;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class IndexPanelFragment : Fragment, IRefreshableFragment
	{
		public const string FragmentTag = "IndexPanelFragment";

		private ListView lvIndexList;
		private IndexListAdaptor indexListAdaptor;
		private TextView tvLeftTopIndex;
		private LinearLayout llNoIndexContainer;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.contentpage_indexpanel_fragment, container, false);

			if(((ContentActivity)Activity).Publication == null)
			{
				return v;
			}

			lvIndexList = v.FindViewById<ListView>(Resource.Id.lvIndexList);
			tvLeftTopIndex = v.FindViewById<TextView>(Resource.Id.tvLeftTopIndex);
			llNoIndexContainer = v.FindViewById<LinearLayout>(Resource.Id.llNoIndexContainer);

			indexListAdaptor = new IndexListAdaptor(Activity as ContentActivity);
			lvIndexList.FastScrollEnabled = true;
			lvIndexList.Adapter = indexListAdaptor;

			lvIndexList.Scroll += OnIndexListScroll;
			lvIndexList.ItemClick += OnIndexListItemClick;

			GetIndexList();
			return v;
		}

		public override void OnHiddenChanged(bool hidden)
		{
			base.OnHiddenChanged(hidden);

			if(!hidden
				&&
				(DataCache.INSTATNCE.IndexList == null
					|| DataCache.INSTATNCE.IndexList.IndexList == null
					|| DataCache.INSTATNCE.IndexList.Publication == null
					|| DataCache.INSTATNCE.IndexList.Publication.Value.BookId != NavigationManagerHelper.GetCurrentBookId()))
			{
				GetIndexList();
			}
		}

		private void OnIndexListScroll (object sender, AbsListView.ScrollEventArgs e)
		{
			var index = indexListAdaptor[e.FirstVisibleItem];
			SetIndexIndicator(index == null ? ' ' : index.Title[0]);
		}

		private void OnIndexListItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			DataCache.INSTATNCE.IndexList.CurrentIndex = e.Position;
			DataCache.INSTATNCE.IndexList.UserSelectedIndexTitle = DataCache.INSTATNCE.IndexList.GetCurrentIndex().Title;
			OpenIndexPage();
		}

		private void OpenIndexPage()
		{
			((ContentActivity)Activity).GetMainFragment().OpenIndexPage();
		}

		public override void OnResume()
		{
			base.OnResume();

			if(((ContentActivity)Activity).Publication == null)
			{
				return;
			}

			var publication = ((ContentActivity)Activity).Publication;
			if(DataCache.INSTATNCE.IndexList ==null
				|| !DataCache.INSTATNCE.IndexList.IsCurrentPublication(publication.Value.BookId))
			{
				GetIndexList();
			}
		}

		private void GetIndexList()
		{
			var publication = ((ContentActivity)Activity).Publication;

			if(DataCache.INSTATNCE.IndexList ==null
				|| !DataCache.INSTATNCE.IndexList.IsCurrentPublication(publication.Value.BookId))
			{
				var indexTree = AsyncHelpers.RunSync(() => {
					return PublicationContentUtil.Instance.GetIndexsByBookId(publication.Value.BookId);});

				List<Index> indexList = null;

				if(indexTree != null)
				{
					indexList = new List<Index>();
					foreach(var pair in indexTree)
					{
						indexList.AddRange(pair.Value);
					}
				}

				DataCache.INSTATNCE.IndexList.SetPublicationIndex(((ContentActivity)Activity).Publication, indexList);
			}

			if(DataCache.INSTATNCE.IndexList.IndexList != null)
			{
				llNoIndexContainer.Visibility = ViewStates.Invisible;
				indexListAdaptor.RefreshIndexList();
				ListViewFastScrollHelper.ForceRecheckItemCount(lvIndexList, indexListAdaptor);

				if(indexListAdaptor.Count > 0)
				{
					var index = indexListAdaptor[0];
					SetIndexIndicator(index == null ? ' ' : index.Title[0]);
				}
			}
			else
			{
				indexListAdaptor.RefreshIndexList();
				llNoIndexContainer.Visibility = ViewStates.Visible;
				SetIndexIndicator(' ');
			}

			OpenIndexPage();

			return;
		}

		private void SetIndexIndicator(char indexIndicator)
		{
			if(indexIndicator == ' ')
			{
				tvLeftTopIndex.Visibility = ViewStates.Invisible;
			}
			else
			{
				tvLeftTopIndex.Visibility = ViewStates.Visible;
				tvLeftTopIndex.Text = indexIndicator.ToString();
			}
		}

		public void Refresh()
		{
			GetIndexList();
		}
	}
}

