using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;


 using LexisNexis.Red.Common.HelpClass;
using System.Linq;
using System.Collections.Generic;



namespace LexisNexis.Red.iOS
{
	public	partial class AnnotationController : UIViewController 
	{
 
		public List<Annotation> PublicationList = new List<Annotation> ();
	
  		public AnnotationController (IntPtr handle) : base (handle)
		{
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("RemoveContentSearchResultViewInAnnotation"), delegate(NSNotification obj) {
				if(AppDisplayUtil.Instance.ContentSearchResController != null){
					AppDisplayUtil.Instance.ContentSearchResController.View.RemoveFromSuperview();
				}
			});
		}

 



		partial void OnSegmentControlValueChanged (Foundation.NSObject sender)
		{
 			var controller = (UISegmentedControl)sender;
			var selectIndex =controller.SelectedSegment; 
 
			AppDataUtil.Instance.AnnotationFilterSelectedIndex = (int)selectIndex;
 			NSNotificationCenter.DefaultCenter.PostNotificationName("filterAnnotationInPublication", this); 
  		}

 		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
 			TagFilterView tfv = new TagFilterView (15, 280, 7);
			tfv.UserInteractionEnabled = true;
			tfv.AddGestureRecognizer(new UITapGestureRecognizer (delegate() {
				TagTableViewController  ttvc = new TagTableViewController();
				UIPopoverController tagFilterPopover = new UIPopoverController(ttvc);
				tagFilterPopover.BackgroundColor = UIColor.White;
				tagFilterPopover.SetPopoverContentSize(new CoreGraphics.CGSize(320, 320), true);
				AppDisplayUtil.Instance.SetPopoverController (tagFilterPopover);
 				tagFilterPopover.PresentFromRect(new CoreGraphics.CGRect(0,0,30,30), tfv, UIPopoverArrowDirection.Up, true);
			}));
				
			FilterNavigationBar.Add (tfv);
		}
	}
}
