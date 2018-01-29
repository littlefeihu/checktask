using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreAnimation;
using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Services;

using MBProgressHUD;

namespace LexisNexis.Red.iOS
{
	partial class ResetPasswordViewController : UIViewController,IUIAlertViewDelegate
	{
		private List<Country> regions;

		
		public ResetPasswordViewController (IntPtr handle) : base (handle)
		{
			regions = ConfigurationService.GetAllCountryMap ();

		}

		#region override UIViewController
		public override void LoadView()
		{
			base.LoadView ();

			//Add tap gesture recognizer for view
			UITapGestureRecognizer tapSpaceRecoginzer = new UITapGestureRecognizer ();
			tapSpaceRecoginzer.AddTarget (() => EmailTextField.ResignFirstResponder ());
			View.AddGestureRecognizer (tapSpaceRecoginzer);
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			ResetPasswordFormContainerView.Layer.CornerRadius = 5;

			UIButton backButton = new UIButton (UIButtonType.Custom);

			UIImageView backImageView = new UIImageView (new UIImage ("Images/Navigation/RedBackIcon.jpg"));
			backImageView.Frame = new CGRect (0, 5, 12, 20);

			UILabel backTextLabel = new UILabel ();
			backTextLabel.Text = "Back";
			backTextLabel.Frame = new CGRect (20, 0, 100, 30);
			backTextLabel.TextColor = UIColor.White;

			backButton.Frame = new CGRect (15, 0, 112, 30 );
			backButton.AddSubview (backImageView);
			backButton.AddSubview (backTextLabel);
			backButton.TouchUpInside += Back;

			CustomizedNavigationBarView.AddSubview (backButton);

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			EmailTextField.ResignFirstResponder();
		}

		/*
		 *处理屏幕转动产生的显示问题 
		 */
		public override void WillAnimateRotation (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillAnimateRotation (toInterfaceOrientation, duration);

			EmailTextField.ResignFirstResponder();

		}
		#endregion


		async partial void SendResetPasswordRequest (Foundation.NSObject sender)
		{

			var hud = new MTMBProgressHUD (View) {
				LabelText = "Loading",
				RemoveFromSuperViewOnHide = true
			};
			View.AddSubview (hud);
			hud.Show (animated: true);

			var selectedRegion = regions.Find(o => o.CountryName == RegionNameTextField.Text);
			string regionCode = selectedRegion == null ? "": selectedRegion.CountryCode;
			
			var resetPasswordResponse = await LoginUtil.Instance.ResetPassword(EmailTextField.Text, regionCode);


			//TODO, I18N, code refactor
			string errorMessage = "";
			switch (resetPasswordResponse) {
			case PasswordResetEnum.EmailNotExist:
				errorMessage = "Email address does not exist.";
				break;
			case PasswordResetEnum.InvalidEmail:
				errorMessage = "Enter valid email address";
				break;
			case PasswordResetEnum.DeviceIdNotMatched:
				errorMessage = "Device id is not mapped to the email address provided.";
				break;
			case PasswordResetEnum.NetDisconnected:
				errorMessage = "Unable to communicate with LexisNexis Red services. Please ensure you have an internet connection, or try again later as the servers may be busy.";
				break;
			case PasswordResetEnum.ResetFailure:
				errorMessage = "Failed to reset password.";
				break;
			case PasswordResetEnum.SelectCountry:
				errorMessage = "Please select a country.";
				break;
			case PasswordResetEnum.ResetSuccess:
				break;
			default:
				errorMessage = "Reset password failed, unknow error.";
				break;
			}


			var alertTitle = "";
			var alertMsg   = "";
			if (errorMessage.Equals ("")) {
				alertTitle = "Done";
				alertMsg = "Your password reset request has been sent.";
			} else {
				alertTitle = resetPasswordResponse == PasswordResetEnum.NetDisconnected ? "Server Error" : "Error!!";
				alertMsg = errorMessage;
			}
			var resetPasswordAlert = UIAlertController.Create (alertTitle, alertMsg , UIAlertControllerStyle.Alert);
			if (errorMessage.Equals ("")) {
				resetPasswordAlert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, Action => DismissResetSuccessAlertView()));
			} else {
				resetPasswordAlert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Cancel, null));
			}
			PresentViewController (resetPasswordAlert, true, null);

			hud.Hide (animated: true, delay: 1);

		}
		 
		void DismissResetSuccessAlertView ()
		{
			DismissViewController (true,null);
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}

		partial void ShowRegionsSelectorTableView (Foundation.NSObject sender)
		{	
			EmailTextField.ResignFirstResponder();

			RegionsTableViewController regionsTVC = new RegionsTableViewController(SetSelectedRegion, RegionNameTextField.Text);

			UIPopoverController popoverController = new UIPopoverController(regionsTVC);
			popoverController.BackgroundColor = UIColor.White;
			popoverController.SetPopoverContentSize(new CoreGraphics.CGSize(340, 230), true);
			AppDisplayUtil.Instance.SetPopoverController (popoverController);

			CGRect anchorFrame = ((UIButton)sender).Frame;
			anchorFrame.Width /= 2;

			popoverController.PresentFromRect(new CGRect(75, 30 , 0, 0),  (UIView)sender, UIPopoverArrowDirection.Any, true);
		}

		public void SetSelectedRegion(string regionName)
		{
			RegionNameTextField.Text = regionName;
		}

		public void Back(object o, EventArgs e)
		{
			AppDisplayUtil.Instance.DismissPopoverView ();
			NavigationController.PopViewController (true);

		}
	}
}
