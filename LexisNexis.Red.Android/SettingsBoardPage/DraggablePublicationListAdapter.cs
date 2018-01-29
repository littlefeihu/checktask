using System;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.Widget.DraggableList;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using LexisNexis.Red.Droid.Business;
using Android.Support.V4.App;
using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.Droid.SettingsBoardPage
{
	public class DraggablePublicationListAdapter: RecyclerView.Adapter, DraggableRecyclerView.DraggableAdapter
	{
		private FragmentActivity context;
		public FragmentActivity Activity
		{
			get
			{
				return context;
			}
		}

		public DraggablePublicationListAdapter (FragmentActivity context)
		{
			this.context = context;
			this.HasStableIds = true;
		}

		#region DraggableAdapter implementation

		public void ShiftItems(int mobileItemIndex, int targetItemIndex)
		{
			var movePub = DataCache.INSTATNCE.PublicationManager.PublicationList[mobileItemIndex].Value;
			DataCache.INSTATNCE.PublicationManager.PublicationList[mobileItemIndex].Value
			= DataCache.INSTATNCE.PublicationManager.PublicationList[targetItemIndex].Value;
			DataCache.INSTATNCE.PublicationManager.PublicationList[targetItemIndex].Value = movePub;
		}

		public void MoveFinished()
		{
			// Call api
			List<int> bookIdList = new List<int>(DataCache.INSTATNCE.PublicationManager.PublicationList.Count);
			DataCache.INSTATNCE.PublicationManager.PublicationList.ForEach(pubHoler => bookIdList.Add(pubHoler.Value.BookId));
			PublicationUtil.Instance.OrganiseDlsOrder(bookIdList);
		}

		#endregion

		#region implemented abstract members of Adapter
		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			DraggablePublicationListViewHolder vh = (DraggablePublicationListViewHolder)holder;
			vh.Publication = DataCache.INSTATNCE.PublicationManager.PublicationList[position];
			vh.Update();
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From (parent.Context)
				.Inflate (Resource.Layout.organise_pub_list_item, parent, false);
			DraggablePublicationListViewHolder vh = new DraggablePublicationListViewHolder(v, this);
			return vh;
		}

		public override int ItemCount
		{
			get
			{
				return DataCache.INSTATNCE.PublicationManager.IsPublicationListEmpty() ? 0 : DataCache.INSTATNCE.PublicationManager.PublicationList.Count;
			}
		}

		public override long GetItemId(int position)
		{
			return (long)(DataCache.INSTATNCE.PublicationManager.PublicationList[position].Value.BookId);
		}
		#endregion
	}
}

