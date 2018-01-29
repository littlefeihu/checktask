
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.View;
using Fragment=Android.Support.V4.App.Fragment;
using LexisNexis.Red.Droid.Widget.SlidingTab;
using LexisNexis.Red.Droid.Widget.Fragment;
using LexisNexis.Red.Droid.AnnotationOrganiserPage;

namespace LexisNexis.Red.Droid.MyPublicationsPage
{
	public class PublicationsMainFragment : Fragment
	{
		private const string MainTabIndex = "MainTabIndex";

		public const int TabPublications = 0;
		public const int TabAnnotations = 1;

		private int mainTabIndex = TabPublications;

		private RadioGroup rgMainTab;
		private RadioButton rbPublications;
		private RadioButton rbAnnotations;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if(savedInstanceState != null)
			{
				mainTabIndex = savedInstanceState.GetInt(MainTabIndex, 0);
			}

			var v = inflater.Inflate(Resource.Layout.publications_main_fragment, container, false);

			rgMainTab = v.FindViewById<RadioGroup>(Resource.Id.rgMainTab);
			rbPublications = v.FindViewById<RadioButton>(Resource.Id.rbPublications);
			rbAnnotations = v.FindViewById<RadioButton>(Resource.Id.rbAnnotations);

			rgMainTab.CheckedChange += MainTabCheckedChange;

			var fragment = Activity.SupportFragmentManager.FindShowingFragmentById (Resource.Id.flMainPanelContainer);
			if(fragment == null)
			{
				var transaction = Activity.SupportFragmentManager.BeginTransaction();

				var publicationListFragment = new PublicationListFragment();
				transaction.Add(Resource.Id.flMainPanelContainer, publicationListFragment, PublicationListFragment.FragmentTag).Commit();
			}

			SetMainTabStatus();

			return v;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutInt(MainTabIndex, mainTabIndex);
			base.OnSaveInstanceState(outState);
		}

		public void SetMainTabStatus()
		{
			if(rgMainTab.CheckedRadioButtonId < 0
				|| GetMainTabIndex(rgMainTab.CheckedRadioButtonId) != mainTabIndex)
			{
				GetMainTab(mainTabIndex).Checked = true;
			}
		}

		public int GetCurrentMainTab()
		{
			return rgMainTab.CheckedRadioButtonId;
		}

		private void MainTabCheckedChange (object sender, RadioGroup.CheckedChangeEventArgs e)
		{
			var changeTo = GetMainTabIndex(e.CheckedId);
			if(changeTo == mainTabIndex)
			{
				return;
			}

			Activity.InvalidateOptionsMenu();

			mainTabIndex = changeTo;

			switch(mainTabIndex)
			{
			case TabPublications:
				{
					var transaction = Activity.SupportFragmentManager.BeginTransaction();

					Activity.SupportFragmentManager.SwitchFragmentById<PublicationListFragment>(
						Resource.Id.flMainPanelContainer,
						PublicationListFragment.FragmentTag,
						f => f.Tag == PublicationListFragment.FragmentTag,
						transaction);

					transaction.Commit();
				}
				break;
			case TabAnnotations:
				{
					var transaction = Activity.SupportFragmentManager.BeginTransaction();

					Activity.SupportFragmentManager.SwitchFragmentById<AnnotationListFragment>(
						Resource.Id.flMainPanelContainer,
						AnnotationListFragment.FragmentTag,
						f => f.Tag == AnnotationListFragment.FragmentTag,
						transaction);

					transaction.Commit();
				}
				break;
			default:
				throw new InvalidProgramException("Invalid tab id.");
			}
		}

		private static int GetMainTabIndex(int resId)
		{
			switch(resId)
			{
			case Resource.Id.rbPublications:
				{
					return TabPublications;
				}
			case Resource.Id.rbAnnotations:
				{
					return TabAnnotations;
				}
			}

			throw new InvalidProgramException("Unknown main tab id.");
		}

		private RadioButton GetMainTab(int index)
		{
			switch(index)
			{
			case TabPublications:
				{
					return rbPublications;
				}
			case TabAnnotations:
				{
					return rbAnnotations;
				}
			}

			throw new InvalidProgramException("Unknown main tab index.");
		}
	}
}

