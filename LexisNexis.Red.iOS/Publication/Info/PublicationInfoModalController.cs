using System;

using Foundation;
using UIKit;
using CoreGraphics;
using CoreText;
using MessageUI;

using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;


namespace LexisNexis.Red.iOS
{
	public partial class PublicationInfoModalController : UIViewController
	{
		public Publication curPublication{ get; set;}

		public PublicationInfoModalController () : base ("PublicationInfoModalController", null)
		{

		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
			View.TranslatesAutoresizingMaskIntoConstraints = false;
			View.AddGestureRecognizer (new UITapGestureRecognizer());//prevent tap guesture pass to super view

			ShowPublication ();
		}

		public void ShowPublication ()
		{
			//TODO,fix layout constraint warning issue
			this.curPublication = AppDataUtil.Instance.GetCurrentPublication ();

			ShowPublicationCoverAndActionButton ();

			ShowPublicationExpireInfo ();
			ShowPublicationDescriptionInfo ();
			ShowPublicationFullTextCaseInfo ();
			ShowPublicationWhatsNewInfo ();
			ShowPublicationMetaInfo ();

			InfoScrollView.SetContentOffset (new CGPoint (0, 0), false);
		}

		private void ShowPublicationCoverAndActionButton ()
		{
			
			var pvInList = AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList.Find (item => item.P.BookId == curPublication.BookId);

			PublicationView pv = PublicationViewFactory.CreatePublicationView(curPublication, pvInList.StartDownload, pvInList.ShowDownloadAlert, 0.85f);
			pv.ZoomInOrOut (0.85f);
			PublicationCoverContainer.AddSubview(pv);

			PublicationNameLabel.Text = curPublication.Name;
			CGSize publicationNameLabelSize = TextDisplayUtil.GetStringBoundRect (curPublication.Name, UIFont.SystemFontOfSize (ViewConstant.INFO_NAME_LABEL_FONT_SIZE), new CGSize(ViewConstant.INFO_AUTHOR_LABEL_WIDTH, 600));
			nfloat expectedNameLabelHeight = publicationNameLabelSize.Height > 26 ? publicationNameLabelSize.Height : 26;
			ChangeViewHeightConstraint (PublicationNameLabel, expectedNameLabelHeight);

			PublicationAuthorLabel.Text = curPublication.Author;
			CGSize publicationAuthorLabelSize = TextDisplayUtil.GetStringBoundRect (curPublication.Author, UIFont.SystemFontOfSize (ViewConstant.INFO_AUTHOR_LABEL_FONT_SIZE), new CGSize(ViewConstant.INFO_AUTHOR_LABEL_WIDTH, 600));
			nfloat expectedAuthorLabelHeight = publicationAuthorLabelSize.Height < ViewConstant.INFO_AUTHOR_LABEL_MIN_HEIGHT ? ViewConstant.INFO_AUTHOR_LABEL_MIN_HEIGHT : publicationAuthorLabelSize.Height;
			ChangeViewHeightConstraint (PublicationAuthorLabel, expectedAuthorLabelHeight);

			ActionButton.Layer.BorderWidth = 1.0f;
			ActionButton.Layer.CornerRadius = 5;

			if (curPublication.DaysRemaining < 0) {
				ActionButton.Layer.BorderColor = UIColor.Red.CGColor;
				ActionButton.SetTitleColor (UIColor.Red, UIControlState.Normal);
			} else {
				ActionButton.Layer.BorderColor = UIColor.FromRGB (22, 132, 250).CGColor;
				ActionButton.SetTitleColor (UIColor.FromRGB (22, 132, 250), UIControlState.Normal);
			}

			if (curPublication.IsLoan) {
				LoanTagLabel.Layer.CornerRadius = 8;
				LoanTagLabel.Layer.MasksToBounds = true;

				ChangeViewWidthConstraint (LoanTagLabel,  ViewConstant.INFO_LOAN_TAG_LABEL_WIDTH);
				ChangeViewWidthConstraint (TagSpaceLabel, 10);
			} else {
				ChangeViewWidthConstraint (LoanTagLabel, 0);
				ChangeViewWidthConstraint (TagSpaceLabel, 0);
			}

			if (curPublication.IsFTC) {
				PlusCaseTagLabel.Layer.CornerRadius = 8;
				PlusCaseTagLabel.Layer.MasksToBounds = true;
				PlusCaseTagLabel.Hidden = false;
			} else {
				PlusCaseTagLabel.Hidden = true;
			}

			nfloat expectedTitleAuthorScrollViewContentHeight = expectedAuthorLabelHeight + expectedNameLabelHeight + 34;
			if (expectedTitleAuthorScrollViewContentHeight > ViewConstant.INFO_NAME_AUTHOR_SCROLL_VIEW_MAX_HEIGHT) {
				ChangeViewHeightConstraint (TitleAuthorScrollView, ViewConstant.INFO_NAME_AUTHOR_SCROLL_VIEW_MAX_HEIGHT);
				TitleAuthorScrollView.ScrollEnabled = true;

			} else {
				ChangeViewHeightConstraint (TitleAuthorScrollView, expectedTitleAuthorScrollViewContentHeight);
				TitleAuthorScrollView.ScrollEnabled = false;
			}
			TitleAuthorScrollView.ContentSize = new CGSize (ViewConstant.INFO_AUTHOR_LABEL_WIDTH, expectedTitleAuthorScrollViewContentHeight);
			TitleAuthorScrollView.SetContentOffset (new CGPoint (0, 0), false);
		}

