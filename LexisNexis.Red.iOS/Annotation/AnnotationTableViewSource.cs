
using System;

using Foundation;
using UIKit;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using LexisNexis.Red.Common.BusinessModel;
using CoreGraphics;
using LexisNexis.Red.Common.Business;
using System.Linq;


namespace LexisNexis.Red.iOS
{
	public class AnnotationTableViewSource : UITableViewSource
	{
		public	List<string> ColorList;
		public  List<string> tagTextLabel;
		public float heightBgView;
		 
		UITableView annoTableView;
 
		public List<Annotation> AnnotationList{ get; set;}
  		public List<AnnotationTag> getGuidList{ get; set;}
 
  
		public AnnotationTableViewSource (UITableView tableview)
		{
			annoTableView = tableview;
 			getGuidList = new List<AnnotationTag> ();
 			AnnotationList = AnnotationUtil.Instance.GetAnnotations();//Get all the list
  
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("filterAnnotationInAnnotation"), AnnotationFilterAnno);
 		}
  
		private void AnnotationFilterAnno(NSNotification obj)
		{
			AnnotationList.Clear();
  			if (AppDataUtil.Instance.AnnotationSegmentSelectedIndex == 0 ) {//all
				AnnotationList = AnnotationUtil.Instance.GetAnnotations(); 
 			} else if (AppDataUtil.Instance.AnnotationSegmentSelectedIndex == 1) {//highlight
				AnnotationList = AnnotationUtil.Instance.GetAnnotations().FindAll (o => o.Type == AnnotationType.Highlight);
 
			} else if (AppDataUtil.Instance.AnnotationSegmentSelectedIndex == 2) {//Note
 				AnnotationList = AnnotationUtil.Instance.GetAnnotations().FindAll (o => o.Type == AnnotationType.StickyNote);
 			} else if(AppDataUtil.Instance.AnnotationSegmentSelectedIndex == 3){
				AnnotationList = AnnotationUtil.Instance.GetAnnotations().FindAll (o => o.Type == AnnotationType.Orphan);//Orphan
 			}
  			annoTableView.ReloadData ();
 		}
		public override nint NumberOfSections (UITableView tableView)
		{
 			return 1;
		}
 		public override nint RowsInSection (UITableView tableview, nint section)
		{
			if(AnnotationList.Count !=0){
				return AnnotationList.Count;	
			} 	
			return 0;
 		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			CustomizedAnnotationTableViewCell cell = tableView.DequeueReusableCell (CustomizedAnnotationTableViewCell.Key) as CustomizedAnnotationTableViewCell;
			if (cell == null)
			cell = CustomizedAnnotationTableViewCell.Create();
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			if (AnnotationList.Count != 0){
				var annotation = AnnotationList[indexPath.Row];

				cell.PublicationNameLabel.Text = annotation.BookTitle;  
				DateTime date = annotation.UpdatedTime;

				cell.DateLabel.Text = date.ToString ("dd MMM yyyy");
 				string guidCardTitle = annotation.GuideCardName + ">" + annotation.TOCTitle;
				cell.GuidecardLabel.Text = guidCardTitle;  
				cell.HighlightTextLabel.Text = annotation.HighlightText;

				cell.NoteLabel.TextColor = ColorUtil.ConvertFromHexColorCode ("#cccccc");
				if (annotation.NoteText != "") {
					cell.NoteLabel.Text = annotation.NoteText;
				} else {
					cell.NoteLabel.Text = "Add a note...";
				} 
				getGuidList = annotation.CategoryTags;
				nfloat TagHeight = (tableView.Frame.Size.Width -60*8)/9;
				if (getGuidList.Count == 0) {
 
					UIView emptyBtn = new UIView (new CGRect (0, 10, 10, 10));
					emptyBtn.BackgroundColor = UIColor.White;
					emptyBtn.Layer.BorderColor = ColorUtil.ConvertFromHexColorCode ("#cccccc").CGColor;
					emptyBtn.Layer.BorderWidth = 1;
					emptyBtn.Layer.CornerRadius = 10 / 2;

					UILabel emptyLabel = new UILabel (new CGRect (18, 5, 120, 18));
					emptyLabel.Text = "Add a tag...";
					emptyLabel.TextColor = ColorUtil.ConvertFromHexColorCode ("#cccccc");
					emptyLabel.Font = UIFont.SystemFontOfSize (14);
					cell.TagContainerView.AddSubview (emptyLabel);
					cell.TagContainerView.AddSubview (emptyBtn);
				} else {
				
					for (int i=0;i<getGuidList.Count;i++){
						AnnotationTag tag = getGuidList[i];
						UIColor	defaultColor = ColorUtil.ConvertFromHexColorCode (tag.Color);
						TagsButton btn = new TagsButton (); 
						btn.Frame = new CGRect ((TagHeight +60)*(i%8),(i/8)*25, 60, 30); 
						btn.BackgroundColor = UIColor.White;
						btn.ColorView.BackgroundColor = defaultColor;
						btn.ColorLabel.Text = tag.Title;
						btn.Layer.BorderColor = UIColor.Clear.CGColor;
						btn.Layer.BorderWidth = 1;
						btn.Layer.CornerRadius = 10 / 2;
						btn.ClipsToBounds = true;
						cell.TagContainerView.AddSubview (btn);
					}
				
				}

 			}
  
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
 			var annotation = AnnotationList[indexPath.Row];

			string guidCardTitle = annotation.GuideCardName + ">" + annotation.TOCTitle;
 			CGSize guidCardHeight = TextDisplayUtil.GetStringBoundRect (guidCardTitle, UIFont.SystemFontOfSize (14), new CGSize (tableView.Frame.Size.Width - 30, 80));
			CGSize TocHeight = TextDisplayUtil.GetStringBoundRect (annotation.TOCTitle, UIFont.SystemFontOfSize (14), new CGSize (tableView.Frame.Size.Width - 30, 80));

 			CGSize ContentHeight = TextDisplayUtil.GetStringBoundRect (annotation.HighlightText, UIFont.SystemFontOfSize (14), new CGSize (tableView.Frame.Size.Width - 30, 80));

			CGSize NoteHeight = TextDisplayUtil.GetStringBoundRect (annotation.NoteText, UIFont.SystemFontOfSize (14), new CGSize (tableView.Frame.Size.Width - 30, 80));

 			getGuidList = annotation.CategoryTags;
			if (getGuidList.Count > 8) {
				heightBgView = 40;
			}else if (getGuidList.Count < 4) {
				heightBgView = 15;
			}

			return guidCardHeight.Height +TocHeight.Height+ContentHeight.Height+NoteHeight.Height+70+heightBgView;
		}  

 	}
}

