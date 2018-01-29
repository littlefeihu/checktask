using System;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Linq;

using UIKit;
using Foundation;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	partial class IndexController : UIViewController
	{

		public IndexController (IntPtr handle) : base (handle)
		{
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("SearchContentInIndex"), ProcessContentSearchRequest);
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("RemoveContentSearchResultViewInIndex"), delegate(NSNotification obj) {
 				if(AppDisplayUtil.Instance.ContentSearchResController != null){  
					//AppDisplayUtil.Instance.ContentSearchResController.View.RemoveFromSuperview();
				}
			});
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			SearchBar.Delegate = new IndexSearchBarDelegate ();

			//hide search bar border
			SearchBar.Layer.BorderWidth = 1;
			SearchBar.Layer.BorderColor = UIColor.FromRGB (194, 194, 194).CGColor;
		}

		public void ProcessContentSearchRequest (NSNotification obj)
		{
			string keyword = SearchBar.Text;
			SearchResult res = SearchUtil.Search(AppDataUtil.Instance.GetCurrentPublication().BookId, AppDataUtil.Instance.GetOpendTOC().ID, keyword);

			if (AppDisplayUtil.Instance.ContentSearchResController != null) {
				AppDisplayUtil.Instance.ContentSearchResController.View.RemoveFromSuperview ();
			}
			AppDisplayUtil.Instance.ContentSearchResController = new ResultViewController (res);
			AppDisplayUtil.Instance.ContentSearchResController.View.TranslatesAutoresizingMaskIntoConstraints = false;
			ContainerView.AddSubview (AppDisplayUtil.Instance.ContentSearchResController.View);
			ContainerView.AddConstraints (new NSLayoutConstraint[]{ 
				NSLayoutConstraint.Create(AppDisplayUtil.Instance.ContentSearchResController.View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Top, 1, 0),
				NSLayoutConstraint.Create(AppDisplayUtil.Instance.ContentSearchResController.View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Bottom, 1, 0),
				NSLayoutConstraint.Create(AppDisplayUtil.Instance.ContentSearchResController.View, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Leading, 1, 0),
				NSLayoutConstraint.Create(AppDisplayUtil.Instance.ContentSearchResController.View, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, ContainerView, NSLayoutAttribute.Trailing, 1, 0)
			});
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (AppDataUtil.Instance.ContentSearchKeyword != null) {
				SearchBar.Text = AppDataUtil.Instance.ContentSearchKeyword;
			}
			SearchBar.ResignFirstResponder ();
		}

	}
}