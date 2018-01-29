using System;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.Widget.DraggableList;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Common.Business;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Utils;
using Android.Graphics.Drawables;
using Android.Graphics;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Draggable;

namespace LexisNexis.Red.Droid.AnnotationUtility
{
	public class EditTagsListViewHolder: AbstractDraggableSwipeableItemViewHolder
	{
		private readonly EditTagsListAdaptor adapter;
		public SwipeableItemDataWrap<AnnotationTag> Tag{ set; get;}

		private readonly LinearLayout llItemContainer;
		private readonly ImageView ivTagIcon;
		private readonly TextView tvTagTitle;
		private readonly ImageView ivLeftEdit;
		private readonly ImageView ivRightDelete;
		private readonly ImageView ivMoveHandler;

		public EditTagsListViewHolder(View v, EditTagsListAdaptor adapter): base(v)
		{
			this.adapter = adapter;

			llItemContainer = v.FindViewById<LinearLayout> (Resource.Id.llItemContainer);
			ivTagIcon = v.FindViewById<ImageView> (Resource.Id.ivTagIcon);
			tvTagTitle = v.FindViewById<TextView> (Resource.Id.tvTagTitle);
			ivLeftEdit = v.FindViewById<ImageView>(Resource.Id.ivLeftEdit);
			ivRightDelete = v.FindViewById<ImageView>(Resource.Id.ivRightDelete);
			ivMoveHandler = v.FindViewById<ImageView>(Resource.Id.ivMoveHandler);

			ivLeftEdit.SetOnClickListener(new OnEditClickedListener(this));
			ivRightDelete.SetOnClickListener(new OnDeleteClickedListener(this));
		}

		private class OnDeleteClickedListener : Java.Lang.Object, View.IOnClickListener
		{
			private EditTagsListViewHolder vh;

			public OnDeleteClickedListener(EditTagsListViewHolder vh)
			{
				this.vh = vh;
			}

			public void OnClick(View v)
			{
				vh.adapter.DeleteTag(vh.Tag.Data);
			}
		}

		private class OnEditClickedListener : Java.Lang.Object, View.IOnClickListener
		{
			private EditTagsListViewHolder vh;

			public OnEditClickedListener(EditTagsListViewHolder vh)
			{
				this.vh = vh;
			}

			public void OnClick(View v)
			{
				if(vh.adapter.EditTagAction != null)
				{
					vh.adapter.EditTagAction(vh.Tag.Data);
				}
			}
		}

		public void Update()
		{
			ivTagIcon.SetImageResource(Resource.Drawable.tag_round_small_icon);
			var icon = ivTagIcon.Drawable;
			((GradientDrawable)icon).SetColor(Color.ParseColor(Tag.Data.Color));
			tvTagTitle.Text = Tag.Data.Title;

			switch(Tag.Status)
			{
			case SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Default:
				{
					ivLeftEdit.Visibility = ViewStates.Gone;
					ivRightDelete.Visibility = ViewStates.Gone;
					ivMoveHandler.Visibility = ViewStates.Visible;
				}
				break;
			case SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Left:
				{
					ivLeftEdit.Visibility = ViewStates.Gone;
					ivRightDelete.Visibility = ViewStates.Visible;
					ivMoveHandler.Visibility = ViewStates.Gone;
				}
				break;
			case SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Right:
				{
					ivLeftEdit.Visibility = ViewStates.Visible;
					ivRightDelete.Visibility = ViewStates.Gone;
					ivMoveHandler.Visibility = ViewStates.Visible;
				}
				break;
			default:
				throw new InvalidProgramException("Invalid Swipe Status");
			}

			if (((DragStateFlags & RecyclerViewDragDropManager.StateFlagIsUpdated) != 0))
			{
				int bgResId;
				if ((DragStateFlags & RecyclerViewDragDropManager.StateFlagIsActive) != 0)
				{
					bgResId = Resource.Drawable.bg_item_dragging_active_state;
				}
				else if ((DragStateFlags & RecyclerViewDragDropManager.StateFlagDragging) != 0)
				{
					bgResId = Resource.Drawable.bg_item_dragging_state;
				}
				else
				{
					bgResId = Resource.Drawable.bg_item_normal_state;
				}

				llItemContainer.SetBackgroundResource(bgResId);
			}

			SwipeItemSlideAmount = 0;
		}

		public override View SwipeableContainerView
		{
			get
			{
				return llItemContainer;
			}
		}
	}
}

