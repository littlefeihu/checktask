
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
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment=Android.Support.V4.App.Fragment;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Widget.StatusBar;
using Android.Graphics;
using Android.Graphics.Drawables;
using LexisNexis.Red.Droid.Utility;
using Android.Content.Res;
using Android.Views.InputMethods;
using Android.Text;

namespace LexisNexis.Red.Droid.AnnotationOrganiserPage
{
	[Activity(Label = "NewTagActivity")]			
	public class NewTagActivity : AppCompatActivity
	{
		private const string CheckedColorIndexKey = "CheckedColorIndexKey";
		private Toolbar toolbar;
		private LinearLayout llRootView;
		private LinearLayout llColorsContainer;
		private IMenuItem menuItemSave;
		private readonly static string[] TagColors = {"#ff0000", "#00ff00", "#0000ff", "#888888", "#ff0000", "#00ff00", "#0000ff", "#888888", "#ff0000", "#00ff00", "#0000ff", "#888888", };
		private int ColorsContainerWidth = 0;
		private int checkedColorIndex;

		private EditText etTagName;

		private class SelectColorItemViewTag
		{
			public string Color;
			public ImageView Icon;

			public SelectColorItemViewTag(string color, ImageView icon)
			{
				this.Color = color;
				this.Icon = icon;
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			if(savedInstanceState != null)
			{
				checkedColorIndex = savedInstanceState.GetInt(CheckedColorIndexKey, 0);
			}
			else
			{
				checkedColorIndex = 0;
			}

			StatusBarTintHelper.SetStatusBarColor(this);

			// Create your application here
			SetContentView(Resource.Layout.annotations_newtag_activity);

			llRootView = FindViewById<LinearLayout>(Resource.Id.llRootView);
			etTagName = FindViewById<EditText>(Resource.Id.etTagName);
			llColorsContainer = FindViewById<LinearLayout>(Resource.Id.llColorsContainer);

			llRootView.Click += delegate
			{
				HideSoftKeyboard();
			};

			etTagName.AddTextChangedListener(new TagNameWatcher(this));

			FindViewById<LinearLayout>(Resource.Id.llStatusBarStub).LayoutParameters =
				new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, StatusBarTintHelper.GetStatusBarHeight());

			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_actionbar);
			SetSupportActionBar(toolbar);

			toolbar.SetNavigationIcon(Resource.Drawable.cancel_icon);
			Title = MainApp.ThisApp.Resources.GetString(Resource.String.AnnotationNewTagPage_NewTag);
		}

		private class TagNameWatcher: Java.Lang.Object, ITextWatcher
		{
			private NewTagActivity hostActivity;

			public TagNameWatcher(NewTagActivity hostActivity)
			{
				this.hostActivity = hostActivity;
			}

			public void AfterTextChanged(IEditable s)
			{
			}

			public void BeforeTextChanged(Java.Lang.ICharSequence s, int start, int count, int after)
			{
			}

			public void OnTextChanged(Java.Lang.ICharSequence s, int start, int before, int count)
			{
				if(hostActivity.etTagName.Text.Length == 0)
				{
					hostActivity.menuItemSave.ActionView.FindViewById<TextView>(Resource.Id.tvSave).SetTextColor(Color.ParseColor("#89ffffff"));
				}
				else
				{
					hostActivity.menuItemSave.ActionView.FindViewById<TextView>(Resource.Id.tvSave).SetTextColor(Color.White);
				}
			}
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutInt(CheckedColorIndexKey, checkedColorIndex);
			base.OnSaveInstanceState(outState);
		}

