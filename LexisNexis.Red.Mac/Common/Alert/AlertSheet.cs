using System;

using AppKit;

using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac
{
	public static class AlertSheet
	{
		public static NSAlert InfoAlert;
		public static NSAlert PromptAlert;
		public static nint RunConfirmAlert (string title, string errorMsg)
		{
			NSAlert alert = NSAlert.WithMessage(title,"Confirm","Cancel",null,errorMsg);
			InfoAlert = alert;

			alert.Window.MakeFirstResponder (null);
			var keyWindow = NSApplication.SharedApplication.KeyWindow;
			return alert.RunSheetModal (keyWindow);
		}

		public static void DestroyConfirmAlert ()
		{
			NSApplication NSApp = NSApplication.SharedApplication;
			if (InfoAlert != null) {
				NSApp.EndSheet (InfoAlert.Window);
				InfoAlert = null;
			}
		}

		public static nint RunPromptAlert (string title, string errorMsg)
		{
			NSAlert alert = NSAlert.WithMessage(title,"OK",null,null,errorMsg);
			if (PromptAlert == null || !PromptAlert.Window.IsVisible) {
				PromptAlert = alert;
				PromptAlert.Window.MakeFirstResponder (null);
				return alert.RunSheetModal (NSApplication.SharedApplication.MainWindow);
			} 

			return 0;
		}

		public static nint RunPromptModal (string title, string errorMsg)
		{
			NSAlert alert = NSAlert.WithMessage(title,"OK",null,null,errorMsg);
			alert.Window.MakeFirstResponder (null);
			return alert.RunModal();
		}

		public static nint ShowMissingConnectAlert ()
		{
			return AlertSheet.RunPromptAlert ("Missing Connection", "Sorry, there appears to no Internet connection. A connection is required to complete this task.");
		}

		public static nint RunDeleteAlert (string title, string errorMsg)
		{
			NSAlert alert = NSAlert.WithMessage(title,"Delete","Cancel",null,errorMsg);
			alert.Window.MakeFirstResponder (null);
			return alert.RunModal();
		}

		public static void RunLoginAlert(LoginStatusEnum loginResponse)
		{
			string errorMsg = "";
			string title;
			switch (loginResponse) {
			case LoginStatusEnum.NetDisconnected:
				title = "Server Error";
				errorMsg += "Unable to communicate with LexisNexis Red services. Please ensure you have an internet connection, or try again later as the servers may be busy.";
				break;

			case LoginStatusEnum.InvalidemailAndValidPwd:
			case LoginStatusEnum.InvalidemailAndInvalidPwd:
				title = "Error";
				errorMsg += "The email address you entered does not look correct.";
				break;

			case LoginStatusEnum.LoginSuccess:
				title = "";
				errorMsg += "Login success";
				break;

			case LoginStatusEnum.EmptyEmailAndEmptyPwd:
				title = "Error";
				errorMsg += "Please enter both your email address and password to log in.";
				break;
				
			case LoginStatusEnum.ValidemailAndEmptyPwd:
				title = "Error";
				errorMsg += "Please enter both your email and password to log in.";
				break;

			case LoginStatusEnum.EmptyemailAndValidPwd:
				title = "Error";
				errorMsg += "The email address you entered does not look correct.";
				break;

			case LoginStatusEnum.ValidemailAndInvalidPwd:
			case LoginStatusEnum.EmailOrPwdError:
				title = "Error";
				errorMsg += "Either the email or password you have entered is incorrect. Please try again.";
				break;

			case LoginStatusEnum.InvalidemailAndEmptyPwd:
				title = "Error";
				errorMsg += "The email address you entered does not look correct.";
				errorMsg += Environment.NewLine;
				errorMsg += "Please enter both your email and password to login.";
				break;

			case LoginStatusEnum.SelectCountry:
				title = "Error";
				errorMsg += "Please select a country from the drop down list.";
				break;

			case LoginStatusEnum.DeviceLimit:
				title = "Error";
				errorMsg += "Exceed device limition.";
				break;

			case LoginStatusEnum.AccountNotExist:
				title = "Error";
				errorMsg += "Email address does not exist.  Pelase try again.";
				break;

			default:
				title = "Error";
				errorMsg += "Login failed, unknow error!";
				break;

			}

			if (errorMsg != "") {
				NSAlert alert = NSAlert.WithMessage(title,"OK",null,null,errorMsg);
				alert.Window.MakeFirstResponder (null);
				Run (alert);
			}
		}

		public static void RunResetPasswordAlert(NSWindow parentWindow, PasswordResetEnum resetResponse)
		{
			string errorMsg = "";
			string title;

			switch (resetResponse) {
			case PasswordResetEnum.NetDisconnected:
				title = "Server Error";
				errorMsg += "Unable to communicate with LexisNexis Red services. Please ensure you have an internet connection, or try again later as the servers may be busy.";
				break;

			case PasswordResetEnum.EmailNotExist:
			case PasswordResetEnum.InvalidEmail:
				title = "Error";
				errorMsg += "The email address you entered does not look correct. Please enter valid email address!";
				break;

			case PasswordResetEnum.ResetSuccess:
				title = "";
				errorMsg += "Your password reset request has been sent.";
				break;

			case PasswordResetEnum.ResetFailure:
				title = "Error";
				errorMsg += "Login failed, unknow error!";
				break;

			case PasswordResetEnum.SelectCountry:
				title = "Error";
				errorMsg += "Please select a country from the drop down list.";
				break;

			case PasswordResetEnum.DeviceIdNotMatched:
				title = "Error";
				errorMsg += "Device ID is not mapped to the email address provided."; //Device ID is not mapped to the email address provided.
				break;

			default:
				title = "Error";
				errorMsg += "Login failed, unknow error!";
				break;

			}

			if (errorMsg != "") {
				NSAlert alert = NSAlert.WithMessage(title,"OK",null,null,errorMsg);
				alert.Window.MakeFirstResponder (null);
				alert.RunSheetModal (parentWindow);
			}
		}

		public static nint RunChangePasswordAlert(NSWindow parentWindow, PasswordChangeEnum changeResponse)
		{
			string errorMsg = "";
			string title = "";
			switch (changeResponse) {
			case PasswordChangeEnum.NetDisconnected:
				title = "Server Error";
				errorMsg += "Unable to communicate with LexisNexis Red services. Please ensure you have an internet connection, or try again later as the servers may be busy.";
				break;
			case PasswordChangeEnum.LengthInvalid:
				title = "Error";
				errorMsg += "Your new password is too short. Please select a password with at least 4 characters.";
				break;
			case PasswordChangeEnum.NotMatch:
				title = "Error";
				errorMsg += "Passwords do not match. Please re-enter.";
				break;
			case PasswordChangeEnum.ChangeFailure:
				title = "Error";
				errorMsg += "Password change failue. Please send request again.";
				break;
			case PasswordChangeEnum.ChangeSuccess:
				errorMsg += "Your password has been changed successfully.";
				title = "";
				break;
			}
				
			NSAlert alert = NSAlert.WithMessage(title,"OK",null,null,errorMsg);
			alert.Window.MakeFirstResponder (null);
			return alert.RunSheetModal (parentWindow);
		}

		public static void Run (NSAlert alert)
		{
			alert.BeginSheetForResponse (NSApplication.SharedApplication.KeyWindow, response => ShowResponse (alert, response));
		}

		public static void ShowResponse (NSAlert alert, nint response)
		{
			string message;

			if (response <= 1) {
				switch (response) {
				case -1:
					message = String.Format ("Non-custom response: -1 (other)");
					break;
				case 0:
					message = String.Format ("Non-custom response: 0 (alternate)");
					break;
				case 1:
					message = String.Format ("Non-custom response: 1 (default)");
					break;
				default:
					message = String.Format ("Unknown Response: {0}", response);
					break;
				}
			} else {
				var buttonIndex = response - (int)NSAlertButtonReturn.First;
				if (buttonIndex >= alert.Buttons.Length)
					message = String.Format ("Unknown Response: {0}", response);
				else
					message = String.Format (
						"\"{0}\"\n\nButton Index: {1}\nResult (NSAlertButtonReturn): {2}\nResult (int): {3}",
						alert.Buttons [buttonIndex].Title,
						buttonIndex,
						response,  //(NSAlertButtonReturn)
						response);
			}

			if (alert.ShowsSuppressionButton)
				message += String.Format ("\nSuppression: {0}", alert.SuppressionButton.State);

			//ResultLabel.StringValue = message;
		}
	}
}

