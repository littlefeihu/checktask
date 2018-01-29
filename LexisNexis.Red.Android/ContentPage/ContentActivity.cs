using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment = Android.Support.V4.App.Fragment;
using LexisNexis.Red.Droid.Widget.StatusBar;
using Android.Support.V7.App;
using SearchView = Android.Support.V7.Widget.SearchView;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.Async;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.RecentHistory;
using Android.Support.V4.Widget;
using Android.Graphics;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Droid.DetailInfoModal;
using LexisNexis.Red.Droid.WebViewUtility;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using LexisNexis.Red.Droid.AnnotationUtility;
using LexisNexis.Red.Droid.PrintUtility;

namespace LexisNexis.Red.Droid.ContentPage
{
    [Activity(Label = "Content")]
    public class ContentActivity
        : AppCompatActivity,
            IAsyncTaskActivity,
            ISimpleDialogListener,
            IDownloadPublicationTaskListener,
            IRecentHistoryChangedListener,
            ITagListUpdateListener
    {
        public const string CacheCatagory = "Content";
        private const string NetworkFlowOverLimitationDialog = "NetworkFlowOverLimitationDialog";
        private const string InstallPublicationCompletedDialog = "InstallPublicationCompletedDialog";
        public const string InstallOfficeAppDialog = "InstallOfficeAppDialog";
        private const string NavigationCacheFile = "Navigation.{0}.cache";
        private const string IndexCacheFile = "Index.{0}.cache";
        private const string StatusCacheFile = "Status.{0}.cache";

        private static int MainHeaderBottom = 0;

        private string asyncTaskActivityGUID;

        private ObjHolder<Publication> publication;

        private Toolbar toolbar;

        private FrameLayout frgContainer;

        private DrawerLayout dlRightDrawer;
        private FrameLayout flRightDrawerPanelContainer;
        private LinearLayout llHeader;
        private TextView tvNoHistoryMessageInDrawer;
        private RecyclerView rcHistoryList;

        private LinearLayoutManager historylistLayoutManager;
        private HistoryListAdaptor hrcAdaptor;

        private ActionMode actionMode;
        private LegalDefinePopup legalDefinePopup;

        private Tuple<int, bool> pboInfo;

        private ContentActivityStatus status = new ContentActivityStatus();

        public ObjHolder<Publication> Publication
        {
            get
            {
                var bookId = Intent.Extras.GetInt("CurrentBookId");
                if (bookId < 0)
                {
                    return null;
                }

                if (publication == null
                    || publication.Value.BookId != bookId)
                {
                    publication = DataCache.INSTATNCE.PublicationManager.GetPublication(bookId);
                }

                return publication;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            StatusBarTintHelper.SetStatusBarColor(this);

            if (savedInstanceState != null)
            {
                asyncTaskActivityGUID = savedInstanceState.GetString(
                    AsyncUIOperationRepeater.ASYNC_ACTIVITY_GUID);
                //if (NavigationManager.Instance.Records.Count == 0)
                //{
                //    // We need not restore data while navigation manager has valid record list.
                //    var navigationCache = FileCacheHelper.ReadCacheFile(
                //        CacheCatagory, string.Format(NavigationCacheFile, asyncTaskActivityGUID));
                //    if (navigationCache != null)
                //    {
                //        NavigationManager.Instance.RestoreRecords(navigationCache);
                //    }

                //    var indexCache = FileCacheHelper.ReadCacheFile(
                //        CacheCatagory, string.Format(IndexCacheFile, asyncTaskActivityGUID));
                //    if (indexCache != null)
                //    {
                //        DataCache.INSTATNCE.IndexList =
                //            JsonConvert.DeserializeObject<PublicationIndexStatus>(indexCache);
                //    }
                //}
            }

            if (string.IsNullOrEmpty(asyncTaskActivityGUID))
            {
                asyncTaskActivityGUID = Guid.NewGuid().ToString();
            }

            SetContentView(Resource.Layout.content_activity);

            //legalDefinePopup = new LegalDefinePopup(this, OnUserDismissLegalDefinePopup);

            FindViewById<LinearLayout>(Resource.Id.llStatusBarStub).LayoutParameters =
                new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, StatusBarTintHelper.GetStatusBarHeight());

            // Fake Update
            //publication.Value.UpdateCount = 3;

            if (Publication == null)
            {
                CloseContent();
                return;
            }


            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_actionbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.Title = Publication.Value.Name;

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

            //rcHistoryList = FindViewById<RecyclerView>(Resource.Id.rcHistoryList);
            //historylistLayoutManager = new LinearLayoutManager(this);
            //historylistLayoutManager.Orientation = LinearLayoutManager.Vertical;
            //rcHistoryList.SetLayoutManager(historylistLayoutManager);
            //hrcAdaptor = new HistoryListAdaptor(
            //    this,
            //    Resource.Layout.content_historylist_item,
            //    OnRecentHistoryItemClick);
            //rcHistoryList.SetAdapter(hrcAdaptor);

            frgContainer = FindViewById<FrameLayout>(Resource.Id.frgContainer);
            var mainFragment = SupportFragmentManager.FindFragmentById(Resource.Id.frgContainer) as ContentMainFragment;

            if (mainFragment == null)
            {
                mainFragment = new ContentMainFragment();
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.frgContainer, mainFragment).Commit();
            }

            SetResult(Result.Canceled);

            AsyncUIOperationRepeater.INSTATNCE.RegisterAsyncTaskActivity(this);
            Android.Util.Log.Info("DBG", "ContentActivity[" + asyncTaskActivityGUID + "] Created.");
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
            }

