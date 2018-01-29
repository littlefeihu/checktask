using System;
using Android.App;
using Android.Content;
using FragmentManager=Android.Support.V4.App.FragmentManager;
using LexisNexis.Red.Droid.App;
using Android.Views;

namespace LexisNexis.Red.Droid.AlertDialogUtility
{
	public static class DialogGenerator
	{
		/// <summary>
		/// Shows the wait dialog.
		/// </summary>
		/// <returns>The wait dialog.</returns>
		/// <param name="fragmentManager">Fragment manager.</param>
		/// <param name="messageResId">Message resource identifier.</param>
		/// <param name="extraTagKey">Extra tag key.</param>
		/// <param name="extraTag">Extra tag.</param>
		public static string ShowWaitDialog(
			FragmentManager fragmentManager,
			int messageResId = 0,
			string extraTagKey = null,
			string extraTag = null)
		{
			var simpleDialogFragment = SimpleDialogFragment.Create(new SimpleDialogProvider(){
				Type = SimpleDialogProvider.DialogType.PleaseWaitDialog,
				DismissAfterProcessRestore = true,
				MessageResId = messageResId,
				ExtTagKey = extraTagKey,
				ExtTag = extraTag,
			});

			return simpleDialogFragment.Show(fragmentManager);
		}

		/// <summary>
		/// Shows the message dialog.
		/// </summary>
		/// <returns>The message dialog.</returns>
		/// <param name="fragmentManager">Fragment manager.</param>
		/// <param name="titleResId">Title res identifier.</param>
		/// <param name="messageResId">Message res identifier.</param>
		/// <param name="positiveButtonTextResId">Positive button text res identifier.</param>
		/// <param name="negativeButtonResId">Negative button res identifier.</param>
		/// <param name="extraTagKey">Extra tag key.</param>
		/// <param name="extraTag">Extra tag.</param>
		public static string ShowMessageDialog(
			FragmentManager fragmentManager,
			int titleResId,
			int messageResId,
			int positiveButtonTextResId = 0,
			int negativeButtonResId = -1,
			string extraTagKey = null,
			string extraTag = null)
		{
			var simpleDialogFragment = SimpleDialogFragment.Create(new SimpleDialogProvider(){
				TitleResId = titleResId,
				MessageResId = messageResId,
				PositiveButtonResId = positiveButtonTextResId,
				NegativeButtonResId = negativeButtonResId,
				ExtTagKey = extraTagKey,
				ExtTag = extraTag,
			});

			return simpleDialogFragment.Show(fragmentManager);
		}
	}
}

