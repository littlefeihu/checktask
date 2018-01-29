using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace LexisNexis.Red.Droid.Widget.SlidingTab
{
    public sealed class SlidingTabStrip : LinearLayout
    {
        //Copy and paste from here................................................................
        private const int DefaultBottomBorderThicknessDips = 2;
        private const byte DefaultBottomBorderColorAlpha = 0X26;
        private const int SelectedIndicatorThicknessDips = 2;
        private readonly int[] _indicatorColors = { 0x19A319, 0x0000FC };
        private readonly int[] _dividerColors = { 0xC5C5C5 };

        private const int DefaultDividerThicknessDips = 1;
        private const float DefaultDividerHeight = 0.5f;

        //Bottom border
        private readonly int _mBottomBorderThickness;
        private readonly Paint _mBottomBorderPaint;

        //Indicator
        private readonly int _mSelectedIndicatorThickness;
        private readonly Paint _mSelectedIndicatorPaint;

        //Divider
        private readonly Paint _mDividerPaint;
        private readonly float _mDividerHeight;

        //Selected position and offset
        private int _mSelectedPosition;
        private float _mSelectionOffset;

        //Tab colorizer
        private SlidingTabScrollView.ITabColorizer _mCustomTabColorizer;
        private readonly SimpleTabColorizer _mDefaultTabColorizer;
        //Stop copy and paste here........................................................................

        //Constructors
        public SlidingTabStrip (Context context) : this(context, null)
        { }

        public SlidingTabStrip (Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SetWillNotDraw(false);

            float density = Resources.DisplayMetrics.Density;

            TypedValue outValue = new TypedValue();
            context.Theme.ResolveAttribute(Android.Resource.Attribute.ColorForeground, outValue, true);
            int themeForeGround = outValue.Data;
            var mDefaultBottomBorderColor = SetColorAlpha(themeForeGround, DefaultBottomBorderColorAlpha);

            _mDefaultTabColorizer = new SimpleTabColorizer
            {
                IndicatorColors = _indicatorColors,
                DividerColors = _dividerColors
            };

            _mBottomBorderThickness = (int)(DefaultBottomBorderThicknessDips * density);
            _mBottomBorderPaint = new Paint {Color = new Color(mDefaultBottomBorderColor)};
            //Gray

            _mSelectedIndicatorThickness = (int)(SelectedIndicatorThicknessDips * density);
            _mSelectedIndicatorPaint = new Paint();

            _mDividerHeight = DefaultDividerHeight;
            _mDividerPaint = new Paint {StrokeWidth = (int) (DefaultDividerThicknessDips*density)};
        }

        public SlidingTabScrollView.ITabColorizer CustomTabColorizer
        {
            set
            {
                _mCustomTabColorizer = value;
                Invalidate();
            }
        }

        public int [] SelectedIndicatorColors
        {
            set
            {
                _mCustomTabColorizer = null;
                _mDefaultTabColorizer.IndicatorColors = value;
                Invalidate();
            }
        }

        public int [] DividerColors
        {
            set
            {
                _mCustomTabColorizer = null;
                _mDefaultTabColorizer.DividerColors = value;
                Invalidate();
            }
        }

        private Color GetColorFromInteger(int color)
        {
            return Color.Rgb(Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }

        private int SetColorAlpha(int color, byte alpha)
        {
            return Color.Argb(alpha, Color.GetRedComponent(color), Color.GetGreenComponent(color), Color.GetBlueComponent(color));
        }

        public void OnViewPagerPageChanged(int position, float positionOffset)
        {
            _mSelectedPosition = position;
            _mSelectionOffset = positionOffset;
            Invalidate();
        }

        protected override void OnDraw(Canvas canvas)
        {
            int height = Height;
            int tabCount = ChildCount;
            int dividerHeightPx = (int)(Math.Min(Math.Max(0f, _mDividerHeight), 1f) * height);
            SlidingTabScrollView.ITabColorizer tabColorizer = _mCustomTabColorizer ?? _mDefaultTabColorizer;

            //Thick colored underline below the current selection
            if (tabCount > 0)
            {
                View selectedTitle = GetChildAt(_mSelectedPosition);
                int left = selectedTitle.Left;
                int right = selectedTitle.Right;
                int color = tabColorizer.GetIndicatorColor(_mSelectedPosition);

                if (_mSelectionOffset > 0f && _mSelectedPosition < (tabCount - 1))
                {
                    int nextColor = tabColorizer.GetIndicatorColor(_mSelectedPosition + 1);
                    if (color != nextColor)
                    {
                        color = BlendColor(nextColor, color, _mSelectionOffset);
                    }

                    View nextTitle = GetChildAt(_mSelectedPosition + 1);
                    left = (int)(_mSelectionOffset * nextTitle.Left + (1.0f - _mSelectionOffset) * left);
                    right = (int)(_mSelectionOffset * nextTitle.Right + (1.0f - _mSelectionOffset) * right);
                }

                _mSelectedIndicatorPaint.Color = GetColorFromInteger(color);

                canvas.DrawRect(left, height - _mSelectedIndicatorThickness, right, height, _mSelectedIndicatorPaint);

                //Creat vertical dividers between tabs
                int separatorTop = (height - dividerHeightPx) / 2;
                for (int i = 0; i < ChildCount; i++)
                {
                    View child = GetChildAt(i);
                    _mDividerPaint.Color = GetColorFromInteger(tabColorizer.GetDividerColor(i));
                    canvas.DrawLine(child.Right, separatorTop, child.Right, separatorTop + dividerHeightPx, _mDividerPaint);
                }

                canvas.DrawRect(0, height - _mBottomBorderThickness, Width, height, _mBottomBorderPaint);
            }
        }

        private int BlendColor(int color1, int color2, float ratio)
        {
            float inverseRatio = 1f - ratio;
            float r = (Color.GetRedComponent(color1) * ratio) + (Color.GetRedComponent(color2) * inverseRatio);
            float g = (Color.GetGreenComponent(color1) * ratio) + (Color.GetGreenComponent(color2) * inverseRatio);
            float b = (Color.GetBlueComponent(color1) * ratio) + (Color.GetBlueComponent(color2) * inverseRatio);

            return Color.Rgb((int)r, (int)g, (int)b);
        }

        private class SimpleTabColorizer : SlidingTabScrollView.ITabColorizer
        {
            private int[] _mIndicatorColors;
            private int[] _mDividerColors;

            public int GetIndicatorColor(int position)
            {
                return _mIndicatorColors[position % _mIndicatorColors.Length];
            }

            public int GetDividerColor (int position)
            {
                return _mDividerColors[position % _mDividerColors.Length];
            }
           
            public int[] IndicatorColors
            {
                set { _mIndicatorColors = value; }
            }

            public int[] DividerColors
            {
                set { _mDividerColors = value; }
            }
        }
    }
}