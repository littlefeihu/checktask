using System;
using System.Collections.Generic;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac
{
	public class SearchNavigationData
	{
		public List<string> FoundWordList { get; set; }
		public SearchDisplayResult SearchItem { get; set; }

		public SearchNavigationData ()
		{
			if (FoundWordList == null) {
				FoundWordList = new List<string> (0);
			} else {
				FoundWordList.Clear ();
			}

			if (SearchItem == null) {
				SearchItem = new SearchDisplayResult ();
			}
		}
	}
}

