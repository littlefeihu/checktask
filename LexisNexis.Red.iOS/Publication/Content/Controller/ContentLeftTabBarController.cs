using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.iOS
{
	partial class ContentLeftTabBarController : UITabBarController
	{
		public ContentLeftTabBarController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

 			ViewControllerSelected += (sender, e) => {
				PublicationContentTypeEnum opendContentType = PublicationContentTypeEnum.None;
				if (SelectedIndex == 0){
					NSNotificationCenter.DefaultCenter.PostNotificationName("RemoveContentSearchResultViewInTOC", this);
					opendContentType = PublicationContentTypeEnum.TOC;
				} else if (SelectedIndex == 1){
					NSNotificationCenter.DefaultCenter.PostNotificationName("RemoveContentSearchResultViewInIndex", this);
 					opendContentType = PublicationContentTypeEnum.Index;
				} else if (SelectedIndex == 2){
					NSNotificationCenter.DefaultCenter.PostNotificationName("RemoveContentSearchResultViewInAnnotation", this);
 					opendContentType = PublicationContentTypeEnum.Annotation;
 				}
				AppDataUtil.Instance.SetOpendContentType(opendContentType);
			};

			//When jump to content from index by click content link, selected index of tab bar supposed to be changed to "Contents"
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("ChangeContentTabBarSelectedIndex"), delegate(NSNotification obj) {
				SelectedIndex = SelectedIndex == 1 ? 0 : SelectedIndex;				
			});

		}

	}
}
