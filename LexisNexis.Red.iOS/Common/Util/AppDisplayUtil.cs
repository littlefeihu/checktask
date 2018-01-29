using System;

using Foundation;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
namespace LexisNexis.Red.iOS
{
	public class AppDisplayUtil
	{
		private static readonly AppDisplayUtil instance;

		public AppDelegate AppDelegateInstance{get; set;}

		public TableOfContentController TOCVC{ get; set;}

		public ResultViewController ContentSearchResController{ get; set;}

 		public ContentViewController  contentVC{get; set;}
		public NoteTextNewTagAnnotationController newAnnotationVC{ get;set;}
		public PublicationAnnotationTableViewController publicationAnnotationVC{get;set;}
		public NoteViewController noteVC{ get; set;}
		public HighlightTableViewControllerController highlightVC{ get; set;}

		public  string  NoTagIndeifier{ get; set;}

		private AppDisplayUtil()
		{
			this.AppDelegateInstance = (AppDelegate)UIApplication.SharedApplication.Delegate;

		}

		static AppDisplayUtil()
		{
			instance = new AppDisplayUtil();
		}
		public static AppDisplayUtil Instance
		{
			get
			{
				return instance;
			}
		}


		public void ShowPublicationInfoView ()
		{
			AppDelegateInstance.MyPublicationController.ShowPublicationInfoView ();
		}

		public void GotoPublicationDetailViewController ()
		{
			AppDelegateInstance.MyPublicationController.GotoPublicationDetailViewController();

		}

		public void DismissPopoverView ()
		{
			if(AppDelegateInstance.CurPopoverController != null) {
				AppDelegateInstance.CurPopoverController.Dismiss(true);
			}
		}

		public void SetPopoverController (UIPopoverController popoverC)
		{
			DismissPopoverView ();
			AppDelegateInstance.CurPopoverController = popoverC;
		}

		public void SetCurrentPopoverViewSize (CGSize size)
		{
			if (AppDelegateInstance.CurPopoverController != null) {
				AppDelegateInstance.CurPopoverController.PopoverContentSize = size;
			}
		}

		public void GoToLoginView ()
		{
			AppDelegateInstance.LoginInterceptorVC.NavigationController.PopToRootViewController(false);
			AppDelegateInstance.LoginInterceptorVC.GoToLoginViewController ();
		}

		public void ClearPublicationViewList ()
		{
			AppDelegateInstance.PublicationViewList.Clear();
		}

		public void AddPublicationView(PublicationView pv)
		{
			AppDelegateInstance.PublicationViewList.Add (pv);
		}

	}
}

