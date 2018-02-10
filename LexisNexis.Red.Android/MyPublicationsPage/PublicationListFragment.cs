using System;
using System.Collections.Generic;

using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Fragment = Android.Support.V4.App.Fragment;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.AlertDialogUtility;
using System.Threading.Tasks;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Droid.App;
using System.Threading;
using LexisNexis.Red.Droid.Widget.SpinnerUtility;
using LexisNexis.Red.Droid.RecentHistory;
using Android.Support.V4.App;
using LexisNexis.Red.Common.HelpClass;
using Android.Content;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.Common.Entity;
using Newtonsoft.Json;
namespace LexisNexis.Red.Droid.MyPublicationsPage
{
    public class PublicationListFragment : Fragment, ISimpleDialogListener
    {
        public const string FragmentTag = "PublicationListFragment";

        public static readonly string Title = MainApp.ThisApp.Resources.GetString(Resource.String.MyPub_TitleBarTab_Publications);

        private const int FILTER_ALL = 0;
        private const int FILTER_LOAN = 1;
        private const int FILTER_SUBSCRIPTION = 2;

        private Spinner publicationfilter;
        private RecyclerView rcPublicationList;
        private LinearLayoutManager publistLayoutManager;
        private PublicationsAdaptor prcAdaptor;
        private TextView tvNoTitleMessage;
        private TextView tvNoHistoryMessage;
        //private RecyclerView rcHistoryList;
        private LinearLayoutManager historylistLayoutManager;
        private HistoryListAdaptor hrcAdaptor;

        private LinearLayout llHistoryListContainer;

        public static PublicationListFragment NewInstance()
        {
            return new PublicationListFragment();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ((MyPublicationsActivity)Activity).PublicationListFragment = this;
            Log.Info("dbg", "set PublicationListFragment.");

            var v = inflater.Inflate(Resource.Layout.publicationlist_fragment, container, false);

            tvNoTitleMessage = v.FindViewById<TextView>(Resource.Id.tvNoTitleMessage);
            tvNoHistoryMessage = v.FindViewById<TextView>(Resource.Id.tvNoHistoryMessage);

            publicationfilter = v.FindViewById<Spinner>(Resource.Id.publicationfilter);
            ArrayAdapter aa = ArrayAdapter.CreateFromResource(Activity, Resource.Array.PublicationFilter, Android.Resource.Layout.SimpleSpinnerItem);
            aa.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            publicationfilter.Adapter = aa;
            publicationfilter.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);

            rcPublicationList = v.FindViewById<RecyclerView>(Resource.Id.rcPublicationList);
            publistLayoutManager = new LinearLayoutManager(Activity);
            publistLayoutManager.Orientation = LinearLayoutManager.Horizontal;
            rcPublicationList.SetLayoutManager(publistLayoutManager);
            prcAdaptor = new PublicationsAdaptor(Activity);
            prcAdaptor.HasStableIds = true;
            rcPublicationList.SetAdapter(prcAdaptor);
            rcPublicationList.GetItemAnimator().SupportsChangeAnimations = false;

            llHistoryListContainer = v.FindViewById<LinearLayout>(Resource.Id.llHistoryListContainer);
            llHistoryListContainer.Visibility = ViewStates.Invisible;

            bool originalyEmptyPublicationList = DataCache.INSTATNCE.PublicationManager.IsPublicationListEmpty();
            //tvNoTitleMessage.Visibility = originalyEmptyPublicationList ? ViewStates.Visible : ViewStates.Gone;
            tvNoTitleMessage.Visibility = ViewStates.Gone;

            // Only when truely new create view, the wait dialog should be poped up.
            // Rotation of screen will not popup wait dialog.
            bool newCreated = savedInstanceState == null;

            GetPublicationListOnline(originalyEmptyPublicationList && newCreated);

            return v;
        }

