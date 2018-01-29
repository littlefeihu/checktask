using Android.Support.V7.Widget;
using Android.Views;
using System.Collections.Generic;
using Android.Widget;
using Android.App;
using Android.Content;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.Business;
using Android.Support.V4.App;
using LexisNexis.Red.Droid.Utility;
using System;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.HelpClass;


namespace LexisNexis.Red.Droid.RecentHistory
{
	public class HistoryListAdaptor : RecyclerView.Adapter
	{
		private readonly List<RecentHistoryItem> historyList;
		private FragmentActivity hostActivity;
		private int ItemResourceId;

		public Action<RecentHistoryItem> OnItemClick
		{
			private set;
			get;
		}

		public FragmentActivity Activity
		{
			get
			{
				return hostActivity;
			}
		}

		public HistoryListAdaptor(FragmentActivity activity, int itemResourceId, Action<RecentHistoryItem> onItemClick)
		{
			historyList = new List<RecentHistoryItem>();
			hostActivity = activity;
			OnItemClick = onItemClick;
			ItemResourceId = itemResourceId;
		}

		public void RefreshHisoryList(int? bookId = null)
		{
			if(GlobalAccess.Instance.CurrentUserInfo == null)
			{
				return;
			}

			historyList.Clear ();

			LogHelper.Debug("dbg", "before GetRecentHistory");
			var newHistoryList = PublicationContentUtil.Instance.GetRecentHistory(bookId);
			if(newHistoryList == null)
			{
				LogHelper.Debug("dbg", "After GetRecentHistory = 0");
			}
			else
			{
				LogHelper.Debug("dbg", "After GetRecentHistory = " + newHistoryList.Count);
			}

			if(newHistoryList == null || newHistoryList.Count == 0)
			{
				NotifyDataSetChanged();
				return;
			}
			
			newHistoryList.ForEach(historyList.Add);
			NotifyDataSetChanged();
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From(parent.Context).Inflate(ItemResourceId, parent, false);
			v.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			var vh = new HistoryListAdaptorViewHolder (v, this);
			return vh;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var vh = (HistoryListAdaptorViewHolder)holder;
			vh.Update(historyList[position], position);
		}

		public override int ItemCount
		{
			get{ return historyList.Count; }
		}

		public RecentHistoryItem At(int position)
		{
			return historyList[position];
		}
	}
}