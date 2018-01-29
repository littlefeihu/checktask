using Android.Widget;
using Android.Animation;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Content;
using Android.Util;
using System;
using Android.Support.V7.Widget;
using Android.Support.V4.View;
using Android.App;
using System.Threading.Tasks;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Utility;



namespace LexisNexis.Red.Droid.Widget.DraggableList
{
	public class DraggableRecyclerView
		: RecyclerView, RecyclerView.IOnItemTouchListener
	{
		private const String TAG = "DraggableListRCView";
		private const int INVALID_POINTER_ID = -1;
		private const int LINE_THICKNESS = 2;
		private static readonly Color BORD_COLOR = Color.ParseColor("#ED1C24");
		private const int SMOOTH_SCROLL_AMOUNT_AT_EDGE_DP = 10;
		private const int INVALID_ID = -1;

		private int activePointerId = INVALID_POINTER_ID;
		private int lastEventY, lastEventX;
		private int downX;
		private int downY;
		private int totalOffsetY, totalOffsetX;

		private BitmapDrawable hoverCell;
		private Rect hoverCellOriginalBounds;
		private Rect hoverCellCurrentBounds;

		private bool cellIsMobile = false;
		private long mobileItemId = INVALID_ID;

		private int smoothScrollAmountAtEdge;
		private bool usWaitingForScrollFinish;

		private GestureDetector longPressGestureDetector;

		private long lastSwapTime;
		public static long GetTimeStamp()
		{
			return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		}

		public DraggableRecyclerView(Context context)
			:base(context)
		{
			init(context);
		}

		public DraggableRecyclerView(Context context, IAttributeSet attrs)
			:base(context, attrs)
		{
			init(context);
		}

		public DraggableRecyclerView(Context context, IAttributeSet attrs, int defStyle)
			:base(context, attrs, defStyle)
		{
			init(context);
		}

		public class LongPressListener: GestureDetector.SimpleOnGestureListener
		{
			private readonly DraggableRecyclerView container;
			public LongPressListener(DraggableRecyclerView container)
			{
				this.container = container;
			}

			public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
			{
				float deltaX = e1.GetX() - e2.GetX();

				if(Math.Abs(velocityX) > 200 && (Math.Abs(deltaX) / Math.Abs(e1.GetY() - e2.GetY()) > 5))
				{
					View view = container.FindChildViewUnder(e1.GetX(), e1.GetY());
					if(view == null)
					{
						return base.OnFling(e1, e2, velocityX, velocityY);
					}

					var vh = container.GetChildViewHolder(view) as DraggableViewHolder;
					if(vh == null)
					{
						return base.OnFling(e1, e2, velocityX, velocityY);
					}

					if(deltaX > 0)
					{
						// to left
						vh.Swip(true);
					}
					else
					{
						// to right
						vh.Swip(false);
					}

					return true;
				}

				return base.OnFling(e1, e2, velocityX, velocityY);
			}

			// Long Press
			public override void OnLongPress(MotionEvent e)
			{
				container.downX = (int) e.GetX();
				container.downY = (int) e.GetY();
				container.activePointerId = e.GetPointerId(0);

				container.totalOffsetY = 0;
				container.totalOffsetX = 0;
				View selectedView = container.FindChildViewUnder(container.downX, container.downY);
				if (selectedView == null) {
					return;
				}

				var vh = container.GetChildViewHolder(selectedView) as DraggableViewHolder;
				if(vh == null)
				{
					return;
				}

				float translationX = ViewCompat.GetTranslationX(selectedView);
				float translationY = ViewCompat.GetTranslationY(selectedView);
				var selectedViewRect = new Rect(
					(int)(selectedView.Left + translationX),
					(int)(selectedView.Top + translationY),
					(int)(selectedView.Right + translationX),
					(int)(selectedView.Bottom + translationY));


				if(!vh.CanDrag(selectedViewRect, container.downX, container.downY))
				{
					return;
				}

				container.mobileItemId = container.GetChildItemId(selectedView);
				container.hoverCell = container.GetAndAddHoverView(selectedView);
				selectedView.Visibility = ViewStates.Invisible;
				container.cellIsMobile = true;
			}
		}

		public void init(Context context)
		{
			DisplayMetrics metrics = context.Resources.DisplayMetrics;
			smoothScrollAmountAtEdge = (int) (SMOOTH_SCROLL_AMOUNT_AT_EDGE_DP * metrics.Density);
			longPressGestureDetector = new GestureDetector(MainApp.ThisApp, new LongPressListener(this));
			AddOnItemTouchListener(this);
			valueAnimatorUpdateListner = new ValueAnimatorUpdateListner(this);
			rcAnimatorListener = new RCAnimatorListener(this);
			lastSwapTime = 0;
		}

		public bool OnInterceptTouchEvent(RecyclerView rv, MotionEvent e)
		{
			if (longPressGestureDetector.OnTouchEvent(e)) {
				return true;
			}

			switch (e.Action)
			{
			case MotionEventActions.Move:
				return cellIsMobile;
			default:
				break;
			}

			return false;
		}

		public void OnRequestDisallowInterceptTouchEvent(bool p0)
		{
		}

		protected override void DispatchDraw(Canvas canvas)
		{
			base.DispatchDraw(canvas);
			if(hoverCell != null)
			{
				hoverCell.Draw(canvas);
			}
		}

		public void OnTouchEvent(RecyclerView rv, MotionEvent e)
		{
			HandleMotionEvent(e);
		}

		private MotionEvent lastMotionEvent;
		private void HandleMotionEvent(MotionEvent e) {
			lastMotionEvent = e;
			switch (e.Action) {
			case MotionEventActions.Move:
				//Log.Info("DBG", string.Format("-----------------{0}", e));

				if (activePointerId == INVALID_POINTER_ID) {
					break;
				}

				int pointerIndex = e.FindPointerIndex(activePointerId);

				lastEventY = (int) e.GetY(pointerIndex);
				lastEventX = (int) e.GetX(pointerIndex);
				int deltaY = lastEventY - downY;
				int deltaX = lastEventX - downX;

				if (cellIsMobile) {
					hoverCellCurrentBounds.OffsetTo(hoverCellOriginalBounds.Left + deltaX + totalOffsetX,
						hoverCellOriginalBounds.Top + deltaY + totalOffsetY);
					hoverCell.SetBounds(
						hoverCellCurrentBounds.Left,
						hoverCellCurrentBounds.Top,
						hoverCellCurrentBounds.Right,
						hoverCellCurrentBounds.Bottom);
					Invalidate();
					var now = GetTimeStamp();
					if(now - lastSwapTime > 500)
					{
						lastSwapTime = now;
						handleCellSwitch();
					}

					handleMobileCellScroll();
				}
				break;
			case MotionEventActions.Up:
				TouchEventsEnded();
				break;
			case MotionEventActions.Cancel:
				TouchEventsCancelled();
				break;
			case MotionEventActions.PointerUp:
				/* If a multitouch event took place and the original touch dictating
                 * the movement of the hover cell has ended, then the dragging event
                 * ends and the hover cell is animated to its corresponding position
                 * in the listview. */
				pointerIndex = (int)(e.Action & MotionEventActions.PointerIdMask) >> (int)(MotionEventActions.PointerIndexShift);
				int pointerId = e.GetPointerId(pointerIndex);
				if (pointerId == activePointerId) {
					TouchEventsEnded();
				}
				break;
			default:
				break;
			}
		}

		/**
	     * Creates the hover cell with the appropriate bitmap and of appropriate
	     * size. The hover cell's BitmapDrawable is drawn on top of the bitmap every
	     * single time an invalidate call is made.
	     */
		private BitmapDrawable GetAndAddHoverView(View v) {
			int w = v.Width;
			int h = v.Height;
			int top = v.Top;
			int left = v.Left;

			Bitmap b = GetBitmapWithBorder(v);

			BitmapDrawable drawable = new BitmapDrawable(this.Resources, b);

			hoverCellOriginalBounds = new Rect(left, top, left + w, top + h);
			hoverCellCurrentBounds = new Rect(hoverCellOriginalBounds);

			drawable.SetBounds(
				hoverCellCurrentBounds.Left,
				hoverCellCurrentBounds.Top,
				hoverCellCurrentBounds.Right,
				hoverCellCurrentBounds.Bottom);

			return drawable;
		}

		/**
     	* Draws a black border over the screenshot of the view passed in.
     	*/
		private Bitmap GetBitmapWithBorder(View v) {
			Bitmap bitmap = getBitmapFromView(v);
			Canvas can = new Canvas(bitmap);

			Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);

			Paint paint = new Paint();
			paint.SetStyle(Paint.Style.Stroke);
			paint.StrokeWidth = Conversion.Dp2Px(LINE_THICKNESS);
			paint.Color = BORD_COLOR;

			can.DrawBitmap(bitmap, 0, 0, null);
			can.DrawRect(rect, paint);

			return bitmap;
		}

