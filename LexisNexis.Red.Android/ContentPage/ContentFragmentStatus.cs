using System;
using System.Collections.Generic;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class ContentFragmentStatus
	{
		public class NavigationItem
		{
			public int TocId;
			public int WebViewScrollPosition;
		}

		public static ContentFragmentStatus Create()
		{
			var newStatus = new ContentFragmentStatus();

			newStatus.NavigationPath = new List<NavigationItem>();
			newStatus.CurrentIndex = 0;
			return newStatus;
		}

		public List<NavigationItem> NavigationPath;
		public int CurrentIndex;

		public NavigationItem GetCurrentNavigation()
		{
			if(NavigationPath == null || NavigationPath.Count == 0)
			{
				return null;
			}

			return NavigationPath[CurrentIndex];
		}

		public NavigationItem PreviousNavigation()
		{
			if(NavigationPath == null || NavigationPath.Count == 0)
			{
				return null;
			}

			if(CurrentIndex == 0)
			{
				return null;
			}

			CurrentIndex--;

			return NavigationPath[CurrentIndex];
		}

		public NavigationItem NextNavigation()
		{
			if(NavigationPath == null || NavigationPath.Count == 0)
			{
				return null;
			}

			if(CurrentIndex == NavigationPath.Count - 1)
			{
				return null;
			}

			CurrentIndex++;

			return NavigationPath[CurrentIndex];
		}

		public void PushNavigation(NavigationItem nav)
		{
			if(NavigationPath == null)
			{
				NavigationPath = new List<NavigationItem>();
			}

			var currentNav = GetCurrentNavigation();
			if(currentNav != null && currentNav.TocId == nav.TocId)
			{
				currentNav.WebViewScrollPosition = nav.WebViewScrollPosition;
				return;
			}

			if(NavigationPath.Count > 0 && CurrentIndex != NavigationPath.Count - 1)
			{
				NavigationPath.RemoveRange(CurrentIndex + 1, NavigationPath.Count - CurrentIndex - 1);
			}

			NavigationPath.Add(nav);
			CurrentIndex = NavigationPath.Count - 1;
		}

		public bool IsFirstNavigation()
		{
			if(NavigationPath == null || NavigationPath.Count == 0)
			{
				return true;
			}

			return CurrentIndex == 0;
		}

		public bool IsLastNavigation()
		{
			if(NavigationPath == null || NavigationPath.Count == 0)
			{
				return true;
			}

			return CurrentIndex == NavigationPath.Count - 1;
		}
	}
}

