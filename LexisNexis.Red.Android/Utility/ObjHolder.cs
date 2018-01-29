using System;

namespace LexisNexis.Red.Droid.Utility
{
	public class ObjHolder<T>
		where T : class
	{
		public T Value
		{
			get;
			set;
		}

		public ObjHolder(T value = null)
		{
			Value = value;
		}

		public static implicit operator T(ObjHolder<T> holder)
		{
			return holder.Value;
		}
	}
}

