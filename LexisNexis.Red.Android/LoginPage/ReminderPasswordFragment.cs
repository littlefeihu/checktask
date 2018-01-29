
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
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Common;
using Fragment=Android.Support.V4.App.Fragment;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.Business;
using Android.Support.V4.App;
using LexisNexis.Red.Droid.Utility;
using System.Threading;

namespace LexisNexis.Red.Droid.LoginPage
{
	public class ReminderPasswordFragment : Fragment, ISimpleDialogListener
	{
		public const string SELECTED_COUNTRY_INDEX = "SelectedCountryIndex";
		public const string ReminderPasswordSucceedDialog = "ReminderPasswordSucceedDialog";

		public static ReminderPasswordFragment NewInstance(int selectedCountryIndex)
		{
			Bundle b = new Bundle();
			b.PutInt(SELECTED_COUNTRY_INDEX, selectedCountryIndex);
			ReminderPasswordFragment fragment = new ReminderPasswordFragment();
			fragment.Arguments = b;
			return fragment;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set the fragment as RetainFragment
			RetainInstance = true;
		}

		private string email;
		private int selectCountryIndex;

		private EditText etEmail;
		private Spinner spCountry;
		private Button btnSubmit;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.remainder_password_fragment, container, false);

			etEmail = v.FindViewById<EditText> (Resource.Id.etEmail);
			spCountry = v.FindViewById<Spinner> (Resource.Id.spCountry);
			btnSubmit = v.FindViewById<Button> (Resource.Id.btnSubmit);

			btnSubmit.Typeface = CustomFontsCache.GetTypeface("Roboto-Medium.ttf");

			etEmail.TextChanged += delegate
			{
				email = etEmail.Text;
			};

			etEmail.SetOnEditorActionListener(new EditorActionListener(this));

			var countryAdapter = new ArrayAdapter<string>
				(Activity, Resource.Layout.loginpage_spinner_item, DataCache.INSTATNCE.ServiceCountryList);
			countryAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

			spCountry.ItemSelected += delegate(object sender, AdapterView.ItemSelectedEventArgs e)
			{
				selectCountryIndex = e.Position;
			};
			spCountry.Adapter = countryAdapter;

			selectCountryIndex = Arguments == null ? 0 : Arguments.GetInt(SELECTED_COUNTRY_INDEX);
			if(selectCountryIndex > 0)
			{
				spCountry.SetSelection(selectCountryIndex);
			}

			btnSubmit.Click += delegate
			{
				// Call Business Api
				processReminderPassword();
			};

			etEmail.RequestFocus();

			return v;
		}

		private class EditorActionListener: Java.Lang.Object, TextView.IOnEditorActionListener
		{
			private readonly ReminderPasswordFragment fragment;

			public EditorActionListener(ReminderPasswordFragment fragment)
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

				fragment.processReminderPassword();
				return true;
			}
		}

		private async void processReminderPassword()
		{
			ILoginActivity loginActivity = Activity as ILoginActivity;
			if(loginActivity == null)
				return;

			var dialogTag = DialogGenerator.ShowWaitDialog(FragmentManager);

			var result = await LoginUtil.Instance.ResetPassword(
				etEmail.Text,
				ConfigurationService.GetAllCountryMap()[selectCountryIndex].CountryCode);

			AsyncUIOperationRepeater.INSTATNCE.SubmitAsyncUIOperation(
				(IAsyncTaskActivity)Activity,
				a =>
					{
						SimpleDialogFragment.DismissDialog(((FragmentActivity)a), dialogTag);
						ProcessResetPasswordResult(result);
					});
		}

		private void ProcessResetPasswordResult(PasswordResetEnum result)
		{
			switch(result)
			{
			case PasswordResetEnum.EmailNotExist:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ReminderPasswordErrorTitle,
						Resource.String.ReminderPasswordError);
					break;
				}
			case PasswordResetEnum.ResetSuccess:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ReminderPasswordTitle,
						Resource.String.ReminderPasswordSucceed,
						0,
						-1,
						ReminderPasswordSucceedDialog
						);
					break;
				}
			case PasswordResetEnum.InvalidEmail:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ReminderPassword_ErrorTitleErrorMessage,
						Resource.String.ReminderPassword_InvalidEmail);
					break;
				}
			case PasswordResetEnum.DeviceIdNotMatched:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ReminderPasswordErrorTitle,
						Resource.String.ReminderPassword_DeviceIdNotMatched);
					break;
				}
			case PasswordResetEnum.SelectCountry:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ReminderPasswordErrorTitle,
						Resource.String.LoginErrorMessage_SelectCountry);
					break;
				}
			case PasswordResetEnum.NetDisconnected:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ReminderPasswordErrorTitle,
						Resource.String.LoginErrorMessage_NetDisconnected);
					break;
				}
			case PasswordResetEnum.ResetFailure:
				{
					DialogGenerator.ShowMessageDialog(
						FragmentManager,
						Resource.String.ReminderPasswordErrorTitle,
						Resource.String.ReminderPassword_ResetFailure);
					break;
				}
			}

			return;
		}

		public bool OnSimpleDialogButtonClick(DialogButtonType buttonType, string fragmentTag, string extTagKey, string extTag)
		{
			if(extTagKey == ReminderPasswordSucceedDialog)
			{
				var loginActivity = Activity as ILoginActivity;
				if(loginActivity != null)
				{
					loginActivity.ReminderPasswordFragment_ReturnLogin();
				}

				return true;
			}

			return false;
		}
	}
}

