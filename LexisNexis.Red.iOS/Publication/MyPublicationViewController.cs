using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

using MBProgressHUD;

namespace LexisNexis.Red.iOS
{
	public delegate void DoPublicationDownload(Publication publication, PublicationView publicationView, bool checkNetLimitation = true);
	public delegate void ShowAlert(Publication publication, PublicationView publicationView);

	public partial class MyPublicationViewController : UIViewController
	{

		const int PUBLICATION_FILTER_ALL = 0;
		const int PUBLICATION_FILTER_LOAN = 1;
		const int PUBLICATION_FILTER_SUBSCRIPTION = 2;


		const int POPUP_OVERLAYER_TAG = 10001;

		private List<Publication> publicationList;
  

		public HistoryTableViewController historyTVC{ get; set; }

		public PublicationInfoModalController PublicationInfoModalController{ get; set; }

		public MyPublicationViewController (IntPtr handle) : base (handle)
		{
 		}

		#region override UIViewController

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			noPublicationLabel.Hidden = true;
			publicationList =  PublicationUtil.Instance.GetPublicationOffline ();

			//PublicationViewList = new List<PublicationView> ();
			AppDisplayUtil.Instance.ClearPublicationViewList ();
			foreach (var p in publicationList) {
				AppDisplayUtil.Instance.AddPublicationView (PublicationViewFactory.CreatePublicationView (p, StartDownload, ShowConfirmCancelDownload));
			}

			AppDisplayUtil.Instance.AppDelegateInstance.MyPublicationController = this;

			NavigationItem.RightBarButtonItems = new UIBarButtonItem[]{TabBarController.NavigationItem.RightBarButtonItem};


			ShowPublicationViews ();
		}
 		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			TabBarController.Title = "Publications";
			noPublicationLabel.Hidden = true;

 			TabBarController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem ("Edit", UIBarButtonItemStyle.Plain, OpenPublicaionSortingPanel);
			TabBarController.NavigationItem.RightBarButtonItems = NavigationItem.RightBarButtonItems;

			List<RecentHistoryItem> historyList = PublicationContentUtil.Instance.GetRecentHistory ();

