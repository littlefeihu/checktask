using System;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using Newtonsoft.Json;
using LexisNexis.Red.Droid.Business;
using System.Collections.Generic;

namespace LexisNexis.Red.Droid.ContentPage
{
	
	public static class NavigationManagerHelper
	{
		public const int TocIdDefaultPage = -1;
		public const int TocIdError = -2;

		public static int GetCurrentBookId()
		{
			var current  = NavigationManager.Instance.CurrentRecord;
			return current == null ? -1 : current.BookID;
		}

		public static int ContentsTabGetTocId(BrowserRecord record)
		{
			switch(record.RecordType)
			{
			case RecordType.ContentRecord:
				return ((ContentBrowserRecord)record).TOCID;
			case RecordType.SearchResultRecord:
				return ((SearchBrowserRecord)record).TOCID;
			default:
				return TocIdError;
			}
		}

		public static bool IsShowContentRecord(int tocId)
		{
			return tocId >= TocIdDefaultPage;
		}

		public static BrowserRecord GetRecord(Guid id)
		{
			return NavigationManager.Instance.Records.Find(r => r.RecordID == id);
		}

		public static void MoveForthAndSetCurrentIndex(Guid id)
		{
			var list = NavigationManager.Instance.Records;
			if(list[list.Count - 1].RecordID == id)
			{
				NavigationManager.Instance.CurrentIndex = list.Count - 1;
				return;
			}

			var index = list.FindIndex(r => r.RecordID == id);
			var record = list[index];
			list.RemoveAt(index);
			list.Add(record);
			NavigationManager.Instance.CurrentIndex = list.Count - 1;
		}

		public static BrowserRecord GetLastReasonableRecord(List<RecordType> types)
		{
			if(types.Contains(NavigationManager.Instance.CurrentRecord.RecordType))
			{
				return NavigationManager.Instance.CurrentRecord;
			}

			var bookId = NavigationManager.Instance.CurrentRecord.BookID;
			for(int i = NavigationManager.Instance.CurrentIndex + 1; i < NavigationManager.Instance.Records.Count; ++i)
			{
				var r = NavigationManager.Instance.Records[i];
				if(r.BookID != bookId)
				{
					break;
				}

				if(types.Contains(r.RecordType))
				{
					return NavigationManager.Instance.Records[i];
				}
			}

			for(int i = NavigationManager.Instance.CurrentIndex - 1; i >= 0; --i)
			{
				var r = NavigationManager.Instance.Records[i];
				if(r.BookID != bookId)
				{
					break;
				}

				if(types.Contains(r.RecordType))
				{
					return NavigationManager.Instance.Records[i];
				}
			}

			return null;
		}

		public static void SetCurrentIndex(Guid id)
		{
			var index = NavigationManager.Instance.Records.FindIndex(r => r.RecordID == id);
			if(index < 0)
			{
				throw new InvalidOperationException("Unable to fin navigation item which id = " + id);
			}

			NavigationManager.Instance.CurrentIndex = index;
		}

		public static bool CompareActualTocId(int id1, int id2)
		{
			if(DataCache.INSTATNCE.Toc == null)
			{
				return false;
			}

			if(id1 == TocIdDefaultPage)
			{
				id1 = DataCache.INSTATNCE.Toc.GetFirstPage().ID;
			}

			if(id2 == TocIdDefaultPage)
			{
				id2 = DataCache.INSTATNCE.Toc.GetFirstPage().ID;
			}

			return id1 == id2;
		}

		public static bool CanBack(ContentMainFragmentState contentMainFragmentState)
		{
			switch(contentMainFragmentState.ShowingTab)
			{
			case ContentMainFragment.TabContents:
				{
					//if(NavigationManager.Instance.CurrentRecord.RecordType == RecordType.ContentRecord
					//   || NavigationManager.Instance.CurrentRecord.RecordType == RecordType.SearchResultRecord)
					//{
					//	return NavigationManager.Instance.CanBack;
					//}

					return true;
				}
			case ContentMainFragment.TabIndex:
				{
					return true;
				}
			case ContentMainFragment.TabAnnotations:
				{
					if(NavigationManager.Instance.CurrentRecord.RecordType == RecordType.AnnotationNavigator)
					{
						return NavigationManager.Instance.CanBack;
					}

					return true;
				}
			default:
				throw new InvalidProgramException("Unknown content page main fragment showing tab id[" + contentMainFragmentState.ShowingTab + "]");
			}
		}

		public static bool CanForth(ContentMainFragmentState contentMainFragmentState)
		{
			return NavigationManager.Instance.CanForth;
		}

		public static BrowserRecord Back(ContentMainFragmentState contentMainFragmentState)
		{
			switch(contentMainFragmentState.ShowingTab)
			{
			case ContentMainFragment.TabContents:
				{
					if(NavigationManager.Instance.CurrentRecord.RecordType == RecordType.ContentRecord
						|| NavigationManager.Instance.CurrentRecord.RecordType == RecordType.SearchResultRecord)
					{
						return NavigationManager.Instance.Back();
					}

					contentMainFragmentState.ShowingTab = ContentMainFragment.TabAnnotations;

					return NavigationManager.Instance.CurrentRecord;
				}
			case ContentMainFragment.TabIndex:
				{
					if(NavigationManager.Instance.CurrentRecord.RecordType == RecordType.ContentRecord
					   || NavigationManager.Instance.CurrentRecord.RecordType == RecordType.SearchResultRecord)
					{
						contentMainFragmentState.ShowingTab = ContentMainFragment.TabContents;
					}
					else
					{
						contentMainFragmentState.ShowingTab = ContentMainFragment.TabAnnotations;
					}

					return NavigationManager.Instance.CurrentRecord;
				}
			case ContentMainFragment.TabAnnotations:
				{
					if(NavigationManager.Instance.CurrentRecord.RecordType == RecordType.AnnotationNavigator)
					{
						return NavigationManager.Instance.Back();
					}

					contentMainFragmentState.ShowingTab = ContentMainFragment.TabContents;

					return NavigationManager.Instance.CurrentRecord;
				}
			default:
				throw new InvalidProgramException("Unknown content page main fragment showing tab id[" + contentMainFragmentState.ShowingTab + "]");
			}
		}

		public static BrowserRecord Forth(ContentMainFragmentState contentMainFragmentState)
		{
			return NavigationManager.Instance.Forth();
		}
	}
}

