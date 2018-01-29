// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace LexisNexis.Red.iOS
{
	[Register ("HomeTabBarController")]
	partial class HomeTabBarController
	{
		[Outlet]
		UIKit.UIBarButtonItem settingBarButtonItem { get; set; }

		[Action ("OpenSettingPanel:")]
		partial void OpenSettingPanel (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (settingBarButtonItem != null) {
				settingBarButtonItem.Dispose ();
				settingBarButtonItem = null;
			}
		}
	}
}
