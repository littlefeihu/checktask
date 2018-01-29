using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Business;
using System.Collections.Generic;
using Android.Text;
using System.Text.RegularExpressions;
using Android.Text.Style;
using Android.Graphics;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class SearchResultListViewHolder: RecyclerView.ViewHolder
	{
		private readonly Color HightLightColor = Color.ParseColor("#4c000000");

		private readonly SearchResultListAdaptor adaptor;
		private readonly LinearLayout llContainer;
		private readonly TextView tvResultType;
		private readonly TextView tvTitle;
		private readonly TextView tvSubTitle;
		private readonly TextView tvPageNumber;

		private SearchDisplayResult searchResultItem;

		public SearchResultListViewHolder(View v, SearchResultListAdaptor adaptor): base(v)
		{
			this.adaptor = adaptor;
			llContainer = v.FindViewById<LinearLayout>(Resource.Id.llContainer);

			tvResultType = v.FindViewById<TextView>(Resource.Id.tvResultType);
			tvTitle = v.FindViewById<TextView>(Resource.Id.tvTitle);
			tvSubTitle = v.FindViewById<TextView>(Resource.Id.tvSubTitle);
			tvPageNumber = v.FindViewById<TextView>(Resource.Id.tvPageNumber);

			llContainer.SetOnClickListener(new OnItemClickListener(this));
		}

		private class OnItemClickListener: Java.Lang.Object, View.IOnClickListener
		{
			private readonly SearchResultListViewHolder vh;

			public OnItemClickListener(SearchResultListViewHolder vh)
			{
				this.vh = vh;
			}

			public void OnClick(View v)
			{
				DataCache.INSTATNCE.SearchResult.SelectedResultItem = vh.searchResultItem;

				if(vh.adaptor != null)
				{
					vh.adaptor.OnItemClick(vh.searchResultItem);
				}
			}
		}

		public void Update(SearchDisplayResult searchResultItem, int position)
		{
			this.searchResultItem = searchResultItem;

			if(adaptor.IsShowResultType(position))
			{
				tvResultType.Visibility = ViewStates.Visible;
				tvResultType.Text = MainApp.ThisApp.Resources.GetString(
					searchResultItem.isDocument
					? Resource.String.ContentSearch_ResultType_Document
					: Resource.String.ContentSearch_ResultType_Publication);
			}
			else
			{
				tvResultType.Visibility = ViewStates.Gone;
			}

			tvTitle.TextFormatted = HighLightKeywords(
				searchResultItem.GetFirstLine(),
				DataCache.INSTATNCE.SearchResult.Result.FoundWordList);
			tvSubTitle.TextFormatted = HighLightKeywords(
				searchResultItem.GetSecondLine(),
				DataCache.INSTATNCE.SearchResult.Result.FoundWordList);
			tvPageNumber.Visibility = ViewStates.Invisible;
		}

		private ISpanned HighLightKeywords(string content, List<string> keywords)
		{
			string keys = @"\b(" + string.Join("|", keywords) + @")\b";
			var regex = new Regex(keys);
			var matchs = regex.Matches(content);

			var ssb = new SpannableStringBuilder(content);
			foreach(Match m in matchs)
			{
				ssb.SetSpan(new ForegroundColorSpan(HightLightColor), m.Index, m.Index + m.Value.Length, 0);
				//ssb.SetSpan(new StyleSpan(TypefaceStyle.Bold), m.Index, m.Index + m.Value.Length, 0);
			}

			return ssb;
		}
	}
}