		private void ShowPublicationExpireInfo ()
		{
			CustomerSupportTelLabel.Text = CustomerSupportTelLabel.Text.Replace ("#PLACEHOLDER_TEL#", GlobalAccess.Instance.CurrentUserInfo.Country.CustomerSupportTEL);

			ExpireInfoDetailTextView.Text = ExpireInfoDetailTextView.Text.Replace ("#PLACEHOLDER_TEL#", GlobalAccess.Instance.CurrentUserInfo.Country.CustomerSupportTEL);
			ExpireInfoDetailTextView.Text = ExpireInfoDetailTextView.Text.Replace ("#PLACEHOLDER_EMAIL#", GlobalAccess.Instance.CurrentUserInfo.Country.CustomerSupportEmail);
			CustomerSupportEmailLabel.Text = GlobalAccess.Instance.CurrentUserInfo.Country.CustomerSupportEmail;
			if (curPublication.DaysRemaining >= 0) {
				foreach (var view in ExpiredInfoContainerView.Subviews) {
					view.Hidden = true;
				}
				ChangeViewHeightConstraint (ExpiredInfoContainerView, 0);
			} else {
				ChangeViewHeightConstraint (MoreExpireInfoDetailLabel, 20);
				ChangeViewHeightConstraint (ExpiredInfoContainerView, 145);
				foreach (var view in ExpiredInfoContainerView.Subviews) {
					view.Hidden = false;
				}
				ExpireInfoDetailTextView.UserInteractionEnabled = true;
				UITapGestureRecognizer tapDetailRecoginzer = new UITapGestureRecognizer ();
				tapDetailRecoginzer.AddTarget (this.ShowAllExpireInfo);
				ExpireInfoDetailTextView.AddGestureRecognizer (tapDetailRecoginzer);

				MoreExpireInfoDetailLabel.UserInteractionEnabled = true;
				UITapGestureRecognizer tapMoreRecoginzer = new UITapGestureRecognizer ();
				tapMoreRecoginzer.AddTarget (this.ShowAllExpireInfo);
				MoreExpireInfoDetailLabel.AddGestureRecognizer (tapMoreRecoginzer);


				UITapGestureRecognizer tapCustomerSupportEmailRecoginzer = new UITapGestureRecognizer ();
				tapCustomerSupportEmailRecoginzer.AddTarget (SendEmailToCustomerSupport);
				CustomerSupportEmailLabel.UserInteractionEnabled = true;
				CustomerSupportEmailLabel.AddGestureRecognizer (tapCustomerSupportEmailRecoginzer);

				/*
				UITapGestureRecognizer tapCSEmailRecoginzer = new UITapGestureRecognizer ();
				tapCSEmailRecoginzer.AddTarget (SendEmailToCustomerSupport);
				CustomerSupportEmailLabelInDetail.UserInteractionEnabled = true;
				CustomerSupportEmailLabelInDetail.AddGestureRecognizer (tapCSEmailRecoginzer);
				*/
			}

		}

