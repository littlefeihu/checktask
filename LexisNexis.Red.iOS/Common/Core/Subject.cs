using System;
using System.Collections.Generic;

namespace LexisNexis.Red.iOS
{
	public abstract class Subject
	{
		private List<Observer> observerList = new List<Observer>();


		protected string content = "";

		public virtual string Content
		{
			get{return content;}
			set{
				content = value;
				Notify ();
			}
		}

		public void AddObserver (Observer o)
		{
			if (observerList.IndexOf (o) < 0) {
				observerList.Add (o);
			}
		}

		public void RemoveObserver (Observer o)
		{
			observerList.Remove (o);
		}

		public void ClearObservers ()
		{
			observerList.Clear ();
		}

		protected void Notify ()
		{
			foreach (var o in observerList) {
				o.Update (this);
			}
		}
	}
}

