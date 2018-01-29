using System;

namespace LexisNexis.Red.Droid.ContentPage
{
	public interface IActionModeTarget
	{
		void DoActionMode(int actionId);
		void AfterDoActionMode(int actionId);
		void OnUserDismissLegalDefinePopup();
	}
}