		/// <summary>
		/// Shows the publication description info.
		/// </summary>
		private void ShowPublicationDescriptionInfo ()
		{
			DescriptionContentLabel.Text = curPublication.Description;
			CGSize labelSize = TextDisplayUtil.GetStringBoundRect (curPublication.Description, UIFont.SystemFontOfSize (12), new CGSize(600, 6000));


			if (labelSize.Height > 56) {// 4 *14
				DescriptionContentLabel.UserInteractionEnabled = true;
				UITapGestureRecognizer tapContentRecoginzer = new UITapGestureRecognizer ();
				tapContentRecoginzer.AddTarget (this.ShowAllDescription);
				DescriptionContentLabel.AddGestureRecognizer (tapContentRecoginzer);

				MoreDescriptionLabel.UserInteractionEnabled = true;
				UITapGestureRecognizer tapMoreRecoginzer = new UITapGestureRecognizer ();
				tapMoreRecoginzer.AddTarget (this.ShowAllDescription);
				MoreDescriptionLabel.AddGestureRecognizer (tapMoreRecoginzer);

				ChangeViewHeightConstraint (MoreDescriptionLabel, 20);
				ChangeViewHeightConstraint (DescriptionInfoContainerView, 145);
			} else {
				ChangeViewHeightConstraint (MoreDescriptionLabel, 0);
				ChangeViewHeightConstraint (DescriptionInfoContainerView, labelSize.Height + 67);
			}
		}

		private void ShowPublicationFullTextCaseInfo ()
		{
			if (curPublication.IsFTC) {//Full text case
				foreach (var view in FullTextCaseInfoContainerView.Subviews) {
					view.Hidden = false;
				}
				ChangeViewHeightConstraint (FullTextCaseInfoContainerView, 165);
				ChangeViewHeightConstraint (MoreFullTextCaseDetailLabel, 20);

				FullTextCaseContentLabel.UserInteractionEnabled = true;
				UITapGestureRecognizer tapDetailRecoginzer = new UITapGestureRecognizer ();
				tapDetailRecoginzer.AddTarget (this.ShowAllFullTextCaseDescription);
				FullTextCaseContentLabel.AddGestureRecognizer (tapDetailRecoginzer);

				MoreFullTextCaseDetailLabel.UserInteractionEnabled = true;
				UITapGestureRecognizer tapMoreRecoginzer = new UITapGestureRecognizer ();
				tapMoreRecoginzer.AddTarget (this.ShowAllFullTextCaseDescription);
				MoreFullTextCaseDetailLabel.AddGestureRecognizer (tapMoreRecoginzer);
			} else {
				foreach (var view in FullTextCaseInfoContainerView.Subviews) {
					view.Hidden = true;
				}
				ChangeViewHeightConstraint (FullTextCaseInfoContainerView, 0);
			}

		}

		private void ShowPublicationWhatsNewInfo ()
		{
			//What's new 
			WhatsNewDateLabel.Text = ((DateTime)curPublication.LastUpdatedDate).ToString("dd MMM yyyy");

			if (curPublication.AddedGuideCard != null && curPublication.AddedGuideCard.Count > 0) {
				string addedGuideCardStr = "";
				AddedGuideCardLabel.Lines = curPublication.AddedGuideCard.Count;
				foreach (GuideCard curGuideCard in curPublication.AddedGuideCard) {
					addedGuideCardStr += curGuideCard.Name;
					addedGuideCardStr += Environment.NewLine;
				}
				addedGuideCardStr = addedGuideCardStr.TrimEnd (Environment.NewLine.ToCharArray ());
				CGSize addedGuideCardLabelSize = TextDisplayUtil.GetStringBoundRect (addedGuideCardStr, UIFont.SystemFontOfSize (12), new CGSize(600, 6000));
				ChangeViewHeightConstraint (AddedGuideCardLabel, new nfloat(addedGuideCardLabelSize.Height));
				AddedGuideCardLabel.Text = addedGuideCardStr;

			}
			//InfoScrollView.AddSubview (AddedGuideCardLabel);
			if (curPublication.DeletedGuideCard != null && curPublication.DeletedGuideCard.Count > 0) {
				string deletedGuideCardStr = "";
				DeletedGuideCardLabel.Lines = curPublication.DeletedGuideCard.Count;
				foreach (GuideCard curGuideCard in curPublication.DeletedGuideCard) {
					deletedGuideCardStr += curGuideCard.Name;
					deletedGuideCardStr += Environment.NewLine;
				}
				deletedGuideCardStr = deletedGuideCardStr.TrimEnd (Environment.NewLine.ToCharArray ());
				CGSize deletedGuideCardLabelSize = TextDisplayUtil.GetStringBoundRect (deletedGuideCardStr, UIFont.SystemFontOfSize (12), new CGSize(600, 6000));
				ChangeViewHeightConstraint (DeletedGuideCardLabel, new nfloat(deletedGuideCardLabelSize.Height));
				DeletedGuideCardLabel.Text = deletedGuideCardStr;
			}
			if (curPublication.UpdatedGuideCard != null && curPublication.UpdatedGuideCard.Count > 0) {
				string updatedGuideCardStr = "";
				UpdateGuideCardLabel.Lines = curPublication.UpdatedGuideCard.Count;

				foreach (GuideCard curGuideCard in curPublication.UpdatedGuideCard) {
					updatedGuideCardStr += curGuideCard.Name;
					updatedGuideCardStr += Environment.NewLine;
				}
				updatedGuideCardStr = updatedGuideCardStr.TrimEnd (Environment.NewLine.ToCharArray ());
				CGSize updateGuideCardLabelSize = TextDisplayUtil.GetStringBoundRect (updatedGuideCardStr, UIFont.SystemFontOfSize (12), new CGSize(600, 6000));
				ChangeViewHeightConstraint (UpdateGuideCardLabel, new nfloat(updateGuideCardLabelSize.Height));

				UpdateGuideCardLabel.Text = updatedGuideCardStr;
			}
		}

