
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
using LexisNexis.Red.Droid.WebViewUtility;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class PublicationTextDialogFragment : DialogFragment
	{
		private Button btnCancel;
		private Button btnOk;

		private RadioGroup rgFontSize;
		private RadioButton rbFontSizeSmall;
		private RadioButton rbFontSizeNormal;
		private RadioButton rbFontSizeLarge;

		private WebContentFontSize originalFontSize;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
			Dialog.SetCanceledOnTouchOutside(false);

			var vwDialog = inflater.Inflate(Resource.Layout.dialog_template, container);
			vwDialog.FindViewById<View>(Resource.Id.llTitleContainer).Visibility = ViewStates.Gone;
			btnCancel = vwDialog.FindViewById<Button>(Resource.Id.btnNegtive);
			btnOk = vwDialog.FindViewById<Button>(Resource.Id.btnPositive);

			var vwPublicationText = inflater.Inflate(Resource.Layout.contentpage_publicationtext_popup, container);
			var layoutParaments = new ViewGroup.LayoutParams(
				(int)Math.Round(MainApp.ThisApp.Resources.GetDimension(Resource.Dimension.content_page_fontsize_dialogwidth)),
				ViewGroup.LayoutParams.WrapContent);

			vwDialog.FindViewById<LinearLayout>(Resource.Id.llDialogContentContainer).AddView(vwPublicationText, layoutParaments);

			rgFontSize = vwPublicationText.FindViewById<RadioGroup>(Resource.Id.rgFontSize);
			rbFontSizeSmall = vwPublicationText.FindViewById<RadioButton>(Resource.Id.rbFontSizeSmall);
			rbFontSizeNormal = vwPublicationText.FindViewById<RadioButton>(Resource.Id.rbFontSizeNormal);
			rbFontSizeLarge = vwPublicationText.FindViewById<RadioButton>(Resource.Id.rbFontSizeLarge);

			originalFontSize = WebContentFontSizeHelper.ParseSize((int)SettingsUtil.Instance.GetFontSize());

			switch(originalFontSize)
			{
			case WebContentFontSize.Small:
				rbFontSizeSmall.Checked = true;
				break;
			case WebContentFontSize.Normal:
				rbFontSizeNormal.Checked = true;
				break;
			case WebContentFontSize.Large:
				rbFontSizeLarge.Checked = true;
				break;
			}


			btnCancel.Click += delegate
			{
				Dismiss();
			};

			btnOk.Click += delegate
			{
				var selectedFontSize = WebContentFontSize.Normal;
				switch(rgFontSize.CheckedRadioButtonId)
				{
				case Resource.Id.rbFontSizeSmall:
					selectedFontSize = WebContentFontSize.Small;
					break;
				case Resource.Id.rbFontSizeLarge:
					selectedFontSize = WebContentFontSize.Large;
					break;
				}

				if(selectedFontSize != originalFontSize)
				{
					SettingsUtil.Instance.SaveFontSize((int)selectedFontSize);
					WebViewManager.Instance.SetFontSize(selectedFontSize);
				}

				Dismiss();
			};

			return vwDialog;
		}

	}
}

