
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Widget.StatusBar;
using Java.Lang;
using Java.Interop;
using Android.Webkit;
using LexisNexis.Red.Droid.WebViewUtility;
using Android.Util;
using System.IO;
using Android.Nfc;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Droid.SettingsBoardPage
{
    [Activity(Label = "巡查任务")]
    public class Activity1 : AppCompatActivity
    {
        public const string FunctionKey = "FunctionKey";

        public const string OrganisePublications = "OrganisePublications";
        public const string TermsAndConditions = "TermsAndConditions";
        public const string LNLegalAndProfessional = "LNLegalAndProfessional";
        public const string LNRed = "LNRed";
        public const string FAQs = "FAQs";
        public const string Contact = "Contact";

        private Toolbar toolbar;
        private LinearLayout llRootView;
        private WebView wvContent;
        private NfcAdapter _nfcAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            StatusBarTintHelper.SetStatusBarColor(this);

            //// Create your application here
            SetContentView(Resource.Layout.settings_board_activity);

            llRootView = FindViewById<LinearLayout>(Resource.Id.llRootView);

            FindViewById<LinearLayout>(Resource.Id.llStatusBarStub).LayoutParameters =
                new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, StatusBarTintHelper.GetStatusBarHeight());

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_actionbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            wvContent = FindViewById<WebView>(Resource.Id.wvContent);
            wvContent.Settings.JavaScriptEnabled = true;
            wvContent.AddJavascriptInterface(this, "wst");

            var HtmlTemplate = "";
            using (var sr = new StreamReader(MainApp.ThisApp.Assets.Open("wst.html")))
            {
                HtmlTemplate = sr.ReadToEnd();
            }

            var taskName = this.Intent.GetStringExtra(SettingsBoardActivity.FunctionKey);


            HtmlTemplate = HtmlTemplate.Replace("##任务名称##", taskName);


            wvContent.LoadDataWithBaseURL(
                        "file:///android_asset/html/",
                        HtmlTemplate,
                        "text/html",
                        "utf-8",
                        null);
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
        }

        public override View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            return base.OnCreateView(name, context, attrs);
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (_nfcAdapter == null)
            {
                Toast.MakeText(this, "NFC is not supported on this device.", ToastLength.Short).Show();
            }
            else
            {
                var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);

                var filters = new[] { tagDetected };

                var intent = new Intent(this, this.GetType()).AddFlags(ActivityFlags.SingleTop);

                var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

                _nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent.Action == NfcAdapter.ActionTagDiscovered)
            {
                var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
                if (tag != null)
                {
                    // First get all the NdefMessage
                    var rawMessages = intent.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);

                    if (rawMessages != null)
                    {
                        var msg = (NdefMessage)rawMessages[0];

                        var record = msg.GetRecords()[0];
                        if (record != null)
                        {
                            if (record.Tnf == NdefRecord.TnfWellKnown)
                            {
                                var data = Encoding.ASCII.GetString(record.GetPayload());
                                Toast.MakeText(this, data, ToastLength.Short).Show();

                                if (!string.IsNullOrEmpty(data))
                                    data = data.Substring(3);

                                HttpResponse taskresponse = new HttpResponse();

                                taskresponse = IoCContainer.Instance.Resolve<IDeliveryService>().GetCheckContentByTaskID(new GetCheckContentRequest { NFC = "1111", taskid = Guid.NewGuid() });

                                wvContent.LoadUrl("javascript:RenderCheckContent('" + data + "','','" + taskresponse.Content + "')");
                            }
                        }
                    }
                }
            }
        }


        public static string GetTitle(string id)
        {
            return MainApp.ThisApp.Resources.GetString(
                MainApp.ThisApp.Resources.GetIdentifier(
                        "SettingsBoard_Activity_Title_" + id,
                        "string",
                        MainApp.ThisApp.PackageName));
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    {
                        Finish();
                        return true;
                    }
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        [Export("startFunction")]
        [JavascriptInterface]
        public void startFunction()
        {


        }
        /// <summary>
        /// 当用户调用了这个方法会传递过来一个参数，我们可以获取出来然后用Android的toast显示
        /// </summary>
        /// <param name="str"></param>
        [Export("startFunction")]
        [JavascriptInterface]
        public void startFunction(string str)
        {
            Toast.MakeText(this, "startFunction()", ToastLength.Short).Show();
        }

        [Export]
        [JavascriptInterface]
        public void ShowToast()
        {
            Toast.MakeText(this, "Hello from C#", ToastLength.Short).Show();
        }
    }
}