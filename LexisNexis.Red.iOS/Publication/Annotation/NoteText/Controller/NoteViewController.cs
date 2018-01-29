
using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	public partial class NoteViewController : UIViewController
	{
		public	List<Guid> getGuidList;
 
		public NoteViewController (IntPtr handle) : base (handle)
		{

		}
		public NoteViewController () : base ("NoteViewController", null)
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

			getGuidList = new List<Guid>();
			AppDisplayUtil.Instance.noteVC = this;
			Title = "Annotation";
			UIBarButtonItem deleteButton = new UIBarButtonItem ("Delete",UIBarButtonItemStyle.Plain,deleteButttonCLick);
			deleteButton.TintColor = UIColor.Red;
   			this.NavigationItem.RightBarButtonItem = deleteButton;

			UIBarButtonItem doneButton = new UIBarButtonItem ("Done",UIBarButtonItemStyle.Plain,doneButttonCLick);
			doneButton.TintColor = UIColor.Red;
			this.NavigationItem.LeftBarButtonItem = doneButton;
  		}

		private async void doneButttonCLick(object o, EventArgs e){
  			string textViewStr = AppDisplayUtil.Instance.newAnnotationVC.TextView.Text;
			var bookId = AppDataUtil.Instance.GetCurrentPublication ().BookId;
			var bookVersionId = AppDataUtil.Instance.GetCurrentPublication ().CurrentVersion;
 			var tocTitle = AppDataUtil.Instance.GetHighlightedTOCNode().Title;
			var guidCardName = AppDataUtil.Instance.GetHighlightedTOCNode().GuideCardTitle;

 //			string highlightStr = EvaluateJavascript("window.getSelection().toString();"); 
 			AnnotationDataItem startDataItem = new AnnotationDataItem  (130,"fileNmae","levelId","xPath","docId");
			AnnotationDataItem endDataItem = new AnnotationDataItem (130,"fileNmae","levelId","xPath","docId");
 			await AnnotationUtil.Instance.AddAnnotation (new Annotation(Guid.NewGuid (),AnnotationType.StickyNote,AnnotationStatusEnum.Created,bookId,bookVersionId,
				"docId",guidCardName,textViewStr,"hightTextLabel selectedTextContent  test textViewText Hi,Work",tocTitle,getGuidList,startDataItem,endDataItem));
			AppDisplayUtil.Instance.DismissPopoverView ();
 		}

		public void deleteButttonCLick(object o, EventArgs e){
 			AppDisplayUtil.Instance.DismissPopoverView ();
 		}
	}
}

