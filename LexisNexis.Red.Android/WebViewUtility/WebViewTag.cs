using System;
using LexisNexis.Red.Common.HelpClass;
using System.Collections.Generic;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.ContentPage;

namespace LexisNexis.Red.Droid.WebViewUtility
{
	public class WebViewTag
	{
		public static WebViewTag CreateWebViewTagByTOC(
			int bookId,
			int tocId)
		{
			var result = new WebViewTag{
				Email = GlobalAccess.Instance.CurrentUserInfo.Email,
				CountryCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode,
				BookId = bookId,
				TOCIdList = new List<int>(),
				IndexTitle = null,
			};

			result.TOCIdList.Add(tocId);
			return result;
		}

		public static WebViewTag CreateWebViewTagByIndex(int bookId, string indexTitle)
		{
			return new WebViewTag{
				Email = GlobalAccess.Instance.CurrentUserInfo.Email,
				CountryCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode,
				BookId = bookId,
				TOCIdList = null,
				IndexTitle = indexTitle,
			};
		}

		private Guid navigationRecordId;

		public Guid NavigationRecordId
		{
			get
			{
				return navigationRecordId;
			}
		}

		public string Email
		{
			get;
			set;
		}

		public string CountryCode
		{
			get;
			set;
		}

		public int BookId
		{
			get;
			set;
		}

		public List<int> TOCIdList
		{
			get;
			set;
		}

		public string IndexTitle
		{
			get;
			set;
		}

		public bool HasTOC(int bookId, int tocId)
		{
			if(Email == GlobalAccess.Instance.CurrentUserInfo.Email
				&& CountryCode == GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode
			   && BookId == bookId
			   && IndexTitle == null
				&& TOCIdList != null
				&& TOCIdList.Count >= 0)
			{
				return TOCIdList.FindIndex(id => id == tocId) >= 0;
			}

			return false;
		}

		public bool IsCurrentTOC(int bookId, int tocId)
		{
			if(Email == GlobalAccess.Instance.CurrentUserInfo.Email
				&& CountryCode == GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode
				&& BookId == bookId
				&& IndexTitle == null
				&& TOCIdList != null
				&& TOCIdList.Count > 0)
			{
				return TOCIdList[0] == tocId;
			}

			return false;
		}

		public int GetCurrentTocId()
		{
			if(TOCIdList != null && TOCIdList.Count > 0)
			{
				return TOCIdList[0];
			}

			return -1;
		}

		public int GetMinTOCId(int bookId)
		{
			if(Email == GlobalAccess.Instance.CurrentUserInfo.Email
				&& CountryCode == GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode
				&& BookId == bookId
				&& IndexTitle == null
				&& TOCIdList != null
				&& TOCIdList.Count > 0)
			{
				var minTocId = -1;
				foreach(var tocId in TOCIdList)
				{
					if(minTocId < 0 || minTocId > tocId)
					{
						minTocId = tocId;
					}
				}

				return minTocId;
			}

			return -1;
		}

		public int GetMaxTOCId(int bookId)
		{
			if(Email == GlobalAccess.Instance.CurrentUserInfo.Email
				&& CountryCode == GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode
				&& BookId == bookId
				&& IndexTitle == null
				&& TOCIdList != null
				&& TOCIdList.Count > 0)
			{
				var maxTocId = -1;
				foreach(var tocId in TOCIdList)
				{
					if(maxTocId < 0 || maxTocId < tocId)
					{
						maxTocId = tocId;
					}
				}

				return maxTocId;
			}

			return -1;
		}

		public void SetCurrentToc(int bookId, int tocId)
		{
			if(Email != GlobalAccess.Instance.CurrentUserInfo.Email
				|| CountryCode != GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode
				|| BookId != bookId
				|| IndexTitle != null
				|| TOCIdList == null
				|| TOCIdList.Count == 0)
			{
				throw new InvalidOperationException("Invalid tag status.");
			}

			TOCIdList.Remove(tocId);
			TOCIdList.Insert(0, tocId);
		}

		public bool IsSameIndex(int bookId, string indexTitle)
		{
			return Email == GlobalAccess.Instance.CurrentUserInfo.Email
				&& CountryCode == GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode
				&& BookId == bookId
				&& TOCIdList == null
				&& IndexTitle == indexTitle;
		}

		public bool IsSameIndexPage(int bookId, string indexTitle)
		{
			return Email == GlobalAccess.Instance.CurrentUserInfo.Email
				&& CountryCode == GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode
				&& BookId == bookId
				&& TOCIdList == null
				&& IndexTitle.Substring(0, 1) == indexTitle.Substring(0, 1);
		}

		public void DebugList()
		{
			string tocIdList = "";
			foreach(var id in TOCIdList)
			{
				tocIdList += id + ",";
			}

			Android.Util.Log.Debug("dbg", "WebView tag TOC lis: " + tocIdList);
		}

		public void BindNavigationItem()
		{
			navigationRecordId = NavigationManager.Instance.CurrentRecord.RecordID;
		}

		public bool IsBindNavigationItem()
		{
			return NavigationManager.Instance.Records.FindIndex(r => r.RecordID == navigationRecordId) >= 0;
		}

		public bool IsCurrentNavigationItem()
		{
			if(NavigationManager.Instance.CurrentRecord == null)
			{
				return false;
			}

			return navigationRecordId == NavigationManager.Instance.CurrentRecord.RecordID;
		}

		public BrowserRecord GetNavigationItem()
		{
			return NavigationManagerHelper.GetRecord(navigationRecordId);
		}
	}
}

