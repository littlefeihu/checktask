using Android.OS;
using Android.Views;
using DialogFragment=Android.Support.V4.App.DialogFragment;
using System.Collections.Generic;
using LexisNexis.Red.Common.BusinessModel;
using System;
using Android.Support.V7.Widget;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Touchguard;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Draggable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Animator;
using Android.Widget;
using LexisNexis.Red.Common.Business;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Utils;

namespace LexisNexis.Red.Droid.AnnotationUtility
{
	public class EditTagsDialogFragment: DialogFragment
	{
		public const string EditTagsDialogFragmentTag = "editTagsDialogFragment";

		private RecyclerView rcAnnotationList;

		private static EditTagsListAdaptor editTagsListAdaptor;

		private RecyclerViewTouchActionGuardManager tagm;
		private RecyclerViewDragDropManager ddm;
		private RecyclerViewSwipeManager sm;
		private GeneralItemAnimator ia;
		private RecyclerView.Adapter wrappedAdapter;
		private LinearLayoutManager llm;

		public static EditTagsDialogFragment NewInstance()
		{
			EditTagsDialogFragment.editTagsListAdaptor = null;
			var fragment = new EditTagsDialogFragment();
			return fragment;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
			Dialog.SetCanceledOnTouchOutside(true);

			var vwDialog = inflater.Inflate(Resource.Layout.annotation_edittags_popup, container);

			rcAnnotationList = vwDialog.FindViewById<RecyclerView>(Resource.Id.rcAnnotationList);
			vwDialog.FindViewById<View>(Resource.Id.ivAddTag).Click += delegate
			{
				NewEditTagDialogFragment.NewInstance().Show(
					Activity.SupportFragmentManager.BeginTransaction(),
					NewEditTagDialogFragment.NewEditTagDialogFragmentTag);
				Dismiss();
			};

			llm = new LinearLayoutManager(Activity);
			llm.Orientation = LinearLayoutManager.Vertical;
			llm.ScrollToPosition(0);
			rcAnnotationList.SetLayoutManager(llm);

			tagm = new RecyclerViewTouchActionGuardManager();
			tagm.SetInterceptVerticalScrollingWhileAnimationRunning(true);
			tagm.Enabled = true;

			ddm = new RecyclerViewDragDropManager();
			ddm.SetInitiateOnLongPress(true);
			ddm.SetInitiateOnMove(false);

			sm = new RecyclerViewSwipeManager();
			sm.UserHandleSwipeUi = true;

			if(editTagsListAdaptor == null)
			{
				editTagsListAdaptor = new EditTagsListAdaptor(OnEditTag, OnDeleteTag, OnSortTag);
			}

			editTagsListAdaptor.SetTagList(AnnCategoryTagUtil.Instance.GetTags());

			wrappedAdapter = ddm.CreateWrappedAdapter(editTagsListAdaptor);
			wrappedAdapter = sm.CreateWrappedAdapter(wrappedAdapter);

			ia = new SwipeDismissItemAnimator();

			rcAnnotationList.SetAdapter(wrappedAdapter);
			rcAnnotationList.SetItemAnimator(ia);

			tagm.AttachRecyclerView(rcAnnotationList);
			sm.AttachRecyclerView(rcAnnotationList);
			ddm.AttachRecyclerView(rcAnnotationList);

			return vwDialog;
		}

		public override void OnDestroyView()
		{
			base.OnDestroyView();

			if(tagm != null)
			{
				tagm.Release();
				tagm = null;
			}

			if(ddm != null)
			{
				ddm.Release();
				ddm = null;
			}

			if(sm != null)
			{
				sm.Release();
				sm = null;
			}

			if(wrappedAdapter != null)
			{
				WrapperAdapterUtils.ReleaseAll(wrappedAdapter);
				wrappedAdapter = null;
			}

			ia = null;
			llm = null;
		}

		private void OnEditTag(AnnotationTag tag)
		{
			NewEditTagDialogFragment.NewInstance(
				tag.TagId.ToString(),
				tag.Title,
				tag.Color).Show(
					Activity.SupportFragmentManager.BeginTransaction(),
					NewEditTagDialogFragment.NewEditTagDialogFragmentTag);
			Dismiss();
		}

		private void OnDeleteTag(AnnotationTag tag)
		{
			AnnCategoryTagUtil.Instance.DeleteTag(tag.TagId);
			NotifyTagListChanged();
		}

		private void OnSortTag(IEnumerable<Guid> ids)
		{
			AnnCategoryTagUtil.Instance.Sort(ids);
			NotifyTagListChanged();
		}

		private void NotifyTagListChanged()
		{
			var updateListener = Activity as ITagListUpdateListener;
			if(updateListener != null)
			{
				updateListener.OnTagListUpdated();
			}
		}
	}
}

