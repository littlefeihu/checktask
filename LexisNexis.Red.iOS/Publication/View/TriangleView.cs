using System;
using System.Threading;

using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	/// <summary>
	/// Triangle view
	/// This view will displayed on the right top of a publication cover when:
	/// 1.Publication is expired;
	/// 2.Publication has update;
	/// 3.Publication has not been downloaded
	/// </summary>
	public class TriangleView : UIView
	{
		const float ENDGE_LENGTH = 90.0f;
		const float DIAGONAL_LENGTH = 128;
		const float ACTION_VIEW_WIDTH = 30.0f;
		const float ACTION_VIEW_HEIGHT = 30.0f;

		public float ZoomRate{ get; set;}

		public UIColor TriangleBackgroundColor{ get; set;}
		public UIView ActionView { get; set;}
		public UILabel ActionLabel { get; set;}

		public CircularProgressView DownloadProgressView{ get; set;}

		public UIButton InvisibleCancelDownloadButton{ get; set;}

		private Publication publication;
		private ShowAlert cancelDownloadAlert;

		public TriangleView (UIColor color, float zoomRate = 1.0f)
		{
			ZoomRate = zoomRate;

			Frame = new CGRect (ViewConstant.PUBLICATION_COVER_WIDTH * zoomRate - ENDGE_LENGTH , 0.0f, ENDGE_LENGTH , ENDGE_LENGTH );
			BackgroundColor = UIColor.Clear;
			TriangleBackgroundColor = color;

			ActionView = new UIView();
			ActionView.Frame = new CGRect (50, 10, ACTION_VIEW_WIDTH, ACTION_VIEW_WIDTH);
			ActionView.BackgroundColor = UIColor.Clear;
		}

		/// <summary>
		/// Draw the triangle
		/// </summary>
		/// <param name="rect">Rect.</param>
		public override void Draw (CGRect rect)
		{
			UIColor.Clear.SetColor ();
			UIGraphics.RectFill (rect);
			CGContext context = UIGraphics.GetCurrentContext ();
			context.BeginPath ();
			context.MoveTo (0.0f, 0.0f);
			context.AddLineToPoint (ENDGE_LENGTH , ENDGE_LENGTH );
			context.AddLineToPoint (ENDGE_LENGTH , 0.0f);
			context.ClosePath ();
			TriangleBackgroundColor.SetFill ();
			context.DrawPath (CGPathDrawingMode.Fill);
			AddSubview (ActionView);
		}


		/// <summary>
		/// Shows the hint text.
		/// </summary>
		/// <param name="hintText">Hint text.</param>
		public void ShowHintText(string hintText)
		{
			if (!string.IsNullOrEmpty(hintText)) {
				if (ActionLabel == null) {
					ActionLabel = new UILabel ();
					//ActionLabel.Frame = new CGRect (-10f, ENDGE_LENGTH / 2 - 15f, DIAGONAL_LENGTH  , 15f );
					ActionLabel.Frame = new CGRect (-5f, ENDGE_LENGTH / 2 - 20f, DIAGONAL_LENGTH  , 15f );
					ActionLabel.Font = UIFont.BoldSystemFontOfSize (15.0f);
					ActionLabel.TextColor = UIColor.White;
					ActionLabel.TextAlignment = UITextAlignment.Center;
					ActionLabel.Transform = CGAffineTransform.MakeRotation ((nfloat)Math.PI / 4);
					ActionLabel.UserInteractionEnabled = true;
					AddSubview (ActionLabel);
				}
				ActionLabel.Text = hintText;
				ActionLabel.Hidden = false;
			}

		}

		/// <summary>
		/// Shows the download action view.
		/// </summary>
		/// <param name="publication">Publication.</param>
		/// <param name="startDownload">Start download.</param>
		/// <param name="cancelDownloadAlert">Cancel download alert.</param>
		/// <param name="hasFailed">If set to <c>true</c> has failed.</param>
		public void ShowDownloadActionView (Publication publication, DoPublicationDownload startDownload, ShowAlert cancelDownloadAlert, bool hasFailed = false)
		{
			RemoveActionSubview ();
			if (publication.PublicationStatus == PublicationStatusEnum.NotDownloaded) {
				
				UIImageView downloadImageView = hasFailed ? new UIImageView (new UIImage ("Images/Publication/Cover/DownloadFailed.png")) : new UIImageView (new UIImage ("Images/Publication/Cover/CloudInstall.png"));
				if (!hasFailed) {
					downloadImageView.Frame = new CGRect (0, 0, ACTION_VIEW_WIDTH, ACTION_VIEW_WIDTH);
				} else {
					TriangleBackgroundColor = UIColor.FromRGB(253, 59, 47);
					SetNeedsDisplay ();
				}

				downloadImageView.UserInteractionEnabled = false;
				ActionView.AddSubview (downloadImageView);
			}


			ActionView.AddGestureRecognizer (new UITapGestureRecognizer (delegate() {
					startDownload (publication, (PublicationView)Superview);
					ShowDownloadProgressView();
			}));
			ActionView.UserInteractionEnabled = true;

			this.publication = publication;
			this.cancelDownloadAlert = cancelDownloadAlert;

			if (ActionLabel != null) {
				ActionLabel.Hidden = false;
				ActionLabel.AddGestureRecognizer (new UITapGestureRecognizer (delegate() {
					startDownload (publication, (PublicationView)Superview);
					ShowDownloadProgressView();
				}));
			}

		}

		/// <summary>
		/// Shows the download progress view.
		/// </summary>
		public  void ShowDownloadProgressView()
		{
			
			DownloadProgressView = new CircularProgressView (new CGRect (0, 0, 30, 30));

			InvisibleCancelDownloadButton = new UIButton (DownloadProgressView.Frame);
			InvisibleCancelDownloadButton.BackgroundColor = UIColor.Clear;
			InvisibleCancelDownloadButton.TouchUpInside += delegate {
				cancelDownloadAlert(publication, (PublicationView)Superview);
			};

			RemoveActionSubview ();	
			ActionView.AddSubview (DownloadProgressView);
			ActionView.AddSubview (InvisibleCancelDownloadButton);

			if (ActionLabel != null) {
				ActionLabel.Hidden = true;
			}
		}



		public void RemoveActionSubview ()
		{
			foreach (UIView view in ActionView.Subviews) {
				view.RemoveFromSuperview ();
			}
		}

	}
}

