using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreAnimation;
using CoreGraphics;

using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Services;

using MBProgressHUD;

namespace LexisNexis.Red.iOS
{
	public delegate void SetTextFieldText (string text);

	partial class LoginViewController : UIViewController
	{
		private List<Country> regions;
 
		public LoginViewController (IntPtr handle) : base (handle)
		{
			regions = ConfigurationService.GetAllCountryMap ();
		}

		#region override UIViewController
		public override void LoadView()
		{
			base.LoadView ();
			//Add tap gesture recognizer for view
			UITapGestureRecognizer tapSpaceRecoginzer = new UITapGestureRecognizer ();
			tapSpaceRecoginzer.AddTarget (ResignKeyboard);
			View.AddGestureRecognizer (tapSpaceRecoginzer);
  
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			LoginFormContainerView.Layer.CornerRadius = 5;

  		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			ResignKeyboard ();
		}


		/// <summary>
		/// Preferreds the status bar style.
		/// override this method to set the font color of status bar
		/// </summary>
		/// <returns>The status bar style.</returns>
		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}
		#endregion

		partial void EmailDidEndOnExit (Foundation.NSObject sender)
		{
			SendLoginRequest();
		}

		partial void PasswordDidEndOnExit (Foundation.NSObject sender)
		{
			SendLoginRequest();
		}

		private async void SendLoginRequest()
		{
			var hud = new MTMBProgressHUD (View) {
				LabelText = "Loading",
				RemoveFromSuperViewOnHide = true
			};
			hud.YOffset = 90.0f;
 			View.AddSubview (hud);
			hud.Show (animated: true);

			//invoke login business logic
			var selectedRegion = regions.Find(o => o.CountryName == selectedRegionTextField.Text);
			string regionCode = selectedRegion == null ? "": selectedRegion.CountryCode;

			var loginResponse = await SignIn (emailTextField.Text, passwordTextField.Text, regionCode);

			string alertTitle = "";
			string alertMsg = "";
			switch (loginResponse) {
			case LoginStatusEnum.LoginSuccess:
				hud.Hide (animated: true, delay: 1);
				var curUser= GlobalAccess.Instance.CurrentUserInfo;
				if (curUser.NeedChangePassword) {
					ChangePasswordController changePasswordVC = new ChangePasswordController ();
					UINavigationController navController = new UINavigationController (changePasswordVC);
					navController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
					PresentViewController (navController, true, null);
				}else{
					NavigationController.PopViewController(false);
					AppDisplayUtil.Instance.AppDelegateInstance.LoginInterceptorVC.GoToMyPublicationViewController();
				}
				break;
			case LoginStatusEnum.EmptyEmailAndEmptyPwd:
			case LoginStatusEnum.EmptyemailAndValidPwd:
			case LoginStatusEnum.EmptyemailAndInvalidPwd:
				alertTitle = "Login Failed";
				alertMsg   = "Please enter your email address to login.";
				break;
			case LoginStatusEnum.InvalidemailAndEmptyPwd:
			case LoginStatusEnum.ValidemailAndEmptyPwd:
				alertTitle = "Login Failed";
				alertMsg   = "Please enter your password to login.";
				break;
			case LoginStatusEnum.SelectCountry:
				alertTitle = "Login Failed";
				alertMsg   = "Please select a country to login.";
				break;
			case LoginStatusEnum.InvalidemailAndInvalidPwd:
			case LoginStatusEnum.InvalidemailAndValidPwd:
			case LoginStatusEnum.ValidemailAndInvalidPwd:
			case LoginStatusEnum.EmailOrPwdError:
				alertTitle = "Login Failed";
				alertMsg  = "Either the email or password you entered is incorrect. Please try again.";
				break;
			case LoginStatusEnum.AccountNotExist:
				alertTitle = "Login Failed";
				alertMsg = "Email address does not exist.";
				break;
			case LoginStatusEnum.DeviceLimit:
				alertTitle = "Login Failed";
				alertMsg  = "Exceed device limition";
				break;
			case LoginStatusEnum.NetDisconnected:
				alertTitle = "Server Error";
				alertMsg = "Unable to communicate with LexisNexis Red services. Please ensure you have an internet connection, or try again later as the servers may be busy.";
				break;
			}


			if (alertMsg != "") {
				var loginAlert = UIAlertController.Create (alertTitle, alertMsg, UIAlertControllerStyle.Alert);
				loginAlert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
				PresentViewController (loginAlert, true, null);
			}

			hud.Hide (animated: true, delay: 1);

		}


		private async Task<LoginStatusEnum> SignIn (string email, string password, string countryCode)
		{
			var loginResponse = await LoginUtil.Instance.ValidateUserLogin (email, password, countryCode);
			return loginResponse;
		}
			
		/// <summary>
		/// Resigns first responder for all text field to close keyboard.
		/// </summary>
		void ResignKeyboard ()
		{
			emailTextField.ResignFirstResponder ();
			passwordTextField.ResignFirstResponder ();
			selectedRegionTextField.ResignFirstResponder ();
		}


		partial void ShowRegionsSelectorTableView (Foundation.NSObject sender)
		{	
			RegionsTableViewController regionsTVC = new RegionsTableViewController(SetSelectedRegion, selectedRegionTextField.Text);

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
			selectedRegionTextField.Text = regionName;
		}

	}
}