		private void ShowPublicationMetaInfo ()
		{
			//Publication information
			VersionLabel.Text = curPublication.CurrentVersion.ToString();
			InstalledDateLabel.Text = ((DateTime)curPublication.InstalledDate).ToString ("dd MMM yyyy");
			CurrencyDateLabel.Text = ((DateTime)curPublication.CurrencyDate).ToString ("dd MMM yyyy");
			double mbSize = ((double)curPublication.Size) / 1024 / 1024;
			string mbSizeStr = mbSize.ToString ();
			int count = mbSizeStr.LastIndexOf (".");
			SizeLabel.Text = mbSizeStr.Substring(0, count + 3)+ " MB";
			PracticeAreaLabel.Text = curPublication.PracticeArea;
			SubcategoryLabel.Text = curPublication.SubCategory;
		}

		partial void OnTouchActionButton (Foundation.NSObject sender)
		{
			//Open Publication
			switch(curPublication.PublicationStatus){
			case PublicationStatusEnum.RequireUpdate:
			case PublicationStatusEnum.Downloaded:
				//OPEN
				AppDisplayUtil.Instance.AppDelegateInstance.MyPublicationController.DismissPopupViewController();
				AppDisplayUtil.Instance.GotoPublicationDetailViewController ();
				break;
			}
		}

		void ShowAllDescription ()
		{
			ChangeViewHeightConstraint (MoreDescriptionLabel, 0);

			CGSize labelSize = TextDisplayUtil.GetStringBoundRect (curPublication.Description, UIFont.SystemFontOfSize (12), new CGSize(600, 6000));
			ChangeViewHeightConstraint (DescriptionInfoContainerView, labelSize.Height + 67);
		}

		void ShowAllFullTextCaseDescription ()
		{
			ChangeViewHeightConstraint (FullTextCaseInfoContainerView, 239);
			ChangeViewHeightConstraint (MoreFullTextCaseDetailLabel, 0);
		}

		void ShowAllExpireInfo ()
		{
			ChangeViewHeightConstraint (ExpiredInfoContainerView, 254);
			ChangeViewHeightConstraint (MoreExpireInfoDetailLabel, 0);
			/*
			MoreExpireInfoDetailLabel.Text = "You can tell your command.";
			MoreExpireInfoDetailLabel.TextColor = UIColor.DarkGray;
			*/
		}



		void ChangeViewHeightConstraint(UIView view, nfloat height)
		{
			ChangeViewSizeConstraint (view, NSLayoutAttribute.Height, height);
		}

		void ChangeViewWidthConstraint(UIView view, nfloat width)
		{
			ChangeViewSizeConstraint (view, NSLayoutAttribute.Width, width);
		}

		void ChangeViewSizeConstraint(UIView view, NSLayoutAttribute layoutAttr, nfloat targetVal)
		{
			for (int i = 0; i < view.Constraints.Length; i++) {
				if (view.Constraints [i].FirstAttribute == layoutAttr) {
					view.Constraints [i].Constant = targetVal;
				}
			}
		}

		void SendEmailToCustomerSupport ()
		{
			
			UIApplication.SharedApplication.OpenUrl(NSUrl.FromString("mailto://" + CustomerSupportEmailLabel.Text));

		}

	}
}

