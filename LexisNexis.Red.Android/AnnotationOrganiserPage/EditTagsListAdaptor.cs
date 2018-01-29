using System;
using Android.Support.V7.Widget;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Draggable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using System.Collections.Generic;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using Android.Views;

namespace LexisNexis.Red.Droid.AnnotationOrganiserPage
{
	public class EditTagsListAdaptor: RecyclerView.Adapter, IDraggableItemAdapter, ISwipeableItemAdapter
	{
		private readonly List<SwipeableItemDataWrap<AnnotationTag>> tagList;
		private readonly EditTagsActivity hostActivity;

		public EditTagsListAdaptor(EditTagsActivity hostActivity)
		{
			this.hostActivity = hostActivity;
			tagList = new List<SwipeableItemDataWrap<AnnotationTag>>();
			HasStableIds = true;
		}

		public void SetTagList(List<AnnotationTag> tagList)
		{
			this.tagList.Clear();

			if(tagList != null || tagList.Count > 0)
			{
				tagList.ForEach(at => this.tagList.Add(new SwipeableItemDataWrap<AnnotationTag>(at)));
			}

			NotifyDataSetChanged();
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var tagViewHoler = (EditTagsListViewHolder)holder;
			tagViewHoler.Tag = tagList[position];
			tagViewHoler.Update();
		}

		public override int ItemCount
		{
			get
			{
				return tagList.Count;
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			LayoutInflater inflater = LayoutInflater.From(parent.Context);
			var itemView = inflater.Inflate(Resource.Layout.annotations_edittags_listitem, parent, false);
			return new EditTagsListViewHolder(itemView, this);
		}

		public override long GetItemId(int position)
		{
			return Conversion.String2MD5Long(tagList[position].Data.TagId.ToString("N"));
		}

		public bool OnCheckCanStartDrag(Java.Lang.Object p0, int position, int x, int y, bool longPressed)
		{
			Console.WriteLine("OnCheckCanStartDrag");
			var holder = p0 as EditTagsListViewHolder;
			return holder != null && longPressed;
		}

		public ItemDraggableRange OnGetItemDraggableRange(Java.Lang.Object p0, int p1)
		{
			return null;
		}

		public void OnMoveItem(int fromPosition, int toPosition)
		{
			var item = tagList[fromPosition];
			tagList.RemoveAt(fromPosition);
			tagList.Insert(toPosition, item);

			NotifyDataSetChanged();
		}

		public int OnGetSwipeReactionType(Java.Lang.Object p0, int p1, int p2, int p3)
		{
			return RecyclerViewSwipeManager.ReactionCanSwipeBoth;
		}

		public void OnPerformAfterSwipeReaction(Java.Lang.Object p0, int p1, int p2, int p3)
		{
		}

		public void OnSetSwipeBackground(Java.Lang.Object p0, int p1, int p2)
		{
		}

		public int OnSwipeItem(Java.Lang.Object p0, int position, int result)
		{
			var holder = p0 as EditTagsListViewHolder;
			var tag = tagList[position];

			switch (result) {
			case RecyclerViewSwipeManager.ResultSwipedRight:
				{
					if(tag.Status == SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Left)
					{
						tag.Status = SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Default;
					}
					else
					{
						tag.Status = SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Right;
					}
				}
				break;
			case RecyclerViewSwipeManager.ResultSwipedLeft:
				{
					if(tag.Status == SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Right)
					{
						tag.Status = SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Default;
					}
					else
					{
						tag.Status = SwipeableItemDataWrap<AnnotationTag>.SwipeStatus.Left;
					}
				}
				break;
			}

			holder.Update();
			return RecyclerViewSwipeManager.AfterSwipeReactionDefault;
		}
	}
}

