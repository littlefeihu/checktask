using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Droid.Async
{
	/// <summary>
	/// AsyncTaskManager class is a singleton class;
	/// The main purpose is that UI thread can check if a specific async task is running.
	/// </summary>
	public class AsyncTaskManager
	{
		private static AsyncTaskManager SINGLETON_INSTANCE = new AsyncTaskManager();
		private AsyncTaskManager()
		{
			runningTaskList = new List<Tuple<Task, object, object, string, string>>();
		}

		/// <summary>
		/// A list used to cach all async task instance
		/// </summary>
		private readonly List<Tuple<
			Task,		// The task
			object,		// Tag1
			object,		// Tag2
			string,		// Email
			string		// CountryCode
		>> runningTaskList;

		/// <summary>
		/// Gets the INSTATNC of AsyncTaskManager.
		/// </summary>
		/// <value>The INSTATNC of AsyncTaskManager.</value>
		public static AsyncTaskManager INSTATNCE
		{
			get
			{
				return SINGLETON_INSTANCE;
			}
		}

		/// <summary>
		/// Registers an async task to AsyncTaskManager.
		/// </summary>
		/// <param name="t">an async task</param>
		/// <param name = "tag1">the tag object of task</param>
		/// <param name = "tag2">the tag object of task</param>
		public void RegisterTask(Task t, object tag1 = null, object tag2 = null)
		{
			if(runningTaskList.FindIndex(_t => _t.Item1 == t) < 0)
			{
				runningTaskList.Add(new Tuple<Task, object, object, string, string>(
					t, tag1, tag2, GetCurrentUserEmail(), GetCurrentUserCountryCode()));
			}
		}

		/// <summary>
		/// Unregisters an async task.
		/// </summary>
		/// <returns><c>true</c>, if task was unregistered, <c>false</c> otherwise.</returns>
		/// <param name="t">an async task</param>
		public bool UnregisterTask(Task t)
		{
			ClearNonCurrentUserTask();
			var count = runningTaskList.RemoveAll(tuple => tuple.Item1 == t);
			return count > 0;
		}

		/// <summary>
		/// Determines whether the task is belong to current user.
		/// </summary>
		/// <returns><c>true</c> if this instance is belong to current user the specified t; otherwise, <c>false</c>.</returns>
		/// <param name="t">an async task</param>
		public bool IsBelongToCurrentUser(Task t)
		{
			ClearNonCurrentUserTask();
			return runningTaskList.Find(tuple => tuple.Item1 == t) != null;
		}

		/// <summary>
		/// Finds an async task by the type.
		/// </summary>
		/// <returns>The async task match the type, if not found return null.</returns>
		/// <param name="t">The data type of async task</param>
		public Task FindTaskByType(Type t)
		{
			ClearNonCurrentUserTask();
			var result = runningTaskList.Find(tuple => tuple.Item1.GetType() == t);
			return result == null ? null : result.Item1;
		}

		/// <summary>
		/// Finds the task through prediation.
		/// </summary>
		/// <returns>The task.</returns>
		/// <param name="match">Match criteria.</param>
		public Task FindTask(Func<Task, object, object, bool> match)
		{
			ClearNonCurrentUserTask();
			var result = runningTaskList.Find(task => match(task.Item1, task.Item2, task.Item3));
			return result == null ? null : result.Item1;
		}

		private void ClearNonCurrentUserTask()
		{
			runningTaskList.RemoveAll(t => !IsCurrentUserTask(t));
		}

		private static bool IsCurrentUserTask(
			Tuple<Task, object, object, string, string> taskItem)
		{
			if(GlobalAccess.Instance.CurrentUserInfo == null)
			{
				return taskItem.Item4 == null && taskItem.Item5 == null;
			}

			return taskItem.Item4 == GlobalAccess.Instance.CurrentUserInfo.Email
				&& taskItem.Item5 == GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode;
		}

		private static string GetCurrentUserEmail()
		{
			return GlobalAccess.Instance.CurrentUserInfo == null ? null : GlobalAccess.Instance.CurrentUserInfo.Email;
		}

		private static string GetCurrentUserCountryCode()
		{
			return GlobalAccess.Instance.CurrentUserInfo == null ? null : GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode;
		}
	}
}

