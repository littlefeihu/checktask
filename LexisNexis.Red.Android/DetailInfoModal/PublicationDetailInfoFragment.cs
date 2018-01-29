
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.TextStyle;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Droid.Widget.ExpandableTextView;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Android.Text.Method;
using Android.Text.Util;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Droid.MyPublicationsPage;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Droid.DetailInfoModal
{
    public class PublicationDetailInfoFragment : DialogFragment, ISimpleExpandableTextViewListerner, ISimpleDialogListener
    {
        public const string PublicationDetailInfoFragmentKey = "PublicationDetailInfoFragment";
        private const string PublicationIdKey = "PublicationId";
        private const string ShowOpenKey = "ShowOpen";
        private const string IsLastTimeExpireInfoPartial = "IsLastTimeExpireInfoPartial";
        private const string IsLastDescriptionPartial = "IsLastDescriptionPartial";
        private const string IsLastCasesPartial = "IsLastCasesPartial";
        public static PublicationDetailInfoFragment NewInstance(int publicationId, bool showOpen = true)
        {
            Bundle b = new Bundle();
            b.PutInt(PublicationIdKey, publicationId);
            b.PutBoolean(ShowOpenKey, showOpen);
            PublicationDetailInfoFragment fragment = new PublicationDetailInfoFragment();
            fragment.Arguments = b;
            return fragment;
        }


        private TextView tvTitle;
        private TextView tvAuthor;
        private TextView tvDescription;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);

            var detailInfo = inflater.Inflate(Resource.Layout.publication_detail_info, container);

            tvTitle = detailInfo.FindViewById<TextView>(Resource.Id.tvTitle);
            tvAuthor = detailInfo.FindViewById<TextView>(Resource.Id.tvAuthor);
            tvDescription = detailInfo.FindViewById<TextView>(Resource.Id.tvDescription);

            return detailInfo;
        }


        public void UpdateWholeInfo(int bookId)
        {
            int argumentBookId = Arguments.GetInt(PublicationIdKey, -1);
            if (argumentBookId < 0)
            {
                throw new InvalidOperationException("Unable to get publication id.");
            }

            if (bookId >= 0 && bookId != argumentBookId)
            {
                return;
            }
            var pub = DataCache.INSTATNCE.PublicationManager.GetPublication(argumentBookId);

            tvTitle.Text = "任务名称：" + pub.Value.Name;
            tvAuthor.Text = "负责人：" + pub.Value.Author;
            tvDescription.Text = "任务说明：" + pub.Value.Description;

        }
        public void Show(FragmentManager fm)
        {
            Show(fm, PublicationDetailInfoFragmentKey);
        }
        public override void OnResume()
        {
            base.OnResume();
            Dialog.Window.SetLayout(700, 900);
            UpdateWholeInfo(-1);
        }

        public void LineCountDetected(TextView tv)
        {

        }

        public bool OnSimpleDialogButtonClick(DialogButtonType buttonType, string fragmentTag, string extTagKey, string extTag)
        {
            return true;
        }
    }
}

