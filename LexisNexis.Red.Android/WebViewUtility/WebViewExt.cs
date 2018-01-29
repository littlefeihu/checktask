using System;
using Android.Webkit;
using Android.Views;
using Android.Graphics;
using LexisNexis.Red.Droid.Utility;
using Android.App;
using Android.Widget;
using Android.Util;

namespace LexisNexis.Red.Droid.WebViewUtility
{
	public class WebViewExt: WebView
	{
		private ScrollOverLoadingPage loadingPage = ScrollOverLoadingPage.None;
		private bool sendFakeTouchUp = false;

		public enum ScrollOverLoadingPage
		{
			None,
			Top,
			Bottom,
		}

		public ScrollOverLoadingPage LoadingPage
		{
			get
			{
				return loadingPage;
			}
		}

		public bool IsLoadingPage
		{
			get
			{
				return loadingPage != ScrollOverLoadingPage.None;
			}
		}

		public void StarLoadingPage(ScrollOverLoadingPage loadingPage, bool up)
		{
			if(loadingPage == ScrollOverLoadingPage.None)
			{
				throw new ArgumentOutOfRangeException("loadingPage", "Can't set as None.");
			}

			this.loadingPage = loadingPage;
			sendFakeTouchUp = false;
		}

		public void LoadingPageCompleted(int newWebViewContentHeight)
		{
			loadingPage = ScrollOverLoadingPage.None;
			//Toast.MakeText(Context, "LoadingPageCompleted", ToastLength.Short).Show();
		}

		public WebViewExt(Android.Content.Context context) : base(context)
		{
		}

		public WebViewExt(Android.Content.Context context, Android.Util.IAttributeSet attrs) : base(context, attrs)
		{
		}

		public WebViewExt(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
		}

		public WebViewExt(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyleAttr, bool privateBrowsing) : base(context, attrs, defStyleAttr, privateBrowsing)
		{
		}

		public WebViewExt(Android.Content.Context context, Android.Util.IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
		{
		}

		private Action<int, float> overScrollHandler;
		public void SetOverScrollHandler(Action<int, float> overScrollHandler)
		{
			this.overScrollHandler = overScrollHandler;
		}

		private Point last;
		//private Point lastMoveEventPosition = new Point(-1, -1);
		public override bool OnTouchEvent(MotionEvent e)
		{
			if(IsLoadingPage)
			{
				if(!sendFakeTouchUp)
				{
					e.Action = MotionEventActions.Up;
					return base.OnTouchEvent(e);
				}

				return true;
			}

			if(e.Action == MotionEventActions.Move)
			{
				if(last != null)
				{
					if(ScrollY <= 0 && Height + ScrollY - this.ComputeVerticalScrollRange() >= 0)
					{
						// current page is small, and can't cover whole webview
						// user can overscroll to both direction
						// so, just fire over scroll event
					}
					else if(ScrollY <= 0)
					{
						// Over top
						if(e.GetY() <= last.Y)
						{
							last = null;
							if(overScrollHandler != null)
							{
								Log.Debug("dbg", "Fire overScrollHandler[1]");
								overScrollHandler(0, 0);
							}

							return base.OnTouchEvent(e);
						}
					}
					else if(Height + ScrollY - this.ComputeVerticalScrollRange() < 0)
					{
						// The content may changed;
						last = null;
						Log.Debug("dbg", "Fire overScrollHandler[2]");
						if(overScrollHandler != null)
						{
							overScrollHandler(0, 0);
						}

						return base.OnTouchEvent(e);
					}
					else
					{
						// Over bottom
						if(e.GetY() >= last.Y)
						{
							last = null;
							if(overScrollHandler != null)
							{
								Log.Debug("dbg", "Fire overScrollHandler[3]");
								overScrollHandler(0, 0);
							}

							return base.OnTouchEvent(e);
						}
					}

					//this.LoadUrl("javascript:android.red.onscroll();");
					var delta = last.Y - e.GetY();
					if(Math.Abs(delta) > 0)
					{
						Log.Debug("dbg", "C# detect reachwebviewbound");
						this.LoadUrl("javascript:android.red.reachwebviewbound(" + delta + ");");
					}

					return true;
				}
				else if(ScrollY <= 0 || Height + ScrollY - this.ComputeVerticalScrollRange() >= 0)
				{
					//this.LoadUrl("javascript:android.red.onscroll();");
					last = new Point((int)e.GetX(), (int)e.GetY());
					return base.OnTouchEvent(e);
				}
			}
			else if(e.Action == MotionEventActions.Up)
			{
				last = null;
				Log.Debug("dbg", "Fire overScrollHandler[4]");
				if(overScrollHandler != null)
				{
					overScrollHandler(0, 0);
				}
			}
			else if(e.Action == MotionEventActions.Down)
			{
				last = null;
			}

			return base.OnTouchEvent(e);
		}

		public void ScrollReachBound(int delta)
		{
			if(IsLoadingPage)
			{
				return;
			}

			Log.Debug("dbg", "JS confirmed reachwebviewbound");
			if(overScrollHandler != null)
			{
				overScrollHandler(delta, delta / Height);
			}
		}

		public int ExportComputeVerticalScrollRange()
		{
			return this.ComputeVerticalScrollRange();
		}
	}
}

