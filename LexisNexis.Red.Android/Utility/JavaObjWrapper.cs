using System;

namespace LexisNexis.Red.Droid.Utility
{
	public class JavaObjWrapper<T>: Java.Lang.Object
	{
		public T Value
		{
			get;
			set;
		}

		public JavaObjWrapper(T value)
		{
			Value = value;
		}

		public static implicit operator T(JavaObjWrapper<T> holder)
		{
			return holder.Value;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}

