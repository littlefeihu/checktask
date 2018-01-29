
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Droid.Widget.StatusBar;
using LexisNexis.Red.Droid.Widget;
using Android.Support.V7.Widget;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Droid.Async;
using Newtonsoft.Json;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Business;
using Android.Views.InputMethods;
using System.Threading.Tasks;
using System.Threading;
using Android.Text;
using LexisNexis.Red.Droid.Widget.Layout;
using Android.Graphics;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.ContentPage
{
	[Activity(Label = "ContentSearchActivity")]			
	public class ContentSearchActivity : Activity
	{
		private const string SearchCacheFile = "Search.{0}.cache";
		public const string CacheCatagory = "Content";
		public const int RequestCodeSearch = 1001;
		public const string SessionId = "SessionId";
		public const string BookId = "BookId";
		public const string PublicationTitle = "PublicationTitle";

		public const string ResultTocId = "ResultTocId";
		public const string ResultLastSearchKeywords = "ResultLastSearchKeywords";
		public const string HeadType = "HeadType";
		public const string HeadSequence  = "HeadSequence";

		private InterceptTouchLinearLayout llBackground;
		private ImageView ivBack;
		private EditText etSearchText;
		private ImageView ivRemoveText;
		private ImageView ivOverflowMenu;
		private RecyclerView rcSearchResult;
		private TextView tvNoSearchResult;

		private LinearLayoutManager searchResultListLayoutManager;
		private SearchResultListAdaptor hrcAdaptor;

		private ContentSearchFilterPopup filterPopup;

		private string searchSessionId;
		private ContentSearchStatus status;

		public ContentSearchStatus Status
		{
			get
			{
				return status;
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			searchSessionId = Intent.GetStringExtra(SessionId);
			if(string.IsNullOrEmpty(searchSessionId))
			{
				throw new InvalidProgramException("Session Id should be provided.");
			}

			var searchBookId = Intent.GetIntExtra(BookId, -1);
			if(searchBookId < 0)
			{
				throw new InvalidProgramException("Book Id should be provided.");
			}

			var searchPublicationTitle = Intent.GetStringExtra(PublicationTitle);
			if(string.IsNullOrEmpty(searchPublicationTitle))
			{
				throw new InvalidProgramException("Publication Title should be provided.");
			}

			var tocId = DataCache.INSTATNCE.Toc.CurrentTOCNode == null
				? DataCache.INSTATNCE.Toc.GetFirstPage().ID
				: DataCache.INSTATNCE.Toc.CurrentTOCNode.ID;

			var searchCache = FileCacheHelper.ReadCacheFile(
				CacheCatagory, string.Format(SearchCacheFile, searchSessionId));
			if(!string.IsNullOrEmpty(searchCache))
			{
				status = JsonConvert.DeserializeObject<ContentSearchStatus>(searchCache);
			}
			else
			{
				status = new ContentSearchStatus(
					searchBookId,
					searchPublicationTitle,
					tocId);
			}

			if(searchBookId != status.BookId)
			{
				status = new ContentSearchStatus(
					searchBookId,
					searchPublicationTitle,
					tocId);
			}

			if(tocId != status.TocId)
			{
				status.TocId = tocId;
			}

			StatusBarTintHelper.SetStatusBarColor(this);

			SetContentView(Resource.Layout.contentpage_search_activity);

			FindViewById<LinearLayout>(Resource.Id.llStatusBarStub).LayoutParameters =
				new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, StatusBarTintHelper.GetStatusBarHeight());

			var toobarHeight = ToolbarHelper.GetToolbarHeight();
			if(toobarHeight > 0)
			{
				FindViewById<LinearLayout>(Resource.Id.llFakeToobar).LayoutParameters =
					new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, toobarHeight);
			}

			llBackground = FindViewById<InterceptTouchLinearLayout>(Resource.Id.llBackground);
			ivBack = FindViewById<ImageView>(Resource.Id.ivBack);
			etSearchText = FindViewById<EditText>(Resource.Id.etSearchText);
			ivRemoveText = FindViewById<ImageView>(Resource.Id.ivRemoveText);
			ivOverflowMenu = FindViewById<ImageView>(Resource.Id.ivOverflowMenu);
			tvNoSearchResult = FindViewById<TextView>(Resource.Id.tvNoSearchResult);

			filterPopup = new ContentSearchFilterPopup(this, OnFilterClicked);

			etSearchText.Parent.RequestDisallowInterceptTouchEvent(true);
			llBackground.SetOnInterceptTouchHanlder(OnBackgroundInterceptTouchEvent);

			ivBack.Click += delegate
			{
				SaveStatusAndFinish();
			};

			etSearchText.Hint = string.Format(
				MainApp.ThisApp.Resources.GetString(
					Resource.String.ContentSearch_EditText_Hint),
					status.PublicationTitle);
			etSearchText.Click += delegate
			{
				if(etSearchText.HasFocus)
				{
					return;
				}

				etSearchText.InputType = InputTypes.ClassText;
				etSearchText.FocusableInTouchMode = true;
				etSearchText.RequestFocus();
				var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
				imm.ShowSoftInput(etSearchText, ShowFlags.Forced);
			};

			etSearchText.TextChanged += delegate
			{
				UpdateRemoveTextIconAndOverFlowMenuIcon();
			};

			etSearchText.OnFocusChangeListener = new EditTextFocusChangeListener(this);

			etSearchText.SetOnEditorActionListener(new EditorActionListener(this));

			etSearchText.Text = status.KeywordInEditText;
			var text = etSearchText.EditableText;
			Selection.SetSelection(text, text.Length());

			ivRemoveText.Visibility = ViewStates.Gone;
			ivOverflowMenu.Visibility = ViewStates.Gone;

			ivRemoveText.Click += delegate
			{
				etSearchText.Text = string.Empty;
				ivRemoveText.Visibility = ViewStates.Invisible;
			};

			ivOverflowMenu.Click += delegate
			{
				PopupSearchFilter();
			};

			rcSearchResult = FindViewById<RecyclerView>(Resource.Id.rcSearchResult);
			searchResultListLayoutManager = new LinearLayoutManager (this);
			searchResultListLayoutManager.Orientation = LinearLayoutManager.Vertical;
			rcSearchResult.SetLayoutManager (searchResultListLayoutManager);
			hrcAdaptor = new SearchResultListAdaptor(
				this,
				OnResultItemClick,
				OnUpdateResult);
			rcSearchResult.SetAdapter(hrcAdaptor);

			SetResult(Result.Canceled);
		}

		protected override void OnStop()
		{
			filterPopup.Dismiss();
			base.OnStop();
		}

		private bool OnBackgroundInterceptTouchEvent(MotionEvent ev)
		{
			if(ev.Action == MotionEventActions.Down)
			{
				if(!IsTarget(etSearchText, ev)
					&& !IsTarget(ivRemoveText, ev)
					&& etSearchText.HasFocus)
				{
					var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
					imm.HideSoftInputFromWindow(etSearchText.WindowToken, 0);

					etSearchText.InputType = InputTypes.Null;
					etSearchText.FocusableInTouchMode = false;
					etSearchText.ClearFocus();

					UpdateRemoveTextIconAndOverFlowMenuIcon();
				}
			}

			return false;
		}

		private static bool IsTarget(View v, MotionEvent ev)
		{
			if(v.Visibility != ViewStates.Visible)
			{
				return false;
			}

			var rect = new Rect();
			v.GetGlobalVisibleRect(rect);
			return rect.Contains((int)ev.GetX(), (int)ev.GetY());
		}

		protected override void OnResume()
		{
			base.OnResume();

			if(status != null)
			{
				if(status.PopupFilter)
				{
					Application.SynchronizationContext.Post(_ =>
					{
						filterPopup.ShowAsDropDown(
							ivOverflowMenu,
							Conversion.Dp2Px(-180),
							Conversion.Dp2Px(-35),
							status.FilterId);
					}, null);
				}

				/*
				if(etSearchText.Text != status.KeywordInEditText)
				{
					// Due to unknown reason, if do not delay set
					// the text of EditText, the app will crash.
					Task.Run(() => {
						Thread.Sleep(100);
						Application.SynchronizationContext.Post(_ =>
						{
							etSearchText.Text = status.KeywordInEditText;
							var text = etSearchText.EditableText;
							Selection.SetSelection(text, text.Length());
						}, null);
					});
				}
				*/

				if(!string.IsNullOrEmpty(status.LastSearchKeywords)
					&& DataCache.INSTATNCE.SearchResult == null)
				{
					DataCache.INSTATNCE.SearchResult = new SearchResultKeeper(
						status.BookId,
						status.TocId,
						etSearchText.Text,
						SearchUtil.Search(
							status.BookId,
							status.TocId,
							status.LastSearchKeywords));
				}
			}

			hrcAdaptor.UpdateSearchResult();
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			SaveStatus();
			base.OnSaveInstanceState(outState);
		}

		private class EditTextFocusChangeListener: Java.Lang.Object, EditText.IOnFocusChangeListener
		{
			private readonly ContentSearchActivity activity;

			public EditTextFocusChangeListener(ContentSearchActivity activity)
			{
				this.activity = activity;
			}

			public void OnFocusChange(View v, bool hasFocus)
			{
				activity.UpdateRemoveTextIconAndOverFlowMenuIcon();
			}
		}

		private class EditorActionListener: Java.Lang.Object, TextView.IOnEditorActionListener
		{
			private readonly ContentSearchActivity activity;

			public EditorActionListener(ContentSearchActivity activity)
			{
				this.activity = activity;
			}

			public bool OnEditorAction(
				TextView v,
				ImeAction actionId,
				KeyEvent e)
			{
				if(actionId != ImeAction.Search)
				{
					return false;
				}

				var imm = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
				imm.HideSoftInputFromWindow(activity.etSearchText.WindowToken, 0);

				string keyword = activity.etSearchText.Text;
				if(keyword.Length == 0)
				{
					Toast.MakeText(activity, "Please input keywords.", ToastLength.Short).Show();
					return true;
				}

				activity.status.LastSearchKeywords = activity.etSearchText.Text;
				DataCache.INSTATNCE.SearchResult = new SearchResultKeeper(
					activity.status.BookId,
					activity.status.TocId,
					activity.status.LastSearchKeywords,
					SearchUtil.Search(
						activity.status.BookId,
						activity.status.TocId,
						activity.status.LastSearchKeywords));

				activity.status.FilterId = Resource.Id.tvFilterAll;

				activity.hrcAdaptor.UpdateSearchResult();

				activity.etSearchText.InputType = InputTypes.Null;
				activity.etSearchText.FocusableInTouchMode = false;
				activity.etSearchText.ClearFocus();

				activity.UpdateRemoveTextIconAndOverFlowMenuIcon();

				return true;
			}
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back && e.RepeatCount == 0)
			{
				SaveStatusAndFinish();
				return true;
			}

			return base.OnKeyDown(keyCode, e);
		}

		private void SaveStatus()
		{
			status.PopupFilter = filterPopup.IsShowing;
			status.KeywordInEditText = etSearchText.Text;

			FileCacheHelper.SaveCacheFile(
				CacheCatagory,
				string.Format(SearchCacheFile, searchSessionId),
				JsonConvert.SerializeObject(status));
		}

		private void SaveStatusAndFinish()
		{
			SaveStatus();
			Finish();
		}

		private void OnFilterClicked(int id)
		{
			status.FilterId = id;
			hrcAdaptor.UpdateSearchResult();
		}

		private void PopupSearchFilter()
		{
			filterPopup.ShowAsDropDown(
				ivOverflowMenu,
				Conversion.Dp2Px(-180),
				Conversion.Dp2Px(-35),
				status.FilterId);
		}

		private void UpdateRemoveTextIconAndOverFlowMenuIcon()
		{
			if(etSearchText.HasFocus)
			{
				ivOverflowMenu.Visibility = ViewStates.Gone;
				ivRemoveText.Visibility = etSearchText.Text.Length > 0 ? ViewStates.Visible : ViewStates.Gone;
			}
			else
			{
				ivOverflowMenu.Visibility =
					DataCache.INSTATNCE.SearchResult != null && DataCache.INSTATNCE.SearchResult.Count > 0
						? ViewStates.Visible : ViewStates.Gone;
				ivRemoveText.Visibility = ViewStates.Gone;
			}
		}

		public static long StartTime;
		private void OnResultItemClick(SearchDisplayResult resultItem)
		{
			Intent.PutExtra(ResultTocId, resultItem.TocId);
			Intent.PutExtra(ResultLastSearchKeywords, Status.LastSearchKeywords);

			if(resultItem.isDocument)
			{
				Intent.PutExtra(HeadType, resultItem.HeadType.ToString());
				Intent.PutExtra(HeadSequence, resultItem.HeadSequence);
			}

			SetResult(Result.Ok, Intent);
			SaveStatusAndFinish();
			StartTime = DateTimeHelper.GetTimeStamp();
		}

		private void OnUpdateResult(bool hasResult)
		{
			tvNoSearchResult.Visibility = hasResult ? ViewStates.Gone : ViewStates.Visible;
		}
	}
}