        public override void OnResume()
        {
            base.OnResume();
            ((MyPublicationsActivity)Activity).PublicationListFragment = this;

            //SetFilter();
            /*
			hrcAdaptor.RefreshHisoryList();
			tvNoHistoryMessage.Visibility = hrcAdaptor.ItemCount == 0 ? ViewStates.Visible : ViewStates.Gone;
			//*/
            UpdateRecentHistory();

            bool showNoPubMessage = false;
            if (DataCache.INSTATNCE.PublicationManager.PublicationList.Count == 0)
            {
                var runningTask = AsyncTaskManager.INSTATNCE.FindTaskByType(typeof(Task<OnlinePublicationResult>));
                if (runningTask == null)
                {
                    showNoPubMessage = true;
                }
            }

            tvNoTitleMessage.Visibility = showNoPubMessage ? ViewStates.Visible : ViewStates.Gone;


            Task.Run(delegate
            {
                Thread.Sleep(100);
                Application.SynchronizationContext.Post(_ => SetFilter(), null);
            });
        }

        public override void OnStop()
        {
            ((MyPublicationsActivity)Activity).PublicationListFragment = null;
            base.OnStop();
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            SetFilter();
        }

        public bool IsPublicationFilterShowingDropDown()
        {
            return SpinnerHelper.IsShowingDropDown(publicationfilter);
        }

        public void ForceShowPublicationFilterDropDown()
        {
            if (!SpinnerHelper.IsShowingDropDown(publicationfilter))
            {
                publicationfilter.PerformClick();
            }
        }

        public void UpdatePublication(int bookId)
        {
            for (int i = 0; i < prcAdaptor.ItemCount; ++i)
            {
                var p = prcAdaptor.At(i);
                if (p.Value.BookId == bookId)
                {
                    prcAdaptor.NotifyItemChanged(i);
                    return;
                }
            }
        }

        public void UpdateRecentHistory()
        {
            llHistoryListContainer.RemoveAllViews();
            List<RecentHistoryItem> newHistoryList = null;
            if (GlobalAccess.Instance.CurrentUserInfo != null)
            {
                newHistoryList = PublicationContentUtil.Instance.GetRecentHistory();
            }

            if (newHistoryList == null || newHistoryList.Count == 0)
            {
                tvNoHistoryMessage.Visibility = ViewStates.Gone;
                llHistoryListContainer.Visibility = ViewStates.Gone;
            }
            else
            {
                tvNoHistoryMessage.Visibility = ViewStates.Gone;
                llHistoryListContainer.Visibility = ViewStates.Gone;
                for (int i = 0; i < newHistoryList.Count; ++i)
                {
                    var v = Activity.LayoutInflater.Inflate(
                        Resource.Layout.publications_historylist_item, null);
                    var vh = new HistoryListAdaptorViewHolder(v, OnRecentHistoryItemClick);
                    vh.Update(newHistoryList[i], i);
                    v.Tag = vh;
                    llHistoryListContainer.AddView(v);
                }
            }
        }

        public void SetFilter()
        {
            if (DataCache.INSTATNCE.PublicationManager.PublicationList == null || DataCache.INSTATNCE.PublicationManager.PublicationList.Count == 0)
            {
                prcAdaptor.setBookList(null);
                return;
            }

            int selectedFilterIndex = publicationfilter.SelectedItemPosition;
            List<ObjHolder<Publication>> filtedBooks = new List<ObjHolder<Publication>>();
            foreach (ObjHolder<Publication> publication in DataCache.INSTATNCE.PublicationManager.PublicationList)
            {
                if ((selectedFilterIndex == FILTER_LOAN && (!publication.Value.IsLoan))
                    || (selectedFilterIndex == FILTER_SUBSCRIPTION && publication.Value.IsLoan))
                {
                    continue;
                }

                filtedBooks.Add(publication);
            }

            prcAdaptor.setBookList(filtedBooks);
        }

