using System;
using System.Threading;

using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	/// <summary>
	/// Publication view.
	/// Including publication cover and other view displayed in or below publication cover 
	/// </summary>
	public class PublicationView : UIView,  IComparable<PublicationView>
	{

		/// <summary>
		/// Gets or sets the status info string which displayed below publication cover
		/// </summary>
		/// <value>The status info string.</value>
		public string StatusInfoStr { get; set;}

		/// <summary>
		/// Gets or sets the more status info string which displayed below publication cover
		/// </summary>
		/// <value>The more status info string.</value>
		public string MoreStatusInfoStr { get; set;}

		/// <summary>
		/// Gets or sets the info button which displayed below publication cover
		/// Click this button will show info dialog
		/// </summary>
		/// <value>The info button.</value>
		public UIButton InfoButton { get; set;}

		/// <summary>
		/// Gets or sets the tint color of the info button, the value is UIColor.Red when publication has been download
		/// otherwise the value of this property is UIColor.LightGray
		/// </summary>
		/// <value>The color of the info button tint.</value>
		public UIColor InfoButtonTintColor { get; set;}

		/// <summary>
		/// Gets or sets the color of the right top triangle.
		/// Light Blue or Red
		/// </summary>
		/// <value>The color of the right top triangle.</value>
		public UIColor RightTopTriangleColor{ get; set;}

		/// <summary>
		/// Gets or sets the right top hint text.
		/// The value is "Update" or "Expired"
		/// </summary>
		/// <value>The right top hint text.</value>
		public string RightTopHintText{ get; set;}

		/// <summary>
		/// Gets or sets the cover.
		/// </summary>
		/// <value>The cover.</value>
		public PublicationCover Cover{ get; set;}

		/// <summary>
		/// Gets or sets the right top view which displayed on the right top of publication cover
		/// </summary>
		/// <value>The right top view.</value>
		public TriangleView RightTopView { get; set;}


		public UILabel StatusLabel{ get; set;}

		public UILabel MoreStatusLabel{ get; set;}

		public CancellationTokenSource DownloadCancelTokenSource{ get; set;}

		private Publication p;
		public Publication P { 
			get{
				return p;
			}
			set {
				p = value;
				Refresh ();
			}
		}

		public float ZoomRate{get;set;}

		public DoPublicationDownload StartDownload{get; set;}
		public ShowAlert ShowDownloadAlert{ get; set;}

		public PublicationView (float x = 0, float y = 0) : base ()
		{
			Frame = new CGRect (x, y, ViewConstant.PUBLICATION_VIEW_WIDTH, ViewConstant.PUBLICATION_VIEW_HEIGHT);
			UserInteractionEnabled = true;

			RightTopTriangleColor = UIColor.Clear;
			DownloadCancelTokenSource = new CancellationTokenSource ();
		}

		public void DrawPublicationActionView ()
		{
			
			DrawStatusInfo ();
			DrawActionControlContainer ();
		}

		/// <summary>
		/// Initializes the status info.
		/// Status info displayed below publication cover
		/// </summary>
		private void DrawStatusInfo ()
		{
			StatusLabel = new UILabel ();
			StatusLabel.Frame= new CGRect (0, ViewConstant.PUBLICATION_COVER_HEIGHT + 12, ViewConstant.STATUS_LABEL_WIDTH, ViewConstant.STATUS_LABEL_HEIGHT);
			StatusLabel.Font = UIFont.BoldSystemFontOfSize (12.0f);
			StatusLabel.Text = StatusInfoStr;
			AddSubview (StatusLabel);

			MoreStatusLabel = new UILabel ();
			MoreStatusLabel.Frame = new CGRect (0, ViewConstant.PUBLICATION_COVER_HEIGHT + ViewConstant.STATUS_LABEL_HEIGHT + 12 + 3, ViewConstant.STATUS_DATE_WIDTH, ViewConstant.STATUS_DATE_HEIGHT);
			MoreStatusLabel.TextColor = UIColor.LightGray;
			MoreStatusLabel.Font = UIFont.SystemFontOfSize (11.0f);
			MoreStatusLabel.Text = MoreStatusInfoStr;
			AddSubview (MoreStatusLabel);

			//Display an info button below publication cover
			InfoButton= new UIButton (UIButtonType.InfoDark);
			InfoButton.Frame = new CGRect (ViewConstant.PUBLICATION_COVER_WIDTH - 22, ViewConstant.PUBLICATION_COVER_HEIGHT + 16, 22, 22);
			InfoButton.TintColor = InfoButtonTintColor;
			if (InfoButton.TintColor.Equals (UIColor.LightGray)) {
				InfoButton.UserInteractionEnabled = false;
			}
			InfoButton.TouchUpInside += (o, e) => {
				AppDataUtil.Instance.SetCurrentPublication(P);
				AppDisplayUtil.Instance.ShowPublicationInfoView();
			};
			AddSubview (InfoButton);

			Cover.InvisibleOpenPublicationButton.TouchUpInside += delegate {
				AppDataUtil.Instance.SetCurrentPublication(P);
				AppDisplayUtil.Instance.DismissPopoverView();
				AppDisplayUtil.Instance.AppDelegateInstance.MyPublicationController.DismissPopupViewController();
				AppDisplayUtil.Instance.GotoPublicationDetailViewController ();
			};
		}

		/// <summary>
		/// Draws the action control container contains download image and download progress view
		/// </summary>
		private void DrawActionControlContainer ()
		{
			RightTopView = new TriangleView (UIColor.FromRGB(253, 59, 47), ZoomRate);
			RightTopView.ShowHintText (RightTopHintText);
			RightTopView.SetNeedsDisplay ();
			AddSubview (RightTopView);
		}
			

		public void AddCover(PublicationCover cover) 
		{
			Cover = cover;
			AddSubview (Cover);
		}


		/// <Docs>To be added.</Docs>
		/// <para>Returns the sort order of the current instance compared to the specified object.</para>
		/// <summary>
		/// Compares to.
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="pv">Pv.</param>
		public int CompareTo(PublicationView pv)
		{
			int result = 0;

			if (this.P.OrderBy >= pv.P.OrderBy) {
				result = 1;
			}else if (this.P.OrderBy < pv.P.OrderBy) {
				result = -1;

			}
			return result;
		}

		public void ZoomInOrOut(float rate)
		{
			Cover.ZoomInOrOut (rate);

			StatusLabel.Frame= new CGRect (0, (ViewConstant.PUBLICATION_COVER_HEIGHT + 12) * rate, ViewConstant.STATUS_LABEL_WIDTH * rate,  ViewConstant.STATUS_LABEL_HEIGHT * rate);
			StatusLabel.Font = UIFont.BoldSystemFontOfSize (12.0f * rate);

			MoreStatusLabel.Frame = new CGRect (0, (ViewConstant.PUBLICATION_COVER_HEIGHT + ViewConstant.STATUS_LABEL_HEIGHT + 12 + 3) * rate, ViewConstant.STATUS_DATE_WIDTH * rate, ViewConstant.STATUS_DATE_HEIGHT * rate);
			MoreStatusLabel.Font = UIFont.SystemFontOfSize (11.0f * rate);

			InfoButton.Frame = new CGRect ((ViewConstant.PUBLICATION_COVER_WIDTH - 36) * rate, (ViewConstant.PUBLICATION_COVER_HEIGHT + 12) * rate, 36 * rate, 36 * rate);


		}


		public void Refresh ()
		{
			switch (p.PublicationStatus) {
			case PublicationStatusEnum.Downloaded:
				StatusLabel.Text = "Up to date";
				MoreStatusLabel.Text = "Currency Date " + ((DateTime)p.LastUpdatedDate).ToString ("dd MMM yyyy");
				break;
			case PublicationStatusEnum.NotDownloaded:
				StatusLabel.Text = "Download";
				double mbSize = ((double)p.Size) / 1024 / 1024;
				string mbSizeStr = mbSize.ToString ();
				int count = mbSizeStr.LastIndexOf (".");
				MoreStatusLabel.Text = mbSizeStr.Substring(0, count + 3)+ " MB";
				break;
			case PublicationStatusEnum.RequireUpdate:
				StatusLabel.Text = p.UpdateCount + (p.UpdateCount > 1 ? " Updates avaiable" : " Update avaiable");
				MoreStatusLabel.Text = "Currency Date " + ((DateTime)p.LastUpdatedDate).ToString ("dd MMM yyyy");
				break;
			}
			if (p.DaysRemaining < 0) {
				StatusLabel.Text = "Expired";
				MoreStatusLabel.Text = "Currency Date " + ((DateTime)p.LastUpdatedDate).ToString ("dd MMM yyyy");
			}

			//TODO, the text of StatusLabel should be determined by publication status other than whether it is empty or not
			if (StatusLabel.Text == "") {
				StatusLabel.Text = "Download Failed";
			}

			if (p.PublicationStatus == PublicationStatusEnum.NotDownloaded) {
				InfoButton.TintColor = UIColor.LightGray;
			} else {
				InfoButton.TintColor = UIColor.Red;
			}

			//If publication has not been downloaded or publication has update
			if (p.PublicationStatus == PublicationStatusEnum.NotDownloaded || p.PublicationStatus == PublicationStatusEnum.RequireUpdate) {
				RightTopView.TriangleBackgroundColor = UIColor.FromRGB (22, 132, 250);
			}

			//If publication expired
			if (p.DaysRemaining < 0) {
				RightTopView.TriangleBackgroundColor = UIColor.FromRGB(253, 59, 47);
			}
			if (p.PublicationStatus == PublicationStatusEnum.RequireUpdate) {
				RightTopView.ShowHintText ("Update");
			}


			if (p.DaysRemaining < 0) {
				RightTopView.ShowHintText ("Expired");
				RightTopView.AddGestureRecognizer (new UITapGestureRecognizer (delegate() {
					AppDataUtil.Instance.SetCurrentPublication(P);
					AppDisplayUtil.Instance.ShowPublicationInfoView();
				}));
			} 
			if (p.DaysRemaining >= 0 && (p.PublicationStatus == PublicationStatusEnum.NotDownloaded || p.PublicationStatus == PublicationStatusEnum.RequireUpdate)) {
				
				RightTopView.ShowDownloadActionView (p, StartDownload, ShowDownloadAlert);
			}

			if (p.DaysRemaining >= 0 && p.PublicationStatus == PublicationStatusEnum.Downloaded) {
				RightTopView.Hidden = true;
				RightTopView.RemoveFromSuperview ();
			} else {
				RightTopView.Hidden = false;
				AddSubview (RightTopView);
			}
			RightTopView.ZoomRate = ZoomRate;

			RightTopView.SetNeedsDisplay ();


			if (p.PublicationStatus == PublicationStatusEnum.NotDownloaded) {
				InfoButton.Enabled = false;
			} else {
				InfoButton.Enabled = true;
			}

			if (p.PublicationStatus == PublicationStatusEnum.NotDownloaded) {
				Cover.InvisibleOpenPublicationButton.Enabled = false;
			} else {
				Cover.InvisibleOpenPublicationButton.Enabled = true;
			}


			DownloadCancelTokenSource = new CancellationTokenSource ();

			//TODO, update publication cover
			if (p.IsLoan) {
				if (p.DaysRemaining == 0) {
					Cover.LoanTagLabel.Text = "Due to Expire";
				} else {
					Cover.LoanTagLabel.Text  = "LOAN" + Environment.NewLine + p.DaysRemaining +  (p.DaysRemaining > 1 ? " days Remaining" : " day Remaining");
				}
			}
			Cover.TitleLabel.Text = p.Name;


			Cover.BackgroundColor = ColorUtil.ConvertFromHexColorCode (p.ColorPrimary);

			Cover.TitleBackgroundView.BackgroundColor = ColorUtil.ConvertFromHexColorCode (p.ColorSecondary);
			Cover.TitleBackgroundView.SetNeedsDisplay ();

			Cover.TitleOuterLineView.Layer.BorderColor = ColorUtil.ConvertFromHexColorCode(p.FontColor).CGColor;
			Cover.TitleOuterLineView.SetNeedsDisplay ();

			Cover.TitleInnerLineView.Layer.BorderColor = ColorUtil.ConvertFromHexColorCode(p.FontColor).CGColor;
			Cover.TitleInnerLineView.SetNeedsDisplay ();

			Cover.TitleLabel.TextColor = ColorUtil.ConvertFromHexColorCode(p.FontColor);

			Cover.FirstBottomLine.BackgroundColor = ColorUtil.ConvertFromHexColorCode(p.FontColor);
			Cover.FirstBottomLine.SetNeedsDisplay ();

			Cover.SecondBottomLine.BackgroundColor = ColorUtil.ConvertFromHexColorCode(p.FontColor);
			Cover.SecondBottomLine.SetNeedsDisplay ();

			Cover.LoanTagLabel.TextColor = ColorUtil.ConvertFromHexColorCode(p.FontColor);

			Cover.SetNeedsDisplay ();


		}


		/// <summary>
		/// Updates the download progress.
		/// </summary>
		/// <param name="intProgress">Int progress.</param>
		/// <param name="size">Size.</param>
		public void UpdateDownloadProgress(int intProgress, long size)
		{
			float floatProgress = ((float)intProgress) / 100;//floatProgress range from 0 ~ 1
			RightTopView.DownloadProgressView.UpdateProgress (floatProgress);

			if (intProgress == 100) {
				RightTopView.InvisibleCancelDownloadButton.RemoveFromSuperview ();
				RightTopView.ActionView.UserInteractionEnabled = false;
				RightTopView.ActionView.RemoveFromSuperview ();
				RightTopView.RemoveFromSuperview ();
			}
		}

	}
}

