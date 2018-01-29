
using System;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.iOS
{
	public partial class PublicationTextSettingController : UIViewController
	{
		public PublicationTextSettingController () : base ("PublicationTextSettingController", null)
		{
			Title = "Publication Text";
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			int currentFontSize = (int)SettingsUtil.Instance.GetFontSize();
			FontSizeSlider.Value = currentFontSize;
		}



		partial void SliderValueChanged (Foundation.NSObject sender)
		{
			float value = (float)Math.Round(((UISlider)sender).Value);

			SettingsUtil.Instance.SaveFontSize((int)value);

			NSNotificationCenter.DefaultCenter.PostNotificationName("changeFont", this, new NSDictionary("size", value)); 

			FontSizeSlider.Value = value;	
		}
	}
}

