using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Interop;

namespace LexisNexis.Red.Droid
{
    public class MyJSInterface : Java.Lang.Object, Java.Lang.IRunnable
    {
        Context context;

        public MyJSInterface(Context context)
        {
            this.context = context;
        }

        public void Run()
        {
            Toast.MakeText(context, "Hello from C#", ToastLength.Short).Show();
        }
        [Export("startFunction")]
        public void startFunction()
        {
            Toast.MakeText(context, "startFunction", ToastLength.Short).Show();
        }
        /// <summary>
        /// 当用户调用了这个方法会传递过来一个参数，我们可以获取出来然后用Android的toast显示
        /// </summary>
        /// <param name="str"></param>
        [Export("startFunction")]
        public void startFunction(string str)
        {
            Toast.MakeText(context, "startFunction()", ToastLength.Short).Show();
        }

        [Export]
        [JavascriptInterface]
        public void ShowToast()
        {
            Toast.MakeText(context, "Hello from C#", ToastLength.Short).Show();
        }
    }

}