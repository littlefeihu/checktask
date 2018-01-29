using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Business;
using System.Collections.Generic;
using Android.Text;
using System.Text;
using System.Text.RegularExpressions;
using Android.Text.Style;
using Android.Graphics;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class PboGoToResultListViewHolder: RecyclerView.ViewHolder
	{
		private readonly PboGoToResultListAdaptor adaptor;
		private readonly LinearLayout llContainer;
		private readonly TextView tvFileTitle;
		private readonly TextView tvGuideCardTitle;

		private PageSearchItem searchResultItem;

		public PboGoToResultListViewHolder(View v, PboGoToResultListAdaptor adaptor): base(v)
		{
			this.adaptor = adaptor;
			llContainer = v.FindViewById<LinearLayout>(Resource.Id.llContainer);

			tvFileTitle = v.FindViewById<TextView>(Resource.Id.tvFileTitle);
			tvGuideCardTitle = v.FindViewById<TextView>(Resource.Id.tvGuideCardTitle);

			llContainer.SetOnClickListener(new OnItemClickListener(this));
		}

		private class OnItemClickListener: Java.Lang.Object, View.IOnClickListener
		{
			private readonly PboGoToResultListViewHolder vh;

			public OnItemClickListener(PboGoToResultListViewHolder vh)
			{
				this.vh = vh;
			}

			public void OnClick(View v)
			{
				if(vh.adaptor != null)
				{
					vh.adaptor.OnItemClick(
						vh.adaptor.ResultList.Item1,
						vh.searchResultItem);
				}
			}
		}

		public void Update(PageSearchItem searchResultItem)
		{
			this.searchResultItem = searchResultItem;

			tvFileTitle.Text = searchResultItem.FileTitle;
			tvGuideCardTitle.Text = searchResultItem.GuideCardTitle;
		}
	}
}

