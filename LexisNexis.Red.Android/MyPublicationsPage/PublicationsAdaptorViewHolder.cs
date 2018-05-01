using System;
using Android.Support.V7.Widget;
using Android.App;
using Android.Widget;
using Android.Views;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.AlertDialogUtility;
using Android.Content;
using System.Threading.Tasks;
using System.Threading;
using LexisNexis.Red.Droid.Utility;
using System.Globalization;
using LexisNexis.Red.Droid.DetailInfoModal;
using LexisNexis.Red.Droid.SettingsBoardPage;

namespace LexisNexis.Red.Droid.MyPublicationsPage
{
    public class PublicationsAdaptorViewHolder : RecyclerView.ViewHolder
    {
        private readonly LinearLayout llPublicationItemBackground;

        private PublicationCoverViewHolder coverHolder;

        private readonly TextView tvPubInfo_1stLine;
        private readonly TextView tvPubInfo_2ndLine;
        private readonly ImageView imgPubInfo_1stLine;
        private readonly ImageView imgPubInfo_2ndLine;
        private readonly TextView tvTaskStatus;
        private readonly PublicationsAdaptor adaptor;
        private ObjHolder<Publication> publication;

        public ObjHolder<Publication> Publication
        {
            get
            {
                return publication;
            }

            set
            {
                publication = value;
                coverHolder.Publication = publication;
            }
        }

        public PublicationsAdaptorViewHolder(View v, PublicationsAdaptor adaptor) : base(v)
        {
            this.adaptor = adaptor;

            llPublicationItemBackground = v.FindViewById<LinearLayout>(Resource.Id.llPublicationItemBackground);

            coverHolder = new PublicationCoverViewHolder(v);

            tvPubInfo_1stLine = v.FindViewById<TextView>(Resource.Id.tvPubInfo_1stLine);
            tvPubInfo_2ndLine = v.FindViewById<TextView>(Resource.Id.tvPubInfo_2ndLine);


            //tvTaskStatus = v.FindViewById<TextView>(Resource.Id.tvTaskStatus);
            imgPubInfo_1stLine = v.FindViewById<ImageView>(Resource.Id.imgPubInfo_1stLine);
            imgPubInfo_2ndLine = v.FindViewById<ImageView>(Resource.Id.imgPubInfo_2ndLine);

            imgPubInfo_1stLine.SetOnClickListener(new InfoButtonClickListener(this));
            imgPubInfo_2ndLine.SetOnClickListener(new DownloadButtonClickListener(this));

            llPublicationItemBackground.SetOnClickListener(new PublicationItemClickListener(this));
        }
        /// <summary>
        /// 打开任务
        /// </summary>
        private class PublicationItemClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly PublicationsAdaptorViewHolder vh;

            public PublicationItemClickListener(PublicationsAdaptorViewHolder vh)
            {
                this.vh = vh;
            }

            public void OnClick(View v)
            {
                if (vh.publication.Value.PublicationStatus == PublicationStatusEnum.Downloaded)
                {
                    //打开任务  进入任务巡查界面
                    Intent intent = new Intent(vh.adaptor.Activity, typeof(Activity1));
                    intent.PutExtra(SettingsBoardActivity.FunctionKey, vh.publication.Value.Name);
                    intent.PutExtra("taskid", vh.publication.Value.DpsiCode);
                    vh.adaptor.Activity.StartActivity(intent);
                }
                else
                {
                    Toast.MakeText(vh.adaptor.Activity, "请先启动任务", ToastLength.Short).Show();
                }
            }
        }

        private class InfoButtonClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly PublicationsAdaptorViewHolder vh;

            public InfoButtonClickListener(PublicationsAdaptorViewHolder vh)
            {
                this.vh = vh;
            }