            //  llHeader.LayoutParameters.Height = MainHeaderBottom  + (int)MainApp.ThisApp.Resources.GetDimension(Resource.Dimension.fixedbar_height);
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            //if (keyCode == Keycode.Back && e.RepeatCount == 0)
            //{
            //    if (dlRightDrawer.IsDrawerOpen(flRightDrawerPanelContainer))
            //    {
            //        dlRightDrawer.CloseDrawer(flRightDrawerPanelContainer);
            //        return true;
            //    }
            //    CloseContent();
            //    return true;
            //}

            return base.OnKeyDown(keyCode, e);
        }

        public override void OnActionModeStarted(ActionMode mode)
        {
            if (webViewContentListener == null)
            {
                webViewContentListener = new WebViewContentListener(this);
            }

            actionMode = mode;
            actionMode.Menu.Clear();
            actionMode.MenuInflater.Inflate(Resource.Menu.webcontent_menu, actionMode.Menu);

            actionMode.Menu.FindItem(Resource.Id.actionContentAnnotate)
                .SetOnMenuItemClickListener(webViewContentListener);

            if (DictionaryUtil.IsDictionaryDownloaded(1))
            {
                actionMode.Menu.FindItem(Resource.Id.actionContentLegalDefine)
                    .SetOnMenuItemClickListener(webViewContentListener);
            }
            else
            {
                actionMode.Menu.RemoveItem(Resource.Id.actionContentLegalDefine);
            }

            actionMode.Menu.FindItem(Resource.Id.actionContentCopy)
                .SetOnMenuItemClickListener(webViewContentListener);

            base.OnActionModeStarted(actionMode);
        }

        private WebViewContentListener webViewContentListener;
        private class WebViewContentListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
        {
            private readonly ContentActivity activity;
            public WebViewContentListener(ContentActivity activity)
            {
                this.activity = activity;
            }

            public bool OnMenuItemClick(IMenuItem item)
            {
                switch (item.ItemId)
                {
                    case Resource.Id.actionContentAnnotate:
                        {
                            activity.GetMainFragment().DoActionMode(item.ItemId);
                            Toast.MakeText(activity, "Annotate", ToastLength.Short).Show();

                            if (activity.actionMode != null)
                            {
                                activity.actionMode.Finish();
                            }

                            activity.GetMainFragment().AfterDoActionMode(item.ItemId);
                        }
                        return true;
                    case Resource.Id.actionContentLegalDefine:
                        {
                            activity.GetMainFragment().DoActionMode(item.ItemId);

                            if (activity.actionMode != null)
                            {
                                activity.actionMode.Finish();
                            }

                            activity.GetMainFragment().AfterDoActionMode(item.ItemId);
                        }
                        return true;
                    case Resource.Id.actionContentCopy:
                        {
                            activity.GetMainFragment().DoActionMode(item.ItemId);

                            if (activity.actionMode != null)
                            {
                                activity.actionMode.Finish();
                            }

                            activity.GetMainFragment().AfterDoActionMode(item.ItemId);
                        }
                        return true;
                }

                return false;
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            FileCacheHelper.SaveCacheFile(
                CacheCatagory,
                string.Format(NavigationCacheFile, asyncTaskActivityGUID),
                NavigationManager.Instance.SerializeRecords());

            if (DataCache.INSTATNCE.IndexList == null)
            {
                FileCacheHelper.DeleteCacheFile(
                    CacheCatagory,
                    string.Format(IndexCacheFile, asyncTaskActivityGUID));
            }
            else
            {
                FileCacheHelper.SaveCacheFile(
                    CacheCatagory,
                    string.Format(IndexCacheFile, asyncTaskActivityGUID),
                    JsonConvert.SerializeObject(DataCache.INSTATNCE.IndexList));
            }

            outState.PutString(AsyncUIOperationRepeater.ASYNC_ACTIVITY_GUID, asyncTaskActivityGUID);
            Android.Util.Log.Info("DBG", "ContentActivity[" + asyncTaskActivityGUID + "] save state.");
            base.OnSaveInstanceState(outState);
        }

        protected override void OnResume()
        {
            base.OnResume();
            AsyncUIOperationRepeater.INSTATNCE.RegisterAsyncTaskActivity(this);
            Android.Util.Log.Info("DBG", "ContentActivity[" + asyncTaskActivityGUID + "] Created.");
            AsyncUIOperationRepeater.INSTATNCE.ExecutePendingUIOperation(this);

            if (Publication == null)
            {
                CloseContent();
                return;
            }

            //llHeader.LayoutParameters.Height = MainHeaderBottom
            //    + (int)MainApp.ThisApp.Resources.GetDimension(Resource.Dimension.fixedbar_height);

            //if (dlRightDrawer.IsDrawerOpen(flRightDrawerPanelContainer))
            //{
            //    hrcAdaptor.RefreshHisoryList();
            //    tvNoHistoryMessageInDrawer.Visibility = hrcAdaptor.ItemCount == 0 ? ViewStates.Visible : ViewStates.Gone;
            //}
        }

        protected override void OnStop()
        {
            //legalDefinePopup.Dismiss(true);
            //AsyncUIOperationRepeater.INSTATNCE.UnregisterAsyncTaskActivity(this);
            //Android.Util.Log.Info("DBG", "Unset ContentActivity[" + asyncTaskActivityGUID + "].");
            base.OnStop();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == ContentSearchActivity.RequestCodeSearch
                && resultCode == Result.Ok)
            {
                //var tocId = data.GetIntExtra(ContentSearchActivity.ResultTocId, -1);
                //if (tocId < 0)
                //{
                //    return;
                //}

                //var lastSearchKeywords = data.GetStringExtra(ContentSearchActivity.ResultLastSearchKeywords);
                //if (string.IsNullOrEmpty(lastSearchKeywords))
                //{
                //    return;
                //}

                //var headType = data.GetStringExtra(ContentSearchActivity.HeadType);
                //var HeadSequence = data.GetIntExtra(ContentSearchActivity.HeadSequence, 0);

                //var keywordList = DataCache.INSTATNCE.SearchResult.GetShrinkedKeywordList();
                //var record = DataCache.INSTATNCE.Toc.GetNavigationItem() as SearchBrowserRecord;
                //if (record == null
                //    || !NavigationManagerHelper.CompareActualTocId(record.TOCID, tocId)
                //    || !SearchResultKeeper.CompareShrinkedKeywordList(record.SpliteKeywords, keywordList)
                //    || record.HeadType != headType
                //    || record.HeadSequence != HeadSequence)
                //{
                //    NavigationManager.Instance.AddRecord(
                //        new SearchBrowserRecord(
                //            Publication.Value.BookId,
                //            tocId,
                //            0,
                //            0,
                //            lastSearchKeywords,
                //            headType,
                //            HeadSequence,
                //            DataCache.INSTATNCE.SearchResult.GetShrinkedKeywordList()));
                //}
                //else
                //{
                //    NavigationManagerHelper.MoveForthAndSetCurrentIndex(record.RecordID);
                //    WebViewManager.Instance.ClearWebViewStatus(WebViewManager.WebViewType.Content);
                //    DataCache.INSTATNCE.Toc.ResetNavigationItem();
                //}

                GetMainFragment().SwitchLogicalMainTab(ContentMainFragment.TabContents);
                GetMainFragment().Refresh();
            }
        }

        public bool IsPbo()
        {
            return true;
        }

        public void OpenContentSearch()
        {
            var intent = new Intent(this, typeof(ContentSearchActivity));
            intent.PutExtra(ContentSearchActivity.SessionId, asyncTaskActivityGUID);
            intent.PutExtra(ContentSearchActivity.BookId, Publication.Value.BookId);
            intent.PutExtra(ContentSearchActivity.PublicationTitle, Publication.Value.Name);
            StartActivityForResult(intent, ContentSearchActivity.RequestCodeSearch);
        }

        public void OpenRecentHistory()
        {
            //dlRightDrawer.OpenDrawer(flRightDrawerPanelContainer);
            //hrcAdaptor.RefreshHisoryList();
            //tvNoHistoryMessageInDrawer.Visibility = hrcAdaptor.ItemCount == 0 ? ViewStates.Visible : ViewStates.Gone;
        }

        public ContentMainFragment GetMainFragment()
        {
            return SupportFragmentManager.FindFragmentById(Resource.Id.frgContainer) as ContentMainFragment;
        }

        public string AsyncTaskActivityGUID
        {
            get
            {
                return asyncTaskActivityGUID;
            }
        }

        private void OnRecentHistoryItemClick(RecentHistoryItem historyItem)
        {
            //dlRightDrawer.CloseDrawer(flRightDrawerPanelContainer);
            //var tocId = AsyncHelpers.RunSync<int>(
            //    () => PublicationContentUtil.Instance.GetTOCIDByDocId(
            //        historyItem.BookId,
            //        historyItem.DOCID));

            //if (historyItem.BookId != Publication.Value.BookId)
            //{

            //    NavigationManager.Instance.AddRecord(
            //        new ContentBrowserRecord(
            //            historyItem.BookId,
            //            tocId,
            //            0));
            //    GetMainFragment().SwitchLogicalMainTab(ContentMainFragment.TabContents);

            //    ShowPleaseWaitDialog();

            //    Task.Run(() =>
            //    {
            //        // Wait the "PleaseWaitDialog" pop up
            //        Thread.Sleep(100);
            //        Application.SynchronizationContext.Post(_ =>
            //        {
            //            SwitchPublication();
            //            GetMainFragment().Refresh();
            //        }, null);
            //    });

            //    return;
            //}

            //var record = DataCache.INSTATNCE.Toc.GetNavigationItem();
            //if (record == null
            //    || !NavigationManagerHelper.CompareActualTocId(NavigationManagerHelper.ContentsTabGetTocId(record), tocId))
            //{
            //    NavigationManager.Instance.AddRecord(
            //        new ContentBrowserRecord(
            //            historyItem.BookId,
            //            tocId,
            //            0));
            //}
            //else
            //{
            //    NavigationManagerHelper.MoveForthAndSetCurrentIndex(record.RecordID);
            //}

            //GetMainFragment().SwitchLogicalMainTab(ContentMainFragment.TabContents);

            //GetMainFragment().Refresh();
        }

        public void SwitchPublication()
        {
            //CloseContent(false);
            //SupportActionBar.Title = Publication.Value.Name;
            //GetMainFragment().RefreshTitleBarInfoIcon();
        }

        public void UpdatePublicationDownloadingProgress(int bookId)
        {
            // We can't get current Publication of ContentActivity, after NavigationManager is cleared;
            // So we just get current book id from NavigationManager directly.
            // Releated bug: http://wiki.lexiscn.com/issues/15647
            if (NavigationManagerHelper.GetCurrentBookId() != bookId)
            {
                return;
            }

        }

        public void ProcessDownloadResult(DownloadResult result, int bookId)
        {

        }

        public bool OnSimpleDialogButtonClick(DialogButtonType buttonType, string fragmentTag, string extTagKey, string extTag)
        {
            //switch (extTagKey)
            //{
            //    case NetworkFlowOverLimitationDialog:
            //        if (buttonType == DialogButtonType.Positive)
            //        {
            //            DataCache.INSTATNCE.PublicationManager.DownloadPublication(this, Int32.Parse(extTag), true);
            //        }

            //        return true;
            //    case PublicationExtInfo.CANCELDOWNLOAD_CONFIRM_DIALOG_EXTRATAG:
            //        if (buttonType == DialogButtonType.Positive)
            //        {
            //            var extInfo = DataCache.INSTATNCE.PublicationManager.GetPublicationExtInfo(Int32.Parse(extTag));
            //            if (extInfo.CancellationSource != null)
            //            {
            //                extInfo.CancellationSource.Cancel();
            //            }
            //        }

            //        return true;
            //    case InstallPublicationCompletedDialog:
            //        if (buttonType == DialogButtonType.Positive)
            //        {
            //            CloseContent();
            //        }

            //        return true;
            //    case InstallOfficeAppDialog:
            //        if (buttonType == DialogButtonType.Positive)
            //        {
            //            var intent = new Intent(Intent.ActionView);
            //            intent.SetData(Android.Net.Uri.Parse("market://details?id=com.mobisystems.office"));
            //            StartActivity(intent);
            //        }

            //        return true;
            //}

            return false;
        }

        public void OnRecentHistoryChanged()
        {
            //if (dlRightDrawer.IsDrawerOpen(flRightDrawerPanelContainer))
            //{
            //    hrcAdaptor.RefreshHisoryList();
            //    tvNoHistoryMessageInDrawer.Visibility =
            //        hrcAdaptor.ItemCount == 0
            //        ? ViewStates.Visible : ViewStates.Gone;
            //}
        }

        public void CloseContent(bool finishActivity = true)
        {
            DataCache.INSTATNCE.ClosePublication();

            if (finishActivity)
            {
                WebViewManager.Instance.ClearAllWebViewStatus();
                NavigationManager.Instance.Clear();
                AsyncUIOperationRepeater.INSTATNCE.DiscardActivitySpecificUIOperation(this);
                PdfPrintCacheHelper.DeleteCacheFolder();
                Finish();
            }
        }

        public void ShowPleaseWaitDialog()
        {
            LogHelper.Debug("dbg", "RotateWait::Try to find wait");
            var dialogTag = SimpleDialogFragment.FindFragmentTagByExtraTag(
                this,
                (extTagKey, extTag) =>
                extTagKey == WebViewManager.PleaseWaitPageLoadDialogExtTagKey);
            if (dialogTag == null)
            {
                LogHelper.Debug("dbg", "RotateWait::Not found Wait");
                DialogGenerator.ShowWaitDialog(
                    SupportFragmentManager,
                    0,
                    WebViewManager.PleaseWaitPageLoadDialogExtTagKey);
            }
            LogHelper.Debug("dbg", "RotateWait::The Wait is shown here.");
        }

        public void ClosePleaseWaitPageLoadDialog()
        {
            LogHelper.Debug("dbg", "RotateWait::Try to find Wait");
            var dialogTag = SimpleDialogFragment.FindFragmentTagByExtraTag(
                this,
                (extTagKey, extTag) =>
                extTagKey == WebViewManager.PleaseWaitPageLoadDialogExtTagKey);
            if (dialogTag != null)
            {
                LogHelper.Debug("dbg", "RotateWait::Fount Wait, and close it");
                SimpleDialogFragment.DismissDialog(this, dialogTag);
            }
            LogHelper.Debug("dbg", "RotateWait::Wait should be closed here.");
        }

        public void ShowLegalDefinePopup(string searchWord, Rect searchWordPos)
        {
            legalDefinePopup.ShowAtLocation(frgContainer, searchWordPos, searchWord);
        }

        private void OnUserDismissLegalDefinePopup()
        {
            GetMainFragment().OnUserDismissLegalDefinePopup();
        }

        public void OnTagListUpdated()
        {
            GetMainFragment().Refresh();
        }
    }
}

