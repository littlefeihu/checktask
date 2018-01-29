
using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common;
using Fragment=Android.Support.V4.App.Fragment;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.MyPublicationsPage;
using LexisNexis.Red.Droid.Business;
using Android.Views.InputMethods;
using Android.Content;
using System.Threading.Tasks;
using System.Threading;
using Android.Support.V4.App;
using LexisNexis.Red.Droid.Utility;

namespace LexisNexis.Red.Droid.LoginPage
{
	public class LoginFragment : Fragment
	{
		private string email;
		private string password;
		private int selectCountryIndex;

		private EditText etEmail;
		private EditText etPassword;
		private Spinner spCountry;
		private TextView tvForgetPassword;
		private Button btnLogin;
		private TextView tvLoginWaringMessage;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set the fragment as RetainFragment
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var countryAdapter = new ArrayAdapter<string>(
				Activity,
				Resource.Layout.loginpage_spinner_item,
				DataCache.INSTATNCE.ServiceCountryList);
			countryAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

			var v = inflater.Inflate(Resource.Layout.login_fragment, container, false);

			etEmail = v.FindViewById<EditText>(Resource.Id.etEmail);
			etPassword = v.FindViewById<EditText>(Resource.Id.etPassword);
			spCountry = v.FindViewById<Spinner>(Resource.Id.spCountry);
			tvForgetPassword = v.FindViewById<TextView>(Resource.Id.tvForgetPassword);
			btnLogin = v.FindViewById<Button>(Resource.Id.btnLogin);
			tvLoginWaringMessage = v.FindViewById<TextView>(Resource.Id.tvLoginWaringMessage);

			/*
			Application.SynchronizationContext.Post(_ =>{
				Task.Run(delegate{
					Thread.Sleep(100);
				});
				etEmail.RequestFocus();
				InputMethodManager imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
				imm.ShowSoftInput(etEmail, ShowFlags.Implicit);
			}, null);
			*/

			btnLogin.Typeface = CustomFontsCache.GetTypeface("Roboto-Medium.ttf");

			etEmail.TextChanged += delegate
			{
				email = etEmail.Text;
			};

			etPassword.TextChanged += delegate
			{
				password = etPassword.Text;
			};

			etPassword.SetOnEditorActionListener(new EditorActionListener(this));

			selectCountryIndex = 0;
			spCountry.ItemSelected += delegate(object sender, AdapterView.ItemSelectedEventArgs e)
			{
				selectCountryIndex = e.Position;
			};

			spCountry.Adapter = countryAdapter;
            spCountry.Visibility = ViewStates.Invisible;
            //var lastLoginUser = LoginUtil.Instance.GetLastUserLoginInfo();
            //if(lastLoginUser != null)
            //{
            //	int lastUserLoginCountryIndex = -1;
            //	for(int i = 0; i < ConfigurationService.GetAllCountryMap().Count; ++i)
            //	{
            //		if(ConfigurationService.GetAllCountryMap()[i].CountryCode == lastLoginUser.Country.CountryCode)
            //		{
            //			lastUserLoginCountryIndex = i;
            //			break;
            //		}
            //	}

            //	if(lastUserLoginCountryIndex >= 0)
            //	{
            //		spCountry.SetSelection(lastUserLoginCountryIndex);
            //	}
            //}

            tvForgetPassword.Click += delegate
			{
				var loginActivity = Activity as ILoginActivity;
				if(loginActivity == null)
					return;

				loginActivity.LoginFragment_ReminderPassowrd(selectCountryIndex);
			};

			btnLogin.Click += delegate
			{
				BtnLogin_Click();
			};
				
			return v;
		}

		private class EditorActionListener: Java.Lang.Object, TextView.IOnEditorActionListener
		{
			private readonly LoginFragment fragment;

			public EditorActionListener(LoginFragment fragment)
			{
				this.fragment = fragment;
			}

			public bool OnEditorAction(
				TextView v,
				ImeAction actionId,
				KeyEvent e)
			{
				if(actionId != ImeAction.Done)
				{
					return false;
				}

				fragment.BtnLogin_Click();
				return true;
			}
		}

