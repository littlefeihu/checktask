
using System;

using Foundation;
using UIKit;

namespace LexisNexis.Red.iOS
{
	
	public partial class LegalDefineViewController : UIViewController
	{
		private string jsStr{ get; set;}
 
		public LegalDefineViewController () : base ("LegalDefineViewController", null)
		{
  
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

			Title = "Admissbility";
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (new UIImage ("Images/Navigation/RedBackArrow.png"), UIBarButtonItemStyle.Plain, backButtonClick);
			NavigationItem.RightBarButtonItem = new UIBarButtonItem(new UIImage ("Images/Navigation/GrayForwardArrow.png"),UIBarButtonItemStyle.Plain,gotoButtonClick);

 
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("selectedTextRect"), changeSelectedText);
 		}


		private void changeSelectedText(NSNotification obj)
		{
			
			if(obj.UserInfo != null ){
				
				Title =	obj.UserInfo.ObjectForKey(new NSString ("textSelect")).ToString();
 //				string str = obj.UserInfo.ObjectForKey(new NSString ("textSelect")).ToString();
   			}
 		
		}
		private void gotoButtonClick (object o, EventArgs e){


		}

		private void backButtonClick(object o, EventArgs e){


		}

	}
}

