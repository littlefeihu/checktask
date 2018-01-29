using System;

using Foundation;
using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.iOS
{
	partial class PublicationDetailController : UIViewController,Observer
	{
		public Publication CurPublication{ get; set; }

		public UIView ToggleSideBarView{ get; set; }

		public UIButton ToggleSidebarButton{ get; set; }

		public UILabel NavigationTitleLabel{ get; set; }

		private LNBadageBarButtonItem infoBarButton;

		public PublicationDetailController (IntPtr handle) : base (handle)
		{
			CurPublication = AppDataUtil.Instance.GetCurrentPublication ();

			NavigationTitleLabel = new UILabel (new CGRect(0, 0, 300, 20));
			NavigationTitleLabel.Text = CurPublication.Name;
			NavigationTitleLabel.LineBreakMode = UILineBreakMode.MiddleTruncation;
			NavigationTitleLabel.TextAlignment = UITextAlignment.Center;
			NavigationTitleLabel.TextColor = UIColor.Red;

			NavigationItem.TitleView = NavigationTitleLabel;
		}

		#region override
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			InitializeNavigationItem ();

			AppDataUtil.Instance.AddOpenedContentObserver (this);//Set current instance as the observer of subject OpendPublication to get notification when opend content changed

		}


		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);
			//this.ShowPopoverHistoryTableView ();
			AppDisplayUtil.Instance.DismissPopoverView ();//Dismiss popover view, since its location will be incorrect if it is poped by PresentFromRect(...)
		}
		#endregion


		public void Back(object o, EventArgs e)
		{
			if (NavigationController.TopViewController is PublicationDetailController) {
				AppDisplayUtil.Instance.DismissPopoverView ();
				AppDataUtil.Instance.ClearOpendPublicationObserver ();
				NavigationController.PopViewController (true);

				NavigationManager.Instance.Clear ();
			}

		}

		void InitializeNavigationItem ()
		{
			UIBarButtonItem shareBarButton = new UIBarButtonItem (new UIImage ("Images/Navigation/ShareIcon.png"), UIBarButtonItemStyle.Plain, ShowShareAcitvity);
			shareBarButton.SetBackgroundVerticalPositionAdjustment (-2, UIBarMetrics.Default);
			shareBarButton.TintColor = UIColor.Red;

			UIBarButtonItem historyBarButton = new UIBarButtonItem (new UIImage ("Images/Navigation/HistoryIcon.png"), UIBarButtonItemStyle.Plain, ShowPopoverHistoryTableView);
			historyBarButton.TintColor = UIColor.Red;

			UIButton infoButton = new UIButton (new CGRect(0, 0, 22, 22));
			infoButton.SetBackgroundImage (new UIImage ("Images/Navigation/InfoIcon.png"), UIControlState.Normal);
			infoButton.TouchUpInside += (object sender, EventArgs e) => AppDisplayUtil.Instance.ShowPublicationInfoView ();
			UIView view = new UIView (new CGRect (0, 0, 35, 22));
			view.AddSubview (infoButton);
			infoBarButton = new LNBadageBarButtonItem (view);
			infoBarButton.SetBadage (CurPublication.UpdateCount);

			UIBarButtonItem settingBarButtonItem = new UIBarButtonItem (new UIImage ("Images/Navigation/SettingsIcon.png"), UIBarButtonItemStyle.Plain, OpenSettingPopover);

			NavigationItem.RightBarButtonItems = new UIBarButtonItem[]{settingBarButtonItem, historyBarButton, shareBarButton, infoBarButton};

			//Add bar button item to the left of navigation item
			ToggleSideBarView = new UIView (new CGRect(0, 0, 33, 30));
			ToggleSideBarView.Layer.CornerRadius = 5;
			ToggleSideBarView.BackgroundColor = UIColor.Red;

			ToggleSidebarButton = new UIButton (UIButtonType.Custom);
			ToggleSidebarButton.SetBackgroundImage (new UIImage("Images/Navigation/SidebarIconWhite.png"), UIControlState.Normal);
			ToggleSidebarButton.Frame = new CGRect (0, 0, 25, 22);
			ToggleSidebarButton.BackgroundColor = UIColor.Clear;
			ToggleSideBarView.AddSubview (ToggleSidebarButton);
			ToggleSidebarButton.Center = ToggleSideBarView.Center;


			UIBarButtonItem sideTOCBarButton = new UIBarButtonItem(ToggleSideBarView);
			ToggleSidebarButton.TouchUpInside += delegate {
				ToggleLeftContainerView ();
			};

			UIButton backButton = new UIButton (UIButtonType.Custom);
			UIImageView backImageView = new UIImageView (new UIImage ("Images/Navigation/BackIcon.png"));
			backImageView.Frame = new CGRect (0, 5, 12, 20);
			UILabel backTextLabel = new UILabel ();
			backTextLabel.Text = "Publications";
			backTextLabel.Frame = new CGRect (20, 0, 100, 30);
			backTextLabel.TextColor = UIColor.Red;
			backButton.Frame = new CGRect (0, 0, 112, 30 );
			backButton.AddSubview (backImageView);
			backButton.AddSubview (backTextLabel);
			backButton.TouchUpInside += Back;
			UIView backView = new UIView ();
			backView.Frame = new CGRect (0, 0, 130, 30);
			backView.AddSubview (backButton);
			UIBarButtonItem backBarButton = new UIBarButtonItem(backView);

			NavigationItem.LeftBarButtonItems = new UIBarButtonItem[]{backBarButton, sideTOCBarButton};
		}


		public void OpenSettingPopover (object sender, EventArgs e)
		{
			SettingListController settingListVC = new SettingListController();
			UINavigationController navController = new UINavigationController (settingListVC);
			navController.NavigationBar.TintColor = UIColor.Red;
			navController.NavigationBar.BarTintColor = UIColor.White;

			UIPopoverController settingPop = new UIPopoverController(navController);
			settingPop.SetPopoverContentSize(new CoreGraphics.CGSize(320, 265), true);
			settingPop.BackgroundColor = UIColor.White;

			//Dismiss current popover controller if it exist
			AppDisplayUtil.Instance.SetPopoverController (settingPop);

			UIView viewOfBarButtonItem = (UIView)((UIBarButtonItem)sender).ValueForKey (new NSString("view"));
			settingPop.PresentFromRect (viewOfBarButtonItem.Frame, viewOfBarButtonItem.Superview, UIPopoverArrowDirection.Up, true);
		}

		public void ShowPopoverHistoryTableView (object sender, EventArgs e)
		{
			PopoverHistoryTableViewController historyTVC = new PopoverHistoryTableViewController (true);

			UINavigationController navController = new UINavigationController (historyTVC);
			navController.NavigationBar.BarTintColor = UIColor.White;

			UIPopoverController historyPop = new UIPopoverController(navController);
			historyPop.BackgroundColor = UIColor.White;
			AppDisplayUtil.Instance.SetPopoverController (historyPop);

			UIView viewOfBarButtonItem = (UIView)((UIBarButtonItem)sender).ValueForKey (new NSString("view"));
			historyPop.PresentFromRect (viewOfBarButtonItem.Frame, viewOfBarButtonItem.Superview, UIPopoverArrowDirection.Up, true);

		}



		/// <summary>
		/// Toggles the left container view.
		/// </summary>
		void ToggleLeftContainerView ()
		{
			if (LeftContainerViewLeadingConstraint.Constant == 0) {
				ToggleSidebarButton.SetBackgroundImage (new UIImage("Images/Navigation/SidebarIcon.png"), UIControlState.Normal);
				ToggleSideBarView.BackgroundColor = UIColor.Clear;

			} else {
				ToggleSidebarButton.SetBackgroundImage (new UIImage ("Images/Navigation/SidebarIconWhite.png"), UIControlState.Normal);
				ToggleSideBarView.BackgroundColor = UIColor.Red;

			}

			UIView.Animate (0.3, delegate {
				LeftContainerViewLeadingConstraint.Constant = LeftContainerViewLeadingConstraint.Constant == 0 ? -321 : 0;
				View.LayoutIfNeeded();
			});
		}


		void ShowShareAcitvity (object sender, EventArgs e)
		{
			
			PdfUtil.SaveContentInWebViewAsPDF ();

			IDirectory dirInstance = IoCContainer.Instance.ResolveInterface<IDirectory> ();
			string fileName = dirInstance.GetAppRootPath () + ViewConstant.SHARE_TMP_PDF_NAME;

			//customize share menu
			var activityController = new UIActivityViewController (new NSObject[]{NSData.FromFile (fileName)}, null);
			activityController.ExcludedActivityTypes = new NSString[]{UIActivityType.Message, UIActivityType.CopyToPasteboard};
			UIPopoverController sharePop = new UIPopoverController(activityController);
			AppDisplayUtil.Instance.SetPopoverController (sharePop);
			sharePop.BackgroundColor = UIColor.White;
  			UIView viewOfBarButtonItem = (UIView)((UIBarButtonItem)sender).ValueForKey (new NSString("view"));
			sharePop.PresentFromRect (viewOfBarButtonItem.Frame, viewOfBarButtonItem.Superview, UIPopoverArrowDirection.Up, true);

		}

		public  void Update(Subject s)
		{
			NavigationTitleLabel.Text = AppDataUtil.Instance.GetCurrentPublication ().Name;
			infoBarButton.SetBadage (AppDataUtil.Instance.GetCurrentPublication ().UpdateCount);
 		}
	}
}
