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
using Fragment=Android.Support.V4.App.Fragment;

namespace LexisNexis.Red.Droid.AnnotationUtility
{
	public class AnnotationListAdaptor : RecyclerView.Adapter
	{
		private List<Annotation> annotationList;
		private readonly Fragment fragment;
		private readonly int itemRes;
		private readonly int tagContainerTotalWidthMargins;
		private int itemTotalWidth;

		public Fragment Fragment
		{
			get
			{
				return fragment;
			}
		}

		public int ItemTotalWidth
		{
			get
			{
				return itemTotalWidth;
			}

			set
			{
				if(itemTotalWidth == value)
				{
					return;
				}

				itemTotalWidth = value;
				NotifyDataSetChanged();
			}
		}

		public int TagContainerWidth
		{
			get
			{
				return itemTotalWidth - tagContainerTotalWidthMargins;
			}
		}

		public AnnotationListAdaptor(Fragment fragment, int itemRes, int tagContainerTotalWidthMargins)
		{
			this.fragment = fragment;
			this.itemRes = itemRes;
			this.tagContainerTotalWidthMargins = tagContainerTotalWidthMargins;
		}

		public void SetAnnotationList(List<Annotation> annotationList)
		{
			this.annotationList = annotationList;
			NotifyDataSetChanged();
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From (parent.Context)
				.Inflate (itemRes, parent, false);
			v.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
			var vh = new AnnotationListAdaptorViewHolder (v, this);
			return vh;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var vh = (AnnotationListAdaptorViewHolder)holder;
			vh.Annotation = annotationList[position];
			vh.Update();
		}

		public override int ItemCount
		{
			get{ return ItemTotalWidth == 0 || annotationList == null ? 0 : annotationList.Count; }
		}
	}
}