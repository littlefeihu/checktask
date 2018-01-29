using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.App;
using LexisNexis.Red.Droid.Utility;
using Android.Util;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common.Business;
using Android.Text.Method;
using Android.Text;
using Android.Text.Style;
using LexisNexis.Red.Droid.TextStyle;
using System.Collections.Generic;
using Uri = Android.Net.Uri;
using Java.Net;
using Android.Provider;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class LegalDefinePopup
	{
		private static readonly Color RelatedKeywordColor = Color.ParseColor("#ed1c24");
		private readonly Activity host;
		private PopupWindow popup;

		private LinearLayout llContainer;
		private TextView tvSearchWord;
		private ImageView ivPreviousPage;
		private ImageView ivNextPage;
		private TextView tvDictWord;
		private ScrollView svContent;
		private TextView tvContent;
		private TextView tvDicInfo;
		private TextView tvSearchWeb;

		private int currentX;
		private int currentY;

		private Rect searchWordPos;

		private Action onDismiss;
		private bool systemDismiss;

		private LegalDefinitionItem legalDefineResult;
		private static Navigation nav;

		public LegalDefinePopup(Activity host, Action onDismiss)
		{
			this.host = host;
			this.onDismiss = onDismiss;
			systemDismiss = false;
		}

		public void ShowAtLocation(View parent, Rect searchWordPos, string searchWord)
		{
			if(nav == null || nav.First.ToLower() != searchWord.ToLower())
			{
				nav = new Navigation(searchWord);
			}

			if(legalDefineResult == null
				|| string.IsNullOrEmpty(legalDefineResult.SearchTerm)
				|| legalDefineResult.SearchTerm.ToLower() != nav.Current.ToLower())
			{
				legalDefineResult = DictionaryUtil.SearchDictionary(nav.Current,1);
				legalDefineResult.SearchTerm = nav.Current;
			}

			this.searchWordPos = searchWordPos;
			var pos = GetPos();
			currentX = pos.Item1;
			currentY = pos.Item2;

			if(popup == null)
			{
				var view = host.LayoutInflater.Inflate(Resource.Layout.contentpage_legaldefine_popup, null);

				llContainer = view.FindViewById<LinearLayout>(Resource.Id.llContainer);

				tvSearchWord = view.FindViewById<TextView>(Resource.Id.tvSearchWord);
				ivPreviousPage = view.FindViewById<ImageView>(Resource.Id.ivPreviousPage);
				ivNextPage = view.FindViewById<ImageView>(Resource.Id.ivNextPage);
				tvDictWord = view.FindViewById<TextView>(Resource.Id.tvDictWord);
				svContent = view.FindViewById<ScrollView>(Resource.Id.svContent);
				tvContent = view.FindViewById<TextView>(Resource.Id.tvContent);
				tvDicInfo = view.FindViewById<TextView>(Resource.Id.tvDicInfo);
				tvSearchWeb = view.FindViewById<TextView>(Resource.Id.tvSearchWeb);

				ivPreviousPage.Click += delegate
				{
					if(nav.HasPrevious())
					{
						nav.Previous();
						SearchDictionary();
					}
				};

				ivNextPage.Click += delegate
				{
					if(nav.HasNext())
					{
						nav.Next();
						SearchDictionary();
					}
				};

				tvSearchWeb.Click += delegate
				{
					Uri uri = Uri.Parse("https://www.google.com/search?q=" + URLEncoder.Encode(nav.First, "utf-8"));
					var intent = new Intent(Intent.ActionView, uri);
					intent.PutExtra(Browser.ExtraApplicationId, host.PackageName);
					host.StartActivity(intent);
				};

				tvContent.MovementMethod = LinkMovementMethod.Instance;

				popup = new PopupWindow(
					view,
					ViewGroup.LayoutParams.WrapContent,
					ViewGroup.LayoutParams.WrapContent,
					true);
				popup.SetBackgroundDrawable(new ColorDrawable(Color.Rgb(255, 255, 255)));
				popup.OutsideTouchable = true;

				popup.SetOnDismissListener(new PopupDismissListener(this));

				//llContainer.SetOnTouchListener(new PopupOnTouchListener(this));
			}

			SetSearchWord();

			if(popup.IsShowing)
			{
				return;
			}

			//llContainer.ViewTreeObserver.AddOnGlobalLayoutListener(new PopupOnGlobalLayoutListener(this));
			popup.ShowAtLocation(parent, GravityFlags.NoGravity, pos.Item1, pos.Item2);
		}

		private class PopupOnGlobalLayoutListener: Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
		{
			private readonly LegalDefinePopup popup;
			public PopupOnGlobalLayoutListener(LegalDefinePopup popup)
			{
				this.popup = popup;
			}

			public void OnGlobalLayout()
			{
				popup.llContainer.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);
				var pos = popup.GetPos(/* popup.llContainer.Height */);
				popup.popup.Update(pos.Item1, pos.Item2, -1, -1);
			}
		}

		private class PopupOnTouchListener: Java.Lang.Object, View.IOnTouchListener
		{
			private readonly LegalDefinePopup popup;

			private float deltaX;
			private float deltaY;

			public PopupOnTouchListener(LegalDefinePopup popup)
			{
				this.popup = popup;
			}

			public bool OnTouch(View v, MotionEvent e)
			{
				if (e.Action == MotionEventActions.Down) {
					deltaX = popup.currentX - e.RawX;
					deltaY = popup.currentY - e.RawY;
				} else
					if (e.Action == MotionEventActions.Move) {
						popup.currentX = (int) (e.RawX + deltaX);
						popup.currentY = (int) (e.RawY + deltaY);
						popup.popup.Update(popup.currentX, popup.currentY, -1, -1);
					}

				return true;
			}
		}
			
		private class PopupDismissListener: Java.Lang.Object, PopupWindow.IOnDismissListener
		{
			private readonly LegalDefinePopup popup;

			public PopupDismissListener(LegalDefinePopup popup)
			{
				this.popup = popup;
			}

			public void OnDismiss()
			{
				if(!popup.systemDismiss
					&& popup.onDismiss != null)
				{
					nav = null;
					popup.onDismiss();
				}
			}
		}

		private Tuple<int /*top*/, int /*left*/> GetPos()
		{
			var displaymetrics = new DisplayMetrics();
			host.WindowManager.DefaultDisplay.GetMetrics(displaymetrics);
			int screenHeight = displaymetrics.HeightPixels;

			var leftPos = (searchWordPos.Left + searchWordPos.Right) / 2
				- (int)(MainApp.ThisApp.Resources.GetDimension(
					Resource.Dimension.contentpage_legaldefine_popup_width) / 2);
			
			var topPos = searchWordPos.Bottom + Conversion.Dp2Px(10);
			var popupHeight = (int)MainApp.ThisApp.Resources.GetDimension(
				Resource.Dimension.contentpage_legaldefine_popup_height);
			if(topPos + popupHeight > screenHeight)
			{
				topPos = searchWordPos.Top - popupHeight - Conversion.Dp2Px(15);
			}

			return new Tuple<int, int>(leftPos, topPos);
		}

		public bool IsShowing
		{
			get
			{
				return popup != null && popup.IsShowing;
			}
		}

		public void Dismiss(bool systemDismiss)
		{
			this.systemDismiss = systemDismiss;
			if(popup != null)
			{
				popup.Dismiss();
			}
		}

		private void SetSearchWord()
		{
			tvSearchWord.Text = nav.Current;
			tvDictWord.Text = " ";

			ivPreviousPage.SetImageResource(
				nav.HasPrevious()
				? Resource.Drawable.previous_page_activy
				: Resource.Drawable.previous_page_disable);

			ivNextPage.SetImageResource(
				nav.HasNext()
				? Resource.Drawable.next_page_activy
				: Resource.Drawable.next_page_disable);

			if(legalDefineResult.ContextualDefinitions == null
				|| legalDefineResult.ContextualDefinitions.Count == 0)
			{
				tvContent.Text = MainApp.ThisApp.Resources.GetString(
					Resource.String.LegalDefine_NotFound_Message);
			}
			else
			{
				tvDictWord.Text = legalDefineResult.ContextualDefinitions[0].Term;
				tvContent.TextFormatted = BuildDefinition();
			}

			tvDicInfo.Text = legalDefineResult.DictionaryVersionText;

			svContent.ScrollTo(0, 0);
		}

		private ISpannable BuildDefinition()
		{
			var ssb = new SpannableStringBuilder();
			for(int i = 0; i < legalDefineResult.ContextualDefinitions.Count; ++i)
			{
				var d = legalDefineResult.ContextualDefinitions[i];

				var defineStart = ssb.Length();
				if(!string.IsNullOrEmpty(d.Context))
				{
					SpannableStringBuilderHelper.Appand(
						ssb,
						d.Context,
						new StyleSpan(TypefaceStyle.BoldItalic),
						SpanTypes.ExclusiveExclusive);
					ssb.Append(" ");
				}

				if(!string.IsNullOrEmpty(d.DefinitionHtml))
				{
					ssb.Append(Html.FromHtml(d.DefinitionHtml));
				}

				if(ssb.Length() > defineStart)
				{
					ssb.Append("\n");
				}

				if(d.AllRelatedKeywords.Count > 0)
				{
					ssb.Append(
						ssb.Length() > 0
						? MainApp.ThisApp.Resources.GetString(Resource.String.LegalDefine_SeeAlso)
						: MainApp.ThisApp.Resources.GetString(Resource.String.LegalDefine_See));
					ssb.Append(" ");

					for(int j = 0; j < d.AllRelatedKeywords.Count; ++j)
					{
						if(j > 0)
						{
							ssb.Append("; ");
						}

						SpannableStringBuilderHelper.Appand(
							ssb,
							d.AllRelatedKeywords[j].Term,
							new NoUnderlineURLSpan(
								d.AllRelatedKeywords[j].Term,
								RelatedKeywordColor,
								onRelatedKeywordClicked),
							SpanTypes.ExclusiveExclusive);
					}

					ssb.Append(".");
				}

				if(i != legalDefineResult.ContextualDefinitions.Count - 1)
				{
					ssb.Append("\n\n");
				}
			}

			return ssb;
		}

		private void onRelatedKeywordClicked(string relatedKeyword)
		{
			nav.Add(relatedKeyword);
			SearchDictionary();
		}

		private void SearchDictionary()
		{
			legalDefineResult = DictionaryUtil.SearchDictionary(nav.Current,1);
			legalDefineResult.SearchTerm = nav.Current;
			SetSearchWord();
		}

		private class Navigation
		{
			private readonly List<string> navigationHistory = new List<string>();
			private int currentIndex = 0;

			public Navigation(string firstSearchWord)
			{
				navigationHistory.Add(firstSearchWord);
			}

			public string Current
			{
				get
				{
					return navigationHistory[currentIndex];
				}
			}

			public bool HasPrevious()
			{
				return currentIndex > 0;
			}

			public bool HasNext()
			{
				return currentIndex < navigationHistory.Count - 1;
			}

			public string First
			{
				get
				{
					return navigationHistory[0];
				}
			}

			public string Previous()
			{
				if(!HasPrevious())
				{
					throw new IndexOutOfRangeException("Legal define can't navigate back.");
				}

				currentIndex--;

				return Current;
			}

			public string Next()
			{
				if(!HasNext())
				{
					throw new IndexOutOfRangeException("Legal define can't navigate forward.");
				}

				currentIndex++;

				return Current;
			}

			public void Add(string newSearchWord)
			{
				if(currentIndex < navigationHistory.Count - 1)
				{
					navigationHistory.RemoveRange(currentIndex + 1, navigationHistory.Count - 1 - currentIndex);
				}

				navigationHistory.Add(newSearchWord);
				currentIndex = navigationHistory.Count - 1;
			}
		}
	}
}