            public void OnClick(View v)
            {
                PublicationDetailInfoFragment.NewInstance(vh.publication.Value.BookId)
                    .Show(vh.adaptor.Activity.SupportFragmentManager);
            }
        }

        private class DownloadButtonClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly PublicationsAdaptorViewHolder vh;

            public DownloadButtonClickListener(PublicationsAdaptorViewHolder vh)
            {
                this.vh = vh;
            }
            /// <summary>
            /// 启动和结束任务
            /// </summary>
            /// <param name="v"></param>
            public void OnClick(View v)
            {
                if (vh.publication.Value.PublicationStatus == PublicationStatusEnum.Downloaded)
                {
                    SimpleDialogFragment.Create(new SimpleDialogProvider
                    {
                        TitleResId = Resource.String.StartTask_Title,
                        MessageResId = Resource.String.EndTaskConfirm_Message,
                        PositiveButtonResId = Resource.String.Confirm,
                        NegativeButtonResId = Resource.String.Cancel,
                        ExtTagKey = "EndTask",
                        ExtTag = vh.publication.Value.DpsiCode.ToString(),
                        CanceledOnTouchOutside = false,
                    }).Show(vh.adaptor.Activity.SupportFragmentManager);
                }
                else
                {
                    SimpleDialogFragment.Create(new SimpleDialogProvider
                    {
                        TitleResId = Resource.String.StartTask_Title,
                        MessageResId = Resource.String.StartTaskConfirm_Message,
                        PositiveButtonResId = Resource.String.Confirm,
                        NegativeButtonResId = Resource.String.Cancel,
                        ExtTagKey = "StartTask",
                        ExtTag = vh.publication.Value.DpsiCode.ToString(),
                        CanceledOnTouchOutside = false,
                    }).Show(vh.adaptor.Activity.SupportFragmentManager);

                }


            }

        }

        public void UpdateWholePublication()
        {
            coverHolder.Update();
            UpdatePublicationStatus();
        }

        public void UpdatePublicationStatus()
        {
            LogHelper.Debug("dbg", "UpdatePublicationStatus[" + Publication.Value.BookId + "]");

            // A publication with downloaded status should be always shown as Download Successfully.
            // In few case, the publication's status is inconsistent with the download status, due to unknown reason.
            // So add [Publication.Value.PublicationStatus != PublicationStatusEnum.Downloaded] to fix:
            // http://wiki.lexiscn.com/issues/12947
            if (DataCache.INSTATNCE.PublicationManager.IsDownloadFailed(Publication.Value)
                && Publication.Value.PublicationStatus != PublicationStatusEnum.Downloaded)
            {
                LogHelper.Debug("dbg", "IsDownloadFailed");
                switch (Publication.Value.PublicationStatus)
                {
                    case PublicationStatusEnum.RequireUpdate:
                        {
                            // Require update
                            tvPubInfo_1stLine.Text = MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_Upload_Failed);
                            imgPubInfo_1stLine.SetImageResource(Resource.Drawable.mypub_info);

                            tvPubInfo_2ndLine.Text = String.Format(
                                MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_CurrencyDate),
                                Publication.Value.CurrencyDate.Value.ToString("dd MMM yyyy"));
                        }
                        break;
                    case PublicationStatusEnum.NotDownloaded:
                        {
                            // NeedDownload
                            tvPubInfo_1stLine.Text = MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_Download_Failed);
                            imgPubInfo_1stLine.SetImageResource(Resource.Drawable.mypub_info_disable);

                            tvPubInfo_2ndLine.Text = Conversion.Byte2ReadableString(Publication.Value.Size);
                        }
                        break;
                    default:
                        throw new InvalidProgramException("Unknown publication status.");
                }

                imgPubInfo_2ndLine.Visibility = ViewStates.Visible;
                imgPubInfo_2ndLine.SetImageResource(Resource.Drawable.mypub_downloadupdate_failed);
            }
            else if (DataCache.INSTATNCE.PublicationManager.IsDownloading(Publication.Value))
            {
                // Downloading
                tvPubInfo_1stLine.Text = MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_Downloading);
                tvPubInfo_2ndLine.Text = Conversion.Byte2ReadableString(Publication.Value.Size);

                imgPubInfo_1stLine.SetImageResource(Resource.Drawable.mypub_info_disable);
                imgPubInfo_2ndLine.Visibility = ViewStates.Visible;
                imgPubInfo_2ndLine.SetImageResource(Resource.Drawable.mypub_download_progress);
                UpdateProgressBar();
            }
            else
            {
                switch (Publication.Value.PublicationStatus)
                {
                    case PublicationStatusEnum.RequireUpdate:
                        {
                            // Require update
                            tvPubInfo_2ndLine.Text = String.Format(
                                MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_CurrencyDate),
                                Publication.Value.CurrencyDate.Value.ToString("dd MMM yyyy"));

                            imgPubInfo_1stLine.SetImageResource(Resource.Drawable.mypub_info);

                            if (Publication.Value.DaysRemaining < 0)
                            {
                                // Expired
                                tvPubInfo_1stLine.Text = MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_Expired);
                                imgPubInfo_2ndLine.Visibility = ViewStates.Visible;
                                imgPubInfo_2ndLine.SetImageResource(Resource.Drawable.mypub_expired);
                            }
                            else
                            {
                                // Not expired
                                tvPubInfo_1stLine.Text = Publication.Value.UpdateCount == 1
                                    ? String.Format(MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_UpdateAvailable), Publication.Value.UpdateCount)
                                    : String.Format(MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_UpdatesAvailable), Publication.Value.UpdateCount);

                                imgPubInfo_2ndLine.Visibility = ViewStates.Visible;
                                imgPubInfo_2ndLine.SetImageResource(Resource.Drawable.mypub_update_available);
                            }
                        }
                        break;
                    case PublicationStatusEnum.NotDownloaded:
                        {
                            // NeedDownload
                            //tvPubInfo_1stLine.Text = MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_Download);
                            tvPubInfo_1stLine.Text = coverHolder.Publication.Value.Name;

                            // tvPubInfo_2ndLine.Text = Conversion.Byte2ReadableString(Publication.Value.Size);
                            tvPubInfo_2ndLine.Text = coverHolder.Publication.Value.Author;
                            imgPubInfo_1stLine.SetImageResource(Resource.Drawable.mypub_info_disable);
                            imgPubInfo_2ndLine.Visibility = ViewStates.Visible;
                            imgPubInfo_2ndLine.SetImageResource(Resource.Drawable.mypub_download);
                        }
                        break;
                    case PublicationStatusEnum.Downloaded:
                        {
                            // Download finished
                            tvPubInfo_2ndLine.Text = String.Format(
                                MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_CurrencyDate),
                                Publication.Value.CurrencyDate.HasValue
                                ? Publication.Value.CurrencyDate.Value.ToString("dd MMM yyyy")
                                : Publication.Value.LastUpdatedDate.Value.ToString("dd MMM yyyy"));

                            imgPubInfo_1stLine.SetImageResource(Resource.Drawable.mypub_info);

                            if (Publication.Value.DaysRemaining < 0)
                            {
                                // Expired
                                tvPubInfo_1stLine.Text = MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_Expired);
                                imgPubInfo_2ndLine.Visibility = ViewStates.Visible;
                                imgPubInfo_2ndLine.SetImageResource(Resource.Drawable.mypub_expired);
                            }
                            else
                            {
                                // Not expired
                                tvPubInfo_1stLine.Text = MainApp.ThisApp.Resources.GetString(Resource.String.PubCover_UpToDate);
                                imgPubInfo_2ndLine.Visibility = ViewStates.Invisible;
                            }
                        }
                        break;
                    default:
                        throw new InvalidProgramException("Unknown publication status.");
                }
                tvPubInfo_1stLine.Text = coverHolder.Publication.Value.Name;
                tvPubInfo_2ndLine.Text = coverHolder.Publication.Value.Author;
            }
        }

        public void UpdateProgressBar()
        {
            imgPubInfo_2ndLine.Drawable.SetLevel(DataCache.INSTATNCE.PublicationManager.GetDownloadProgress(Publication.Value) * 100);
        }
    }
}

