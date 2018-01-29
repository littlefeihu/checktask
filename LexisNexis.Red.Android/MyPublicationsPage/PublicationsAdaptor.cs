using Android.Support.V7.Widget;
using Android.Views;
using System.Collections.Generic;
using Android.Widget;
using Android.App;
using Android.Content;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.Business;
using Android.Support.V4.App;
using LexisNexis.Red.Droid.Utility;


namespace LexisNexis.Red.Droid.MyPublicationsPage
{
	public class PublicationsAdaptor : RecyclerView.Adapter
	{
		private readonly List<ObjHolder<Publication>> publicationList;
		private FragmentActivity hostActivity;

		public FragmentActivity Activity
		{
			get
			{
				return hostActivity;
			}
		}

		public PublicationsAdaptor(FragmentActivity activity)
		{
			publicationList = new List<ObjHolder<Publication>>();
			hostActivity = activity;
		}

		public void setBookList(List<ObjHolder<Publication>> publicationList)
		{
			this.publicationList.Clear ();

			if(publicationList == null || publicationList.Count == 0)
			{
				NotifyDataSetChanged();
				return;
			}
			
			publicationList.ForEach (this.publicationList.Add);
			NotifyDataSetChanged();
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From (parent.Context)
				.Inflate (Resource.Layout.publication_item, parent, false);
			PublicationsAdaptorViewHolder vh = new PublicationsAdaptorViewHolder (v, this);
			return vh;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			PublicationsAdaptorViewHolder vh = (PublicationsAdaptorViewHolder)holder;
			vh.Publication = publicationList[position];
			vh.UpdateWholePublication();
		}

		public override int ItemCount
		{
			get{ return publicationList.Count; }
		}

		public override long GetItemId(int position)
		{
			return publicationList[position].Value.BookId;
		}

		public ObjHolder<Publication> At(int position)
		{
			return publicationList[position];
		}
	}
}