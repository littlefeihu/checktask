
using System;

using Foundation;
using UIKit;

using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.iOS
{
	public partial class ChangePasswordController : UIViewController
	{
		public ChangePasswordController () : base ("ChangePasswordController", null)
		{
			Title = "Welcome to LexisNexis Red";
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			UIBarButtonItem sendBarButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, SendChangePasswordRequest);
			sendBarButton.Title = "Send";
			UIBarButtonItem cancelBarButton = new UIBarButtonItem (UIBarButtonSystemItem.Cancel, CancelChangePasswordAction);

			NavigationItem.RightBarButtonItem = sendBarButton;
			NavigationItem.LeftBarButtonItem = cancelBarButton;
		}

		void CancelChangePasswordAction(object sender, EventArgs e)
		{
			PresentingViewController.DismissViewController(true, null);
			//go back to login view
			AppDisplayUtil.Instance.GoToLoginView ();
		}



		async void SendChangePasswordRequest(object sender, EventArgs e)
		{
			PasswordChangeEnum changePasswordResponse =  await LoginUtil.Instance.ChangePassword (passwordTextField.Text, retypePasswordField.Text);

			var errorMessage = "";
			switch (changePasswordResponse) {
			case PasswordChangeEnum.NetDisconnected:
				errorMessage = "Network connection is not available";
				break;
			case PasswordChangeEnum.LengthInvalid:
				errorMessage = "Your new password is too short. Please select a password with at least 4 characters.";
				break;
			case PasswordChangeEnum.ChangeFailure:
				errorMessage = "Failed to change password";
				break;
			case PasswordChangeEnum.NotMatch:
				errorMessage = "Passwords do not match. Please re-enter.";
				break;
			}

			var alertTitle = "";
			var alertMsg = errorMessage;
			if (errorMessage.Length == 0) {
				alertMsg = "Your password has been successfully changed";
			} else {
				alertTitle = "Error Message";
			}

			var changePasswordAlertController = UIAlertController.Create (alertTitle, alertMsg, UIAlertControllerStyle.Alert);
			if (errorMessage.Length == 0) {
				//changePasswordAlertController.AddAction (UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, Action => DismissChangePasswordSuccessAlertView()));
				changePasswordAlertController.AddAction (UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, (UIAlertAction o)=>{
					DismissChangePasswordSuccessAlertView();
					AppDisplayUtil.Instance.AppDelegateInstance.LoginInterceptorVC.GoToMyPublicationViewController();
				}));

			} else {
				changePasswordAlertController.AddAction (UIAlertAction.Create("OK", UIAlertActionStyle.Cancel, null));
			}

			PresentViewController (changePasswordAlertController, true, null);

		}

		void DismissChangePasswordSuccessAlertView () 
		{
			PresentingViewController.DismissViewController(true, null);
			
		}

	}
}

