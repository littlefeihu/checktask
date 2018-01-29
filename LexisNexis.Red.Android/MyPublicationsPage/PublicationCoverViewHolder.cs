using System;
using Android.Support.V7.Widget;
using Android.Widget;
using Android.Views;
using LexisNexis.Red.Common.BusinessModel;
using Android.Graphics;
using LexisNexis.Red.Droid.App;
using Android.Graphics.Drawables;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Droid.Business;

namespace LexisNexis.Red.Droid.MyPublicationsPage
{
	public class PublicationCoverViewHolder: RecyclerView.ViewHolder
	{
		//private static readonly Typeface Garamond = Typeface.CreateFromAsset(MainApp.ThisApp.Assets, "GARABD.TTF");
		private readonly TextView tvBookTitle;
		private readonly RelativeLayout rlBookBackground;
		private readonly FrameLayout frmBookTitle;
		private readonly ImageView ivPlusCaseMark;

		private readonly TextView tvLoan;
		private readonly TextView tvDaysRemaining;

		private readonly LinearLayout llBottomLine1;
		private readonly LinearLayout llBottomLine2;

		public ObjHolder<Publication> Publication
		{
			get;
			set;
		}

		public PublicationCoverViewHolder(View v): base(v)
		{
			rlBookBackground = v.FindViewById<RelativeLayout>(Resource.Id.rlCoverBookBackground);
			frmBookTitle = v.FindViewById<FrameLayout>(Resource.Id.frmCoverBookTitle);
			tvBookTitle = v.FindViewById<TextView>(Resource.Id.tvCoverBookTitle);
			ivPlusCaseMark = v.FindViewById<ImageView>(Resource.Id.ivPlusCaseMark);

			tvLoan = v.FindViewById<TextView>(Resource.Id.tvCoverLoan);
			tvDaysRemaining = v.FindViewById<TextView>(Resource.Id.tvCoverDaysRemaining);

			llBottomLine1 = v.FindViewById<LinearLayout>(Resource.Id.llBottomLine1);
			llBottomLine2 = v.FindViewById<LinearLayout>(Resource.Id.llBottomLine2);
		}

		public void Update(bool pureCover = false)
		{
			var fontColor = Color.ParseColor(Publication.Value.FontColor);

			rlBookBackground.SetBackgroundColor(Color.ParseColor(Publication.Value.ColorPrimary));
			var frameBackground = (LayerDrawable)frmBookTitle.Background;
			((GradientDrawable)frameBackground.FindDrawableByLayerId(Resource.Id.background))
				.SetColor(Color.ParseColor(Publication.Value.ColorSecondary));
			((GradientDrawable)frameBackground.FindDrawableByLayerId(Resource.Id.outframe))
				.SetStroke(Conversion.Dp2Px(1), fontColor);
			((GradientDrawable)frameBackground.FindDrawableByLayerId(Resource.Id.innerframe))
				.SetStroke(Conversion.Dp2Px(3), fontColor);

			tvBookTitle.Text = Publication.Value.Name;

			tvBookTitle.SetTextColor(fontColor);

			ivPlusCaseMark.Visibility = Publication.Value.IsFTC ? ViewStates.Visible : ViewStates.Gone;

			if(pureCover)
			{
				tvLoan.Visibility = ViewStates.Invisible;
				tvDaysRemaining.Visibility = ViewStates.Invisible;
			}
			else
			{
				tvLoan.SetTextColor(fontColor);
				tvLoan.Visibility = Publication.Value.IsLoan ? ViewStates.Visible : ViewStates.Invisible;

				if((!Publication.Value.IsLoan) || Publication.Value.DaysRemaining < 0)
				{
					tvDaysRemaining.Visibility = ViewStates.Invisible;
				}
				else
				{
					tvDaysRemaining.SetTextColor(fontColor);
					tvDaysRemaining.Visibility = ViewStates.Visible;

					if(Publication.Value.DaysRemaining == 0)
					{
						tvDaysRemaining.Text = MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_DueToExpire);
					}
					else if(Publication.Value.DaysRemaining == 1)
					{
						tvDaysRemaining.Text = MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_1DayRemaining);
					}
					else
					{
						tvDaysRemaining.Text = String.Format(
							MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_DaysRemaining),
							Publication.Value.DaysRemaining);
					}
				}
			}

			llBottomLine1.SetBackgroundColor(fontColor);
			llBottomLine2.SetBackgroundColor(fontColor);
		}
	}
}

