using System;
using System.Collections.Generic;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.Business
{
	public static class SearchDisplayResultExt
	{
		public static string GetFirstLine(this SearchDisplayResult result)
		{
			return result.isDocument ? result.Head : result.TocTitle;
		}

		public static string GetSecondLine(this SearchDisplayResult result)
		{
			return result.isDocument ? result.SnippetContent : result.GuideCardTitle;
		}
	}

	public class SearchResultKeeper
	{
		private readonly int bookId;
		private readonly int tocId;
		private readonly string keywords;
		private readonly SearchResult result;

		public object SelectedResultItem
		{
			get;
			set;
		}

		public SearchResultKeeper(int bookId, int tocId, string keywords, SearchResult result)
		{
			this.bookId = bookId;
			this.tocId = tocId;
			this.keywords = keywords;
			this.result = result;
		}

		public bool IsResultOf(int bookId, int tocId, string keywords)
		{
			return this.bookId == bookId
				&& this.tocId == tocId
				&& this.keywords == keywords;
		}

		public SearchResult Result
		{
			get
			{
				return result;
			}
		}

		public int Count
		{
			get
			{
				if(result == null || result.SearchDisplayResultList == null)
				{
					return 0;
				}

				return result.SearchDisplayResultList.Count;
			}
		}

		public List<string> GetShrinkedKeywordList()
		{
			if(result == null
				|| result.FoundWordList == null)
			{
				return null;
			}

			var keywordList = new List<string>();

			if(Result.KeyWordList != null
				&& Result.KeyWordList.Count > 0)
			{
				foreach(var kw in Result.KeyWordList)
				{
					var lkw = kw.ToLower();
					if(!keywordList.Contains(lkw))
					{
						keywordList.Add(lkw);
					}
				}
			}

			foreach(var kw in Result.FoundWordList)
			{
				var lkw = kw.ToLower();
				if(!keywordList.Contains(lkw))
				{
					keywordList.Add(lkw);
				}
			}

			return keywordList;
		}

		public static bool CompareShrinkedKeywordList(List<string> l1, List<string> l2)
		{
			if(l1 == null && l2 == null)
			{
				return true;
			}

			if((l1 == null || l2 == null)
				|| l1.Count != l2.Count)
			{
				return false;
			}

			foreach(var str in l1)
			{
				if(l2.FindIndex(c => c == str) < 0)
				{
					return false;
				}
			}

			return true;
		}
	}
}

