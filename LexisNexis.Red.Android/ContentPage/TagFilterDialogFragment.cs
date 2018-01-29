
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
using Android.Views.InputMethods;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Entity;
using DialogFragment=Android.Support.V4.App.DialogFragment;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Droid.Async;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.WebViewUtility;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.AnnotationUtility;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class TagFilterDialogFragment : DialogFragment, ITagFilterListener
	{
		public const string FragmentTag = "tagFilterDialogFragment";

		private RecyclerView rvTagFilter;
		private Button btnOk;

		private LinearLayoutManager tagFilterLayoutManager;
		private TagFilterAdaptor tfrvAdaptor;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
			Dialog.SetCanceledOnTouchOutside(true);

			var vwTagFilter = inflater.Inflate(Resource.Layout.contentpage_annotationlist_tagfilter_popup, container);

			rvTagFilter = vwTagFilter.FindViewById<RecyclerView>(Resource.Id.rvTagFilter);
			btnOk = vwTagFilter.FindViewById<Button>(Resource.Id.btnOk);

			tagFilterLayoutManager = new LinearLayoutManager (Activity);
			tagFilterLayoutManager.Orientation = LinearLayoutManager.Vertical;
			rvTagFilter.SetLayoutManager (tagFilterLayoutManager);
			tfrvAdaptor = new TagFilterAdaptor (this);
			tfrvAdaptor.SetTagList(AnnCategoryTagUtil.Instance.GetTags());

			var annotationPanel = GetAnnotationsPanelFragment();
			if(annotationPanel != null)
			{
				if(annotationPanel.Status.SelectedTagFilter == null)
				{
					annotationPanel.UpdateSelectedTagFilter(tfrvAdaptor.GetSelectedTagList());
				}
				else
				{
					tfrvAdaptor.SetSelectedTagFilterList(annotationPanel.Status.SelectedTagFilter);
				}
			}

			rvTagFilter.SetAdapter(tfrvAdaptor);

			btnOk.Click += delegate
			{
				Dismiss();
			};

			return vwTagFilter;
		}

		private AnnotationsPanelFragment GetAnnotationsPanelFragment()
		{
			return Activity.SupportFragmentManager
				.FindFragmentByTag(AnnotationsPanelFragment.FragmentTag) as AnnotationsPanelFragment;
		}

		public void UpdateTagFilterList()
		{
			var annotationPanel = GetAnnotationsPanelFragment();
			if(annotationPanel == null)
			{
				return;
			}

			annotationPanel.UpdateSelectedTagFilter(tfrvAdaptor.GetSelectedTagList());
		}

	}
}

