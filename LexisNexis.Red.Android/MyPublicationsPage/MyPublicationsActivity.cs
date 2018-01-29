using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Droid.SettingsPage;
using Android.Support.V4.Widget;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Common.Business;
using Android.Graphics;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Widget.StatusBar;
using System.Threading.Tasks;
using System.Threading;
using LexisNexis.Red.Droid.ContentPage;
using LexisNexis.Red.Droid.SettingsBoardPage;
using LexisNexis.Red.Droid.DetailInfoModal;
using LexisNexis.Red.Droid.RecentHistory;
using LexisNexis.Red.Droid.AnnotationOrganiserPage;
using LexisNexis.Red.Droid.Widget;
using LexisNexis.Red.Droid.AnnotationUtility;
using AnnotationListFragment = LexisNexis.Red.Droid.AnnotationOrganiserPage.AnnotationListFragment;

namespace LexisNexis.Red.Droid.MyPublicationsPage
{
    [Activity(Label = "智慧消防", Icon = "@drawable/icon", HardwareAccelerated = false)]
    public class MyPublicationsActivity
        : AppCompatActivity,
            ISettingsDialogHostActivity,
            IAsyncTaskActivity,
            ISimpleDialogListener,
            IDownloadPublicationTaskListener,
            IRecentHistoryChangedListener,
            ITagListUpdateListener
    {
        private string asyncTaskActivityGUID;

        private const string IsSettingsDrawerOpen = "IsSettingsDrawerOpen";
        private const string IsPublicationFilterShowingDropDown = "IsPublicationFilterShowingDropDown";
        private const string LogoutDialog = "LogoutDialog";
        private const string NetworkFlowOverLimitationDialog = "NetworkFlowOverLimitationDialog";

        public const int RequestCodeLogout = 1001;

        private IMenuItem actionMenuLastSync;
        private DrawerLayout dlRightDrawer;
        private FrameLayout flRightDrawerPanelContainer;
        private LinearLayout llHeader;
        private TextView tvNoHistoryMessageInDrawer;
        private RecyclerView rcHistoryList;
        private LinearLayoutManager historylistLayoutManager;
        private HistoryListAdaptor hrcAdaptor;

        private Toolbar toolbar;
        //private MainMenuPopup mainMenu;

        private static int MainHeaderBottom = 0;

        public PublicationListFragment PublicationListFragment
        {
            get;
            set;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //RegisterRecentHistoryChangeCallback();

            StatusBarTintHelper.SetStatusBarColor(this);

            Android.Util.Log.Info("DBG", "Set MyPublicationsActivity[" + asyncTaskActivityGUID + "].");

            // Create your application here
            SetContentView(Resource.Layout.mypublications_activity);

            FindViewById<LinearLayout>(Resource.Id.llStatusBarStub).LayoutParameters =
                new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, StatusBarTintHelper.GetStatusBarHeight());

            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_actionbar);
            SetSupportActionBar(toolbar);

            dlRightDrawer = FindViewById<DrawerLayout>(Resource.Id.dlRightDrawer);
            flRightDrawerPanelContainer = FindViewById<FrameLayout>(Resource.Id.flRightDrawerPanelContainer);
            llHeader = FindViewById<LinearLayout>(Resource.Id.llHeader);
            tvNoHistoryMessageInDrawer = FindViewById<TextView>(Resource.Id.tvNoHistoryMessageInDrawer);

            // Set the shadow layer of drawer layout to totally transparent
            dlRightDrawer.SetScrimColor(Color.Transparent);
            dlRightDrawer.DrawerStateChanged += (object sender, DrawerLayout.DrawerStateChangedEventArgs e) =>
            {
                if (e.NewState == 1)
                {
                    // STATE_DRAGGING
                    // Indicates that a drawer is currently being dragged by the user.
                    hrcAdaptor.RefreshHisoryList();
                    tvNoHistoryMessageInDrawer.Visibility = hrcAdaptor.ItemCount == 0 ? ViewStates.Visible : ViewStates.Gone;
                }
            };

