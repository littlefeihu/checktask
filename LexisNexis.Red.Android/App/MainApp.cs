using System;
using Android.Runtime;
using Android.App;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common;
using LexisNexis.Red.Droid.Implementation;
using Android.Preferences;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.Common.Interface;
using System.Collections.Generic;
using LexisNexis.Red.Common.BusinessModel;
using System.Security.Cryptography;
using System.Text;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Droid.LoginPage;
using LexisNexis.Red.Droid.TextStyle;
using Android.Text;
using LexisNexis.Red.Implementation;
using LexisNexis.Red.Droid.Utility;
using Xamarin;
using System.IO;

namespace LexisNexis.Red.Droid.App
{
	[Application(Theme="@style/MainAppTheme")]
	public class MainApp: Application
	{
		private const string PREF_DEVICEID = "deviceid";
		private static MainApp thisApp;
		private static string deviceId;

		public static MainApp ThisApp
		{
			get
			{
				return thisApp;
			}
		}

		public static string DeviceId
		{
			get
			{
				return deviceId;
			}
		}

		public MainApp(IntPtr handle, JniHandleOwnership transfer)
			: base(handle,transfer)
		{
		}

		public override void OnCreate ()
		{
			base.OnCreate ();

			System.Net.ServicePointManager.DefaultConnectionLimit = 10;

			thisApp = this;

#if !DEBUG
#if PREVIEW
Insights.Initialize("5c0f70a4d1326b40377a47d4cbea702a24c978f9", this);
#elif TESTING
Insights.Initialize("90497c422289e75c72756ed2e324aac80fcb559e", this);
#endif
#endif


			deviceId = Android.Provider.Settings.Secure.GetString(this.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
			if(string.IsNullOrEmpty(deviceId))
			{
				deviceId = PreferenceManager.GetDefaultSharedPreferences(this).GetString(PREF_DEVICEID, "");
				if(deviceId.Length == 0)
				{
					deviceId = Guid.NewGuid().ToString();
					var pref = PreferenceManager.GetDefaultSharedPreferences(this);
					var prefEditor = pref.Edit();
					prefEditor.PutString(PREF_DEVICEID, deviceId);
					prefEditor.Commit();
				}
			}
			else
			{
				deviceId = Sha1Hash(deviceId);
			}

#if DEBUG
			var sdCardRoot = Android.OS.Environment.ExternalStorageDirectory.Path;
			var redRoot = Path.Combine (sdCardRoot, @"RedTemp");
			DirectoryInfo di = new DirectoryInfo(redRoot);
			if(!di.Exists)
			{
				di.Create();
			}

			FileDirectory.Init(redRoot, redRoot);
#else
			FileDirectory.Init(FilesDir.AbsolutePath, GetExternalFilesDir(null).AbsolutePath);
#endif
			IoCContainer.Instance.RegisterInterface<IDirectory, FileDirectory>();
			IoCContainer.Instance.RegisterInterface<IDevice, AndroidDevice>();
			IoCContainer.Instance.RegisterInterface<INetwork, AndroidNetwork>();
			IoCContainer.Instance.RegisterInterface<IPackageFile, PackageFile>();
			IoCContainer.Instance.RegisterInterface<ICryptogram, AndroidCryptogram>();
			IoCContainer.Instance.RegisterInterface<ILogger, AndroidLogger>();

			AsyncHelpers.RunSync(GlobalAccess.Instance.Init);

			var serviceCountryMapList = ConfigurationService.GetAllCountryMap();
			DataCache.INSTATNCE.ServiceCountryList = new string[serviceCountryMapList.Count];
			for(int i = 0; i < serviceCountryMapList.Count; ++i)
			{
				DataCache.INSTATNCE.ServiceCountryList[i] = this.Resources.GetString(
					this.Resources.GetIdentifier(
						"ServiceCountry_Name_" + serviceCountryMapList[i].CountryCode,
						"string",
						this.PackageName));
			}
		}

		static private string Sha1Hash(string str_sha1_in)
		{
			SHA1 sha1 = new SHA1CryptoServiceProvider();
			byte[] bytes_sha1_in = UTF8Encoding.Default.GetBytes(str_sha1_in);
			byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
			string str_sha1_out = BitConverter.ToString(bytes_sha1_out);
			str_sha1_out = str_sha1_out.Replace("-", "");
			return str_sha1_out;
		}
	}
}