        private void GetPublicationListOnline(bool showWaitDialog)
        {
            var runningTask = AsyncTaskManager.INSTATNCE.FindTaskByType(typeof(Task<OnlinePublicationResult>));
            if (runningTask == null)
            {
                var task = PublicationUtil.Instance.GetPublicationOnline();
                AsyncTaskManager.INSTATNCE.RegisterTask(task);

                string dialogTag = null;
                if (showWaitDialog)
                {
                    dialogTag = DialogGenerator.ShowWaitDialog(Activity.SupportFragmentManager);
                }

                Application.SynchronizationContext.Post(async delegate
                {
                    var result = await task;
                    if (!AsyncTaskManager.INSTATNCE.UnregisterTask(task))
                    {
                        // User has logged off
                        return;
                    }

                    if (result.RequestStatus == RequestStatusEnum.Success)
                    {
                        DataCache.INSTATNCE.PublicationManager.SetPublicationList(result.Publications);
                    }

                    AsyncUIOperationRepeater.INSTATNCE.SubmitAsyncUIOperation(
                        (IAsyncTaskActivity)Activity,
                        a =>
                    {
                        if (dialogTag != null)
                        {
                            SimpleDialogFragment.DismissDialog(((FragmentActivity)a), dialogTag);
                        }

                        if (DataCache.INSTATNCE.PublicationManager.PublicationList == null || DataCache.INSTATNCE.PublicationManager.PublicationList.Count == 0)
                        {
                            tvNoTitleMessage.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            tvNoTitleMessage.Visibility = ViewStates.Gone;
                        }

                        if (result.RequestStatus == RequestStatusEnum.Success)
                        {
                            SetFilter();
                        }
                    });
                }, null);
            }
        }

        private void OnRecentHistoryItemClick(RecentHistoryItem historyItem)
        {
            ((MyPublicationsActivity)Activity).OpenPublication(
                historyItem.BookId,
                AsyncHelpers.RunSync<int>(
                    () => PublicationContentUtil.Instance.GetTOCIDByDocId(
                        historyItem.BookId,
                        historyItem.DOCID)));
        }

        public bool OnSimpleDialogButtonClick(DialogButtonType buttonType, string fragmentTag, string extTagKey, string extTag)
        {
            switch (buttonType)
            {
                case DialogButtonType.Negative:
                    break;
                case DialogButtonType.Neutral:
                    break;
                case DialogButtonType.Positive:

                    HttpResponse taskresponse = new HttpResponse();
                    if (extTagKey == "StartTask")
                    {//启动任务
                        taskresponse = IoCContainer.Instance.Resolve<IDeliveryService>().StartTask(new Common.Entity.StartTaskRequest { keyValue = extTag, UserName = GlobalAccess.Instance.UserCredential.Email });
                    }
                    if (extTagKey == "EndTask")
                    {//结束任务
                        taskresponse = IoCContainer.Instance.Resolve<IDeliveryService>().EndTask(new Common.Entity.EndTaskRequest { result = "", keyValue = extTag });
                    }

                    if (taskresponse.IsSuccess)
                    {
                        var response = taskresponse.DeserializeObject<ServiceResponse>();

                        if (response.Success)
                        {
                            Toast.MakeText(Activity, response.Message, ToastLength.Short).Show();
                            bool originalyEmptyPublicationList = DataCache.INSTATNCE.PublicationManager.IsPublicationListEmpty();
                            //tvNoTitleMessage.Visibility = originalyEmptyPublicationList ? ViewStates.Visible : ViewStates.Gone;
                            tvNoTitleMessage.Visibility = ViewStates.Gone;

                            // Only when truely new create view, the wait dialog should be poped up.
                            // Rotation of screen will not popup wait dialog.
                            bool newCreated = false;
                            GetPublicationListOnline(originalyEmptyPublicationList && newCreated);
                        }
                        else
                        {//操作失败提示给用户
                            Toast.MakeText(Activity, response.Message, ToastLength.Short).Show();
                        }
                    }
                    return true;
                default:
                    break;
            }

            return false;
        }
    }
}

