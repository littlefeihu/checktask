using System;
using Android.Content;

namespace LexisNexis.Red.Droid.AlertDialogUtility
{
	public interface ISimpleDialogListener
	{
		/// <summary>
		/// Raises the simple dialog button click event.
		/// </summary>
		/// <param name="buttonType">Button type.</param>
		/// <param name="fragmentTag">Fragment tag.</param>
		/// <param name="extTagKey">Ext tag key.</param>
		/// <param name="extTag">Ext tag.</param>
		bool OnSimpleDialogButtonClick(DialogButtonType buttonType,string fragmentTag, string extTagKey, string extTag);
	}
}

