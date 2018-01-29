using System;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class ContentActivityStatus
	{
		public bool IsShowLegalDefinePopup;

		public ContentActivityStatus()
		{
			this.IsShowLegalDefinePopup = false;
		}

		public ContentActivityStatus(bool isShowLegalDefinePopup)
		{
			this.IsShowLegalDefinePopup = isShowLegalDefinePopup;
		}
	}
}

