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

namespace LexisNexis.Red.Droid.AnnotationOrganiserPage
{
	public class EditTagsListViewHolder: AbstractDraggableSwipeableItemViewHolder
	{
		private readonly EditTagsListAdaptor adapter;
		public SwipeableItemDataWrap<AnnotationTag> Tag{ set; get;}

		private readonly LinearLayout llItemContainer;
		private readonly ImageView ivTagIcon;
		private readonly TextView tvTagTitle;
		private readonly ImageView ivLeftDelete;
		private readonly ImageView ivRightDelete;
		private readonly ImageView ivMoveHandler;
		private readonly OnDeleteClickedListener onDeleteClickedListener;

		public EditTagsListViewHolder(View v, EditTagsListAdaptor adapter): base(v)
		{
			this.adapter = adapter;
			onDeleteClickedListener = new OnDeleteClickedListener(this);

			llItemContainer = v.FindViewById<LinearLayout> (Resource.Id.llItemContainer);
			ivTagIcon = v.FindViewById<ImageView> (Resource.Id.ivTagIcon);
			tvTagTitle = v.FindViewById<TextView> (Resource.Id.tvTagTitle);
			ivLeftDelete = v.FindViewById<ImageView>(Resource.Id.ivLeftDelete);
			ivRightDelete = v.FindViewById<ImageView>(Resource.Id.ivRightDelete);
			ivMoveHandler = v.FindViewById<ImageView>(Resource.Id.ivMoveHandler);

			ivLeftDelete.SetOnClickListener(onDeleteClickedListener);
			ivRightDelete.SetOnClickListener(onDeleteClickedListener);
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
				throw new NotImplementedException();
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
					ivLeftDelete.Visibility = ViewStates.Gone;
					ivRightDelete.Visibility = ViewStates.Gone;
					ivMoveHandler.Visibility = ViewStates.Visible;
				}
				break;
			case SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Left:
				{
					ivLeftDelete.Visibility = ViewStates.Gone;
					ivRightDelete.Visibility = ViewStates.Visible;
					ivMoveHandler.Visibility = ViewStates.Gone;
				}
				break;
			case SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Right:
				{
					ivLeftDelete.Visibility = ViewStates.Visible;
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

