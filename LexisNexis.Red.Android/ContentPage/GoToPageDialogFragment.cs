
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
using Android.Views.InputMethods;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Entity;
using DialogFragment=Android.Support.V4.App.DialogFragment;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.WebViewUtility;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.Business;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class GoToPageDialogFragment : DialogFragment
	{
		private const string PageNumKey = "PageNum";

		private TextView tvPageNum;
		private Button btn0;
		private Button btn1;
		private Button btn2;
		private Button btn3;
		private Button btn4;
		private Button btn5;
		private Button btn6;
		private Button btn7;
		private Button btn8;
		private Button btn9;
		private ImageView ivDel;
		private Button btnDone;
		private RecyclerView rcSearchResult;
		private TextView tvNoResultMessage;

		private LinearLayoutManager searchResultListLayoutManager;
		private PboGoToResultListAdaptor hrcAdaptor;

		public static GoToPageDialogFragment NewInstance()
		{
			var b = new Bundle();
			b.PutString(PageNumKey, "");
			var fragment = new GoToPageDialogFragment();
			fragment.Arguments = b;
			return fragment;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
			Dialog.SetCanceledOnTouchOutside(true);
			var vwPboGoToPage = inflater.Inflate(Resource.Layout.contentpage_pbogotopage_popup, container);

			tvPageNum = vwPboGoToPage.FindViewById<TextView>(Resource.Id.tvPageNum);
			btn0 = vwPboGoToPage.FindViewById<Button>(Resource.Id.btn0);
			btn1 = vwPboGoToPage.FindViewById<Button>(Resource.Id.btn1);
			btn2 = vwPboGoToPage.FindViewById<Button>(Resource.Id.btn2);
			btn3 = vwPboGoToPage.FindViewById<Button>(Resource.Id.btn3);
			btn4 = vwPboGoToPage.FindViewById<Button>(Resource.Id.btn4);
			btn5 = vwPboGoToPage.FindViewById<Button>(Resource.Id.btn5);
			btn6 = vwPboGoToPage.FindViewById<Button>(Resource.Id.btn6);
			btn7 = vwPboGoToPage.FindViewById<Button>(Resource.Id.btn7);
			btn8 = vwPboGoToPage.FindViewById<Button>(Resource.Id.btn8);
			btn9 = vwPboGoToPage.FindViewById<Button>(Resource.Id.btn9);
			ivDel = vwPboGoToPage.FindViewById<ImageView>(Resource.Id.ivDel);
			btnDone = vwPboGoToPage.FindViewById<Button>(Resource.Id.btnDone);
			rcSearchResult = vwPboGoToPage.FindViewById<RecyclerView>(Resource.Id.rcSearchResult);
			tvNoResultMessage = vwPboGoToPage.FindViewById<TextView>(Resource.Id.tvNoResultMessage);

			tvPageNum.Text = string.Empty;

			btn0.Click += OnBtnClick;
			btn1.Click += OnBtnClick;
			btn2.Click += OnBtnClick;
			btn3.Click += OnBtnClick;
			btn4.Click += OnBtnClick;
			btn5.Click += OnBtnClick;
			btn6.Click += OnBtnClick;
			btn7.Click += OnBtnClick;
			btn8.Click += OnBtnClick;
			btn9.Click += OnBtnClick;
			ivDel.Click += OnBtnClick;
			btnDone.Click += OnBtnClick;

			searchResultListLayoutManager = new LinearLayoutManager (Activity);
			searchResultListLayoutManager.Orientation = LinearLayoutManager.Vertical;
			rcSearchResult.SetLayoutManager (searchResultListLayoutManager);
			hrcAdaptor = new PboGoToResultListAdaptor(
				this,
				OnResultItemClick);
			rcSearchResult.SetAdapter(hrcAdaptor);

			var pageNum = Arguments.GetString(PageNumKey);
			if(!string.IsNullOrEmpty(pageNum))
			{
				tvPageNum.Text = pageNum;
				OnPageNumChanged();
			}

			return vwPboGoToPage;
		}

		private void OnBtnClick(object sender, EventArgs e)
		{
			var btn = (View)sender;
			if(btn.Id == Resource.Id.ivDel)
			{
				if(tvPageNum.Text.Length == 0)
				{
					return;
				}

				tvPageNum.Text = tvPageNum.Text.Substring(0, tvPageNum.Text.Length - 1);
				OnPageNumChanged();
			}
			else if(btn.Id == Resource.Id.btnDone)
			{
				if(hrcAdaptor.ItemCount == 0)
				{
					return;
				}

				OnResultItemClick(hrcAdaptor.ResultList.Item1, hrcAdaptor.ResultList.Item2[0]);
				return;
			}
			else
			{
				if(tvPageNum.Text.Length >= 7)
				{
					return;
				}

				tvPageNum.Text += ((Button)btn).Text;
				OnPageNumChanged();
			}
		}

		private void OnPageNumChanged()
		{
			Arguments.PutString(PageNumKey, tvPageNum.Text);
			if(string.IsNullOrEmpty(tvPageNum.Text))
			{
				hrcAdaptor.ResultList = null;
				tvNoResultMessage.Text = MainApp.ThisApp.Resources.GetString(
					Resource.String.ContentPboGoTo_EmptyInputMessage);
				tvNoResultMessage.Visibility = ViewStates.Visible;
				return;
			}

			var bookId = NavigationManagerHelper.GetCurrentBookId();
			var pageNum = Int32.Parse(tvPageNum.Text);
			hrcAdaptor.ResultList = new Tuple<int, List<PageSearchItem>>(
				pageNum,
				AsyncHelpers.RunSync<List<PageSearchItem>>(
					() => PageSearchUtil.Instance.SeachByPageNum(bookId, pageNum)));

			if(hrcAdaptor.ItemCount == 0)
			{
				tvNoResultMessage.Text = MainApp.ThisApp.Resources.GetString(
					Resource.String.ContentPboGoTo_NotFoundMessage);
				tvNoResultMessage.Visibility = ViewStates.Visible;
			}
			else
			{
				tvNoResultMessage.Visibility = ViewStates.Gone;
			}
		}

		private void OnResultItemClick(int pageNum, PageSearchItem searchItem)
		{
			var record = DataCache.INSTATNCE.Toc.GetNavigationItem() as ContentBrowserRecord;
			if(record == null
				|| !NavigationManagerHelper.CompareActualTocId(record.TOCID, searchItem.TOCID)
				|| record.PageNum != pageNum)
			{
				NavigationManager.Instance.AddRecord(
					new ContentBrowserRecord(
						NavigationManagerHelper.GetCurrentBookId(),
						searchItem.TOCID,
						pageNum,
						0));
				//WebViewManager.Instance.ClearWebViewStatus(WebViewManager.WebViewType.Content);
			}
			else
			{
				NavigationManagerHelper.MoveForthAndSetCurrentIndex(record.RecordID);
				WebViewManager.Instance.ClearWebViewStatus(WebViewManager.WebViewType.Content);
				DataCache.INSTATNCE.Toc.ResetNavigationItem();
			}

			((ContentActivity)Activity).GetMainFragment().SwitchLogicalMainTab(ContentMainFragment.TabContents);
			((ContentActivity)Activity).GetMainFragment().Refresh();

			Dismiss();
		}
	}
}

