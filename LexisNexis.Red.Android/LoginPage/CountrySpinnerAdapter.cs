using System;
using Android.Widget;
using Android.App;
using LexisNexis.Red.Droid.App;
using Android.Views;
using System.Runtime.Remoting.Contexts;
using System.Collections.Generic;
using LexisNexis.Red.Droid.Business;

namespace LexisNexis.Red.Droid.LoginPage
{
	public class CountrySpinnerAdapter: ArrayAdapter<String>
	{
		private Activity context;

		public CountrySpinnerAdapter(Activity context, String[] objects)
			: base(context, 0, objects)
		{
			this.context = context;
		}

		public override View GetDropDownView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view, if one is available
			if(view == null) // otherwise create a new one
			{
				//view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleSpinnerDropDownItem, null);
				view = context.LayoutInflater.Inflate(Resource.Layout.test_spinner_item, null);
			}

			//((TextView)view).Text = MainApp.ThisApp.ServiceCountryList[position];

			if(position == 0)
				view.LayoutParameters = new LinearLayout.LayoutParams(0, 0);
			else
				view.Visibility = ViewStates.Visible;
			
			return view;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; // re-use an existing view, if one is available
			if (view == null) // otherwise create a new one
				view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleSpinnerItem, null);
			
			((TextView)view).Text = DataCache.INSTATNCE.ServiceCountryList[position];
			return view;
		}
	}
}

