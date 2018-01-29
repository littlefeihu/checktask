using System;

using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public class PublicationCover : UIView
	{
		/// <summary>
		/// Gets or sets the color of the primary.
		/// </summary>
		/// <value>Primary color code in hexadecimal format of publication cover, #FF0000 for example</value>
		public string PrimaryColor { get; set;}

		/// <summary>
		/// Gets or sets the color of the secondary.
		/// </summary>
		/// <value>Secondary color code of publication cover</value>
		public string SecondaryColor { get; set;}

		/// <summary>
		/// Gets or sets the color of the font.
		/// </summary>
		/// <value>Font color of text displayed on publication cover</value>
		public string FontColor { get; set;}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set;}

		/// <summary>
		/// Gets or sets the loan info string which displayed on the bottom of publication cover
		/// </summary>
		/// <value>The loan info string.</value>
		public string LoanInfoStr { get; set;}

		public float ZoomRate { get; set;}

		public UIButton InvisibleOpenPublicationButton{ get; set;}

		public UIView TitleBackgroundView{ get; set;}

		public UIView TitleOuterLineView { get; set;}

		public UIView TitleInnerLineView{ get; set;}

		public UILabel TitleLabel{ get; set;}

		public UIView FirstBottomLine{ get; set;}

		public UIView SecondBottomLine{ get; set;}

		public UILabel LoanTagLabel{ get; set;}

		public PublicationCover (string primaryColor, string secondaryColor, string fontColor, string title, string loanInfoStr, float zoomRate = 1.0f) : base()
		{
			PrimaryColor = primaryColor;
			SecondaryColor = secondaryColor;
			FontColor = fontColor;
			Title = title;
			LoanInfoStr = loanInfoStr;
			ZoomRate = zoomRate;
			InvisibleOpenPublicationButton = new UIButton ();//touch this button will go to publication detail view
			Frame = new CGRect (0, 0, ViewConstant.PUBLICATION_COVER_WIDTH * ZoomRate, ViewConstant.PUBLICATION_COVER_HEIGHT * ZoomRate);

		}
		/*
		public override void Draw (CGRect rect)
		{
			DrawPublicationCoverView ();
			DrawPublicationTitleView ();
			DrawLoanInfo ();

			DrawInvisibleOpenPublicationButton ();
		}
		*/

		public void DrawSubviews ()
		{
			DrawPublicationCoverView ();
			DrawPublicationTitleView ();
			DrawLoanInfo ();

			DrawInvisibleOpenPublicationButton ();
		}

		/// <summary>
		/// Draws the publication cover view.
		/// </summary>
		private void DrawPublicationCoverView ()
		{
			BackgroundColor = ColorUtil.ConvertFromHexColorCode (PrimaryColor);
			Frame = new CGRect (0, 0, ViewConstant.PUBLICATION_COVER_WIDTH * ZoomRate, ViewConstant.PUBLICATION_COVER_HEIGHT * ZoomRate);

			UserInteractionEnabled = true;
		}

		/// <summary>
		/// Initializes the publication title view.
		/// Publication title displayed in publication cover
		/// </summary>
		private void DrawPublicationTitleView ()
		{

			TitleBackgroundView = new UIView ();
			TitleBackgroundView.Frame = new CGRect (ViewConstant.PUBLICATION_TITLE_LABEL_X * ZoomRate, ViewConstant.PUBLICATION_TITLE_LABEL_Y * ZoomRate, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH * ZoomRate, ViewConstant.PUBLICATION_TITLE_LABEL_HEIGHT * ZoomRate);
			TitleBackgroundView.BackgroundColor = ColorUtil.ConvertFromHexColorCode (SecondaryColor);

			TitleOuterLineView = new UIView ();
			TitleOuterLineView.Frame = new CGRect (0, 0, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH * ZoomRate, ViewConstant.PUBLICATION_TITLE_LABEL_HEIGHT * ZoomRate);
			TitleOuterLineView.Layer.BorderWidth = 1.0f ;
			TitleOuterLineView.Layer.BorderColor = ColorUtil.ConvertFromHexColorCode(FontColor).CGColor;
			TitleBackgroundView.AddSubview (TitleOuterLineView);

			TitleInnerLineView = new UIView ();
			TitleInnerLineView.Frame = new CGRect (2, 2, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH  * ZoomRate - 4, ViewConstant.PUBLICATION_TITLE_LABEL_HEIGHT  * ZoomRate - 4);
			TitleInnerLineView.Layer.BorderWidth = 3.0f ;
			TitleInnerLineView.Layer.BorderColor = ColorUtil.ConvertFromHexColorCode(FontColor).CGColor;
			TitleBackgroundView.AddSubview (TitleInnerLineView);


			TitleLabel = new UILabel (new CGRect (5, 5, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH * ZoomRate - 10, ViewConstant.PUBLICATION_TITLE_LABEL_HEIGHT * ZoomRate - 10));
			TitleLabel.TextAlignment = UITextAlignment.Center;
			TitleLabel.Text = Title;
			TitleLabel.TextColor = ColorUtil.ConvertFromHexColorCode(FontColor);
			TitleLabel.Lines = new nint (ViewConstant.PUBLICATION_TITLE_LABEL_LINES);
			TitleLabel.Font = UIFont.FromName ("AppleGaramond-Bold", 17 * ZoomRate);
			TitleLabel.LineBreakMode = UILineBreakMode.MiddleTruncation;
			TitleBackgroundView.AddSubview (TitleLabel);

			AddSubview (TitleBackgroundView);

			FirstBottomLine = new UIView ();
			FirstBottomLine.Frame = new CGRect (15  * ZoomRate, 250 * ZoomRate, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH  * ZoomRate, 2  * ZoomRate);
			FirstBottomLine.BackgroundColor = ColorUtil.ConvertFromHexColorCode(FontColor);
			AddSubview (FirstBottomLine);

			SecondBottomLine = new UIView ();
			SecondBottomLine.Frame = new CGRect (15  * ZoomRate, 253  * ZoomRate, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH  * ZoomRate, 2 * ZoomRate);
			SecondBottomLine.BackgroundColor = ColorUtil.ConvertFromHexColorCode(FontColor);
			AddSubview (SecondBottomLine);
		}

		/// <summary>
		/// Display loan information on publication cover
		/// </summary>
		public void DrawLoanInfo ()
		{
			if (LoanInfoStr != null) {
				LoanTagLabel = new UILabel (new CGRect (ViewConstant.PUBLICATION_TITLE_LABEL_X  * ZoomRate, 200  * ZoomRate, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH  * ZoomRate, 50  * ZoomRate));
				LoanTagLabel.Text = LoanInfoStr;
				LoanTagLabel.TextAlignment = UITextAlignment.Center;
				LoanTagLabel.TextColor = ColorUtil.ConvertFromHexColorCode(FontColor);
				LoanTagLabel.Font = UIFont.FromName ("AppleGaramond-Bold", 17 * ZoomRate);
				LoanTagLabel.Lines = new nint (2);
				AddSubview (LoanTagLabel);
			}
		}

		/// <summary>
		/// Adds the invisible open publication button.
		/// Touch publication cover will go to publication detail view
		/// </summary>
		public void DrawInvisibleOpenPublicationButton ()
		{
			InvisibleOpenPublicationButton.Frame = Frame;
			InvisibleOpenPublicationButton.BackgroundColor = UIColor.Clear;
			AddSubview (InvisibleOpenPublicationButton);
		}

		public void ZoomInOrOut(float rate)
		{
			Frame = new CGRect (0, 0, ViewConstant.PUBLICATION_COVER_WIDTH * rate, ViewConstant.PUBLICATION_COVER_HEIGHT * rate);
			TitleBackgroundView.Frame = new CGRect (ViewConstant.PUBLICATION_TITLE_LABEL_X * rate, ViewConstant.PUBLICATION_TITLE_LABEL_Y * rate, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH * rate, ViewConstant.PUBLICATION_TITLE_LABEL_HEIGHT * rate);
			TitleOuterLineView.Frame = new CGRect (0, 0, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH * rate, ViewConstant.PUBLICATION_TITLE_LABEL_HEIGHT * rate);
			TitleInnerLineView.Frame = new CGRect (2, 2, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH * rate - 4, ViewConstant.PUBLICATION_TITLE_LABEL_HEIGHT * rate - 4);
			TitleLabel.Frame = new  CGRect (5, 5, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH * rate - 10, ViewConstant.PUBLICATION_TITLE_LABEL_HEIGHT * rate - 10);
			TitleLabel.Font = UIFont.FromName ("AppleGaramond-Bold", 17 * rate);
			FirstBottomLine.Frame = new CGRect (15  * rate, 250 * rate, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH  * rate, 2  * rate);
			SecondBottomLine.Frame = new CGRect (15  * rate, 253  * rate, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH  * rate, 2 * rate);
			LoanTagLabel.Frame = new CGRect (ViewConstant.PUBLICATION_TITLE_LABEL_X  * rate, 200  * rate, ViewConstant.PUBLICATION_TITLE_LABEL_WIDTH  * rate, 50  * rate);
			LoanTagLabel.Font = UIFont.FromName ("AppleGaramond-Bold", 17 * rate);

		}
			
	}
}

