using System;

namespace LexisNexis.Red.Droid.Async
{
	/// <summary>
	/// IAsyncTaskActivity should be implemented by all activities which require to use
	/// AsyncUIOperationRepeate. Because the Activity of android system can't report it
	/// status.
	/// </summary>
	public interface IAsyncTaskActivity
	{
		/// <summary>
		/// Gets the async task activity GUID.
		/// The guid should be remain same after each times of recreation of host activity.
		/// </summary>
		/// <value>The async task activity GUID.</value>
		string AsyncTaskActivityGUID
		{
			get;
		}
	}
}

