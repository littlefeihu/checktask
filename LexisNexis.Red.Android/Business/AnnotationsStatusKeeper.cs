using System;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.ContentPage;

namespace LexisNexis.Red.Droid.Business
{
	public class AnnotationsStatusKeeper
	{
		public bool IsShow
		{
			get;
			set;
		}

		public Guid NavigationRecordId
		{
			get;
			private set;
		}

		public void BindNavigationItem()
		{
			NavigationRecordId = NavigationManager.Instance.CurrentRecord.RecordID;
		}

		public bool IsCurrentNavigationItem()
		{
			if(NavigationManager.Instance.CurrentRecord == null)
			{
				return false;
			}

			return NavigationRecordId == NavigationManager.Instance.CurrentRecord.RecordID;
		}

		public BrowserRecord GetNavigationItem()
		{
			return NavigationManagerHelper.GetRecord(NavigationRecordId);
		}

		public bool IsBindNavigationItem()
		{
			return NavigationRecordId != Guid.Empty;
		}
	}
}

