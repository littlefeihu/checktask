﻿
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
using Newtonsoft.Json;
using Android.Provider;
using LexisNexis.Red.Common;
using System.Xml.Linq;
using System.Threading.Tasks;

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
        Guid taskId = Guid.Empty;
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
            taskId = Guid.Parse(this.Intent.GetStringExtra("taskid"));

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
        private string bytesToHexString(byte[] src)
        {
            return BitConverter.ToString(src, 0).Replace("-", string.Empty);
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

                                //var data = Encoding.ASCII.GetString(record.GetPayload());

                                //if (!string.IsNullOrEmpty(data))
                                //    data = data.Substring(3);

                                var data = bytesToHexString(tag.GetId());
                                try
                                {
                                    HttpResponse taskresponse = new HttpResponse();

                                    taskresponse = IoCContainer.Instance.Resolve<IDeliveryService>().GetCheckContentByTaskID(new GetCheckContentRequest { NFC = data, taskid = taskId.ToString() });

                                    wvContent.LoadUrl("javascript:RenderCheckContent('" + data + "','','" + taskresponse.Content + "')");
                                }
                                catch (System.Exception ex)
                                {
                                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result ResultStatus, Intent data)
        {
            if (ResultStatus == Result.Ok)
            {
                if (requestCode == Camera_RequestCode)
                {
                    var fileUrl = Android.Net.Uri.FromFile(originalFile);
                    wvContent.LoadUrl("javascript:SetImg('" + fileUrl + "')");
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
            string msg = "";
            try
            {
                var requests = JsonConvert.DeserializeObject<List<CreateCheckRercordRequest>>(str);
                HttpResponse taskresponse = IoCContainer.Instance.Resolve<IDeliveryService>().CreateCheckRercord(requests);
                if (taskresponse.IsSuccess)
                {
                    msg = "操作成功";
                    wvContent.LoadUrl("javascript:SubmitCheckRecord('0')");
                }
            }
            catch (System.Exception)
            {
                msg = "操作出错";
            }
            Toast.MakeText(this, msg, ToastLength.Short).Show();
        }
        private Java.IO.File originalFile;
        private const int Camera_RequestCode = 0xa2;

        [Export]
        [JavascriptInterface]
        public void submitCheck()
        {

        }


        [Export]
        [JavascriptInterface]
        public void cutImageByCamera()
        {
            try
            {
                originalFile = new Java.IO.File(IoCContainer.Instance.Resolve<IDirectory>().GetAppRootPath(), "zcb_pic_" + Guid.NewGuid().ToString("N") + ".png");

                Intent getImageByCamera = new Intent("android.media.action.IMAGE_CAPTURE");
                getImageByCamera.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(originalFile));
                getImageByCamera.PutExtra(MediaStore.ExtraVideoQuality, 0);
                StartActivityForResult(getImageByCamera, Camera_RequestCode);
            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, "App Camera Error:" + ex.InnerException, ToastLength.Short).Show();
            }
        }

        [Export]
        [JavascriptInterface]
        public void saveRepair(string deviceid, string faultDesc, string imgUrls)
        {
            try
            {
                UploadRepairRequest request = new UploadRepairRequest();
                XElement root = new XElement("Repair");
                XElement Imgs = new XElement("Imgs");
                var imgpaths = JsonConvert.DeserializeObject<List<string>>(imgUrls);
                request.imgs = new List<string>();
                foreach (var imgpath in imgpaths)
                {
                    using (var stream = IoCContainer.Instance.Resolve<IDirectory>().OpenFile1(imgpath, Common.BusinessModel.FileModeEnum.Open).Result)
                    {
                        byte[] arr = new byte[stream.Length];
                        stream.Read(arr, 0, arr.Length);
                        var imgcontent = Convert.ToBase64String(arr);
                        Imgs.Add(new XElement("Img", imgcontent));
                        request.imgs.Add(imgcontent);
                    }
                }
                root.Add(new XElement("deviceid", deviceid));
                root.Add(new XElement("faultDesc", faultDesc));
                root.Add(new XElement("username", GlobalAccess.Instance.CurrentUserInfo.Email));
                root.Add(new XElement("userid", GlobalAccess.Instance.CurrentUserInfo.FullName));
                root.Add(Imgs);

                request.userid = GlobalAccess.Instance.CurrentUserInfo.FullName;
                request.username = GlobalAccess.Instance.CurrentUserInfo.Email;
                request.faultDesc = faultDesc;
                request.deviceid = deviceid;
                request.xmlName = Guid.NewGuid().ToString("N") + ".xml";
                request.Content = System.Text.Encoding.UTF8.GetBytes(root.ToString());

                SaveToLocal(request.xmlName, request.Content);
                var response = IoCContainer.Instance.Resolve<IDeliveryService>().UploadRepair(request);
                if (response.Content.Contains("操作成功"))
                {
                    wvContent.LoadUrl("javascript:SubmitRepairResult('0')");
                    Toast.MakeText(this, "保存成功", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "保存出错,请重试", ToastLength.Short).Show();
                }
            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, "保存出错,请重试" + ex.Message, ToastLength.Short).Show();
            }
        }


        private void SaveToLocal(string xmlname, byte[] content)
        {

            Task.Run(async () =>
            {
                await IoCContainer.Instance.Resolve<IDirectory>().SaveFileToInternal(xmlname, content);

            }).GetAwaiter().GetResult();

        }

        [Export]
        [JavascriptInterface]
        public void submitCheck(string str)
        {
            Toast.MakeText(this, str + ",taskId:" + taskId, ToastLength.Short).Show();
        }
        [Export]
        [JavascriptInterface]
        public void ShowToast()
        {
            Toast.MakeText(this, "Hello from C#", ToastLength.Short).Show();
        }



    }
}