using System;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Graphics;

namespace LexisNexis.Red.Droid.Widget.SlidingTab
{
    public sealed class SlidingTabScrollView : HorizontalScrollView
    {

        private const int TitleOffsetDips = 24;
		private const int TabViewPaddingTopBottomDips = 8;
		private const int TabViewPaddingLeftRightDips = 16;
        private const int TabViewTextSizeSp = 12;

		private readonly int titleOffset;

		private ViewPager viewPager;
		private ViewPager.IOnPageChangeListener viewPagerPageChangeListener;

		private readonly SlidingTabStrip tabStrip;

		private int scrollState;

        public interface ITabColorizer
        {
            int GetIndicatorColor(int position);
            int GetDividerColor(int position);
        }

        public SlidingTabScrollView(Context context) : this(context, null) { }

        public SlidingTabScrollView(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }

        public SlidingTabScrollView(Context context, IAttributeSet attrs, int defaultStyle)
            : base(context, attrs, defaultStyle)
        {
            //Disable the scroll bar
            HorizontalScrollBarEnabled = false;

            //Make sure the tab strips fill the view
            FillViewport = true;
            //SetBackgroundColor(Android.Graphics.Color.Rgb(0xE5, 0xE5, 0xE5)); //Gray color

            titleOffset = (int)(TitleOffsetDips * Resources.DisplayMetrics.Density);

            tabStrip = new SlidingTabStrip(context);
            AddView(tabStrip, ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        }

        public ITabColorizer CustomTabColorizer
        {
            set { tabStrip.CustomTabColorizer = value; }
        }

        public int[] SelectedIndicatorColor
        {
            set { tabStrip.SelectedIndicatorColors = value; }
        }

        public int[] DividerColors
        {
            set { tabStrip.DividerColors = value; }
        }

        public ViewPager.IOnPageChangeListener OnPageListener
        {
            set { viewPagerPageChangeListener = value; }
        }

		public ViewPager ViewPager
		{
			set
			{
				tabStrip.RemoveAllViews();

				viewPager = value;
				if (value != null)
				{
					value.PageSelected += value_PageSelected;
					value.PageScrollStateChanged += value_PageScrollStateChanged;
					value.PageScrolled += value_PageScrolled;
					PopulateTabStrip();
				}
			}
		}

        void value_PageScrolled(object sender, ViewPager.PageScrolledEventArgs e)
        {
            int tabCount = tabStrip.ChildCount;

            if ((tabCount == 0) || (e.Position < 0) || (e.Position >= tabCount))
            {
                //if any of these conditions apply, return, no need to scroll
                return;
            }

            tabStrip.OnViewPagerPageChanged(e.Position, e.PositionOffset);

            View selectedTitle = tabStrip.GetChildAt(e.Position);

            int extraOffset = (selectedTitle != null ? e.Position * selectedTitle.Width : 0);

            ScrollToTab(e.Position, extraOffset);

            if (viewPagerPageChangeListener != null)
            {
                viewPagerPageChangeListener.OnPageScrolled(e.Position, e.PositionOffset, e.PositionOffsetPixels);
            }

        }

        void value_PageScrollStateChanged(object sender, ViewPager.PageScrollStateChangedEventArgs e)
        {
            scrollState = e.State;

            if (viewPagerPageChangeListener != null)
            {
                viewPagerPageChangeListener.OnPageScrollStateChanged(e.State);
            }
        }

        void value_PageSelected(object sender, ViewPager.PageSelectedEventArgs e)
        {
            if (scrollState == ViewPager.ScrollStateIdle)
            {
                tabStrip.OnViewPagerPageChanged(e.Position, 0f);
                ScrollToTab(e.Position, 0);
            }

            if (viewPagerPageChangeListener != null)
            {
                viewPagerPageChangeListener.OnPageSelected(e.Position);
            }
        }

        private void PopulateTabStrip()
        {
            PagerAdapter adapter = viewPager.Adapter;

            for (int i = 0; i < adapter.Count; i++)
            {
                TextView tabView = CreateDefaultTabView(Context);
				tabView.Text = ((IPagerWithTitleAdapter)adapter).GetTabTitle(i);
				tabView.SetTextColor(Color.White);
                tabView.Tag = i;
                tabView.Click += tabView_Click;
                tabStrip.AddView(tabView);
            }
        }

        void tabView_Click(object sender, EventArgs e)
        {
            TextView clickTab = (TextView)sender;
            int pageToScrollTo = (int)clickTab.Tag;
            viewPager.CurrentItem = pageToScrollTo;
        }

        private TextView CreateDefaultTabView(Context context)
        {
            TextView textView = new TextView(context) { Gravity = GravityFlags.Center };
			textView.SetTextSize(ComplexUnitType.Sp, TabViewTextSizeSp);
            textView.Typeface = Typeface.DefaultBold;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                TypedValue outValue = new TypedValue();
                Context.Theme.ResolveAttribute(Android.Resource.Attribute.SelectableItemBackground, outValue, false);
                textView.SetBackgroundResource(outValue.ResourceId);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                textView.SetAllCaps(true);
            }

            int topBottomPadding = (int)(TabViewPaddingTopBottomDips * Resources.DisplayMetrics.Density);
			int leftRightPadding = (int)(TabViewPaddingLeftRightDips * Resources.DisplayMetrics.Density);
			textView.SetPadding(leftRightPadding, topBottomPadding, leftRightPadding, topBottomPadding);

            return textView;
        }

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (viewPager != null)
            {
                ScrollToTab(viewPager.CurrentItem, 0);
            }
        }

        private void ScrollToTab(int tabIndex, int extraOffset)
        {
            int tabCount = tabStrip.ChildCount;

            if (tabCount == 0 || tabIndex < 0 || tabIndex >= tabCount)
            {
                //No need to go further, dont scroll
                return;
            }

            View selectedChild = tabStrip.GetChildAt(tabIndex);
            if (selectedChild != null)
            {
                int scrollAmountX = selectedChild.Left + extraOffset;

                if (tabIndex > 0 || extraOffset > 0)
                {
                    scrollAmountX -= titleOffset;
                }

                ScrollTo(scrollAmountX, 0);
            }

        }

    }
}