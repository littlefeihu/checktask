using System;
using Android.Support.V7.Widget;
using LexisNexis.Red.Common.Entity;
using System.Collections.Generic;
using LexisNexis.Red.Droid.Business;
using Android.Views;
using Android.Widget;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class TOCListAdaptor : RecyclerView.Adapter
	{
		public List<TOCNode> NodeList
		{
			get;
			private set;
		}

		public ContentActivity Activity
		{
			get;
			private set;
		}

		public TOCListAdaptor(ContentActivity activity)
		{
			Activity = activity;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From (parent.Context)
				.Inflate (Resource.Layout.contentpage_tocpanel_item, parent, false);
			v.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			var vh = new TOCListAdaptorViewHolder (v, this);
			return vh;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			TOCListAdaptorViewHolder vh = (TOCListAdaptorViewHolder)holder;
			vh.Node = NodeList[position];
			vh.Update();
		}

		public override int ItemCount
		{
			get{ return NodeList == null ? 0 : NodeList.Count; }
		}

		public void SetCurrentNode(TOCNode current, bool expanded)
		{
			DataCache.INSTATNCE.Toc.CurrentTOCNode = current;
			DataCache.INSTATNCE.Toc.CurrentNodeExpanded = expanded;
			RefreshNodeList();
		}

		public void RefreshNodeList()
		{
			if(DataCache.INSTATNCE.Toc == null)
			{
				NodeList = null;
			}
			else
			{
				NodeList = DataCache.INSTATNCE.Toc.GetPresentNodeList();
			}

			NotifyDataSetChanged();
		}
	}
}