            rcHistoryList = FindViewById<RecyclerView>(Resource.Id.rcHistoryList);
            historylistLayoutManager = new LinearLayoutManager(this);
            historylistLayoutManager.Orientation = LinearLayoutManager.Vertical;
            rcHistoryList.SetLayoutManager(historylistLayoutManager);
            hrcAdaptor = new HistoryListAdaptor(
                this,
                Resource.Layout.content_historylist_item,
                OnRecentHistoryItemClick);
            rcHistoryList.SetAdapter(hrcAdaptor);
            rcHistoryList.Visibility = ViewStates.Invisible;

            var frgContainer = SupportFragmentManager.FindFragmentById(Resource.Id.frgContainer);

            if (frgContainer == null)
            {
                var publicationsMainFragment = new PublicationsMainFragment();
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.frgContainer, publicationsMainFragment).Commit();
            }

            if (savedInstanceState != null)
            {
                asyncTaskActivityGUID = savedInstanceState.GetString(
                    AsyncUIOperationRepeater.ASYNC_ACTIVITY_GUID);
                if (savedInstanceState.GetBoolean(IsPublicationFilterShowingDropDown))
                {
                    Task.Run(delegate
                    {
                        Thread.Sleep(100);
                        Application.SynchronizationContext.Post(_ =>
                        {
                            if (PublicationListFragment != null)
                            {
                                PublicationListFragment.ForceShowPublicationFilterDropDown();
                            }
                        }, null);
                    });
                }
            }

            if (string.IsNullOrEmpty(asyncTaskActivityGUID))
            {
                asyncTaskActivityGUID = Guid.NewGuid().ToString();
            }

            AsyncUIOperationRepeater.INSTATNCE.RegisterAsyncTaskActivity(this);
            Android.Util.Log.Info("DBG", "MyPublicationsActivity[" + asyncTaskActivityGUID + "] Created.");
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            if (GetPublicationsMainFragment().GetCurrentMainTab() == Resource.Id.rbAnnotations)
            {
                MenuInflater.Inflate(Resource.Menu.annotations_actionbar, menu);
                menu.FindItem(Resource.Id.action_search).ActionView.FindViewById<View>(Resource.Id.flBackground).Click += delegate
                {

                };

                menu.FindItem(Resource.Id.action_edittags).ActionView.FindViewById<View>(Resource.Id.flBackground).Click += delegate
                {
                    EditTagsDialogFragment.NewInstance().Show(
                        SupportFragmentManager.BeginTransaction(),
                        EditTagsDialogFragment.EditTagsDialogFragmentTag);
                };

                menu.FindItem(Resource.Id.action_recenthistory).ActionView.FindViewById<View>(Resource.Id.flBackground).Click += delegate
                {
                    OpenRecentHistory();
                };
            }
            else
            {
                MenuInflater.Inflate(Resource.Menu.mypublications_actionbar, menu);
            }

            //actionMenuLastSync = menu.FindItem(Resource.Id.action_lastsync);
            //actionMenuLastSync.SetEnabled(false);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            //actionMenuLastSync.SetTitle(
            //	string.Format(
            //		MainApp.ThisApp.Resources.GetString(
            //			Resource.String.MainMenuPopup_LastSync),
            //		LastSyncedTimeHelper.Get().ToString("hh:mmtt, d MMM yyyy")));
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    {
                        Finish();
                    }
                    break;
                case Resource.Id.action_organisepublications:
                    {
                        Intent intent = new Intent(this, typeof(SettingsBoardActivity));
                        intent.PutExtra(
                            SettingsBoardActivity.FunctionKey,
                            SettingsBoardActivity.OrganisePublications);
                        StartActivity(intent);
                    }
                    break;
                case Resource.Id.action_termsconditions:
                    {
                        Intent intent = new Intent(this, typeof(SettingsBoardActivity));
                        intent.PutExtra(
                            SettingsBoardActivity.FunctionKey,
                            SettingsBoardActivity.TermsAndConditions);
                        StartActivity(intent);
                    }
                    break;
                case Resource.Id.action_about_lnlegalprofessional:
                    {
                        Intent intent = new Intent(this, typeof(SettingsBoardActivity));
                        intent.PutExtra(
                            SettingsBoardActivity.FunctionKey,
                            SettingsBoardActivity.LNLegalAndProfessional);
                        StartActivity(intent);
                    }
                    break;
                case Resource.Id.action_about_lnred:
                    {
                        Intent intent = new Intent(this, typeof(SettingsBoardActivity));
                        intent.PutExtra(
                            SettingsBoardActivity.FunctionKey,
                            SettingsBoardActivity.LNRed);
                        StartActivity(intent);
                    }
                    break;
                case Resource.Id.action_faq:
                    {
                        Intent intent = new Intent(this, typeof(SettingsBoardActivity));
                        intent.PutExtra(
                            SettingsBoardActivity.FunctionKey,
                            SettingsBoardActivity.FAQs);
                        StartActivity(intent);
                    }
                    break;
                case Resource.Id.action_contactus:
                    {
                        Intent intent = new Intent(this, typeof(SettingsBoardActivity));
                        intent.PutExtra(
                            SettingsBoardActivity.FunctionKey,
                            SettingsBoardActivity.Contact);
                        StartActivity(intent);
                    }
                    break;
                case Resource.Id.action_logout:
                    {
                        OnLogoutClicked();
                    }
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
            }

            return true;
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);

            if (MainHeaderBottom <= 0)
            {
                var llTabContainer = FindViewById<View>(Resource.Id.llTabContainer);
                int[] locations = new int[2];
                llTabContainer.GetLocationInWindow(locations);
                MainHeaderBottom = locations[1] - StatusBarTintHelper.GetStatusBarHeight();

                var flToolbarBottom = FindViewById<FrameLayout>(Resource.Id.flToolbarBottom);
                flToolbarBottom.GetLocationInWindow(locations);
                ToolbarHelper.SetToolbarHeight(locations[1] - StatusBarTintHelper.GetStatusBarHeight());
            }

            llHeader.LayoutParameters.Height = MainHeaderBottom
                + StatusBarTintHelper.GetStatusBarHeight()
                + (int)MainApp.ThisApp.Resources.GetDimension(Resource.Dimension.fixedbar_height);
        }

        private PublicationsMainFragment GetPublicationsMainFragment()
        {
            return (PublicationsMainFragment)SupportFragmentManager.FindFragmentById(Resource.Id.frgContainer);
        }

        private void OnLogoutClicked()
        {
            SimpleDialogFragment.Create(new SimpleDialogProvider
            {
                TitleResId = Resource.String.LogoutConfirm_Title,
                MessageResId = Resource.String.LogoutConfirm_Message,
                PositiveButtonResId = Resource.String.Confirm,
                NegativeButtonResId = Resource.String.Cancel,
                ExtTagKey = LogoutDialog,
                CanceledOnTouchOutside = false,
            }).Show(SupportFragmentManager);
        }

        public bool OnSimpleDialogButtonClick(DialogButtonType buttonType, string fragmentTag, string extTagKey, string extTag)
        {
            switch (extTagKey)
            {
                case LogoutDialog:
                    if (buttonType == DialogButtonType.Positive)
                    {
                        ProcessLogout();
                    }

                    return true;
                case NetworkFlowOverLimitationDialog:
                    if (buttonType == DialogButtonType.Positive)
                    {
                        DataCache.INSTATNCE.PublicationManager.DownloadPublication(this, Int32.Parse(extTag), true);
                    }

                    return true;
                case PublicationExtInfo.CANCELDOWNLOAD_CONFIRM_DIALOG_EXTRATAG:
                    if (buttonType == DialogButtonType.Positive)
                    {
                        var extInfo = DataCache.INSTATNCE.PublicationManager.GetPublicationExtInfo(Int32.Parse(extTag));
                        if (extInfo.CancellationSource != null)
                        {
                            extInfo.CancellationSource.Cancel();
                        }
                    }

                    return true;
            }

            return false;
        }

        protected override void OnResume()
        {
            base.OnResume();
            AsyncUIOperationRepeater.INSTATNCE.RegisterAsyncTaskActivity(this);
            AsyncUIOperationRepeater.INSTATNCE.ExecutePendingUIOperation(this);

            llHeader.LayoutParameters.Height = MainHeaderBottom
                + StatusBarTintHelper.GetStatusBarHeight()
                + (int)MainApp.ThisApp.Resources.GetDimension(Resource.Dimension.fixedbar_height);

            if (dlRightDrawer.IsDrawerOpen(flRightDrawerPanelContainer))
            {
                hrcAdaptor.RefreshHisoryList();
                tvNoHistoryMessageInDrawer.Visibility =
                    hrcAdaptor.ItemCount == 0
                        ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(AsyncUIOperationRepeater.ASYNC_ACTIVITY_GUID, asyncTaskActivityGUID);
            outState.PutBoolean(
                IsPublicationFilterShowingDropDown,
                PublicationListFragment != null && PublicationListFragment.IsPublicationFilterShowingDropDown());
            Android.Util.Log.Info("DBG", "MyPublicationsActivity[" + asyncTaskActivityGUID + "] save state.");
            base.OnSaveInstanceState(outState);
        }

        protected override void OnStop()
        {
            AsyncUIOperationRepeater.INSTATNCE.UnregisterAsyncTaskActivity(this);
            Android.Util.Log.Info("DBG", "Unset MyPublicationsActivity[" + asyncTaskActivityGUID + "].");
            base.OnStop();
        }

        private void RegisterRecentHistoryChangeCallback()
        {
            if (PublicationUtil.Instance.RecenthistoryChanged != null)
            {
                return;
            }

            PublicationUtil.Instance.RecenthistoryChanged = () =>
            {
                AsyncUIOperationRepeater.INSTATNCE.SubmitAsyncUIOperation(
                    null,
                    currentActiviy =>
                    {
                        var recentHistoryChangedListener = currentActiviy as IRecentHistoryChangedListener;
                        if (recentHistoryChangedListener != null)
                        {
                            recentHistoryChangedListener.OnRecentHistoryChanged();
                        }
                    },
                    true,
                    currentActivity => currentActivity is IRecentHistoryChangedListener);
            };
        }

        public void OnRecentHistoryChanged()
        {
            if (PublicationListFragment != null)
            {
                PublicationListFragment.UpdateRecentHistory();
            }

            if (dlRightDrawer.IsDrawerOpen(flRightDrawerPanelContainer))
            {
                hrcAdaptor.RefreshHisoryList();
                tvNoHistoryMessageInDrawer.Visibility =
                    hrcAdaptor.ItemCount == 0
                    ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        private void ProcessLogout()
        {
            DataCache.INSTATNCE.PublicationManager.CancelAllDownloadingTask();
            DataCache.INSTATNCE.PublicationManager.SetPublicationList(null, true);
            LoginUtil.Instance.Logout();
            ReturnToLoginPage();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == RequestCodeLogout)
            {
                if (resultCode == Result.Ok)
                {
                    ProcessLogout();
                }
            }
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back
                && e.RepeatCount == 0
                && dlRightDrawer.IsDrawerOpen(flRightDrawerPanelContainer))
            {
                dlRightDrawer.CloseDrawer(flRightDrawerPanelContainer);
                return true;
            }

            return base.OnKeyDown(keyCode, e);
        }

        public void OpenRecentHistory()
        {
            dlRightDrawer.OpenDrawer(flRightDrawerPanelContainer);
            hrcAdaptor.RefreshHisoryList();
            tvNoHistoryMessageInDrawer.Visibility = hrcAdaptor.ItemCount == 0 ? ViewStates.Visible : ViewStates.Gone;
        }

        private void OnRecentHistoryItemClick(RecentHistoryItem historyItem)
        {
            OpenPublication(
                historyItem.BookId,
                AsyncHelpers.RunSync<int>(
                    () => PublicationContentUtil.Instance.GetTOCIDByDocId(
                        historyItem.BookId,
                        historyItem.DOCID)));
            dlRightDrawer.CloseDrawer(flRightDrawerPanelContainer);
        }

        public void OpenPublication(int bookId, int tocId = -1)
        {
            var intent = new Intent(this, typeof(ContentActivity));
            intent.PutExtra("CurrentBookId", bookId);
            StartActivityForResult(intent, RequestCodeLogout);
         
        }

        public void UpdatePublicationDownloadingProgress(int bookId)
        {
            if (PublicationListFragment != null)
            {
                PublicationListFragment.UpdatePublication(bookId);
            }

            //var detailInfo = SupportFragmentManager.FindFragmentByTag(PublicationDetailInfoFragment.PublicationDetailInfoFragmentKey);
            //if(detailInfo != null)
            //{
            //	((PublicationDetailInfoFragment)detailInfo).UpdateProgress(bookId);
            //}
        }

        public void UpdatePublicationDetailInfo(int bookId)
        {
            var detailInfo = SupportFragmentManager.FindFragmentByTag(PublicationDetailInfoFragment.PublicationDetailInfoFragmentKey);
            if (detailInfo != null)
            {
                ((PublicationDetailInfoFragment)detailInfo).UpdateWholeInfo(bookId);
            }
        }

        public void ReturnToLoginPage()
        {
            var intent = new Intent(this, typeof(LoginPage.LoginActivity));
            StartActivity(intent);
            Finish();
        }

        public string AsyncTaskActivityGUID
        {
            get
            {
                return asyncTaskActivityGUID;
            }
        }

        public void ProcessDownloadResult(DownloadResult result, int bookId)
        {
            var dialogTag = SimpleDialogFragment.FindFragmentTagByExtraTag(
                this,
                (extTagKey, extTag) =>
                    extTagKey == PublicationExtInfo.CANCELDOWNLOAD_CONFIRM_DIALOG_EXTRATAG
                    && Int32.Parse(extTag) == bookId);
            if (dialogTag != null)
            {
                SimpleDialogFragment.DismissDialog(this, dialogTag);
            }

            switch (result.DownloadStatus)
            {
                case DownLoadEnum.Success:
                    // need not do any thing.
                    break;
                case DownLoadEnum.Canceled:
                    // need not do any thing.
                    break;
                case DownLoadEnum.Failure:
                    DialogGenerator.ShowMessageDialog(
                        SupportFragmentManager,
                        Resource.String.DetailInfo_InstallError_Title,
                        Resource.String.DetailInfo_InstallError_Message);
                    break;
                case DownLoadEnum.OverLimitation:
                    // raise a dialog;
                    DialogGenerator.ShowMessageDialog(
                        this.SupportFragmentManager,
                        Resource.String.NetworkFlow_Warning_Title,
                        Resource.String.NetworkFlow_Warning_Message,
                        Resource.String.NetworkFlow_Warning_Download,
                        0,
                        NetworkFlowOverLimitationDialog);
                    break;
                case DownLoadEnum.NetDisconnected:
                    DialogGenerator.ShowMessageDialog(
                        SupportFragmentManager,
                        Resource.String.DownloadFailded_NetDisconnected_Title,
                        Resource.String.DownloadFailded_NetDisconnected_Message);
                    break;
            }

            this.UpdatePublicationDownloadingProgress(bookId);
            this.UpdatePublicationDetailInfo(bookId);
        }

        public void OnTagListUpdated()
        {
            var fragment = SupportFragmentManager.FindFragmentByTag(AnnotationListFragment.FragmentTag) as AnnotationListFragment;
            if (fragment != null)
            {
                fragment.Refresh();
            }
        }
    }
}