			UIView tableView = HistoryContainerView.ViewWithTag (1);
 			if ( historyList != null && historyList.Count > 0) {
				tableView.Hidden = false;
				if (historyTVC == null) {
					historyTVC = new HistoryTableViewController ();
					historyTVC.TableView = (UITableView)tableView;
				}

				HistoryTableViewSource TableViewSource = new HistoryTableViewSource (historyTVC.TableView);
				TableViewSource.HistoryList = historyList;
 				historyTVC.TableView.Source = TableViewSource;

				//Fix constraint warning: Detected a case where constraints ambiguously suggest a height of zero for a tableview cell's content view. We're considering the collapse unintentional and using standard height instead.
				historyTVC.TableView.RowHeight = 44;
				historyTVC.TableView.TableFooterView = new UIView ();
			} else {
				tableView.Hidden = true;
			}

		}

		public async override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			var hud = new MTMBProgressHUD (View) {
				LabelText = "Loading",
				RemoveFromSuperViewOnHide = true
			};
			View.AddSubview (hud);
			hud.Show (animated: true);
			hud.Hide (animated: true, delay: 0.5);

			var onlinePublicationList = await  GetLatestPublicationList ();
			if (onlinePublicationList != null) {
				publicationList = onlinePublicationList;
				UpdatePublicationViewList ();
				ShowPublicationViews ();
			}
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);

			//Dismiss current popover controller if it exist
			AppDisplayUtil.Instance.DismissPopoverView ();
		}
		#endregion

		/// <summary>
		/// Gets the latest publication list.
		/// </summary>
		/// <returns>The latest publication list. return null when failed to get online publication</returns>
		private async Task<List<Publication>> GetLatestPublicationList ()
		{
			OnlinePublicationResult getOnlinePublicationResult = await PublicationUtil.Instance.GetPublicationOnline ();
			return getOnlinePublicationResult.RequestStatus == RequestStatusEnum.Success ? getOnlinePublicationResult.Publications : null;
		}


		/// <summary>
		/// Raises the publication filter(segment control) selected changed event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void OnPublicationFilterSelectedChanged (Foundation.NSObject sender)
		{
			ShowPublicationViews();
		}

		/// <summary>
		/// Updates the publication view list according publication list
		/// </summary>
		public void UpdatePublicationViewList ()
		{
			publicationList =  PublicationUtil.Instance.GetPublicationOffline ();
			foreach (var p in publicationList) {
				var pView = AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList.Find (pv => pv.P.BookId == p.BookId);
				if (pView == null) {//new subscribed publication
					AppDisplayUtil.Instance.AddPublicationView (PublicationViewFactory.CreatePublicationView (p, StartDownload, ShowConfirmCancelDownload));
				} else {
					//pView.P = p;
					int count = AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList.Count;
					for (int i = 0; i < count; i++) { 
						if (AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList [i].P.BookId == p.BookId){
							if (AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList [i].P.PublicationStatus != p.PublicationStatus)
								AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList [i].P = p;

							if((AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList [i].P.DaysRemaining < 0) ^ (p.DaysRemaining < 0)){
								AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList [i].P = p;
							}
							AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList [i].P.OrderBy = p.OrderBy;
						}
					}
				}
			}

			List<PublicationView> toBeRemovedPV = new List<PublicationView> ();
			foreach(var pv in  AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList){
				var coorP = publicationList.Find (p => p.BookId == pv.P.BookId);
				if (coorP == null) {
					toBeRemovedPV.Add (pv);
				}
			}

			foreach (var pv in toBeRemovedPV) {
				AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList.Remove (pv);
			}

			//Sort publication view list by publication orderby property
			AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList.Sort();
		}

		/// <summary>
		/// Shows the publication views.
		/// </summary>
		public void ShowPublicationViews ()
		{
			float frameX = 0;

			foreach (UIView view in publicationViewScrollContainer.Subviews) {
				view.RemoveFromSuperview ();
			}

			foreach (var pv in AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList) {
				if (pv.P.IsLoan && publicationFilterSegmentControl.SelectedSegment == PUBLICATION_FILTER_SUBSCRIPTION) {
					continue;
				}

				if (!pv.P.IsLoan && publicationFilterSegmentControl.SelectedSegment == PUBLICATION_FILTER_LOAN) {
					continue;
				}
				pv.Frame = new CGRect (frameX, pv.Frame.Y, pv.Frame.Width, pv.Frame.Height);
				frameX += ViewConstant.PUBLICATION_COVER_WIDTH + ViewConstant.PUBLICATION_COVER_HORIZONTAL_SPACING;
				publicationViewScrollContainer.AddSubview (pv);
			}
			if (AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList.Count == 0 ) {
				noPublicationLabel.Hidden = false;
				return;
			} else {
				noPublicationLabel.Hidden = true;
				publicationViewScrollContainer.ContentSize = new CGSize (frameX, ViewConstant.PUBLICATION_COVER_HEIGHT);
				//publicationViewScrollContainer.ContentOffset = new CGPoint (0, 0);
			}
		}
			
		/// <summary>
		/// Starts the download.
		/// </summary>
		/// <param name="publication">Publication.</param>
		/// <param name="publicationView">Publication view.</param>
		/// <param name="checkNetLimitation">If set to <c>true</c> check net limitation.</param>
		private async void StartDownload(Publication publication, PublicationView publicationView, bool checkNetLimitation = true)
		{
			string prevInfoStr = publicationView.StatusLabel.Text;
			string downloadFailedMsg = "";
			if (publication.PublicationStatus == PublicationStatusEnum.NotDownloaded) {
				downloadFailedMsg = "Download Failed";
			}
			if (publication.PublicationStatus == PublicationStatusEnum.RequireUpdate) {
				downloadFailedMsg = "Update Failed";
			}

			publicationView.StatusLabel.Text = "Downloading";



			var pView = AppDisplayUtil.Instance.AppDelegateInstance.PublicationViewList.Find (pv => pv.P.BookId == publication.BookId);
			if (pView != null && pView != publicationView) {
				pView.RightTopView.ShowDownloadProgressView ();
			}

			DownloadResult downloadResult = await PublicationUtil.Instance.DownloadPublicationByBookId (publication.BookId, publicationView.DownloadCancelTokenSource.Token, delegate(int downloadedProgress, long downloadSize) {
				//更新publication对应的所有view的下载进度及状态
				publicationView.UpdateDownloadProgress(downloadedProgress, downloadSize);
				if(pView != null && pView != publicationView){
					pView.UpdateDownloadProgress(downloadedProgress, downloadSize);
				}
			} , checkNetLimitation);

			switch (downloadResult.DownloadStatus) {
			case DownLoadEnum.OverLimitation:
				var overLimitationAlert = UIAlertController.Create ("Download Over 20MB", "This Digital Looseleaf title is over 20MB. We recommend you connect to a Wi-Fi network to download it.", UIAlertControllerStyle.Alert);
				overLimitationAlert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));
				overLimitationAlert.AddAction (UIAlertAction.Create ("Download", UIAlertActionStyle.Default, alert => StartDownload(publication, publicationView, false)));
				PresentViewController (overLimitationAlert, true, null);
				break;
			case DownLoadEnum.Canceled:
				publicationView.StatusLabel.Text = downloadFailedMsg;
				if (pView != null && pView != publicationView) {
					pView.P = publication;
					pView.StatusLabel.Text = downloadFailedMsg;
				}
				break;
			case DownLoadEnum.Failure:
				publicationView.StatusLabel.Text = downloadFailedMsg;
				publicationView.RightTopView.RemoveActionSubview ();
				publicationView.RightTopView.ShowDownloadActionView (publication, StartDownload, ShowConfirmCancelDownload, true);
				ShowAlert (downloadFailedMsg);
				break;
			case DownLoadEnum.NetDisconnected:
				publicationView.StatusLabel.Text = downloadFailedMsg;
				publicationView.RightTopView.RemoveActionSubview ();
				publicationView.RightTopView.ShowDownloadActionView (publication, StartDownload, ShowConfirmCancelDownload, true);
				ShowAlert ("Missing connection", "Sorry, there appears to be no Internet connection. A connection is required to complete this task.");
				break;
			case DownLoadEnum.Success:
				//update publication view after publication has been download(download date, practice area, subcategory etc)
				//UpdatePublicationViewInScrollView (publication, publicationView);
				publicationList = PublicationUtil.Instance.GetPublicationOffline ();
				publication = publicationList.Find (p => p.BookId == publication.BookId);//Get latest publication meta data after it has been downloaded
				publication.PublicationStatus = PublicationStatusEnum.Downloaded;
				publicationView.P = publication;
				if (pView != null && pView != publicationView) {
					pView.P = publication;
				}
				break;
			}
		}
 

		/// <summary>
		/// Opens the publicaion sorting panel.
		/// </summary>
		/// <param name="o">O.</param>
		/// <param name="e">E.</param>
		private void OpenPublicaionSortingPanel (object o, EventArgs e)
		{
			PublicationSortingController publicationSortingController = new PublicationSortingController ();
			publicationSortingController.CurPublicationListController = this;
			UIPopoverController sortingPop = new UIPopoverController(publicationSortingController);

			//Dismiss current popover controller if it exist
			AppDisplayUtil.Instance.SetPopoverController (sortingPop);

			sortingPop.PresentFromBarButtonItem ((UIBarButtonItem)o, UIPopoverArrowDirection.Any, true);
			sortingPop.SetPopoverContentSize(new CGSize(320, 400), true);

			publicationList = PublicationUtil.Instance.GetPublicationOffline ();
			publicationSortingController.SetEditing (true, true);
		}

		/// <summary>
		/// refresh the publication list which displayed in scroll view
		/// invoked when user delete or sorting publication
		/// </summary>
		public void ReloadPublicationList()
		{
			var hud = new MTMBProgressHUD (View) {
				LabelText = "Loading",
				RemoveFromSuperViewOnHide = true
			};
			View.AddSubview (hud);
			hud.Show (animated: true);


			UpdatePublicationViewList ();
			ShowPublicationViews();

			hud.Hide (animated: true, delay: 0.2);

		}

		/// <summary>
		/// Shows the confirm cancel download.
		/// When user click download progress view
		/// </summary>
		/// <param name="publication">Publication.</param>
		/// <param name="publicationView">Publication view.</param>
		private void ShowConfirmCancelDownload (Publication publication, PublicationView publicationView)
		{
			var confirmCancelDownload = UIAlertController.Create ("Cancel Download", "Are you sure you want to cancel the download of this LexisNexis Red publication?", UIAlertControllerStyle.Alert);
			confirmCancelDownload.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));
			confirmCancelDownload.AddAction (UIAlertAction.Create ("Confirm", UIAlertActionStyle.Default, alert => ConfirmCancelDownload(publication, publicationView)));

			PresentViewController (confirmCancelDownload, true, null);
		}

		/// <summary>
		/// Confirms the cancel download.
		/// Invoked when user click confirm button of cancel download alert view
		/// </summary>
		/// <param name="publication">Publication.</param>
		/// <param name="publicationView">Publication view.</param>
		private void ConfirmCancelDownload (Publication publication, PublicationView publicationView)
		{
			publicationView.DownloadCancelTokenSource.Cancel ();
			//UpdatePublicationViewInScrollView (publication, publicationView);
			publicationView.P = publication;
			//ShowAlert ("Download Failed");
		}


		/// <summary>
		/// Updates the publication view in scroll view.
		/// Invoked when publication has been download or some other occasion
		/// </summary>
		/// <param name="publication">Publication.</param>
		/// <param name="publicationView">Publication view.</param>
		private void UpdatePublicationViewInScrollView (Publication publication, PublicationView publicationView)
		{
			
			publicationList = PublicationUtil.Instance.GetPublicationOffline ();
			foreach (var p in publicationList) {
				if (p.BookId == publication.BookId) {
					publication = p;
					break;
				}
			}
			PublicationView updatePublicationView = PublicationViewFactory.CreatePublicationView (publication, StartDownload, ShowConfirmCancelDownload);

			publicationView.RemoveFromSuperview ();
			publicationView = null;
			publicationViewScrollContainer.AddSubview (updatePublicationView);
			
		}

		/// <summary>
		/// Shows the alert.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="msg">Message.</param>
		private void ShowAlert(string title = "", string msg = "")
		{
			if (title != null) {
				var alertController = UIAlertController.Create (title, msg, UIAlertControllerStyle.Alert);
				alertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null));
				PresentViewController (alertController, true, null);
			}
		}


		public void ShowPublicationInfoView ()
		{
			if (NavigationController.View.ViewWithTag (POPUP_OVERLAYER_TAG) != null)
				return;
			
			//Dismiss current popover controller if it exist
			AppDisplayUtil.Instance.DismissPopoverView ();


			if(PublicationInfoModalController == null)
				PublicationInfoModalController = new PublicationInfoModalController ();
			else
				PublicationInfoModalController.ShowPublication ();

			//overlayer
			UIView overlayerView = new UIView(NavigationController.View.Bounds);
			overlayerView.BackgroundColor = UIColor.Black.ColorWithAlpha (0.5f);
			overlayerView.Tag =  POPUP_OVERLAYER_TAG;
			NSLayoutConstraint topConstraint = NSLayoutConstraint.Create(overlayerView, NSLayoutAttribute.Top, NSLayoutRelation.Equal, NavigationController.View, NSLayoutAttribute.Top, 1, 0);
			NSLayoutConstraint leadingConstraint = NSLayoutConstraint.Create(overlayerView, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, NavigationController.View, NSLayoutAttribute.Leading, 1, 0);
			NSLayoutConstraint trailingConstraint = NSLayoutConstraint.Create(overlayerView, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, NavigationController.View, NSLayoutAttribute.Trailing, 1, 0);
			NSLayoutConstraint bottomConstraint = NSLayoutConstraint.Create(overlayerView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, NavigationController.View, NSLayoutAttribute.Bottom, 1, 0);
			NavigationController.View.AddSubview (overlayerView);
			NavigationController.View.AddConstraints(new NSLayoutConstraint[]{topConstraint, leadingConstraint, trailingConstraint, bottomConstraint});

			UITapGestureRecognizer tapOverLayerRecoginzer = new UITapGestureRecognizer ();
			tapOverLayerRecoginzer.AddTarget (DismissPopupViewController);
			overlayerView.AddGestureRecognizer (tapOverLayerRecoginzer);

			NSLayoutConstraint centerXConstraint = NSLayoutConstraint.Create(PublicationInfoModalController.View, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, overlayerView, NSLayoutAttribute.CenterX, 1, 0);
			NSLayoutConstraint centerYConstraint = NSLayoutConstraint.Create(PublicationInfoModalController.View, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, overlayerView, NSLayoutAttribute.CenterY, 1, 0);
			overlayerView.AddSubview (PublicationInfoModalController.View);
			overlayerView.AddConstraints(new NSLayoutConstraint[]{centerXConstraint, centerYConstraint});
			overlayerView.TranslatesAutoresizingMaskIntoConstraints = false;

			overlayerView.Transform = CGAffineTransform.MakeScale (0.01f, 0.01f);
			UIView.BeginAnimations ("ShowInfoModalView");
			UIView.SetAnimationDuration (0.3);
			overlayerView.Transform = CGAffineTransform.MakeScale (1.0f, 1.0f);
			UIView.CommitAnimations ();

		}

		/// <summary>
		/// Dismisses the popup view controller.
		/// </summary>
		public void DismissPopupViewController()
		{
			UIView overlayerView = NavigationController.View.ViewWithTag (POPUP_OVERLAYER_TAG);
			if (overlayerView != null) {
				overlayerView.RemoveFromSuperview ();
			}
		}

		/// <summary>
		/// Go to the publication detail view controller.
		/// </summary>
		public void GotoPublicationDetailViewController ()
		{
			if (NavigationController.TopViewController == TabBarController) {
				PerformSegue ("PublicationListToPublicationDetail", this);
			}
		}

	}
}
