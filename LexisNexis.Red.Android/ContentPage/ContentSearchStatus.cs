using System;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class ContentSearchStatus
	{
		public int BookId;
		public string PublicationTitle;
		public int TocId;
		public bool PopupFilter;
		public int FilterId;
		public string KeywordInEditText;
		public string LastSearchKeywords;

		public ContentSearchStatus(int bookId, string publicationTitle, int tocId)
		{
			BookId = bookId;
			PublicationTitle = publicationTitle;
			TocId = tocId;
			PopupFilter = false;
			FilterId = Resource.Id.tvFilterAll;
			KeywordInEditText = string.Empty;
			LastSearchKeywords = string.Empty;
		}
	}
}

