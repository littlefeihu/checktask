using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.App;
using LexisNexis.Red.Droid.Utility;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class ContentSearchFilterPopup
	{
		private static readonly Color Selected = Color.ParseColor("#ff0000");
		private static readonly Color Normal = Color.ParseColor("#89000000");

		private readonly Activity host;
		private PopupWindow popup;
		private readonly Action<int> filterItemClicked;

		private TextView tvFilterAll;
		private TextView tvFilterLegislation;
		private TextView tvFilterCommentary;
		private TextView tvFilterFormsPrecedents;

		public ContentSearchFilterPopup(Activity host, Action<int> filterItemClicked)
		{
			this.host = host;
			this.filterItemClicked = filterItemClicked;
		}

		public void ShowAsDropDown(View anchor, int x, int y, int selectedFilterId = -1)
		{
			if(popup == null)
			{
				var view = host.LayoutInflater.Inflate(Resource.Layout.contentpage_searchfilter_popup, null);
				tvFilterAll = view.FindViewById<TextView>(Resource.Id.tvFilterAll);
				tvFilterLegislation = view.FindViewById<TextView>(Resource.Id.tvFilterLegislation);
				tvFilterCommentary = view.FindViewById<TextView>(Resource.Id.tvFilterCommentary);
				tvFilterFormsPrecedents = view.FindViewById<TextView>(Resource.Id.tvFilterFormsPrecedents);

				tvFilterAll.Click += OnFilterItemClicked;
				tvFilterLegislation.Click += OnFilterItemClicked;
				tvFilterCommentary.Click += OnFilterItemClicked;
				tvFilterFormsPrecedents.Click += OnFilterItemClicked;

				popup = new PopupWindow(
					view,
					ViewGroup.LayoutParams.WrapContent,
					ViewGroup.LayoutParams.WrapContent,
					true);
				popup.SetBackgroundDrawable(new ColorDrawable(Color.Rgb(255, 255, 255)));
				popup.OutsideTouchable = true;
			}

			if(selectedFilterId >= 0)
			{
				SetFilter(selectedFilterId);
			}

			if(popup.IsShowing)
			{
				return;
			}

			popup.ShowAsDropDown(anchor, x, y);
		}

		public bool IsShowing
		{
			get
			{
				return popup != null && popup.IsShowing;
			}
		}

		public void Dismiss()
		{
			if(popup != null)
			{
				popup.Dismiss();
			}
		}

		private void SetFilter(int Id)
		{
			tvFilterAll.SetTextColor(tvFilterAll.Id == Id ? Selected : Normal);
			tvFilterLegislation.SetTextColor(tvFilterLegislation.Id == Id ? Selected : Normal);
			tvFilterCommentary.SetTextColor(tvFilterCommentary.Id == Id ? Selected : Normal);
			tvFilterFormsPrecedents.SetTextColor(tvFilterFormsPrecedents.Id == Id ? Selected : Normal);
		}

		void OnFilterItemClicked (object sender, EventArgs e)
		{
			var id = ((View)sender).Id;
			SetFilter(id);
			popup.Dismiss();
			if(filterItemClicked != null)
			{
				filterItemClicked(id);
			}
		}
	}
}