		/**
	     * Returns a bitmap showing a screenshot of the view passed in.
	     */
		private Bitmap getBitmapFromView(View v) {
			Bitmap bitmap = Bitmap.CreateBitmap(v.Width, v.Height, Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas(bitmap);
			v.Draw(canvas);
			return bitmap;
		}

		/**
	     * This method determines whether the hover cell has been shifted far enough
	     * to invoke a cell swap. If so, then the respective cell swap candidate is
	     * determined and the data set is changed. Upon posting a notification of the
	     * data set change, a layout is invoked to place the cells in the right place.
	     */
		private void handleCellSwitch() {
			ViewHolder mobileViewHolder = FindViewHolderForItemId(mobileItemId);
			View mobileView = mobileViewHolder != null ? mobileViewHolder.ItemView : null;
			if (mobileView != null)
			{
				//View childViewUnder = FindChildViewUnder(lastEventX, lastEventY);
				View childViewUnder = FindViewUnder();

				if(childViewUnder != null)
				{
					int childPosition = GetChildPosition(childViewUnder);
					int originalItem = GetChildPosition(mobileView);
					swapElements(originalItem, childPosition);
				}
				else
				{
					LogHelper.Debug(TAG, "Not Found");
				}
			}
		}

		private View FindViewUnder()
		{
			int count = this.GetAdapter().ItemCount;
			for (int i = count - 1; i >= 0; i--)
			{
				View child = GetChildAt(i);
				if(child == null)
				{
					continue;
				}

				Region r = null;
				if(((LinearLayoutManager)GetLayoutManager()).Orientation == LinearLayoutManager.Horizontal)
				{
					float translationX = ViewCompat.GetTranslationX(child);
					r = new Region(
						new Rect(
							(int)(child.Left + translationX),
							hoverCellCurrentBounds.Top,
							(int)(child.Right + translationX),
							hoverCellCurrentBounds.Bottom));
				}
				else
				{
					float translationY = ViewCompat.GetTranslationY(child);
					r = new Region(
						new Rect(
							hoverCellCurrentBounds.Left,
							(int)(child.Top + translationY),
							hoverCellCurrentBounds.Right,
							(int)(child.Bottom + translationY)));
				}

				r.InvokeOp(hoverCellCurrentBounds, Region.Op.Intersect);

				if(!r.IsEmpty)
				{
					var ratio = (float)(r.Bounds.Width() * r.Bounds.Height())
						/ (float)(hoverCellCurrentBounds.Width() * hoverCellCurrentBounds.Height());
					LogHelper.Debug(TAG, string.Format("#{0}# is overlapped, ratio = {1}", (char)GetChildItemId(child), ratio));
					if(ratio > 0.5)
					{
						if(GetChildItemId(child) == mobileItemId)
						{
							return null;
						}

						LogHelper.Debug(TAG, string.Format("Swap with: {0}", (char)GetChildItemId(child)));
						return child;
					}
				}
			}
			return null;
		}

		public interface DraggableAdapter
		{
			void ShiftItems(int mobileItemIndex, int targetItemIndex);
			void MoveFinished();
		}

		public interface DraggableViewHolder
		{
			void Swip(bool left);
			bool CanDrag(Rect viewRect, int longPressX, int longPressY);
		}

		/**
	     * Swaps the the elements with the given indices.
	     *
	     * @param fromIndex the from-element index
	     * @param toIndex   the to-element index
	     */
		private void swapElements(int mobileItemIndex, int targetItemIndex) {
			var adapter = (RecyclerView.Adapter) this.GetAdapter();
			((DraggableAdapter)adapter).ShiftItems(mobileItemIndex, targetItemIndex);
			adapter.NotifyDataSetChanged();
		}

		/**
	     * Determines whether this recyclerview is in a scrolling state invoked
	     * by the fact that the hover cell is out of the bounds of the recyclerview;
	     */
		private void handleMobileCellScroll(bool bigStep = false) {
			handleMobileCellScroll(hoverCellCurrentBounds, bigStep);
		}

		/**
	     * This method is in charge of determining if the hover cell is above/below or
	     * left/right the bounds of the recyclerview. If so, the recyclerview does an appropriate
	     * upward or downward smooth scroll so as to reveal new items.
	     */
		private long lastRequireFutureScroll;
		public bool handleMobileCellScroll(Rect r, bool bigStep = false) {
			if (GetLayoutManager().CanScrollVertically()) {
				int offset = ComputeVerticalScrollOffset();
				int height = Height;
				int extent = ComputeVerticalScrollExtent();
				int range = ComputeVerticalScrollRange();
				int hoverViewTop = r.Top;
				int hoverHeight = r.Height();

				if (hoverViewTop <= 0 && offset > 0) {
					//LogHelper.Debug(TAG, String.Format("scrolling vertically by {0}", -smoothScrollAmountAtEdge));
					if(bigStep)
					{
						ScrollBy(0, -1 * r.Height() / 2);
					}
					else
					{
						ScrollBy(0, -smoothScrollAmountAtEdge);
					}
					DelayedScroll();
					return true;
				}

				if (hoverViewTop + hoverHeight >= height && (offset + extent) < range) {
					//LogHelper.Debug(TAG, String.Format("scrolling vertically by {0}", smoothScrollAmountAtEdge));
					if(bigStep)
					{
						ScrollBy(0, r.Height() / 2);
					}
					else
					{
						ScrollBy(0, smoothScrollAmountAtEdge);
					}
					DelayedScroll();
					return true;
				}
			}

			if (GetLayoutManager().CanScrollHorizontally()) {
				int offset = ComputeHorizontalScrollOffset();
				int width = Width;
				int extent = ComputeHorizontalScrollExtent();
				int range = ComputeHorizontalScrollRange();
				int hoverViewLeft = r.Left;
				int hoverWidth = r.Width();

				if (hoverViewLeft <= 0 && offset > 0) {
					//LogHelper.Debug(TAG, String.Format("scrolling horizontally by {0}", -smoothScrollAmountAtEdge));
					if(bigStep)
					{
						ScrollBy(-1 * r.Width() / 2, 0);
					}
					else
					{
						ScrollBy(-smoothScrollAmountAtEdge, 0);
					}
					DelayedScroll();
					return true;
				}

				if (hoverViewLeft + hoverWidth >= width && (offset + extent) < range) {
					//LogHelper.Debug(TAG, String.Format("scrolling horizontally by {0}", smoothScrollAmountAtEdge));
					if(bigStep)
					{
						ScrollBy(r.Width() / 2, 0);
					}
					else
					{
						ScrollBy(smoothScrollAmountAtEdge, 0);
					}
					DelayedScroll();
					return true;
				}
			}

			return false;
		}

		private void DelayedScroll()
		{
			Application.SynchronizationContext.Post(async _ => 
			{
				long requireFutureScroll = GetTimeStamp() + 500;
				if(requireFutureScroll == lastRequireFutureScroll)
				{
					return;
				}

				lastRequireFutureScroll = requireFutureScroll;
				await Task.Delay(500);

				if(lastRequireFutureScroll == requireFutureScroll)
				{
					if(cellIsMobile)
					{
						LogHelper.Debug("dbg", "Continue Scroll");
						handleMobileCellScroll(true);
						handleCellSwitch();
					}
				}

			}, null);
		}

		/**
	     * Resets all the appropriate fields to a default state.
	     */
		private void TouchEventsCancelled() {
			ViewHolder viewHolderForItemId = FindViewHolderForItemId(mobileItemId);
			if (viewHolderForItemId == null) {
				return;
			}
			View mobileView = viewHolderForItemId.ItemView;
			if (cellIsMobile) {
				mobileItemId = INVALID_ID;
				mobileView.Visibility = ViewStates.Visible;
				hoverCell = null;
				Invalidate();
			}
			cellIsMobile = false;
			activePointerId = INVALID_POINTER_ID;
		}
			
		private static readonly RectEvaluator sBoundEvaluator = new RectEvaluator();

		private ValueAnimatorUpdateListner valueAnimatorUpdateListner;
		private class ValueAnimatorUpdateListner: Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
		{
			public DraggableRecyclerView container;
			public ValueAnimatorUpdateListner(DraggableRecyclerView container)
			{
				this.container = container;
			}

			public void OnAnimationUpdate(ValueAnimator animation)
			{
				container.Invalidate();
			}
		}

		private RCAnimatorListener rcAnimatorListener;
		private class RCAnimatorListener: AnimatorListenerAdapter
		{
			public DraggableRecyclerView container;
			public RCAnimatorListener(DraggableRecyclerView container)
			{
				this.container = container;
			}

			public override void OnAnimationStart(Animator animation)
			{
				container.Enabled = false;
			}

			public override void OnAnimationEnd(Animator animation) {
				container.mobileItemId = INVALID_ID;
				container.mobileViewTouchEventsEnded.Visibility = ViewStates.Visible;
				container.hoverCell = null;
				container.Enabled = true;
				container.Invalidate();
			}
		}

		/**
	     * Resets all the appropriate fields to a default state while also animating
	     * the hover cell back to its correct location.
	     */
		private View mobileViewTouchEventsEnded;
		private void TouchEventsEnded() {
			ViewHolder viewHolderForItemId = FindViewHolderForItemId(mobileItemId);
			if (viewHolderForItemId == null) {
				return;
			}
			mobileViewTouchEventsEnded = viewHolderForItemId.ItemView;
			if (cellIsMobile || usWaitingForScrollFinish) {
				cellIsMobile = false;
				usWaitingForScrollFinish = false;
				activePointerId = INVALID_POINTER_ID;

				// If the autoscroller has not completed scrolling, we need to wait for it to
				// finish in order to determine the final location of where the hover cell
				// should be animated to.
				if (ScrollState != (int)Android.Widget.ScrollState.Idle) {
					usWaitingForScrollFinish = true;
					return;
				}

				hoverCellCurrentBounds.OffsetTo(mobileViewTouchEventsEnded.Left, mobileViewTouchEventsEnded.Top);

				ObjectAnimator hoverViewAnimator = ObjectAnimator.OfObject(hoverCell, "bounds",
					sBoundEvaluator, hoverCellCurrentBounds);
				hoverViewAnimator.AddUpdateListener(valueAnimatorUpdateListner);
				hoverViewAnimator.AddListener(rcAnimatorListener);
				hoverViewAnimator.Start();

				// Tell adpter movement of an item has completed.
				((DraggableAdapter)GetAdapter()).MoveFinished();
			} else {
				TouchEventsCancelled();
			}

		}
	}
}