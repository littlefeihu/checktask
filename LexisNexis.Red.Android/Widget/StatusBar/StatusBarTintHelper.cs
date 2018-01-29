using System;
using Com.Readystatesoftware.Systembartint;
using Android.OS;
using Android.App;
using LexisNexis.Red.Droid.App;

namespace LexisNexis.Red.Droid.Widget.StatusBar
{
	public static class StatusBarTintHelper
	{
		public static void SetStatusBarColor(Activity activity)
		{
			if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
			{
				SystemBarTintManager tintManager = new SystemBarTintManager(activity);
				tintManager.StatusBarTintEnabled = true;
				tintManager.SetStatusBarTintResource(Resource.Color.StatusBarBackground);
			}
		}

		public static int GetStatusBarHeight(bool force = false)
		{
			if(!force && Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
			{
				// If the OS is before Kitkat, we will not tint the status bar.
				// And then the height of status bar will not count in to the whole layout of each activity
				// But if caller actually neet the height of status bar, call the method and set the force
				// parameter to true.
				return 0;
			}

			int result = 0;
			int resourceId = MainApp.ThisApp.Resources.GetIdentifier("status_bar_height", "dimen", "android");
			if (resourceId > 0)
			{
				result = MainApp.ThisApp.Resources.GetDimensionPixelSize(resourceId);
			}

			return result;
		}
	}
}

