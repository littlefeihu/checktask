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
	[Register ("SettingTableViewItem")]
	partial class SettingTableViewItem
	{
		[Outlet]
		public UIKit.UILabel SettingDescLabel { get; set; }

		[Outlet]
		public UIKit.UIImageView SettingItemImage { get; set; }

		[Outlet]
		public UIKit.UILabel SettingNameLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SettingItemImage != null) {
				SettingItemImage.Dispose ();
				SettingItemImage = null;
			}

			if (SettingNameLabel != null) {
				SettingNameLabel.Dispose ();
				SettingNameLabel = null;
			}

			if (SettingDescLabel != null) {
				SettingDescLabel.Dispose ();
				SettingDescLabel = null;
			}
		}
	}
}
