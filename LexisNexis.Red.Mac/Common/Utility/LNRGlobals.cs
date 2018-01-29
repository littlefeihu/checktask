using System;
using Foundation;

namespace LexisNexis.Red.Mac
{
	public static class LNRConstants
	{
		public static readonly NSString LNPublicationDidDeleteNotification = new NSString("PUBLICATION_DELETE_NOTIFICATION");
		public static readonly NSString LNChangeHistoryItemNotification = new NSString("CHANGE_HISTORYITEM_NOTIFICATION");
		public static readonly NSString LNPublicationDidFinishedDownload = new NSString("PUBLICATION_FINISHDOWNLOAD");
		public static readonly int ID_CLOSE = 200;
		public static readonly int TOCLEVEL_MAX = 7;
		public static readonly float TOCITEMHEIGHT_MIN = 44;
		public static readonly string TITLE_CONTENT = "Contents";
		public static readonly string TITLE_INDEX = "Index";
		public static readonly string TITLE_ANNOTATIONS = "Annotations";
		public static readonly string TITLE_ALL = "All";
		public static readonly string TITLE_NOTES = "Notes";
		public static readonly string TITLE_HIGHLIGHTS = "Highlights";
		public static readonly string TITLE_ORPHANS = "Orphans";
		public static readonly nint NSModalResponseStop     = (-1000); // Also used as the default response for sheets
		public static readonly nint NSModalResponseAbort    = (-1001);
		public static readonly nint NSModalResponseContinue = (-1002);
		public static readonly nint ContentFont_MIN = 11;
		public static readonly nint ContentFont_MAX = 17;
		public static readonly nfloat WindowHeight_MIN=600;
		public static readonly string LOADING_INFO = "Loading, Please wait.";
		public static readonly string LOADING_SEARCH_INFO = "Search results loading, please wait.";

	}
}

