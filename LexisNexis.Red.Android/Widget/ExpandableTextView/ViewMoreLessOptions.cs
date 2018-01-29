using System;

namespace LexisNexis.Red.Droid
{
	public class ViewMoreLessOptions
	{
		public string ViewMore
		{
			get;
			set;
		}

		public string ViewLess
		{
			get;
			set;
		}

		public ViewMoreLessOptions(string viewMore, string viewLess)
		{
			ViewMore = viewMore;
			ViewLess = viewLess;
		}
	}
}

