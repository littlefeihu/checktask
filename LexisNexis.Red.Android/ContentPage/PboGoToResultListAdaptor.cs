using System;
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class PboGoToResultListAdaptor: RecyclerView.Adapter
	{
		private readonly GoToPageDialogFragment fragment;
		private Tuple<int, List<PageSearchItem>> resultList;

		public Tuple<int, List<PageSearchItem>> ResultList
		{
			get
			{
				return resultList;
			}

			set
			{
				resultList = value;
				NotifyDataSetChanged();
			}
		}

		public Action<int, PageSearchItem> OnItemClick
		{
			private set;
			get;
		}

		public GoToPageDialogFragment Fragment
		{
			get
			{
				return fragment;
			}
		}

		public PboGoToResultListAdaptor(
			GoToPageDialogFragment fragment,
			Action<int, PageSearchItem> onItemClick)
		{
			this.fragment = fragment;
			OnItemClick = onItemClick;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From(parent.Context).Inflate(
				Resource.Layout.contentpage_pbogotopage_resultitem, parent, false);
			v.LayoutParameters = new LinearLayout.LayoutParams(
				ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			var vh = new PboGoToResultListViewHolder(v, this);
			return vh;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var vh = (PboGoToResultListViewHolder)holder;
			vh.Update(resultList.Item2[position]);
		}

		public override int ItemCount
		{
			get
			{
				return resultList == null || resultList.Item2 == null ? 0 : resultList.Item2.Count;
			}
		}

		public object At(int position)
		{
			return resultList.Item2[position];
		}
	}
}

