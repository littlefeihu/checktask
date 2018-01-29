
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
using Fragment=Android.Support.V4.App.Fragment;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class AnnotationListFragment : Fragment, IExpandableRightPanel, IRefreshableFragment
	{
		public const string FragmentTag = "AnnotationListFragment";

		private ImageView ivExpand;
		private ImageView ivPreviousPage;
		private ImageView ivNextPage;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.contentpage_annotationlist_fragment, container, false);

			//ivPreviousPage = v.FindViewById<ImageView>(Resource.Id.ivPreviousPage);
			//ivNextPage = v.FindViewById<ImageView>(Resource.Id.ivNextPage);

			//ivExpand = v.FindViewById<ImageView>(Resource.Id.ivExpand);
			//ivExpand.Click += OnIvExpandClick;

			//ivPreviousPage.Click += delegate
			//{
			//	var mainFragmentStatus = ((ContentActivity)Activity).GetMainFragment().MainFragmentStatus;
			//	if(!NavigationManagerHelper.CanBack(mainFragmentStatus))
			//	{
			//		return;
			//	}

			//	var fromBookId = NavigationManagerHelper.GetCurrentBookId();
			//	NavigationManagerHelper.Back(mainFragmentStatus);
			//	((ContentActivity)Activity).GetMainFragment().NavigateTo(fromBookId);
			//};

			//ivNextPage.Click += delegate
			//{
			//	var mainFragmentStatus = ((ContentActivity)Activity).GetMainFragment().MainFragmentStatus;
			//	if(!NavigationManagerHelper.CanForth(mainFragmentStatus))
			//	{
			//		return;
			//	}

			//	var fromBookId = NavigationManagerHelper.GetCurrentBookId();
			//	NavigationManagerHelper.Forth(mainFragmentStatus);
			//	((ContentActivity)Activity).GetMainFragment().NavigateTo(fromBookId);
			//};

			//((ContentActivity)Activity).GetMainFragment().SetLeftPanelStatus();

			return v;
		}

		public override void OnHiddenChanged(bool hidden)
		{
			base.OnHiddenChanged(hidden);
			if(!hidden)
			{
				SetExpandableStatus();
				Refresh();
			}
		}

		public override void OnResume()
		{
			base.OnResume();
			Refresh();
		}

		private void OnIvExpandClick (object sender, EventArgs e)
		{
			((ContentActivity)Activity).GetMainFragment().SwitchLeftPanelStatus();
		}

		public void SetExpandableStatus()
		{
			//if(ivExpand == null)
			//{
			//	// OnCreateView did not called.
			//	return;
			//}

			//var status = ((ContentActivity)Activity).GetMainFragment().MainFragmentStatus;
			//if(status.LeftPanelOpen)
			//{
			//	ivExpand.SetImageResource(Resource.Drawable.expand_content_view);
			//}
			//else
			//{
			//	ivExpand.SetImageResource(Resource.Drawable.collapse_content_view);
			//}
		}

		public void Refresh()
		{
			UpdateNavigationIcon();
		}

		private void UpdateNavigationIcon()
		{
			//var mainFragmentStatus = ((ContentActivity)Activity).GetMainFragment().MainFragmentStatus;
			//if(!NavigationManagerHelper.CanBack(mainFragmentStatus))
			//{
			//	ivPreviousPage.SetImageResource(Resource.Drawable.previous_page_disable);
			//}
			//else
			//{
			//	ivPreviousPage.SetImageResource(Resource.Drawable.previous_page_activy);
			//}

			//if(!NavigationManagerHelper.CanForth(mainFragmentStatus))
			//{
			//	ivNextPage.SetImageResource(Resource.Drawable.next_page_disable);
			//}
			//else
			//{
			//	ivNextPage.SetImageResource(Resource.Drawable.next_page_activy);
			//}
		}
	}
}

