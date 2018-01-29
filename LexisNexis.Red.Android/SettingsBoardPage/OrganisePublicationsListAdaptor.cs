using System;
using Android.Support.V7.Widget;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Draggable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using System.Collections.Generic;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using Android.Views;

namespace LexisNexis.Red.Droid.SettingsBoardPage
{
	public class OrganisePublicationsListAdaptor: RecyclerView.Adapter, IDraggableItemAdapter, ISwipeableItemAdapter
	{
		private List<SwipeableItemDataWrap<ObjHolder<Publication>>> pubList;

		private Action<ObjHolder<Publication>> deletePubAction;
		private Action<int, int, List<int>> sortPubAction;

		public OrganisePublicationsListAdaptor(
			Action<ObjHolder<Publication>> deletePubAction,
			Action<int, int, List<int>> sortPubAction)
		{
			this.deletePubAction = deletePubAction;
			this.sortPubAction = sortPubAction;

			pubList = new List<SwipeableItemDataWrap<ObjHolder<Publication>>>();
			HasStableIds = true;
		}

		public void SetPubList(List<ObjHolder<Publication>> pubList)
		{
			var oldPubList = this.pubList;
			this.pubList = new List<SwipeableItemDataWrap<ObjHolder<Publication>>>();

			if(pubList != null || pubList.Count > 0)
			{
				pubList.ForEach(at =>
				{
					var foundOldPub = oldPubList.Find(old => old.Data.Value.BookId == at.Value.BookId);
					this.pubList.Add(foundOldPub ?? new SwipeableItemDataWrap<ObjHolder<Publication>>(at));
				});
			}

			NotifyDataSetChanged();
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var pubViewHoler = (OrganisePublicationsListViewHolder)holder;
			pubViewHoler.Pub = pubList[position];
			pubViewHoler.Update();
		}

		public override int ItemCount
		{
			get
			{
				return pubList.Count;
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			LayoutInflater inflater = LayoutInflater.From(parent.Context);
			var itemView = inflater.Inflate(Resource.Layout.organise_pub_list_item, parent, false);
			return new OrganisePublicationsListViewHolder(itemView, this);
		}

		public override long GetItemId(int position)
		{
			return pubList[position].Data.Value.BookId;
		}

		public bool OnCheckCanStartDrag(Java.Lang.Object p0, int position, int x, int y, bool longPressed)
		{
			var holder = p0 as OrganisePublicationsListViewHolder;
			return holder != null
				&& longPressed
				&& pubList[position].Status == SwipeableItemDataWrap<ObjHolder<Publication>>.SwipeStatus.Default;
		}

		public ItemDraggableRange OnGetItemDraggableRange(Java.Lang.Object p0, int p1)
		{
			return null;
		}

		public void OnMoveItem(int fromPosition, int toPosition)
		{
			var movePub = pubList[fromPosition].Data.Value;
			pubList[fromPosition].Data.Value = pubList[toPosition].Data.Value;
			pubList[toPosition].Data.Value = movePub;

			var ids = new List<int>(pubList.Count);
			foreach(var t in pubList)
			{
				ids.Add(t.Data.Value.BookId);
			}

			if(sortPubAction != null)
			{
				sortPubAction(fromPosition, toPosition, ids);
			}

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
			var holder = p0 as OrganisePublicationsListViewHolder;
			var pub = pubList[position];

			switch (result) {
			case RecyclerViewSwipeManager.ResultSwipedRight:
				{
					if(pub.Status == SwipeableItemDataWrap<ObjHolder<Publication>>.SwipeStatus.Left)
					{
						pub.Status = SwipeableItemDataWrap<ObjHolder<Publication>>.SwipeStatus.Default;
					}
					else
					{
						pub.Status = SwipeableItemDataWrap<ObjHolder<Publication>>.SwipeStatus.Right;
					}
				}
				break;
			case RecyclerViewSwipeManager.ResultSwipedLeft:
				{
					if(pub.Status == SwipeableItemDataWrap<ObjHolder<Publication>>.SwipeStatus.Right)
					{
						pub.Status = SwipeableItemDataWrap<ObjHolder<Publication>>.SwipeStatus.Default;
					}
					else
					{
						pub.Status = SwipeableItemDataWrap<ObjHolder<Publication>>.SwipeStatus.Left;
					}
				}
				break;
			}

			holder.Update();
			return RecyclerViewSwipeManager.AfterSwipeReactionDefault;
		}

		public void DeletePub(ObjHolder<Publication> pub)
		{
			var index = pubList.FindIndex(at => at.Data.Value.BookId == pub.Value.BookId);

			if(index >= 0)
			{
				if(deletePubAction != null)
				{
					deletePubAction(pub);
				}
			}
		}

		public void DeletePub(int bookId)
		{
			var index = pubList.FindIndex(at => at.Data.Value.BookId == bookId);

			if(index >= 0)
			{
				pubList.RemoveAt(index);
				NotifyItemRemoved(index);
			}
		}
	}
}

