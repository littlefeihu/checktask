using System;

using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.iOS
{
	public class Tag
	{
		public AnnotationTag AnnoTag{ get; set; }
		public bool IsSelected{ get; set;}

		public Tag (AnnotationTag at, bool isSelected = false)
		{
			AnnoTag = at;
			IsSelected = isSelected;
		}
	}
}

