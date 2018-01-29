
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Entity;
using DialogFragment=Android.Support.V4.App.DialogFragment;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Common.Business;
using Android.Support.V4.App;

namespace LexisNexis.Red.Droid.LoginPage
{
	public class ResetPasswordDialogFragment : DialogFragment, ISimpleDialogListener
	{
		private const string ChangePasswordSucceedDialog = "ChangePasswordSucceedDialog";
		private string newPassword;
		private string retypeNewPassword;

		private EditText etNewPassword;
		private EditText etRetypeNewPassword;
		private Button btnCancel;
		private Button btnSend;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
			Dialog.SetCanceledOnTouchOutside(false);

			var vwDialog = inflater.Inflate(Resource.Layout.dialog_template, container);
			vwDialog.FindViewById<TextView>(Resource.Id.tvDialogTitle).Text =
				Activity.Resources.GetString(Resource.String.Welcome2LNRed);
			btnCancel = vwDialog.FindViewById<Button>(Resource.Id.btnNegtive);
			btnSend = vwDialog.FindViewById<Button>(Resource.Id.btnPositive);
			btnSend.Text = Activity.Resources.GetString(Resource.String.Send);

			var vwResetPassword = inflater.Inflate(Resource.Layout.reset_password_fragment, container);
			etNewPassword = vwResetPassword.FindViewById<EditText> (Resource.Id.etNewPassword);
			etRetypeNewPassword = vwResetPassword.FindViewById<EditText> (Resource.Id.etRetypeNewPassword);

			var layoutParaments = new ViewGroup.LayoutParams(
				(int)Math.Round(MainApp.ThisApp.Resources.GetDimension(Resource.Dimension.login_page_resetpassword_dialogwidth)),
				ViewGroup.LayoutParams.WrapContent);
			
			vwDialog.FindViewById<LinearLayout>(Resource.Id.llDialogContentContainer).AddView(vwResetPassword, layoutParaments);

			btnCancel.Click += delegate
			{
				Dismiss();
			};

			btnSend.Click += delegate
			{
				ProcessSend();
			};

			etNewPassword.TextChanged += delegate
			{
				newPassword = etNewPassword.Text;
			};

			etRetypeNewPassword.TextChanged += delegate
			{
				retypeNewPassword = etRetypeNewPassword.Text;
			};

			etRetypeNewPassword.EditorAction += (sender, e) => {
				if (e.ActionId == ImeAction.Send) 
				{
					var imm = (InputMethodManager)etRetypeNewPassword.Context.GetSystemService(Context.InputMethodService);
					if(imm.IsActive)
					{
						imm.HideSoftInputFromWindow(etRetypeNewPassword.ApplicationWindowToken, 0);
					}

					ProcessSend();
				}
				else
				{
					e.Handled = false;
				}
			};

			return vwDialog;
		}

		private async void ProcessSend()
		{
			ILoginActivity loginActivity = Activity as ILoginActivity;
			if(loginActivity == null)
				return;

			var dialogTag = DialogGenerator.ShowWaitDialog(FragmentManager);

			var result = await LoginUtil.Instance.ChangePassword(etNewPassword.Text, etRetypeNewPassword.Text);

			AsyncUIOperationRepeater.INSTATNCE.SubmitAsyncUIOperation(
				(IAsyncTaskActivity)Activity,
				a =>
					{
						SimpleDialogFragment.DismissDialog(((FragmentActivity)a),dialogTag);
						ProcessResetPasswordResult(result);
					});
		}

		private void ProcessResetPasswordResult(PasswordChangeEnum result)
		{
			var loginActivity = Activity as ILoginActivity;
			if(loginActivity == null)
				return;
			switch(result)
			{
			case PasswordChangeEnum.ChangeSuccess:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ChangePasswordTitle,
						Resource.String.ChangePasswordSucceed,
						0,
						-1,
						ChangePasswordSucceedDialog);
					break;
				}
			case PasswordChangeEnum.LengthInvalid:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ChangePasswordTitle,
						Resource.String.ChangePasswordError_LengthInvalid);
					break;
				}
			case PasswordChangeEnum.NotMatch:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ChangePasswordTitle,
						Resource.String.ChangePasswordError_NotMatch);
					break;
				}
			case PasswordChangeEnum.ChangeFailure:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ChangePasswordTitle,
						Resource.String.ChangePasswordError_ChangeFail);
					break;
				}
			case PasswordChangeEnum.NetDisconnected:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ChangePasswordTitle,
						Resource.String.LoginErrorMessage_NetDisconnected);
					break;
				}
			}
		}

		public bool OnSimpleDialogButtonClick(DialogButtonType buttonType, string fragmentTag, string extTagKey, string extTag)
		{
			if(extTagKey == ChangePasswordSucceedDialog)
			{
				var currentLoginActivity = Activity as ILoginActivity;
				if(currentLoginActivity != null)
				{
					currentLoginActivity.LoginSucceed();
				}

				return true;
			}

			return false;
		}
	}
}