		private async void BtnLogin_Click()
		{
			// Call Business Api
			ILoginActivity loginActivity = Activity as ILoginActivity;
			if(loginActivity == null)
				return;

			var dialogTag = DialogGenerator.ShowWaitDialog(FragmentManager);

			LoginStatusEnum result = LoginStatusEnum.LoginFailure;
			try
			{
				result = await LoginUtil.Instance.ValidateUserLogin(
					etEmail.Text, etPassword.Text,
					ConfigurationService.GetAllCountryMap()[selectCountryIndex].CountryCode);
			}
			catch(Exception ee)
			{
				// force app exit
				// The common api should not throw any exception!
				throw ee;
			}

			AsyncUIOperationRepeater.INSTATNCE.SubmitAsyncUIOperation(
				(IAsyncTaskActivity)Activity,
				a =>
					{
						SimpleDialogFragment.DismissDialog(((FragmentActivity)a),dialogTag);
						ProcessLoginResult(result);
					});
		}

		private void ProcessLoginResult(LoginStatusEnum result)
		{
			var loginActivity = Activity as ILoginActivity;
			if(loginActivity == null)
				return;
			
			switch(result)
			{
			case LoginStatusEnum.LoginSuccess:
				{
					if(GlobalAccess.Instance.CurrentUserInfo.NeedChangePassword)
					{
						ResetPasswordDialogFragment resetPasswordDialogFragment = new ResetPasswordDialogFragment();
						resetPasswordDialogFragment.Show(FragmentManager.BeginTransaction(), "resetPasswordDialogFragment");
					}
					else
					{
						loginActivity.LoginSucceed();
					}
					break;
				}
			case LoginStatusEnum.NetDisconnected:
				{
					// 2
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.LoginErrorMessage_NetDisconnected_Title,
						Resource.String.LoginErrorMessage_NetDisconnected);
					break;
				}
			case LoginStatusEnum.InvalidemailAndValidPwd:
				{
					// 7
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_InvalidemailAndValidPwd));
					break;
				}
			case LoginStatusEnum.ValidemailAndInvalidPwd:
				{
					// 8
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_ValidemailAndInvalidPwd));
					break;
				}
			case LoginStatusEnum.InvalidemailAndInvalidPwd:
				{
					// 9
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_InvalidemailAndInvalidPwd));
					break;
				}
			case LoginStatusEnum.ValidemailAndEmptyPwd:
				{
					// 10
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_ValidemailAndEmptyPwd));
					break;
				}
			case LoginStatusEnum.EmptyemailAndValidPwd:
				{
					// 11
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_EmptyemailAndValidPwd));
					break;
				}
			case LoginStatusEnum.InvalidemailAndEmptyPwd:
				{
					// 12
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_InvalidemailAndEmptyPwd));
					break;
				}
			case LoginStatusEnum.SelectCountry:
				{
					// 13
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_SelectCountry));
					break;
				}
			case LoginStatusEnum.EmailOrPwdError:
				{
					// may be 7
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_EmailOrPwdError));
					break;
				}
			case LoginStatusEnum.AccountNotExist:
				{
					// 19
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.LoginErrorMessage_AccountNotExistTitle,
						Resource.String.LoginErrorMessage_AccountNotExist);
					break;
				}
			case LoginStatusEnum.EmptyEmailAndEmptyPwd:
				{
					// may be 11
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_EmptyEmailAndEmptyPwd));
					break;
				}

			case LoginStatusEnum.DeviceLimit:
				{
					// Not provide
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_DeviceLimit));
					break;
				}
			case LoginStatusEnum.LoginFailure:
				{
					// Not provide
					SetLoginWaringMessage(
						Activity.Resources.GetString(Resource.String.LoginErrorMessage_LoginFailure));
					break;
				}
			}
		}

		private void SetLoginWaringMessage(string waringMessage)
		{
			if(string.IsNullOrEmpty(waringMessage))
			{
				tvLoginWaringMessage.Visibility = ViewStates.Gone;
			}
			else
			{
				tvLoginWaringMessage.Visibility = ViewStates.Visible;
				tvLoginWaringMessage.Text = waringMessage;
			}
		}
	}
}

