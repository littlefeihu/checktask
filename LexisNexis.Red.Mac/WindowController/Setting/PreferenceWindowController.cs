
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.Mac
{
	public partial class PreferenceWindowController : AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public PreferenceWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PreferenceWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public PreferenceWindowController () : base ("PreferenceWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			Window.Delegate = new WindowDelegate();
		}

		#endregion

		//strongly typed window accessor
		public new PreferenceWindow Window {
			get {
				return (PreferenceWindow)base.Window;
			}
		}

		partial void logoutButtonClick (NSObject sender) 
		{
			string title = "Sign Out";
			string info = "Thank you for using LexisNexis Red application.Are you sure you want to exit ?";
			nint result = AlertSheet.RunConfirmAlert(title, info);
			if (result == (int)NSAlertType.DefaultReturn) {
				ConfirmLogout ();
			} else {
				WindowStopModal ();
			}
		}

		void ConfirmLogout ()
		{
			LoginUtil.Instance.Logout();

			WindowStopModal ();

			//Go back to login view
			AppDelegate appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
			appDelegate.SwitchWindowByWindowName ("LoginWindowController");
		}

		void WindowStopModal ()
		{
			NSApplication.SharedApplication.StopModal();
			preferenceWindow.OrderOut(null);
		}
	}
}

