using System;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Utils;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Draggable;

namespace LexisNexis.Red.Droid.SettingsBoardPage
{
	public class OrganisePublicationsListViewHolder: AbstractDraggableSwipeableItemViewHolder
	{
		private readonly OrganisePublicationsListAdaptor adapter;
		public SwipeableItemDataWrap<ObjHolder<Publication>> Pub{ set; get;}

		private readonly LinearLayout llItemContainer;
		private readonly TextView tvTitle;
		private readonly ImageView ivLeftDelete;
		private readonly ImageView ivRightDelete;
		private readonly ImageView ivMoveHandler;

		private OnDeleteClickedListener onDeleteClickedListener;

		public OrganisePublicationsListViewHolder(View v, OrganisePublicationsListAdaptor adapter): base(v)
		{
			this.adapter = adapter;

			onDeleteClickedListener = new OnDeleteClickedListener(this);

			llItemContainer = v.FindViewById<LinearLayout> (Resource.Id.llItemContainer);

			tvTitle = v.FindViewById<TextView> (Resource.Id.tvTitle);
			ivLeftDelete = v.FindViewById<ImageView>(Resource.Id.ivLeftDelete);
			ivRightDelete = v.FindViewById<ImageView>(Resource.Id.ivRightDelete);
			ivMoveHandler = v.FindViewById<ImageView>(Resource.Id.ivMoveHandler);

			ivLeftDelete.SetOnClickListener(onDeleteClickedListener);
			ivRightDelete.SetOnClickListener(onDeleteClickedListener);
		}

		private class OnDeleteClickedListener : Java.Lang.Object, View.IOnClickListener
		{
			private OrganisePublicationsListViewHolder vh;

			public OnDeleteClickedListener(OrganisePublicationsListViewHolder vh)
			{
				this.vh = vh;
			}

			public void OnClick(View v)
			{
				vh.adapter.DeletePub(vh.Pub.Data);
			}
		}

		public void Update()
		{
			tvTitle.Text = Pub.Data.Value.Name;

			switch(Pub.Status)
			{
			case SwipeableItemDataWrap<ObjHolder<Publication>>.SwipeStatus.Default:
				{
					ivLeftDelete.Visibility = ViewStates.Gone;
					ivRightDelete.Visibility = ViewStates.Gone;
					ivMoveHandler.Visibility = ViewStates.Visible;
				}
				break;
			case SwipeableItemDataWrap<ObjHolder<Publication>>.SwipeStatus.Left:
				{
					ivLeftDelete.Visibility = ViewStates.Gone;
					ivRightDelete.Visibility = ViewStates.Visible;
					ivMoveHandler.Visibility = ViewStates.Gone;
				}
				break;
			case SwipeableItemDataWrap<ObjHolder<Publication>>.SwipeStatus.Right:
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

