using System;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using LexisNexis.Red.Droid.App;

namespace LexisNexis.Red.Droid.AnnotationUtility
{
	public class TagFilterAdaptorViewHolder: RecyclerView.ViewHolder
	{
		private readonly TagFilterAdaptor adaptor;


		private readonly ImageView ivTagIcon;
		private readonly TextView tvTagTitle;
		private readonly ImageView ivTagSelected;

		public SelectableAnnotationTag Tag
		{
			get;
			set;
		}

		public TagFilterAdaptorViewHolder(View v, TagFilterAdaptor adaptor): base(v)
		{
			this.adaptor = adaptor;
			ivTagIcon = v.FindViewById<ImageView>(Resource.Id.ivTagIcon);
			tvTagTitle = v.FindViewById<TextView>(Resource.Id.tvTagTitle);
			ivTagSelected = v.FindViewById<ImageView>(Resource.Id.ivTagSelected);
			ivTagSelected.SetOnClickListener(new CheckButtonClickListener(this));
		}

		private class CheckButtonClickListener: Java.Lang.Object, View.IOnClickListener
		{
			private readonly TagFilterAdaptorViewHolder vh;

			public CheckButtonClickListener(TagFilterAdaptorViewHolder vh)
			{
				this.vh = vh;
			}

			public void OnClick(View v)
			{
				vh.Tag.Selected = !vh.Tag.Selected;
				vh.adaptor.ItemSelectChanged(vh);
			}
		}

		public void Update()
		{
			ivTagSelected.SetImageResource(
				Tag.Selected ? Resource.Drawable.checkbox_checked : Resource.Drawable.checkbox_unchecked);
			
			switch(Tag.Type)
			{
			case SelectableAnnotationTag.TagType.AllTags:
				{
					ivTagIcon.SetImageResource(Resource.Drawable.alltags_small_icon);
					tvTagTitle.Text = MainApp.ThisApp.Resources.GetString(Resource.String.Annotation_TagName_AllTags);
				}
				break;
			case SelectableAnnotationTag.TagType.NoTag:
				{
					ivTagIcon.SetImageResource(Resource.Drawable.notag_round_small_icon);
					tvTagTitle.Text = MainApp.ThisApp.Resources.GetString(Resource.String.Annotation_TagName_NoTag);
				}
				break;
			case SelectableAnnotationTag.TagType.Normal:
				{
					ivTagIcon.SetImageResource(Resource.Drawable.tag_round_small_icon);
					var icon = ivTagIcon.Drawable;
					((GradientDrawable)icon).SetColor(Color.ParseColor(Tag.Tag.Color));
					tvTagTitle.Text = Tag.Tag.Title;
				}
				break;
			default:
				throw new InvalidProgramException("Unknown tag type.");
			}
		}
	}
}

