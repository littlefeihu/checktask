using System;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class SearchResultListAdaptor: RecyclerView.Adapter
	{
		private readonly ContentSearchActivity activity;
		private readonly List<SearchDisplayResult> resultList;

		public Action<SearchDisplayResult> OnItemClick
		{
			private set;
			get;
		}

		public Action<bool> OnUpdateResult
		{
			private set;
			get;
		}

		public ContentSearchActivity Activity
		{
			get
			{
				return activity;
			}
		}

		public SearchResultListAdaptor(
			ContentSearchActivity activity,
			Action<SearchDisplayResult> onItemClick,
			Action<bool> onUpdateResult)
		{
			resultList = new List<SearchDisplayResult>();
			this.activity = activity;
			OnItemClick = onItemClick;
			OnUpdateResult = onUpdateResult;
		}

		public void UpdateSearchResult()
		{
			resultList.Clear();
			if(DataCache.INSTATNCE.SearchResult == null
				|| DataCache.INSTATNCE.SearchResult.Result == null)
			{
				NotifyDataSetChanged();
				return;
			}

			switch(activity.Status.FilterId)
			{
			case Resource.Id.tvFilterAll:
				{
					DataCache.INSTATNCE.SearchResult.Result.SearchDisplayResultList.ForEach(resultList.Add);
				}
				break;
			case Resource.Id.tvFilterLegislation:
				{
					DataCache.INSTATNCE.SearchResult.Result.SearchDisplayResultList.ForEach(r => {
						if(r.ContentType == ContentCategory.LegislationType)
						{
							resultList.Add(r);
						}
					});
				}
				break;
			case Resource.Id.tvFilterCommentary:
				{
					DataCache.INSTATNCE.SearchResult.Result.SearchDisplayResultList.ForEach(r => {
						if(r.ContentType == ContentCategory.CommentaryType)
						{
							resultList.Add(r);
						}
					});
				}
				break;
			case Resource.Id.tvFilterFormsPrecedents:
				{
					DataCache.INSTATNCE.SearchResult.Result.SearchDisplayResultList.ForEach(r => {
						if(r.ContentType == ContentCategory.FormsPrecedentsType)
						{
							resultList.Add(r);
						}
					});
				}
				break;
			}

			NotifyDataSetChanged();

			if(OnUpdateResult != null)
			{
				OnUpdateResult(resultList.Count > 0);
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From(parent.Context).Inflate(
				Resource.Layout.contentpage_searchresult_item, parent, false);
			v.LayoutParameters = new LinearLayout.LayoutParams(
				ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			var vh = new SearchResultListViewHolder(v, this);
			return vh;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var vh = (SearchResultListViewHolder)holder;
			vh.Update(resultList[position], position);
		}

		public override int ItemCount
		{
			get
			{
				return resultList.Count;
			}
		}

		public object At(int position)
		{
			return resultList[position];
		}

		public bool IsShowResultType(int position)
		{
			if(position == 0)
			{
				return true;
			}

			return resultList[position].isDocument != resultList[position - 1].isDocument;
		}
	}
}

