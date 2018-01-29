using System;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using LexisNexis.Red.Common.BusinessModel;
using System.Collections.Generic;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.Business;
using Android.Text;
using Android.Text.Style;
using Annotation=LexisNexis.Red.Common.BusinessModel.Annotation;

namespace LexisNexis.Red.Droid.AnnotationUtility
{
	public class AnnotationListAdaptorViewHolder: RecyclerView.ViewHolder
	{
		private readonly static string[] fakeTags = {"Green", "Red", "Custom name here", "Indigo"};
		private readonly static LinearLayout.LayoutParams TagLineLayoutParams;
		private readonly static LinearLayout.LayoutParams TagItemLayoutParams;
		private readonly static Color HighlightBackgroundColor;

		private readonly static Random r = new Random();

		private readonly static Color[] FakeTagColors;

		static AnnotationListAdaptorViewHolder()
		{
			HighlightBackgroundColor = Color.ParseColor("#FFE993");

			var colors = AnnCategoryTagUtil.Instance.GetTagColors();
			FakeTagColors = new Color[colors.Count];
			for(int i = 0; i < colors.Count; ++i)
			{
				FakeTagColors[i] = Color.ParseColor(colors[i].ColorValue);
			}

			TagLineLayoutParams = new LinearLayout.LayoutParams(
				ViewGroup.LayoutParams.MatchParent,
				ViewGroup.LayoutParams.WrapContent);
			TagLineLayoutParams.Gravity = GravityFlags.CenterVertical;
			TagLineLayoutParams.TopMargin = Conversion.Dp2Px(3);
			TagLineLayoutParams.BottomMargin = Conversion.Dp2Px(3);

			TagItemLayoutParams = new LinearLayout.LayoutParams(
				ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			TagItemLayoutParams.Gravity = GravityFlags.CenterVertical;
		}

		private readonly AnnotationListAdaptor adaptor;

		public Annotation Annotation
		{
			get;
			set;
		}

		private readonly LinearLayout llContainer;
		private readonly TextView tvDate;
		private readonly TextView tvPublicationTitle;
		private readonly TextView tvTocTitle;
		private readonly TextView tvExcerptText;
		private readonly TextView tvAddANote;
		private readonly TextView tvNote;
		private readonly LinearLayout llTagContainer;

		public AnnotationListAdaptorViewHolder(View v, AnnotationListAdaptor adaptor): base(v)
		{
			this.adaptor = adaptor;

			llContainer = v.FindViewById<LinearLayout>(Resource.Id.llContainer);
			tvDate = v.FindViewById<TextView>(Resource.Id.tvDate);
			tvPublicationTitle = v.FindViewById<TextView>(Resource.Id.tvPublicationTitle);
			tvTocTitle = v.FindViewById<TextView>(Resource.Id.tvTocTitle);
			tvExcerptText = v.FindViewById<TextView>(Resource.Id.tvExcerptText);
			tvAddANote = v.FindViewById<TextView>(Resource.Id.tvAddANote);
			tvNote = v.FindViewById<TextView>(Resource.Id.tvNote);
			llTagContainer = v.FindViewById<LinearLayout>(Resource.Id.llTagContainer);

			llContainer.SetOnClickListener(new ItemClickListener(this));
		}

		private class ItemClickListener: Java.Lang.Object, View.IOnClickListener
		{
			private readonly AnnotationListAdaptorViewHolder vh;

			public ItemClickListener(AnnotationListAdaptorViewHolder vh)
			{
				this.vh = vh;
			}

			public void OnClick(View v)
			{
			}
		}

		public void Update()
		{
			tvDate.Text = Annotation.UpdatedTime.ToString();

			if(tvPublicationTitle != null)
			{
				tvPublicationTitle.Text = Annotation.BookTitle;
			}

			tvTocTitle.Text = Annotation.TOCTitle;
			tvExcerptText.TextFormatted = HighLightExcerptText(Annotation.HighlightText);

			if(string.IsNullOrEmpty(Annotation.NoteText))
			{
				tvAddANote.Visibility = ViewStates.Visible;
				tvNote.Visibility = ViewStates.Gone;
			}
			else
			{
				tvAddANote.Visibility = ViewStates.Gone;
				tvNote.Visibility = ViewStates.Visible;
				tvNote.Text = Annotation.NoteText;
			}

			llTagContainer.RemoveAllViews();
			var currentLineWidth = 0;
			LinearLayout currentLine = CreateNewTagsLine();
			/*
			var tagCount = r.Next(1, 10);
			for(int i = 0; i < tagCount; ++i)
			{
				var annotation_tagitem = adaptor.Fragment.Activity.LayoutInflater.Inflate(
					Resource.Layout.annotation_tagitem, null);
				annotation_tagitem.FindViewById<TextView>(Resource.Id.tvTagTitle).Text = fakeTags[r.Next(0, 4)];
				annotation_tagitem.Measure(
					View.MeasureSpec.MakeMeasureSpec(ViewGroup.LayoutParams.WrapContent, MeasureSpecMode.Unspecified),
					View.MeasureSpec.MakeMeasureSpec(ViewGroup.LayoutParams.WrapContent, MeasureSpecMode.Unspecified));

				if(currentLineWidth + annotation_tagitem.MeasuredWidth > TagContainerWidth)
				{
					currentLine = CreateNewTagsLine();
					currentLineWidth = 0;
				}

				currentLine.AddView(annotation_tagitem, TagItemLayoutParams);
				currentLineWidth += annotation_tagitem.MeasuredWidth;
			}
			*/
			//*
			foreach(var tag in fakeTags)
			{
				var annotation_tagitem = adaptor.Fragment.Activity.LayoutInflater.Inflate(
					Resource.Layout.annotation_tagitem, null);

				var ivTagIcon = annotation_tagitem.FindViewById<ImageView>(Resource.Id.ivTagIcon);
				ivTagIcon.SetImageResource(Resource.Drawable.tag_round_small_icon);
				var icon = ivTagIcon.Drawable;
				((GradientDrawable)icon).SetColor(FakeTagColors[r.Next(0, FakeTagColors.Length)]);

				annotation_tagitem.FindViewById<TextView>(Resource.Id.tvTagTitle).Text = tag;
				annotation_tagitem.Measure(
					View.MeasureSpec.MakeMeasureSpec(ViewGroup.LayoutParams.WrapContent, MeasureSpecMode.Unspecified),
					View.MeasureSpec.MakeMeasureSpec(ViewGroup.LayoutParams.WrapContent, MeasureSpecMode.Unspecified));

				if(currentLineWidth + annotation_tagitem.MeasuredWidth > adaptor.TagContainerWidth)
				{
					currentLine = CreateNewTagsLine();
					currentLineWidth = 0;
				}

				currentLine.AddView(annotation_tagitem, TagItemLayoutParams);
				currentLineWidth += annotation_tagitem.MeasuredWidth;
			}
			//*/
		}

		private LinearLayout CreateNewTagsLine()
		{
			var ll = new LinearLayout(adaptor.Fragment.Activity);
			ll.Orientation = Orientation.Horizontal;
			llTagContainer.AddView(ll, TagLineLayoutParams);
			return ll;
		}

		private static ISpanned HighLightExcerptText(string excerptText)
		{
			var ssb = new SpannableStringBuilder(excerptText);
			ssb.SetSpan(new BackgroundColorSpan(HighlightBackgroundColor), 0, excerptText.Length, 0);
			return ssb;
		}
	}
}

