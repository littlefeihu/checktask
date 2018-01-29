using System;
using Android.Widget;
using Android.Runtime;

namespace LexisNexis.Red.Droid.Widget.SpinnerUtility
{
	public static class SpinnerHelper
	{
		private static readonly IntPtr popupFieldId;
		private static readonly IntPtr isShowingMethodId;

		static SpinnerHelper()
		{
			try
			{
				popupFieldId = JNIEnv.GetFieldID(
					JNIEnv.FindClass(typeof(Spinner)), "mPopup", "Landroid/widget/Spinner$SpinnerPopup;");
				var spinnerPopupInterface = JNIEnv.FindClass ("android/widget/Spinner$SpinnerPopup");
				isShowingMethodId = JNIEnv.GetMethodID(spinnerPopupInterface, "isShowing", "()Z");

			}
			catch
			{
				popupFieldId = IntPtr.Zero;
				isShowingMethodId = IntPtr.Zero;
			}
		}

		public static bool IsShowingDropDown(Spinner spinner)
		{
			if(popupFieldId == IntPtr.Zero || isShowingMethodId == IntPtr.Zero)
			{
				return false;
			}

			using(var popup = new Java.Lang.Object(
				JNIEnv.GetObjectField(spinner.Handle, popupFieldId),
				JniHandleOwnership.TransferLocalRef))
			{
				return JNIEnv.CallBooleanMethod(popup.Handle, isShowingMethodId);
			}
		}
	}
}

