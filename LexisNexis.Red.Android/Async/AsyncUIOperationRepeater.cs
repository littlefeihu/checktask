using System;
using System.Collections.Generic;
using Android.App;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Droid.Async
{
	/// <summary>
	/// AsyncUIOperationRepeater used to help activity to handle the return of async task;
	/// Scenario:
	/// 1. An activity run a async task
	/// 2. User press "home" button to leave app
	/// 3. The activity is destoried by system
	/// 4. The async finished and require some ui operation
	/// The AsyncUIOperationRepeater will check the status of activity to decide if execute
	/// the ui operation immediately.
	/// Android system requires the main shift of ui operation should be execute between
	/// OnCreate and OnStop. (http://www.androiddesignpatterns.com/2013/08/fragment-transaction-commit-state-loss.html)
	/// If the ui operation can't be executed immediately, the AsyncUIOperationRepeater will cache
	/// the ui operation. After recreation, the activity can call ExecutePendingUIOperation method
	/// in OnCreate to execute all pending ui operation.
	/// 
	/// The resolution can refer to (http://stackoverflow.com/questions/8040280/how-to-handle-handler-messages-when-activity-fragment-is-paused)
	/// 
	/// An supported activity should be implemented as:
	/// 	public class XXXActivity : IAsyncTaskActivity
	///		{
	///			protected override void OnCreate(Bundle savedInstanceState)
	///			{
	///				if(savedInstanceState != null)
	///				{
	///					asyncTaskActivityGUID = savedInstanceState.GetString(
	///						AsyncUIOperationRepeater.ASYNC_ACTIVITY_GUID);
	///				}
	/// 
	///				if(string.IsNullOrEmpty(asyncTaskActivityGUID))
	///				{
	///					asyncTaskActivityGUID = Guid.NewGuid().ToString();
	///				}
	/// 
	///				AsyncUIOperationRepeater.INSTATNCE.RegisterAsyncTaskActivity(this);
	///			}
	///
	///			protected override void OnSaveInstanceState(Bundle outState)
	///			{
	///				outState.PutString(AsyncUIOperationRepeater.ASYNC_ACTIVITY_GUID, asyncTaskActivityGUID);
	///				base.OnSaveInstanceState(outState);
	///			}
	///
	///			protected override void OnResume()
	///			{
	///				base.OnResume();
	///				AsyncUIOperationRepeater.INSTATNCE.RegisterAsyncTaskActivity(this);
	///				AsyncUIOperationRepeater.INSTATNCE.ExecutePendingUIOperation(this);
	///			}
	/// 
	///			protected override void OnStop()
	///			{
	///				AsyncUIOperationRepeater.INSTATNCE.UnregisterAsyncTaskActivity(this);
	///				base.OnStop();
	///			}
	///
	///			public string AsyncTaskActivityGUID
	///			{
	///				get
	///				{
	///					return asyncTaskActivityGUID;
	///				}
	///			}
	///		}
	/// 
	/// 
	/// </summary>
	public class AsyncUIOperationRepeater
	{
		public const string ASYNC_ACTIVITY_GUID = "ASYNC_ACTIVITY_GUID";

		private static readonly AsyncUIOperationRepeater SINGLETON_INSTANCE = new AsyncUIOperationRepeater();
		private AsyncUIOperationRepeater()
		{
			pendingUIOperationList = new List<Tuple<IAsyncTaskActivity,
										Action<IAsyncTaskActivity>,
										Predicate<IAsyncTaskActivity>,
										string, string>>();
			currentResponsibleActivityList = new List<IAsyncTaskActivity>();
		}

		private readonly List<IAsyncTaskActivity> currentResponsibleActivityList;

		/// <summary>
		/// The pending user interface operation list.
		/// Type: host activity type
		/// string: ASYNC_ACTIVITY_GUID
		/// UIOperation: ui operation
		/// </summary>
		private readonly List<Tuple<IAsyncTaskActivity,
			Action<IAsyncTaskActivity>,
			Predicate<IAsyncTaskActivity>,
			string,	// Email
			string>	// CountryCode
			> pendingUIOperationList;

		public static AsyncUIOperationRepeater INSTATNCE
		{
			get
			{
				return SINGLETON_INSTANCE;
			}
		}

		public void RegisterAsyncTaskActivity(IAsyncTaskActivity activity)
		{
            var found = currentResponsibleActivityList.Find(a =>
                a.AsyncTaskActivityGUID == activity.AsyncTaskActivityGUID);

            if (found != null)
            {
                if (found == activity)
                {
                    return;
                }
                else
                {
                    throw new ArgumentException("2 IAsyncTaskActivity has same guid.", "activity");
                }
            }

            currentResponsibleActivityList.Add(activity);
        }

		public void UnregisterAsyncTaskActivity(IAsyncTaskActivity activity)
		{
			if(!currentResponsibleActivityList.Remove(activity))
			{
				throw new ArgumentException("Unable to find IAsyncTaskActivity which should be remove.", "activity");
			}
		}

		public bool IsActivityActive(IAsyncTaskActivity activity)
		{
			return currentResponsibleActivityList.Contains(activity);
		}

		/// <summary>
		/// Submits an async ui operation to AsyncUIOperationRepeater;
		/// The AsyncUIOperationRepeater will check the status of host activity to
		/// decide if execute the ui operation immediately. If the ui operation can
		/// execute now, the AsyncUIOperationRepeater will cache the ui operation.
		/// After recreation, the activity can call ExecutePendingUIOperation in OnCreate
		/// to execute all pending ui operation.
		/// </summary>
		/// <param name="currentActiviy">Current activiy</param>
		/// <param name="op">UI operation</param>
		/// <param name="discardable">If set to <c>true</c>, the ui operation will be discarded if
		/// the operation can't be execute immediately.</param>
		/// <param name = "addtionalMatch">Addtional critrial</param>
		public void SubmitAsyncUIOperation(
			IAsyncTaskActivity currentActiviy,
			Action<IAsyncTaskActivity> op,
			bool discardable = false,
			Predicate<IAsyncTaskActivity> addtionalMatch = null)
		{
			var found = currentResponsibleActivityList.Find(a =>
							{
								if(currentActiviy != null
									&& a.AsyncTaskActivityGUID == currentActiviy.AsyncTaskActivityGUID)
								{
									return true;
								}
								else if(addtionalMatch != null)
								{
									return addtionalMatch(a);
								}

								return false;
							});

			if(found != null)
			{
				Application.SynchronizationContext.Post(_ =>{
					if(currentResponsibleActivityList.Contains(found))
					{
						op(found);
					}
					else if(!discardable)
					{
						pendingUIOperationList.Add(
							new Tuple<IAsyncTaskActivity,
							Action<IAsyncTaskActivity>,
							Predicate<IAsyncTaskActivity>,
							string, string>(
								currentActiviy, op, addtionalMatch,
								GetCurrentUserEmail(),
								GetCurrentUserCountryCode()));
					}
				}, null);
				return;
			}
 
			if(!discardable)
			{
				pendingUIOperationList.Add(
					new Tuple<IAsyncTaskActivity,
						Action<IAsyncTaskActivity>,
						Predicate<IAsyncTaskActivity>,
						string, string>(
							currentActiviy, op, addtionalMatch,
							GetCurrentUserEmail(),
							GetCurrentUserCountryCode()));
			}
		}

		/// <summary>
		/// After recreation, the host activity call this method to executes all pending ui operation.
		/// </summary>
		/// <returns><c>true</c>, if one or more pending ui operation was executed, <c>false</c> otherwise.</returns>
		/// <param name="currentActiviy">Current activiy.</param>
		public bool ExecutePendingUIOperation(IAsyncTaskActivity currentActiviy)
		{
			bool hasPendingUIOp = false;
			pendingUIOperationList.RemoveAll(opItem =>
			{
				if(!IsCurrentUserOperation(opItem))
				{
					// The task is not belong to current user,
					// just remove it.
					return true;
				}

				bool match = opItem.Item1 != null
					&& opItem.Item1.AsyncTaskActivityGUID == currentActiviy.AsyncTaskActivityGUID;
				if(!match && opItem.Item3 != null)
				{
					match = opItem.Item3(currentActiviy);
				}

				if(match)
				{
					Application.SynchronizationContext.Post(_ =>
						opItem.Item2(currentActiviy), null);
					hasPendingUIOp = true;
					return true;
				}

				return false;
			});

			return hasPendingUIOp;
		}

		public bool HavePendingUIOperation(IAsyncTaskActivity currentActiviy)
		{
			return null != pendingUIOperationList.Find(opItem =>
								{
									if(!IsCurrentUserOperation(opItem))
									{
										return false;
									}

									bool match = opItem.Item1 != null
										&& opItem.Item1.AsyncTaskActivityGUID == currentActiviy.AsyncTaskActivityGUID;
									if(!match && opItem.Item3 != null)
									{
										match = opItem.Item3(currentActiviy);
									}

									return match;
								});
		}

		public void DiscardActivitySpecificUIOperation(IAsyncTaskActivity currentActiviy)
		{
			pendingUIOperationList.RemoveAll(opItem =>
			{
				if(!IsCurrentUserOperation(opItem))
				{
					// The task is not belong to current user,
					// just remove it.
					return true;
				}

				return opItem.Item3 == null && opItem.Item1 != null
					&& opItem.Item1.AsyncTaskActivityGUID == currentActiviy.AsyncTaskActivityGUID;
			});
		}

		private static bool IsCurrentUserOperation(
			Tuple<IAsyncTaskActivity,
			Action<IAsyncTaskActivity>,
			Predicate<IAsyncTaskActivity>,
			string, string> opItem)
		{
			if(GlobalAccess.Instance.CurrentUserInfo == null)
			{
				return opItem.Item4 == null && opItem.Item5 == null;
			}

			return opItem.Item4 == GlobalAccess.Instance.CurrentUserInfo.Email
				&& opItem.Item5 == GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode;
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

