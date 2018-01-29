using System;
using LexisNexis.Red.Common;

namespace LexisNexis.Red.Droid.LoginPage
{
	public interface ILoginActivity
	{
		void LoginFragment_ReminderPassowrd(int selectedCountryIndex);
		void LoginSucceed();
		void ReminderPasswordFragment_ReturnLogin();
		void HideSoftKeyboard();
	}
}

