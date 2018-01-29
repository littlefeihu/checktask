using System;
using Android.Views;

namespace LexisNexis.Red.Droid.Widget.ViewUtility
{
	public class LimitMaxDimensHelper: Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
	{
		private readonly View view;
		private int originalWidth;
		private int originalHeigth;
		private readonly int maxWidth;
		private readonly int maxHeight;

		private LimitMaxDimensHelper(View view, int maxWidth, int maxHeight)
		{
			this.view = view;
			this.originalWidth = view.LayoutParameters.Width;
			this.originalHeigth = view.LayoutParameters.Height;
			this.maxWidth = maxWidth;
			this.maxHeight = maxHeight;
		}

		public static void Limit(View view, int maxWidth, int maxHeight)
		{
			LimitMaxDimensHelper helper = view.Tag as LimitMaxDimensHelper;
			if(helper == null)
			{
				helper = new LimitMaxDimensHelper(view, maxWidth, maxHeight);
				view.Tag = helper;
			}

			view.LayoutParameters.Width = helper.originalWidth;
			view.LayoutParameters.Height = helper.originalHeigth;

			view.ViewTreeObserver.AddOnGlobalLayoutListener(helper);
			view.RequestLayout();
		}

		public void OnGlobalLayout()
		{
			view.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);

			bool relayout = false;
			var layoutParameters = view.LayoutParameters;
			if(maxWidth > 0 && view.Width > maxWidth)
			{
				layoutParameters.Width = maxWidth;
				relayout = true;
			}

			if(maxHeight > 0 && view.Height > maxHeight)
			{
				layoutParameters.Height = maxHeight;
				relayout = true;
			}

			if(relayout)
			{
				view.LayoutParameters = layoutParameters;
				view.RequestLayout();
			}
		}
	}
}

