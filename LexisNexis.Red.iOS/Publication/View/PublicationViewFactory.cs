using System;

using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.BusinessModel;


namespace LexisNexis.Red.iOS
{
	public static class PublicationViewFactory
	{

		public static PublicationView CreatePublicationView(Publication publication,DoPublicationDownload startDownload, ShowAlert cancelDownloadAlert, float zoomRate = 1.0f)
		{
			PublicationView publicationView = new PublicationView ();
			publicationView.ZoomRate = zoomRate;
			publicationView.StartDownload = startDownload;
			publicationView.ShowDownloadAlert = cancelDownloadAlert;

			PublicationCover cover = CreatePublicationCover (publication);
			publicationView.AddCover (cover);
			publicationView.DrawPublicationActionView ();

			publicationView.P = publication;

			return publicationView;
		}


		public static PublicationCover CreatePublicationCover(Publication publication , float zoomRate = 1.0f)
		{
			PublicationCover cover;
			var loanInfoStr = "";
			if (publication.IsLoan) {
				if (publication.DaysRemaining == 0) {
					loanInfoStr = "Due to Expire";
				} else {
					loanInfoStr = "LOAN" + Environment.NewLine + publication.DaysRemaining +  (publication.DaysRemaining > 1 ? " days Remaining" : " day Remaining");
				}
			}

			cover = new PublicationCover (publication.ColorPrimary, publication.ColorSecondary, publication.FontColor, publication.Name, loanInfoStr, zoomRate);
			cover.DrawSubviews ();

			if (publication.IsFTC) {
				UIImageView ftcFlagImageView = new UIImageView (new UIImage ("Images/Publication/Cover/PlusCasesSash.png"));
				ftcFlagImageView.Frame = new CGRect (10, 0, 30, 50);
				cover.AddSubview (ftcFlagImageView);

			}

			return cover;
		}


		/// <summary>
		/// Determines whether publication p is expired or not
		/// </summary>
		/// <returns><c>true</c> if publication p is expired; otherwise, <c>false</c>.</returns>
		/// <param name="p">P.</param>
		public static bool IsPublicationExpired(Publication p)
		{
			return p.DaysRemaining < 0;
		}
	}
}

