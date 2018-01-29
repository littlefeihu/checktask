﻿
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
using Fragment = Android.Support.V4.App.Fragment;
using Android.Webkit;
using LexisNexis.Red.Droid.App;
using Java.Interop;
using static LexisNexis.Red.Droid.SettingsBoardPage.SettingsBoardActivity;

namespace LexisNexis.Red.Droid.SettingsBoardPage
{
    public class FAQsFragment : Fragment, ISettingsBoardFragment
    {
        private WebView wvContent;

        public string Title
        {
            get
            {
                return SettingsBoardActivity.GetTitle(SettingsBoardActivity.FAQs);
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

            var html = MainApp.ThisApp.GetString(Resource.String.settings_faqs);
            wvContent.Settings.JavaScriptEnabled = true;//设置webserver支持js
            wvContent.AddJavascriptInterface(this, "Test");
            wvContent.LoadDataWithBaseURL("file:///android_asset/html/", html, "text/html", "utf-8", null);
            return v;
        }

        public void startFunction()
        {
            Toast.MakeText(this.Activity.BaseContext, "Hello from C#", ToastLength.Short).Show();
        }


        public void startFunction(string str)
        {
            Toast.MakeText(this.Activity.BaseContext, "Hello from C# 1", ToastLength.Short).Show();
        }
    }

}