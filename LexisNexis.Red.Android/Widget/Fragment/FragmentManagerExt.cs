using System;
using FragmentManager=Android.Support.V4.App.FragmentManager;
using Android.OS;
using Android.Support.V4.App;

namespace LexisNexis.Red.Droid.Widget.Fragment
{
	using Fragment=Android.Support.V4.App.Fragment;

	public static class FragmentManagerExt
	{
		public static Fragment FindShowingFragmentById(this FragmentManager fm, int id)
		{
			Fragment candicator = null;
			foreach(var f in fm.Fragments)
			{
				if(f == null)
				{
					continue;
				}

				if(f.Id == id && (!f.IsHidden))
				{
					if(f.IsAdded)
					{
						return f;
					}

					candicator = f;
				}
			}

			return candicator;
		}

		public static void SwitchFragmentById<TargetFragment>(
			this FragmentManager fm,
			int id,
			string targetFragmentTag,
			Func<Fragment, bool> compare,
			FragmentTransaction transaction = null,
			Bundle args = null
		)
			where TargetFragment : Fragment, new()
		{
			var currentTransaction = transaction ?? fm.BeginTransaction();

			var orgFragment = FindShowingFragmentById(fm, id);
			if(!compare(orgFragment))
			{
				currentTransaction.Hide(orgFragment);

				var target = fm.FindFragmentByTag(targetFragmentTag);
				if(target == null)
				{
					target = new TargetFragment();
					if(args != null)
					{
						target.Arguments = args;
					}

					currentTransaction.Add(id, target, targetFragmentTag);
				}
				else
				{
					if(args != null)
					{
						target.Arguments = args;
					}

					currentTransaction.Show(target);
				}
			}

			if(transaction == null)
			{
				currentTransaction.Commit();
			}
		}
	}
}

