using System;
using Android.Support.V7.Widget;
using Android.App;
using Android.Widget;
using Android.Views;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.AlertDialogUtility;
using Android.Content;
using System.Threading.Tasks;
using System.Threading;
using LexisNexis.Red.Droid.Utility;
using System.Globalization;
using Android.Graphics;

namespace LexisNexis.Red.Droid.RecentHistory
{
	public class HistoryListAdaptorViewHolder: RecyclerView.ViewHolder
	{
		private HistoryListAdaptor adaptor;
		private Action<RecentHistoryItem> onClickAction;

		private readonly FrameLayout flDivider;
		private readonly FrameLayout flPublicationBar;
		private readonly TextView tvPublicationTitle;
		private readonly TextView tvTOCTitle;
		private readonly TextView tvLastReadDate;
		private readonly LinearLayout llItemBackground;

		public RecentHistoryItem RecentHistory
		{
			get;
			private set;
		}

		public HistoryListAdaptorViewHolder(View v, Action<RecentHistoryItem> onClickAction): base(v)
		{
			this.onClickAction = onClickAction;
			llItemBackground = v.FindViewById<LinearLayout>(Resource.Id.llItemBackground);

			flDivider = v.FindViewById<FrameLayout>(Resource.Id.flDivider);
			flPublicationBar = v.FindViewById<FrameLayout>(Resource.Id.flPublicationBar);
			tvPublicationTitle = v.FindViewById<TextView>(Resource.Id.tvPublicationTitle);
			tvTOCTitle = v.FindViewById<TextView>(Resource.Id.tvTOCTitle);

			tvLastReadDate = v.FindViewById<TextView>(Resource.Id.tvLastReadDate);

			llItemBackground.SetOnClickListener(new OnItemClickListener(this));
		}

		public HistoryListAdaptorViewHolder(View v, HistoryListAdaptor adaptor): base(v)
		{
			this.adaptor = adaptor;
			llItemBackground = v.FindViewById<LinearLayout>(Resource.Id.llItemBackground);

			flDivider = v.FindViewById<FrameLayout>(Resource.Id.flDivider);
			flPublicationBar = v.FindViewById<FrameLayout>(Resource.Id.flPublicationBar);
			tvPublicationTitle = v.FindViewById<TextView>(Resource.Id.tvPublicationTitle);
			tvTOCTitle = v.FindViewById<TextView>(Resource.Id.tvTOCTitle);

			tvLastReadDate = v.FindViewById<TextView>(Resource.Id.tvLastReadDate);

			llItemBackground.SetOnClickListener(new OnItemClickListener(this));
		}

		private class OnItemClickListener: Java.Lang.Object, View.IOnClickListener
		{
			private readonly HistoryListAdaptorViewHolder vh;

			public OnItemClickListener(HistoryListAdaptorViewHolder vh)
			{
				this.vh = vh;
			}

			public void OnClick(View v)
			{
				if(!PreventMultipleUIOperation.IsValid(500))
				{
					return;
				}

				if(vh.adaptor != null)
				{
					vh.adaptor.OnItemClick(vh.RecentHistory);
				}
				else if(vh.onClickAction != null)
				{
					vh.onClickAction(vh.RecentHistory);
				}
			}
		}

		public void Update(RecentHistoryItem recentHistory, int position)
		{
			RecentHistory = recentHistory;

			flDivider.Visibility = position == 0 ? ViewStates.Gone : ViewStates.Visible;
			flPublicationBar.SetBackgroundColor(Color.ParseColor(RecentHistory.ColorPrimary));
			tvPublicationTitle.Text = RecentHistory.PublicationTitle;
			tvTOCTitle.Text = RecentHistory.TOCTitle;
			tvLastReadDate.Text = FormateLastReadDate();
		}

		private string FormateLastReadDate()
		{
			return RecentHistory.LastReadDate.ToString("h:m")
				+ RecentHistory.LastReadDate.ToString("tt").ToLower()
				+ RecentHistory.LastReadDate.ToString(", d MMM yyyy");
		}
	}
}

