using System;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.App;
using LexisNexis.Red.Droid.App;

namespace LexisNexis.Red.Droid.SettingsPage
{
	public class MainMenuPopup
	{
		private readonly Activity host;
		private PopupWindow mainMenu;
		private readonly Action<int> menuItemClicked;

		public MainMenuPopup(Activity host, Action<int> menuItemClicked)
		{
			this.host = host;
			this.menuItemClicked = menuItemClicked;
		}

		public void ShowAtLocation(View parent, GravityFlags gravity, int x, int y)
		{
			if(mainMenu == null)
			{
				var view = host.LayoutInflater.Inflate(Resource.Layout.main_menu_popup, null);
				view.FindViewById<TextView>(Resource.Id.tvLogout).Click += OnMenuItemClicked;
			
				mainMenu = new PopupWindow(
					view,
					ViewGroup.LayoutParams.WrapContent,
					ViewGroup.LayoutParams.WrapContent,
					true);
				//mainMenu.SetBackgroundDrawable(MainApp.ThisApp.Resources.GetDrawable(Resource.Drawable.mainmenu_popup_background));
				mainMenu.OutsideTouchable = true;
			}

			if(mainMenu.IsShowing)
			{
				return;
			}

			mainMenu.ShowAtLocation(parent, gravity, x, y);
		}

		void OnMenuItemClicked (object sender, EventArgs e)
		{
			mainMenu.Dismiss();
			if(menuItemClicked != null)
			{
				menuItemClicked.Invoke(((View)sender).Id);
			}
		}
	}
}

