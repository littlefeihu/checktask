using System;
using Android.App;
using Newtonsoft.Json;

namespace LexisNexis.Red.Droid.AlertDialogUtility
{
	public class SimpleDialogProvider
	{
		public enum DialogType
		{
			/// <summary>
			/// Open an alert dialog.
			/// </summary>
			AlertDialog,

			/// <summary>
			/// Open a please wait dialog.
			/// </summary>
			PleaseWaitDialog,
		}

		public SimpleDialogProvider()
		{
			Type = DialogType.AlertDialog;
			DismissAfterProcessRestore = false;
			Cancelable = true;
			CanceledOnTouchOutside = true;
			PositiveButtonResId = 0;
			NegativeButtonResId = -1;
		}

		/// <summary>
		/// The type of dialog
		/// </summary>
		/// <value>The type.</value>
		public DialogType Type
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the fragment tag of the dialog.
		/// </summary>
		/// <value>The fragment tag.</value>
		public string FragmentTag
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the ext tag key, which can be used to identify the dialog.
		/// </summary>
		/// <value>The ext tag key.</value>
		public string ExtTagKey
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the ext tag, which can be used to identify the dialog.
		/// </summary>
		/// <value>The ext tag.</value>
		public string ExtTag
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this dialog dismiss after process restore.
		/// </summary>
		/// <value><c>true</c> if dismiss after process restore; otherwise, <c>false</c>.</value>
		public bool DismissAfterProcessRestore
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the process session identifier.
		/// </summary>
		/// <value>The process session identifier.</value>
		public long ProcessSessionId
		{
			get;
			set;
		}

		#region IDevice implementation
		/// <summary>
		/// Gets or sets the title resouce identifier.
		/// </summary>
		/// <value>The title res identifier.</value>
		public int TitleResId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the message resource identifier.
		/// </summary>
		/// <value>The message res identifier.</value>
		public int MessageResId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this dialog cancelable.
		/// </summary>
		/// <value><c>true</c> if this dialog cancelable; otherwise, <c>false</c>.</value>
		public bool Cancelable
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this dialog canceled on touch outside.
		/// </summary>
		/// <value><c>true</c> if this dialog canceled on touch outside; otherwise, <c>false</c>.</value>
		public bool CanceledOnTouchOutside
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the positive button resource identifier.
		/// </summary>
		/// <value>The positive button resource identifier.</value>
		public int PositiveButtonResId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the negative button resource identifier.
		/// </summary>
		/// <value>The negative button resource identifier.</value>
		public int NegativeButtonResId
		{
			get;
			set;
		}
		#endregion
	}
}

