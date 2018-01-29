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
using System;

namespace LexisNexis.Red.Droid.AnnotationUtility
{
	public class TagFilterAdaptor : RecyclerView.Adapter
	{
		private List<SelectableAnnotationTag> tagList;
		private readonly Fragment fragment;

		public Fragment Fragment
		{
			get
			{
				return fragment;
			}
		}

		public TagFilterAdaptor(Fragment fragment)
		{
			tagList = new List<SelectableAnnotationTag>();
			this.fragment = fragment;
		}

		public void SetTagList(List<AnnotationTag> rowTagList)
		{
			var oldTagList = tagList;
			tagList = new List<SelectableAnnotationTag>();

			var found = oldTagList.Find(t => t.Type == SelectableAnnotationTag.TagType.AllTags);
			tagList.Add(new SelectableAnnotationTag(
				SelectableAnnotationTag.TagType.AllTags, found == null || found.Selected));
			found = oldTagList.Find(t => t.Type == SelectableAnnotationTag.TagType.NoTag);
			tagList.Add(new SelectableAnnotationTag(
				SelectableAnnotationTag.TagType.NoTag, found == null || found.Selected));

			if(rowTagList != null && rowTagList.Count > 0)
			{
				rowTagList.ForEach (t =>{
					found = oldTagList.Find(ot => ot.Tag != null && ot.Tag.TagId == t.TagId);
					tagList.Add(new SelectableAnnotationTag(t, found == null || found.Selected));
				});
			}

			NotifyDataSetChanged();
		}

		public void SetSelectedTagFilterList(List<string> selectedTagFilterList)
		{
			if(selectedTagFilterList == null)
			{
				tagList.ForEach(t => t.Selected = t.Type == SelectableAnnotationTag.TagType.AllTags);
			}
			else
			{
				tagList.ForEach(t => t.Selected = selectedTagFilterList.Find(id => id == t.GetId()) != null);
			}

			NotifyDataSetChanged();
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View v = LayoutInflater.From (parent.Context)
				.Inflate (Resource.Layout.annotationlist_tagitem, parent, false);
			var vh = new TagFilterAdaptorViewHolder (v, this);
			return vh;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var vh = (TagFilterAdaptorViewHolder)holder;
			vh.Tag = tagList[position];
			vh.Update();
		}

		public override int ItemCount
		{
			get{ return tagList.Count; }
		}

		public List<AnnotationTag> GetSelectedTags()
		{
			var tags = new List<AnnotationTag>();
			tagList.ForEach(t =>
			{
				if(t.Selected)
				{
					tags.Add(t.Tag);
				}
			});

			return tags;
		}

		public void ItemSelectChanged(TagFilterAdaptorViewHolder holder)
		{
			if(holder.Tag.Type == SelectableAnnotationTag.TagType.AllTags)
			{
				var currentAllTagSelected = holder.Tag.Selected;
				tagList.ForEach(t => t.Selected = currentAllTagSelected);
				NotifyDataSetChanged();
			}
			else
			{
				if(!holder.Tag.Selected && tagList[0].Selected)
				{
					tagList[0].Selected = false;
					NotifyDataSetChanged();
				}
				else
				{
					holder.Update();
				}
			}

			var listener = Fragment as ITagFilterListener;
			if(listener != null)
			{
				listener.UpdateTagFilterList();
			}
		}

		public List<string> GetSelectedTagList()
		{
			var result = new List<string>();
			foreach(var t in tagList)
			{
				if(t.Selected)
				{
					result.Add(t.GetId());
				}
			}

			return result;
		}
	}
}