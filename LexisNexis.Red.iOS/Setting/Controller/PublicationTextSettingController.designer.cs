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
	[Register ("PublicationTextSettingController")]
	partial class PublicationTextSettingController
	{
		[Outlet]
		UIKit.UISlider FontSizeSlider { get; set; }

		[Action ("SliderValueChanged:")]
		partial void SliderValueChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (FontSizeSlider != null) {
				FontSizeSlider.Dispose ();
				FontSizeSlider = null;
			}
		}
	}
}
