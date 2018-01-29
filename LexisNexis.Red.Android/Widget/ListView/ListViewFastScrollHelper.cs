using System;
using Android.Widget;
using Android.Views;

namespace LexisNexis.Red.Droid.Widget.ListView
{
	using ListView = Android.Widget.ListView;

	public class ListViewFastScrollHelper: Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
	{
		private readonly ListView lv;
		private readonly BaseAdapter adaptor;

		public ListViewFastScrollHelper(ListView lv, BaseAdapter adaptor)
		{
			this.lv = lv;
			this.adaptor = adaptor;
		}

		public static void ForceRecheckItemCount(ListView lv, BaseAdapter adaptor)
		{
			var helper = new ListViewFastScrollHelper(lv, adaptor);

			lv.ViewTreeObserver.AddOnGlobalLayoutListener(helper);
			lv.RequestLayout();
		}

		public void OnGlobalLayout()
		{
			lv.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);
			adaptor.NotifyDataSetChanged();
		}
	}
}

