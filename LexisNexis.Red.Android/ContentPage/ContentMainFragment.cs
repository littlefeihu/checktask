using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment=Android.Support.V4.App.Fragment;
using SearchView = Android.Support.V7.Widget.SearchView;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.SettingsBoardPage;
using LexisNexis.Red.Droid.AlertDialogUtility;
using Newtonsoft.Json;
using LexisNexis.Red.Common.Business;
using Android.Graphics;
using LexisNexis.Red.Droid.Widget.Fragment;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.DetailInfoModal;
using Android.Animation;
using System.Threading.Tasks;
using System.Threading;
using Uri = Android.Net.Uri;
using Android.Provider;
using PopupMenu=Android.Support.V7.Widget.PopupMenu;
using LexisNexis.Red.Droid.Implementation;
using LexisNexis.Red.Droid.AnnotationUtility;
using LexisNexis.Red.Droid.PrintUtility;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class ContentMainFragment : Fragment, ISimpleDialogListener, IRefreshableFragment
	{
		private const string ContentMainFragmentStateKey = "ContentMainFragmentStateKey";
		private const string LogoutDialog = "LogoutDialog";
		private const string MainFragmentStatusCacheFile = "MainFragmentStatus.{0}.cache";

		public const int TabContents = 0;
		public const int TabIndex = 1;
		public const int TabAnnotations = 2;

		private View actionMenuInfo;
		private IMenuItem actionMenuLastSync;


		private FrameLayout frLeftPanelContainer;
		private LinearLayout llLeftPanelContainer;
		private LinearLayout llLeftPanelContainerStub;

		private LinearLayout llMainBoard;

		private RadioGroup rgMainTab;
		private RadioButton rbContents;
		private RadioButton rbIndex;
		private RadioButton rbAnnotations;

		public ContentMainFragmentState MainFragmentStatus
		{
			get;
			set;
		}

		public ContentMainFragment()
		{
			leftPanelAnimateListener = new LeftPanelAnimateListener(this);
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			HasOptionsMenu = true;
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if(MainFragmentStatus == null)
			{
				var mainFragmentStatusCache = FileCacheHelper.ReadCacheFile(
					ContentActivity.CacheCatagory,
					string.Format(
						MainFragmentStatusCacheFile,
						((ContentActivity)Activity).AsyncTaskActivityGUID));
				if(!string.IsNullOrEmpty(mainFragmentStatusCache))
				{
					MainFragmentStatus = JsonConvert.DeserializeObject<ContentMainFragmentState>(mainFragmentStatusCache);
				}
			}

			if(MainFragmentStatus == null)
			{
				MainFragmentStatus = new ContentMainFragmentState();
			}

			var v = inflater.Inflate(Resource.Layout.content_main_fragment, container, false);

			if(((ContentActivity)Activity).Publication == null)
			{
				return v;
			}

            //llLeftPanelContainer = v.FindViewById<LinearLayout>(Resource.Id.llLeftPanelContainer);
            //frLeftPanelContainer = v.FindViewById<FrameLayout>(Resource.Id.frLeftPanelContainer);
            llLeftPanelContainerStub = v.FindViewById<LinearLayout>(Resource.Id.llLeftPanelContainerStub);

			llMainBoard = v.FindViewById<LinearLayout>(Resource.Id.llMainBoard);

			//rgMainTab = v.FindViewById<RadioGroup>(Resource.Id.rgMainTab);
			//rbContents = v.FindViewById<RadioButton>(Resource.Id.rbContents);
			//rbIndex = v.FindViewById<RadioButton>(Resource.Id.rbIndex);
			rbAnnotations = v.FindViewById<RadioButton>(Resource.Id.rbAnnotations);

			//rgMainTab.CheckedChange += MainTabCheckedChange;

			LogHelper.Debug("dbg", "ContentMainFragment::OnCreateView");

            //var fragment = Activity.SupportFragmentManager.FindShowingFragmentById(Resource.Id.frLeftPanelContainer);
            //if (fragment == null)
            //{

            //    //var tocPanelFragment = new TOCPanelFragment();
            //    //transaction.Add(Resource.Id.frLeftPanelContainer, tocPanelFragment, TOCPanelFragment.FragmentTag);


            //}
            //var transaction = Activity.SupportFragmentManager.BeginTransaction();

            //transaction.Add(Resource.Id.frRightPanelContainer, contentFragment, ContentFragment.FragmentTag).Commit();
            //Activity.SupportFragmentManager.SaveFragmentInstanceState(new ContentFragment());

            var loginFragment = new ContentFragment();
     
            Activity.SupportFragmentManager.BeginTransaction().Add(Resource.Id.frRightPanelContainer, loginFragment).Commit();


            OpenContentPage();
            //Refresh();
			//SetLeftPanelStatus();

			return v;
		}

		private void MainTabCheckedChange (object sender, RadioGroup.CheckedChangeEventArgs e)
		{
			if(!GetMainTab(GetMainTabIndex(e.CheckedId)).Checked)
			{
				return;
			}

			Activity.InvalidateOptionsMenu();

			var tabIndex = GetMainTabIndex(e.CheckedId);
			SwitchLogicalMainTab(tabIndex);

			switch(tabIndex)
			{
			case TabContents:
				{
					var transaction = Activity.SupportFragmentManager.BeginTransaction();

					//Activity.SupportFragmentManager.SwitchFragmentById<TOCPanelFragment>(
					//	Resource.Id.frLeftPanelContainer,
					//	TOCPanelFragment.FragmentTag,
					//	f => f.Tag == TOCPanelFragment.FragmentTag,
					//	transaction);

					Activity.SupportFragmentManager.SwitchFragmentById<ContentFragment>(
						Resource.Id.frRightPanelContainer,
						ContentFragment.FragmentTag,
						f => f.Tag == ContentFragment.FragmentTag,
						transaction);

					transaction.Commit();
				}
				break;
			case TabIndex:
				//{
				//	var transaction = Activity.SupportFragmentManager.BeginTransaction();

				//	Activity.SupportFragmentManager.SwitchFragmentById<IndexPanelFragment>(
				//		Resource.Id.frLeftPanelContainer,
				//		IndexPanelFragment.FragmentTag,
				//		f => f.Tag == IndexPanelFragment.FragmentTag,
				//		transaction);

				//	Activity.SupportFragmentManager.SwitchFragmentById<IndexContentFragment>(
				//		Resource.Id.frRightPanelContainer,
				//		IndexContentFragment.FragmentTag,
				//		f => f.Tag == IndexContentFragment.FragmentTag,
				//		transaction);

				//	transaction.Commit();
				//}
				break;
			case TabAnnotations:
				//{
				//	var transaction = Activity.SupportFragmentManager.BeginTransaction();

				//	Activity.SupportFragmentManager.SwitchFragmentById<AnnotationsPanelFragment>(
				//		Resource.Id.frLeftPanelContainer,
				//		AnnotationsPanelFragment.FragmentTag,
				//		f => f.Tag == AnnotationsPanelFragment.FragmentTag,
				//		transaction);

				//	Activity.SupportFragmentManager.SwitchFragmentById<AnnotationListFragment>(
				//		Resource.Id.frRightPanelContainer,
				//		AnnotationListFragment.FragmentTag,
				//		f => f.Tag == AnnotationListFragment.FragmentTag,
				//		transaction);

				//	transaction.Commit();
				//}
				break;
			default:
				throw new InvalidProgramException("Invalid tab id.");
			}
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			FileCacheHelper.SaveCacheFile(
				ContentActivity.CacheCatagory,
				string.Format(MainFragmentStatusCacheFile, ((ContentActivity)Activity).AsyncTaskActivityGUID),
				JsonConvert.SerializeObject(MainFragmentStatus));
			base.OnSaveInstanceState(outState);
		}

		public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
		{
			if(((ContentActivity)Activity).Publication == null)
			{
				base.OnCreateOptionsMenu(menu, inflater);
				return;
			}

			//if(GetCurrentMainTab() == Resource.Id.rbAnnotations)
			//{
			//	inflater.Inflate(Resource.Menu.content_annotations_actionbar, menu);
			//	menu.FindItem(Resource.Id.action_edittags).ActionView.FindViewById<View>(Resource.Id.flBackground).Click += delegate
			//	{
			//		EditTagsDialogFragment.NewInstance().Show(
			//			Activity.SupportFragmentManager.BeginTransaction(),
			//			EditTagsDialogFragment.EditTagsDialogFragmentTag);
			//	};
			//}
			//else
			//{
			//	inflater.Inflate(Resource.Menu.content_actionbar, menu);
			//	actionMenuInfo = menu.FindItem(Resource.Id.action_info).ActionView;
			//	RefreshTitleBarInfoIcon();
			//	actionMenuInfo.FindViewById<View>(Resource.Id.rlBackground).Click += delegate
			//	{
			//		//PublicationDetailInfoFragment
			//		//	.NewInstance(((ContentActivity)Activity).Publication.Value.BookId, true)
			//		//	.Show(Activity.SupportFragmentManager);
			//	};

			//	menu.FindItem(Resource.Id.action_search).ActionView.FindViewById<View>(Resource.Id.flBackground).Click += delegate
			//	{
			//		((ContentActivity)Activity).OpenContentSearch();
			//	};

			//	menu.FindItem(Resource.Id.action_share).ActionView.FindViewById<View>(Resource.Id.flBackground).Click += delegate(object sender, EventArgs e)
			//	{
			//		var popup = new PopupMenu(Activity, (View)sender);
			//		popup.MenuInflater.Inflate(Resource.Menu.content_share_popup, popup.Menu);
			//		popup.SetOnMenuItemClickListener(new SharePopupMenuItemClickListener(this));

			//		#if PREVIEW
			//		if(!LoginPage.LoginActivity.AllowDebug)
			//		{
			//			popup.Menu.RemoveItem(Resource.Id.action_dumptoc);
			//		}
			//		#endif

			//		#if RELEASE
			//		popup.Menu.RemoveItem(Resource.Id.action_dumptoc);
			//		#endif


			//		popup.Show();

			//		#if false
			//		var contentFragment = Activity.SupportFragmentManager.FindShowingFragmentById(Resource.Id.frRightPanelContainer) as ContentFragment;
			//		if(contentFragment != null)
			//		{
			//		contentFragment.SaveAsPdf();
			//		}
			//		#endif
			//	};
			//}

			//menu.FindItem(Resource.Id.action_recenthistory).ActionView.FindViewById<View>(Resource.Id.flBackground).Click += delegate
			//{
			//	((ContentActivity)Activity).OpenRecentHistory();
			//};

			//actionMenuLastSync = menu.FindItem(Resource.Id.action_lastsync);
			//actionMenuLastSync.SetEnabled(false);

			base.OnCreateOptionsMenu(menu, inflater);
		}

		private class SharePopupMenuItemClickListener: Java.Lang.Object, PopupMenu.IOnMenuItemClickListener
		{
			private readonly ContentMainFragment hostFragment;

			public SharePopupMenuItemClickListener(ContentMainFragment hostFragment)
			{
				this.hostFragment = hostFragment;
			}

			public bool OnMenuItemClick(IMenuItem item)
			{
				switch(item.ItemId)
				{
				case Resource.Id.action_shareaspdf:
					{
						var contentFragment = hostFragment.Activity.SupportFragmentManager.FindShowingFragmentById(
							Resource.Id.frRightPanelContainer) as ContentFragment;
						if(contentFragment != null)
						{
							contentFragment.Print(PrintType.PDF);
						}
					}
					break;
				case Resource.Id.action_print:
					{
						/*
						var contentFragment = hostFragment.Activity.SupportFragmentManager.FindShowingFragmentById(
							Resource.Id.frRightPanelContainer) as ContentFragment;
						if(contentFragment != null)
						{
							contentFragment.DumpToc();
						}
						Toast.MakeText(hostFragment.Activity, "Print", ToastLength.Short).Show();
						*/
						var contentFragment = hostFragment.Activity.SupportFragmentManager.FindShowingFragmentById(
							Resource.Id.frRightPanelContainer) as ContentFragment;
						if(contentFragment != null)
						{
							contentFragment.Print(PrintType.PhysicalPrinter);
						}
					}
					break;
				case Resource.Id.action_dumptoc:
					{
						var contentFragment = hostFragment.Activity.SupportFragmentManager.FindShowingFragmentById(
							Resource.Id.frRightPanelContainer) as ContentFragment;
						if(contentFragment != null)
						{
							contentFragment.DumpToc();
						}
						Toast.MakeText(hostFragment.Activity, "Dump TOC completed.", ToastLength.Short).Show();
					}
					break;
				default:
					//throw new InvalidProgramException("Unknown share popup menu item");
					return false;
				}

				return true;
			}
		}


		public override void OnPrepareOptionsMenu(IMenu menu)
		{
			//if(((ContentActivity)Activity).Publication != null)
			//{
			//	actionMenuLastSync.SetTitle(
			//		string.Format(
			//			MainApp.ThisApp.Resources.GetString(
			//				Resource.String.MainMenuPopup_LastSync),
			//			LastSyncedTimeHelper.Get().ToString("hh:mmtt, d MMM yyyy")));
			//}
			
			base.OnPrepareOptionsMenu(menu);
		}

		/*
		public void OpenTOC()
		{
			if(MainFragmentStatus.MainTabIndex == TabContents)
			{
				var tocPanelFragment = Activity.SupportFragmentManager.FindShowingFragmentById(Resource.Id.frLeftPanelContainer) as TOCPanelFragment;
				if(tocPanelFragment != null)
				{
					tocPanelFragment.Refresh();
				}
			}
			else
			{
				rgMainTab.Check(Resource.Id.rbContents);
			}
		}
		*/

		public void OpenContentPage()
		{
			var contentFragment = Activity.SupportFragmentManager.FindFragmentById(Resource.Id.frRightPanelContainer) as ContentFragment;
			if(contentFragment != null)
			{
				contentFragment.OpenContentPage();
			}
		}

		public void OpenIndexPage()
		{
			var indexContentFragment = Activity.SupportFragmentManager.FindShowingFragmentById(Resource.Id.frRightPanelContainer) as IndexContentFragment;
			if(indexContentFragment != null)
			{
				indexContentFragment.OpenIndexPage();
			}
		}

		public void DoActionMode(int actionId)
		{
			var actionModeTarget = Activity.SupportFragmentManager.FindShowingFragmentById(Resource.Id.frRightPanelContainer) as IActionModeTarget;
			if(actionModeTarget != null)
			{
				actionModeTarget.DoActionMode(actionId);
			}
		}

		public void AfterDoActionMode(int actionId)
		{
			var actionModeTarget = Activity.SupportFragmentManager.FindShowingFragmentById(Resource.Id.frRightPanelContainer) as IActionModeTarget;
			if(actionModeTarget != null)
			{
				actionModeTarget.AfterDoActionMode(actionId);
			}
		}

		public void OnUserDismissLegalDefinePopup()
		{
			var actionModeTarget = Activity.SupportFragmentManager.FindShowingFragmentById(Resource.Id.frRightPanelContainer) as IActionModeTarget;
			if(actionModeTarget != null)
			{
				actionModeTarget.OnUserDismissLegalDefinePopup();
			}
		}

		public void SwitchLeftPanelStatus()
		{
			MainFragmentStatus.LeftPanelOpen = !MainFragmentStatus.LeftPanelOpen;

			if(MainFragmentStatus.LeftPanelOpen)
			{
				// Open
				llLeftPanelContainer.Visibility = ViewStates.Visible;
				llLeftPanelContainer.Animate().TranslationX(0.0f).SetListener(leftPanelAnimateListener);
			}
			else
			{
				// Close
				SetLeftPanelStatus(true, false);
				llLeftPanelContainer.Animate().TranslationX(-1 * llLeftPanelContainer.Width).SetListener(leftPanelAnimateListener);
			}
		}

		public void NavigateTo(int fromBookId)
		{
			var a = (ContentActivity)Activity;

			var currentRecord = NavigationManager.Instance.CurrentRecord;
			if(currentRecord == null)
			{
				throw new InvalidProgramException("Navigation manager has no record.");
			}

			if(currentRecord.RecordType == RecordType.ContentRecord
				|| currentRecord.RecordType == RecordType.SearchResultRecord)
			{
				MainFragmentStatus.ShowingTab = TabContents;
			}
			else if(currentRecord.RecordType == RecordType.AnnotationNavigator)
			{
				MainFragmentStatus.ShowingTab = TabAnnotations;
			}
			else
			{
				throw new NotImplementedException();
			}

			if(fromBookId != a.Publication.Value.BookId)
			{
				a.ShowPleaseWaitDialog();

				Task.Run(() =>
				{
					// Wait the "PleaseWaitDialog" pop up
					Thread.Sleep(100);
					Application.SynchronizationContext.Post(_ =>
					{
						a.SwitchPublication();
						//OpenTOC(contentBrowserRecord.TOCID, false);
						Refresh();
					}, null);
				});

				return;
			}

			Refresh();
		}

		public void LoadUrl(Hyperlink url)
		{
			var bookId = ((ContentActivity)Activity).Publication.Value.BookId;
			switch(url.LinkType)
			{
			case HyperLinkType.IntraHyperlink:
				{
					var intralink = url as IntraHyperlink;
					NavigationManager.Instance.AddRecord(
						new ContentBrowserRecord(
							bookId,
							intralink.TOCID,
							0,
							0,
							intralink.Refpt));
					NavigateTo(bookId);
				}
				break;
			case HyperLinkType.InternalHyperlink:
				{
					var internallink = url as InternalHyperlink;
					NavigationManager.Instance.AddRecord(
						new ContentBrowserRecord(
							internallink.BookID,
							internallink.TOCID,
							0,
							0,
							internallink.Refpt));
					NavigateTo(bookId);
				}
				break;
			case HyperLinkType.ExternalHyperlink:
				{
					var exterlink = url as ExternalHyperlink;
					Uri uri = Uri.Parse(exterlink.Url.Replace("file:///android_asset/html/", "http://"));
					var intent = new Intent(Intent.ActionView, uri);
					intent.PutExtra(Browser.ExtraApplicationId, Activity.PackageName);
					Activity.StartActivity(intent);
				}
				break;
			case HyperLinkType.AttachmentHyperlink:
				{
					var attachmentlink = url as AttachmentHyperlink;
					var fullPath = System.IO.Path.Combine(
						FileDirectory.AppExternalStorage,
						attachmentlink.TargetFileName);

					var intent = new Intent(Intent.ActionView);
					intent.AddCategory(Intent.CategoryDefault);
					intent.AddFlags(ActivityFlags.NewTask);
					var file = new Java.IO.File(fullPath);
					if(attachmentlink.FileType == FileType.PDF)
					{
						intent.SetDataAndType(Uri.FromFile(file), "application/pdf");
					}
					else if(attachmentlink.FileType == FileType.Word)
					{
						intent.SetDataAndType(Uri.FromFile(file), "application/msword");
					}

					try
					{
						Activity.StartActivity(intent);
					}
					catch(ActivityNotFoundException)
					{
						SimpleDialogFragment.Create(new SimpleDialogProvider {
							TitleResId = Resource.String.InstallOfficeDialog_Title,
							MessageResId = Resource.String.InstallOfficeDialog_Message,
							PositiveButtonResId = Resource.String.OK,
							NegativeButtonResId = 0,
							ExtTagKey = ContentActivity.InstallOfficeAppDialog,
							DismissAfterProcessRestore = true,
							CanceledOnTouchOutside = false,
							Cancelable = false,
						}).Show(Activity.SupportFragmentManager);
					}
				}
				break;
			default:
				throw new InvalidProgramException("Unknown link type.");
			}
		}

		public int GetCurrentMainTab()
		{
			return rgMainTab.CheckedRadioButtonId;
		}

		private readonly LeftPanelAnimateListener leftPanelAnimateListener;

		private class LeftPanelAnimateListener: AnimatorListenerAdapter
		{
			private readonly ContentMainFragment fragment;

			public LeftPanelAnimateListener(ContentMainFragment fragment)
			{
				this.fragment = fragment;
			}

			public override void OnAnimationEnd(Animator animation)
			{
				if(fragment.MainFragmentStatus.LeftPanelOpen)
				{
					// Open End
					fragment.SetLeftPanelStatus();
				}
				else
				{
					// Close End
					fragment.SetLeftPanelStatus();
				}
			}
		}

		public void SetLeftPanelStatus(bool setRightPanel = true, bool setLeftPanel = true)
		{
			if(MainFragmentStatus.LeftPanelOpen)
			{
				llMainBoard.SetBackgroundColor(Color.White);
				llLeftPanelContainerStub.Visibility = ViewStates.Visible;
				llLeftPanelContainer.Visibility = ViewStates.Visible;
				if(setLeftPanel)
				{
					llLeftPanelContainer.TranslationX = 0.0f;
				}
			}
			else
			{
				llMainBoard.SetBackgroundColor(Color.ParseColor("#eeeeee"));
				llLeftPanelContainerStub.Visibility = ViewStates.Gone;
				if(setLeftPanel)
				{
					llLeftPanelContainer.Visibility = ViewStates.Gone;
				}
			}

			if(setRightPanel)
			{
				SetRightPanelExpandableStatus();
			}
		}

		private void SetRightPanelExpandableStatus()
		{
			var rightFragment = Activity.SupportFragmentManager.FindShowingFragmentById(Resource.Id.frRightPanelContainer) as IExpandableRightPanel;
			if(rightFragment != null)
			{
				rightFragment.SetExpandableStatus();
			}
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
			case Android.Resource.Id.Home:
				{
					((ContentActivity)Activity).CloseContent();
				}
				break;
			case Resource.Id.action_organisepublications:
				{
					Intent intent=new Intent(Activity, typeof(SettingsBoardActivity));
					intent.PutExtra(
						SettingsBoardActivity.FunctionKey,
						SettingsBoardActivity.OrganisePublications);
					StartActivity(intent);
				}
				break;
			case Resource.Id.action_termsconditions:
				{
					Intent intent=new Intent(Activity, typeof(SettingsBoardActivity));
					intent.PutExtra(
						SettingsBoardActivity.FunctionKey,
						SettingsBoardActivity.TermsAndConditions);
					StartActivity(intent);
				}
				break;
			case Resource.Id.action_publicationtext:
				{
					var publicationTextDialogFragment = new PublicationTextDialogFragment();
					publicationTextDialogFragment.Show(FragmentManager.BeginTransaction(), "publicationTextDialogFragment");
				}
				break;
			case Resource.Id.action_about_lnlegalprofessional:
				{
					Intent intent=new Intent(Activity, typeof(SettingsBoardActivity));
					intent.PutExtra(
						SettingsBoardActivity.FunctionKey,
						SettingsBoardActivity.LNLegalAndProfessional);
					StartActivity(intent);
				}
				break;
			case Resource.Id.action_about_lnred:
				{
					Intent intent=new Intent(Activity, typeof(SettingsBoardActivity));
					intent.PutExtra(
						SettingsBoardActivity.FunctionKey,
						SettingsBoardActivity.LNRed);
					StartActivity(intent);
				}
				break;
			case Resource.Id.action_faq:
				{
					Intent intent=new Intent(Activity, typeof(SettingsBoardActivity));
					intent.PutExtra(
						SettingsBoardActivity.FunctionKey,
						SettingsBoardActivity.FAQs);
					StartActivity(intent);
				}
				break;
			case Resource.Id.action_contactus:
				{
					Intent intent=new Intent(Activity, typeof(SettingsBoardActivity));
					intent.PutExtra(
						SettingsBoardActivity.FunctionKey,
						SettingsBoardActivity.Contact);
					StartActivity(intent);
				}
				break;
			case Resource.Id.action_logout:
				{
					SimpleDialogFragment.Create(new SimpleDialogProvider() {
						TitleResId = Resource.String.LogoutConfirm_Title,
						MessageResId = Resource.String.LogoutConfirm_Message,
						PositiveButtonResId = Resource.String.Confirm,
						NegativeButtonResId = Resource.String.Cancel,
						ExtTagKey = LogoutDialog,
						CanceledOnTouchOutside = false,
					}).Show(Activity.SupportFragmentManager);
				}
				break;
			default:
				return base.OnOptionsItemSelected(item);
			}

			return true;
		}

		public bool OnSimpleDialogButtonClick(DialogButtonType buttonType,string fragmentTag, string extTagKey, string extTag)
		{
			if(extTagKey == LogoutDialog)
			{
				if(buttonType == DialogButtonType.Positive)
				{
					Activity.SetResult(Result.Ok);
					((ContentActivity)Activity).CloseContent();
				}

				return true;
			}

			return false;
		}

		public void Refresh()
		{
			//var tab = GetMainTab(MainFragmentStatus.ShowingTab);
			//if(tab.Checked)
			//{
			//	RefreshCurrentTab();
			//}
			//else
			//{
			//	rgMainTab.Check(tab.Id);
			//}
		}

		private void RefreshCurrentTab()
		{
			//var leftFragment = Activity.SupportFragmentManager.FindShowingFragmentById(Resource.Id.frLeftPanelContainer) as IRefreshableFragment;
			//if(leftFragment != null)
			//{
			//	leftFragment.Refresh();
			//}

			RefreshTitleBarInfoIcon();
		}

		private static int GetMainTabIndex(int resIdex)
		{
			switch(resIdex)
			{
			//case Resource.Id.rbContents:
			//	{
			//		return TabContents;
			//	}
			//case Resource.Id.rbIndex:
			//	{
			//		return TabIndex;
			//	}
			case Resource.Id.rbAnnotations:
				{
					return TabAnnotations;
				}
			}

			throw new InvalidProgramException("Unknown main tab id.");
		}

		private RadioButton GetMainTab(int index)
		{
			switch(index)
			{
			case TabContents:
				{
					return rbContents;
				}
			case TabIndex:
				{
					return rbIndex;
				}
			case TabAnnotations:
				{
					return rbAnnotations;
				}
			}

			throw new InvalidProgramException("Unknown main tab index.");
		}

		public void SwitchLogicalMainTab(int tabId)
		{
			MainFragmentStatus.ShowingTab = tabId;

			/*
			switch(tabId)
			{
			case TabContents:
				{
					if(DataCache.INSTATNCE.Toc == null
						|| DataCache.INSTATNCE.Toc.Publication.BookId != ((ContentActivity)Activity).Publication.Value.BookId)
					{
						// scenario:
						// 1. Open a book (Contents Tab);
						// 2. Switch to Annotations Tab;
						// 3. Open Recent History
						// 4. Switch to another book;
						// 5. Navigate back (first book Annotations Tab);
						// 6. Navigate back (first book Contents Tab);

						// Show the page according to NavigationManagerHelper.GetLastReasonableRecord
					}
					else if(!DataCache.INSTATNCE.Toc.IsCurrentNavigationItem())
					{
						if(NavigationManager.Instance.CurrentRecord.RecordType == RecordType.ContentRecord)
						{
							// Force to show the CurrentRecord
						}
						else
						{
							// 1. Show current page
							// 2. the page according to NavigationManagerHelper.GetLastReasonableRecord
						}
					}
				}
				break;
			case TabIndex:
				{
					// Do nothing
				}
				break;
			case TabAnnotations:
				{
					if(DataCache.INSTATNCE.AnnotationsStatus == null)
					{
						DataCache.INSTATNCE.AnnotationsStatus = new AnnotationsStatusKeeper();
					}

					var currentBookid = NavigationManagerHelper.GetCurrentBookId();
					if(!DataCache.INSTATNCE.AnnotationsStatus.IsBindNavigationItem()
					   || DataCache.INSTATNCE.AnnotationsStatus.GetNavigationItem().BookID != currentBookid)
					{
						NavigationManager.Instance.AddRecord(new AnnotationNavigatorRecord(
							currentBookid, 0, 0, 0, Guid.Empty, null, null));
						DataCache.INSTATNCE.AnnotationsStatus.BindNavigationItem();
					}
					else
					{
						NavigationManagerHelper.SetCurrentIndex(
							DataCache.INSTATNCE.AnnotationsStatus.NavigationRecordId);
					}
				}
				break;
			default:
				throw new InvalidProgramException("Unknown Tab id");
			}
			*/
		}

		public void RefreshTitleBarInfoIcon()
		{
			if(actionMenuInfo == null)
			{
				return;
			}

			var pub = ((ContentActivity)Activity).Publication;
			var updateCount = actionMenuInfo.FindViewById<TextView>(Resource.Id.tvNumber);
			if(pub.Value.UpdateCount > 0)
			{
				updateCount.Visibility = ViewStates.Visible;
				updateCount.Text = pub.Value.UpdateCount.ToString();
			}
			else
			{
				updateCount.Visibility = ViewStates.Invisible;
			}
		}
	}
}

