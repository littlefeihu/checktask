using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
namespace LexisNexis.Red.iOS
{
	partial class TagEditViewController : UIViewController
	{

		public	AnnotationTag AnnoTag { get; set;}
		private string SelectedCode; 
		private string tagNameText;

		public TagEditViewController (IntPtr handle) : base (handle)
		{
			Title = "New Tag";
		}

		public TagEditViewController () : base ("TagEditViewController", null)
		{
		}


		partial void TagEditDone (Foundation.NSObject sender)
		{
			string defaultColor =((ColorCollectionViewController)ChildViewControllers[0]).SelectedColorCode;
			if(defaultColor != null){
				SelectedCode = defaultColor;
			}else{
				SelectedCode = "#FF00FF";
			}
			tagNameText= TagNameTextField.Text;

		     if(tagNameText == ""){
				var tagNameAlert = UIAlertController.Create ("The TagName cannot be empty!", "", UIAlertControllerStyle.Alert);
				tagNameAlert.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
				PresentViewController(tagNameAlert,true,null);

			}else if(tagNameText != "" && SelectedCode != null){
				Guid tagId = AnnCategoryTagUtil.Instance.AddTag(tagNameText, SelectedCode);
				AppDataUtil.Instance.TagList.Add(new Tag(new AnnotationTag{ Title = tagNameText, Color = SelectedCode, TagId = tagId  },false));
				NSNotificationCenter.DefaultCenter.PostNotificationName("addAnnotationTag", this);
				this.NavigationController.PopViewController(true);
				NSNotificationCenter.DefaultCenter.PostNotificationName("RefreshTagTableView", this);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			UIColor lineColor = ColorUtil.ConvertFromHexColorCode ("#E1E3E0");
			lineView.BackgroundColor = lineColor;

			line2View.BackgroundColor = lineColor;

			UIColor backGroundColor = ColorUtil.ConvertFromHexColorCode ("#F1F1F4");
			this.View.BackgroundColor = backGroundColor;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			TagNameTextField.Text = AnnoTag != null ? AnnoTag.Title :TagNameTextField.Text;

			((ColorCollectionViewController)ChildViewControllers[0]).ColorTag = AnnoTag;


		}

	}
}