		private static readonly int SelectColorItemWidth =
			(int)MainApp.ThisApp.Resources.GetDimension(Resource.Dimension.newtag_selectcolor_item_width);
		private static readonly int SelectColorItemHeight =
			(int)MainApp.ThisApp.Resources.GetDimension(Resource.Dimension.newtag_selectcolor_item_height);
		private List<View> selectColorItemList = new List<View>();
		public override void OnWindowFocusChanged(bool hasFocus)
		{
			base.OnWindowFocusChanged(hasFocus);

			if(ColorsContainerWidth == llColorsContainer.Width)
			{
				return;
			}

			ColorsContainerWidth = llColorsContainer.Width;
			llColorsContainer.RemoveAllViews();

			int currentLinePos = 0;
			LinearLayout currentLine = null;
			bool needCreateNewItem = (selectColorItemList.Count == 0);

			for(int i = 0; i < TagColors.Count(); ++i)
			{
				var color = TagColors[i];
				if(currentLine == null || currentLinePos + SelectColorItemWidth > ColorsContainerWidth)
				{
					currentLine = CreateNewColorsLine();
					currentLinePos = 0;
				}

				View selectColorItem = null;
				if(needCreateNewItem)
				{
					selectColorItem = LayoutInflater.Inflate(Resource.Layout.newtag_selectcolor_item, null);
					var flRoot = selectColorItem.FindViewById<View>(Resource.Id.flRoot);
					var iv = selectColorItem.FindViewById<ImageView>(Resource.Id.iv);
					flRoot.Click += OnSelectColorItemClicked;
					flRoot.Tag =
						new JavaObjWrapper<SelectColorItemViewTag>(
							new SelectColorItemViewTag(color, iv));
					iv.SetImageDrawable(
						GetSelectColorButton(Color.ParseColor(color), i == checkedColorIndex));
					selectColorItemList.Add(flRoot);
				}
				else
				{
					selectColorItem = selectColorItemList[i];
				}

				var lp = new LinearLayout.LayoutParams(SelectColorItemWidth, SelectColorItemHeight);
				
				currentLine.AddView(selectColorItem, lp);
				currentLinePos += SelectColorItemWidth;
			}
		}


		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.newtag_actionbar, menu);

			menuItemSave = menu.FindItem(Resource.Id.action_save);

			if(etTagName.Text.Length == 0)
			{
				menuItemSave.ActionView.FindViewById<TextView>(Resource.Id.tvSave).SetTextColor(Color.ParseColor("#89ffffff"));
			}
			else
			{
				menuItemSave.ActionView.FindViewById<TextView>(Resource.Id.tvSave).SetTextColor(Color.White);
			}
			menuItemSave.ActionView.FindViewById<View>(Resource.Id.flBackground).Click += delegate
			{
				Toast.MakeText(this, "Save", ToastLength.Short).Show();
			};

			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
			case Android.Resource.Id.Home:
				{
					Finish();
					return true;
				}
			default:
				return base.OnOptionsItemSelected(item);
			}
		}

		private void OnSelectColorItemClicked(object sender, EventArgs e)
		{
			var selectColorItem = sender as View;
			if(selectColorItem == null)
			{
				return;
			}

			var tag = selectColorItem.Tag as JavaObjWrapper<SelectColorItemViewTag>;
			if(tag == null)
			{
				return;
			}

			var itemIndex = selectColorItemList.IndexOf(selectColorItem);
			if(itemIndex == checkedColorIndex)
			{
				return;
			}

			var checkedItem = selectColorItemList[checkedColorIndex];
			var checkedItemTag = (JavaObjWrapper<SelectColorItemViewTag>)checkedItem.Tag;
			checkedItemTag.Value.Icon.SetImageDrawable(GetSelectColorButton(
				Color.ParseColor(checkedItemTag.Value.Color), false));

			tag.Value.Icon.SetImageDrawable(GetSelectColorButton(
				Color.ParseColor(tag.Value.Color), true));
			checkedColorIndex = itemIndex;
		}

		private LinearLayout CreateNewColorsLine()
		{
			var ll = new LinearLayout(this);
			ll.Orientation = Android.Widget.Orientation.Horizontal;
			var lp = new LinearLayout.LayoutParams(
				ViewGroup.LayoutParams.MatchParent,
				ViewGroup.LayoutParams.WrapContent);
			this.llColorsContainer.AddView(ll, lp);
			return ll;
		}

		private Drawable GetSelectColorButton(Color color, bool isChecked)
		{
			if(isChecked)
			{
				var drawable = (GradientDrawable)Resources.GetDrawable(Resource.Drawable.tag_selectcolor_round_icon);
				drawable.SetStroke(Conversion.Dp2Px(5), color);
				drawable.SetColor(color);
				return drawable;
			}
			else
			{
				var drawable = (GradientDrawable)Resources.GetDrawable(Resource.Drawable.tag_selectcolor_round_icon);
				drawable.SetStroke(Conversion.Dp2Px(5), color);
				drawable.SetColor(Color.White);
				return drawable;
			}
		}

		public void HideSoftKeyboard()
		{
			var imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
			imm.HideSoftInputFromWindow(llRootView.WindowToken, 0);
		}
	}
}

