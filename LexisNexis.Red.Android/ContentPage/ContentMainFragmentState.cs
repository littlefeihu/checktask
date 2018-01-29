using System;
using Newtonsoft.Json;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class ContentMainFragmentState
	{
		public ContentMainFragmentState()
		{
			LeftPanelOpen = true;
			ShowingTab = ContentMainFragment.TabContents;
		}

		[JsonProperty("LeftPanelOpen")]
		public bool LeftPanelOpen
		{
			get;
			set;
		}

		[JsonProperty("ShowingTab")]
		public int ShowingTab
		{
			get;
			set;
		}
	}
}

