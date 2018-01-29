using System;

namespace LexisNexis.Red.Droid.Utility
{
	public class SwipeableItemDataWrap<T>
		where T : class
	{
		public enum SwipeStatus
		{
			Default,
			Left,
			Right,
		}

		public SwipeStatus Status{ get; set;}
		public T Data{ get; set;}

		public SwipeableItemDataWrap(T data)
		{
			Data = data;
			Status = SwipeStatus.Default;
		}
	}
}

