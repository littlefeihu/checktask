using System;
using Android.Support.V7.Widget;
using LexisNexis.Red.Common.Entity;
using System.Collections.Generic;
using LexisNexis.Red.Droid.Business;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Droid.Utility;
using Android.Graphics;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class IndexListAdaptor: BaseAdapter<Index>, ISectionIndexer
	{
		//private static readonly Color SelectedItemBackgroundColor = Color.ParseColor("#D1D1D1");
		private static readonly Color DefaultBackgroundColor = Color.ParseColor("#00ffffff");
		private JavaObjWrapper<char>[] sectionList;

		private List<Index> indexList;
		public List<Index> IndexList
		{
			get
			{
				return indexList;
			}

			private set
			{
				if(value == null || value.Count == 0)
				{
					indexList = null;
					sectionList = null;
					return;
				}

				indexList = value;
				var tempSections = new List<JavaObjWrapper<char>>();
				foreach(var i in indexList)
				{
					if(tempSections.Count == 0 || i.Title[0] != tempSections[tempSections.Count - 1].Value)
					{
						tempSections.Add(new JavaObjWrapper<char>(i.Title[0]));
					}
				}

				sectionList = tempSections.ToArray();
			}
		}

		public ContentActivity Activity
		{
			get;
			private set;
		}

		public IndexListAdaptor(ContentActivity activity)
		{
			Activity = activity;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView ?? Activity.LayoutInflater.Inflate(Resource.Layout.contentpage_indexpanel_item, parent, false);
			if(view.Tag == null)
			{
				view.Tag = new JavaObjWrapper<Tuple<TextView>>(
					new Tuple<TextView>(
						view.FindViewById<TextView>(Resource.Id.tvIndex)));
			}

			var holder = ((JavaObjWrapper<Tuple<TextView>>)view.Tag);
			holder.Value.Item1.Text = IndexList[position].Title;
			holder.Value.Item1.SetBackgroundColor(DefaultBackgroundColor);

			return view;
		}

		public override int Count
		{
			get
			{
				return IndexList == null ? 0 : IndexList.Count;
			}
		}

		public override Index this[int index]
		{
			get
			{
				return IndexList == null ? null : IndexList[index];
			}
		}

		public int GetPositionForSection(int sectionIndex)
		{
			var c = sectionList[sectionIndex].Value;
			for(int i = 0; i < indexList.Count; ++i)
			{
				if(c == indexList[i].Title[0])
				{
					return i;
				}
			}

			return -1;
		}

		public int GetSectionForPosition(int position)
		{
			var c = IndexList[position].Title[0];
			for(int i = 0; i < sectionList.Length; ++i)
			{
				if(c == sectionList[i].Value)
				{
					return i;
				}
			}

			return -1;
		}

		public Java.Lang.Object[] GetSections()
		{
			return sectionList;
		}

		public void RefreshIndexList()
		{
			IndexList = DataCache.INSTATNCE.IndexList.IndexList;
			NotifyDataSetChanged();
		}
	}
}

