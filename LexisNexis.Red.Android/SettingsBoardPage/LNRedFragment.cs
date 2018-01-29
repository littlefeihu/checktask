
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
using Fragment=Android.Support.V4.App.Fragment;
using Android.Webkit;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.Droid.SettingsBoardPage
{
	public class LNRedFragment: Fragment, ISettingsBoardFragment
	{
		private WebView wvContent;

		public string Title
		{
			get
			{
				return SettingsBoardActivity.GetTitle(SettingsBoardActivity.LNRed);
			}
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.settings_board_webview_fragment, container, false);
			wvContent = v.FindViewById<WebView>(Resource.Id.wvContent);

			var content = SettingsUtil.Instance.GetLexisNexisRedInfo();
			var lastSyncTime = string.Format(
				MainApp.ThisApp.Resources.GetString(
					Resource.String.MainMenuPopup_LastSync),
				LastSyncedTimeHelper.Get().ToString("hh:mmtt, d MMM yyyy"));
			content = content.Replace(
				"<!--#SyncTime#-->",
				"<p class='small'>" + lastSyncTime + "</p>");

			wvContent.LoadData(content, "text/html", "utf-8");
			return v;
		}
	}
}

