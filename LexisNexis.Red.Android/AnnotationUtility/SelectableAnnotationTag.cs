using System;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.AnnotationUtility
{
	public class SelectableAnnotationTag
	{
		public enum TagType
		{
			AllTags,
			NoTag,
			Normal,
		}

		public AnnotationTag Tag;
		public bool Selected;
		public TagType Type;

		public SelectableAnnotationTag(AnnotationTag tag, bool selected)
		{
			Tag = tag;
			Selected = selected;
			Type = TagType.Normal;
		}

		public SelectableAnnotationTag(TagType type, bool selected)
		{
			Tag = null;
			Selected = selected;
			Type = type;
		}

		public string GetId()
		{
			return Type == TagType.Normal ? Tag.TagId.ToString() : Type.ToString();
		}
	}
}

