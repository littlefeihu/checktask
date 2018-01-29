using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using LexisNexis.Red.Common;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Common.HelpClass;
using Android.Support.V4.App;
using LexisNexis.Red.Droid.Async;
//using Com.Readystatesoftware.Systembartint;
using Android.Graphics;
using LexisNexis.Red.Droid.Widget.StatusBar;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.WebViewUtility;
using LexisNexis.Red.Common.Business;
using Android.Content.Res;
using Android.Views.InputMethods;
using LexisNexis.Red.Droid.Utility;

namespace LexisNexis.Red.Droid.LoginPage
{
	[Activity(Label = "智慧消防", Theme = "@style/MainAppTheme", MainLauncher = true, Icon = "@drawable/icon")]
	public class LoginActivity : FragmentActivity, ILoginActivity, IAsyncTaskActivity, ViewTreeObserver.IOnGlobalLayoutListener
	{
		private string asyncTaskActivityUUID;

		#if PREVIEW
		public static bool AllowDebug = false;
		#endif

		private FrameLayout frgForm;

		private RelativeLayout rlRoot;
		private ImageView ivLogo;
		private int lastRootViewHeight = 0;
		private FrameLayout flSoftKeyboardProber;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

			this.Window.SetSoftInputMode(SoftInput.AdjustResize);

			StatusBarTintHelper.SetStatusBarColor(this);

			AsyncUIOperationRepeater.INSTATNCE.RegisterAsyncTaskActivity(this);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.login_activity);

			rlRoot = FindViewById<RelativeLayout>(Resource.Id.rlRoot);
			ivLogo = FindViewById<ImageView>(Resource.Id.ivLogo);
			flSoftKeyboardProber = FindViewById<FrameLayout>(Resource.Id.flSoftKeyboardProber);
			frgForm = FindViewById<FrameLayout>(Resource.Id.frgForm);

			#if PREVIEW
			FindViewById<ImageView>(Resource.Id.ivLogo).LongClick += delegate {
				AllowDebug = true;
				Toast.MakeText(this, "Allow Debug", ToastLength.Short).Show();
			};
			#endif

			//*
			rlRoot.Click += delegate
			{
				HideSoftKeyboard();
			};
			//*/

			rlRoot.ViewTreeObserver.AddOnGlobalLayoutListener(this);

			if(savedInstanceState != null)
			{
				asyncTaskActivityUUID = savedInstanceState.GetString(
					AsyncUIOperationRepeater.ASYNC_ACTIVITY_GUID);
			}

			if(DataCache.INSTATNCE.ServiceCountryList != null)
			{
				InitActivity();
			}
        }

		protected override void OnResume()
		{
			base.OnResume();
			AsyncUIOperationRepeater.INSTATNCE.RegisterAsyncTaskActivity(this);
			if(SupportFragmentManager.FindFragmentById(Resource.Id.frgForm) == null && DataCache.INSTATNCE.ServiceCountryList != null)
			{
				InitActivity();
			}

			AsyncUIOperationRepeater.INSTATNCE.ExecutePendingUIOperation(this);
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutString(AsyncUIOperationRepeater.ASYNC_ACTIVITY_GUID, asyncTaskActivityUUID);
			base.OnSaveInstanceState(outState);
		}

		protected override void OnStop()
		{
			AsyncUIOperationRepeater.INSTATNCE.UnregisterAsyncTaskActivity(this);
			base.OnStop();
		}

		public void OnGlobalLayout()
		{
			var rootViewHeight = flSoftKeyboardProber.Top + flSoftKeyboardProber.Height;
			if(lastRootViewHeight == rootViewHeight)
			{
				return;
			}

			lastRootViewHeight = rootViewHeight;

			if(rootViewHeight - rlRoot.RootView.Height / 2 > Conversion.Dp2Px(100))
			{
				// the screen is heigh enough;
				// need not move the form
				var lp = (RelativeLayout.LayoutParams)ivLogo.LayoutParameters;
							lp.TopMargin = Math.Max(Conversion.Dp2Px(20), (rlRoot.RootView.Height / 2) - Conversion.Dp2Px(250));
				ivLogo.LayoutParameters = lp;
			}
			else
			{
				var lp = (RelativeLayout.LayoutParams)ivLogo.LayoutParameters;
				lp.TopMargin = Math.Max(Conversion.Dp2Px(20), (rootViewHeight / 2) - Conversion.Dp2Px(250));
				ivLogo.LayoutParameters = lp;
			}
		}

		public void InitActivity()
		{
			bool havePendingUIOperation = false;
			if(string.IsNullOrEmpty(asyncTaskActivityUUID))
			{
				asyncTaskActivityUUID = Guid.NewGuid().ToString();
			}
			else
			{
				havePendingUIOperation = AsyncUIOperationRepeater.INSTATNCE.HavePendingUIOperation(this);
			}

			// If the activity has pending ui operation, that means it was destroyed by os and recreate.
			// And a wait dialog might block the ui. We need to execute ui operation instead of jump to next page.
			if(!havePendingUIOperation)
			{
				if(GlobalAccess.Instance.CurrentUserInfo != null
					&& (!GlobalAccess.Instance.CurrentUserInfo.NeedChangePassword))
				{
					// User has login, jump to next page
					LoginSucceed();
					return;
				}
			}

			var fragment = SupportFragmentManager.FindFragmentById (Resource.Id.frgForm);

			if (fragment == null)
			{
				var loginFragment = new LoginFragment ();
				SupportFragmentManager.BeginTransaction ().Add (Resource.Id.frgForm, loginFragment).Commit ();
			}
		}

		public void HideSoftKeyboard()
		{
			var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(rlRoot.WindowToken, 0);
			Console.WriteLine("Root clicked");
		}

		public void LoginFragment_ReminderPassowrd(int selectedCountryIndex)
		{
			var reminderPasswordFragment = ReminderPasswordFragment.NewInstance(selectedCountryIndex);
			SupportFragmentManager.BeginTransaction()
				.Replace(Resource.Id.frgForm, reminderPasswordFragment)
				.AddToBackStack(null)
				.Commit();
		}

		public void LoginSucceed()
		{
			WebViewManager.Instance.SetFontSize(
				WebContentFontSizeHelper.ParseSize(
					(int)SettingsUtil.Instance.GetFontSize()));
			var intent = new Intent(this, typeof(MyPublicationsPage.MyPublicationsActivity));
			StartActivity(intent);

			Finish();
		}

		public void ReminderPasswordFragment_ReturnLogin()
		{
			SupportFragmentManager.PopBackStack();
		}

		public string AsyncTaskActivityGUID
		{
			get
			{
				return asyncTaskActivityUUID;
			}
		}

    }
}

