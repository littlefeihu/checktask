
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
using Android.Graphics;
using LexisNexis.Red.Droid.Widget.ViewUtility;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.Widget.Layout;
using LexisNexis.Red.Droid.WebViewUtility;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Common.Business;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class IndexContentFragment : Fragment, IExpandableRightPanel
	{
		public const string FragmentTag = "IndexContentFragment";

		private BoundedFrameLayout bflContentContainer;
		private FrameLayout flContentContainer;
		private WebView wvContent;
		private TextView tvLeftTopIndex;
		private TextView tvNoIndexInFrame;
		private TextView tvNoIndexOutFrame;
		private ImageView ivExpand;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.contentpage_indexcontent_fragment, container, false);

			if(((ContentActivity)Activity).Publication == null)
			{
				return v;
			}

			tvLeftTopIndex = v.FindViewById<TextView>(Resource.Id.tvLeftTopIndex);
			tvNoIndexInFrame = v.FindViewById<TextView>(Resource.Id.tvNoIndexInFrame);
			tvNoIndexOutFrame = v.FindViewById<TextView>(Resource.Id.tvNoIndexOutFrame);
			bflContentContainer = v.FindViewById<BoundedFrameLayout>(Resource.Id.bflContentContainer);
			flContentContainer = v.FindViewById<FrameLayout>(Resource.Id.flContentContainer);
			ivExpand = v.FindViewById<ImageView>(Resource.Id.ivExpand);
			ivExpand.Click += OnIvExpandClick;

			((ContentActivity)Activity).GetMainFragment().SetLeftPanelStatus();

			return v;
		}

		public override void OnHiddenChanged(bool hidden)
		{
			base.OnHiddenChanged(hidden);

			if(hidden)
			{
				// Hidden
				if(wvContent != null)
				{
					WebViewManager.Instance.ReleaseWebView(
						WebViewManager.WebViewType.Index, flContentContainer);
					wvContent = null;
				}
			}
			else
			{
				// Show
				if(wvContent == null)
				{
					wvContent = WebViewManager.Instance.RequestWebView(
						WebViewManager.WebViewType.Index,
						flContentContainer,
						OnPageLoaded,
						OnGetSelectedText,
						OnLoadUrl);
					OpenIndexPage();
				}

				SetExpandableStatus();
			}
		}

		public override void OnStop()
		{
			if(wvContent != null)
			{
				WebViewManager.Instance.ReleaseWebView(
					WebViewManager.WebViewType.Index, flContentContainer);
				wvContent = null;
			}

			base.OnStop();
		}

		public override void OnResume()
		{
			base.OnResume();

			if(((ContentActivity)Activity).Publication == null)
			{
				return;
			}

			if(wvContent == null)
			{
				wvContent = WebViewManager.Instance.RequestWebView(
					WebViewManager.WebViewType.Index,
					flContentContainer,
					OnPageLoaded,
					OnGetSelectedText,
					OnLoadUrl);
				OpenIndexPage();
			}
		}

		private void OnPageLoaded()
		{
			((ContentActivity)Activity).ClosePleaseWaitPageLoadDialog();
		}

		private void OnGetSelectedText(string purpose, string text, float left, float top, float right, float bottom)
		{
			if("copy" == purpose.ToLower())
			{
				var clipboardManager = (ClipboardManager)Activity.GetSystemService(Context.ClipboardService);  
				clipboardManager.PrimaryClip = ClipData.NewPlainText(null, text);
				Toast.MakeText(Activity, "Copied", ToastLength.Short).Show();
			}
		}

		private void OnLoadUrl(Hyperlink url)
		{
			((ContentActivity)Activity).GetMainFragment().LoadUrl(url);
		}

		private void OnIvExpandClick (object sender, EventArgs e)
		{
			((ContentActivity)Activity).GetMainFragment().SwitchLeftPanelStatus();
		}

		public void SetExpandableStatus()
		{
			if(flContentContainer == null)
			{
				// OnCreateView did not called.
				return;
			}

			var status = ((ContentActivity)Activity).GetMainFragment().MainFragmentStatus;
			if(status.LeftPanelOpen)
			{
				bflContentContainer.SetBackgroundColor(Color.White);
				ivExpand.SetImageResource(Resource.Drawable.expand_content_view);
			}
			else
			{
				bflContentContainer.SetBackgroundResource(Resource.Drawable.miscinfo_frame_background);
				ivExpand.SetImageResource(Resource.Drawable.collapse_content_view);
			}

			SetNoIndexStatus();
		}

		private void SetNoIndexStatus()
		{
			var noIndex =
				DataCache.INSTATNCE.IndexList == null
				|| DataCache.INSTATNCE.IndexList.GetCurrentIndex() == null;
			tvNoIndexInFrame.Visibility = ViewStates.Gone;
			tvNoIndexOutFrame.Visibility = ViewStates.Gone;
			if(noIndex)
			{
				var status = ((ContentActivity)Activity).GetMainFragment().MainFragmentStatus;
				if(status.LeftPanelOpen)
				{
					tvNoIndexOutFrame.Visibility = ViewStates.Visible;
				}
				else
				{
					tvNoIndexInFrame.Visibility = ViewStates.Visible;
				}
			}
		}

		public void OpenIndexPage()
		{
			if(IsHidden)
			{
				// This fragment is hidden, need not open page
				return;
			}

			if(wvContent == null	// WebView does not requested
				|| DataCache.INSTATNCE.IndexList == null	// Index has not been retrieved
				)
			{
				return;
			}

			var selectedIndex = DataCache.INSTATNCE.IndexList.GetCurrentIndex();
			if(selectedIndex == null)
			{
				tvLeftTopIndex.Text = " ";
			}
			else
			{
				tvLeftTopIndex.Text = selectedIndex.Title.Substring(0, 1);
			}

			SetNoIndexStatus();

			if(selectedIndex == null)
			{
				wvContent.Tag = null;
				wvContent.LoadData("", "text/html", "utf-8");
				((ContentActivity)Activity).ClosePleaseWaitPageLoadDialog();
				return;
			}

			// Open Page
			var pub = ((ContentActivity)Activity).Publication;
			var tag = wvContent.Tag as JavaObjWrapper<WebViewTag>;
			if(tag != null && tag.Value.IsSameIndexPage(pub.Value.BookId, selectedIndex.Title))
			{
				((ContentActivity)Activity).ClosePleaseWaitPageLoadDialog();
				wvContent.LoadUrl("javascript:android.red.scrollByIndex();");
				return;
			}

			var result = AsyncHelpers.RunSync<string>(
				() => PublicationContentUtil.Instance.GetContentFromIndex(pub.Value.BookId, selectedIndex, false));

			//DumpToc(pub.Value.BookId, selectedIndex.Title, result);

			// Only large content need show wait dialog
			if(result.Length > WebViewManager.ShowWaitContentMinLength
				|| WebViewManager.CountATag(result) > WebViewManager.ShowWaitMinATagCount)
			{
				((ContentActivity)Activity).ShowPleaseWaitDialog();

				Task.Run(() =>
				{
					Thread.Sleep(100);

					result = PublicationContentUtil.Instance.RenderHyperLink(result, pub.Value.BookId);

					Application.SynchronizationContext.Post(_ =>{
						wvContent.LoadDataWithBaseURL(
							"file:///android_asset/html/",
							WebViewManager.Instance.ApplyTemplate(WebViewManager.WebViewType.Index, result),
							"text/html",
							"utf-8",
							null);
						wvContent.Tag = new JavaObjWrapper<WebViewTag>(WebViewTag.CreateWebViewTagByIndex(pub.Value.BookId, selectedIndex.Title));
					}, null);
				});

				return;
			}

			result = PublicationContentUtil.Instance.RenderHyperLink(result, pub.Value.BookId);

			wvContent.LoadDataWithBaseURL(
				"file:///android_asset/html/",
				WebViewManager.Instance.ApplyTemplate(WebViewManager.WebViewType.Index, result),
				"text/html",
				"utf-8",
				null);
			wvContent.Tag = new JavaObjWrapper<WebViewTag>(WebViewTag.CreateWebViewTagByIndex(pub.Value.BookId, selectedIndex.Title));
		}

		private void DumpToc(int bookId, string index, string content)
		{
			var sdCardRoot = Android.OS.Environment.ExternalStorageDirectory.Path;
			var tempFolder = System.IO.Path.Combine (sdCardRoot, @"Temp");
			var tempFolderInfo = new DirectoryInfo(tempFolder);
			if(!tempFolderInfo.Exists)
			{
				tempFolderInfo.Create();
			}

			var dumpFile = System.IO.Path.Combine (tempFolder, "pub_" + bookId + "_i_" + index + ".html");
			var fi = new FileInfo(dumpFile);
			fi.Delete();
			using(var fs = new FileStream(fi.FullName, FileMode.CreateNew, FileAccess.Write))
			using(var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				sw.Write(content);
			}
		}
	}
}

