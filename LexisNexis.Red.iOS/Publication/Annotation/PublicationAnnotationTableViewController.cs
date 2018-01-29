using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using System.Linq;

namespace LexisNexis.Red.iOS
{
	public partial class PublicationAnnotationTableViewController : UITableViewController
	{

		public float heightBgView;

		public List<Annotation> AnnotationList{ get; set;}
		public Dictionary<string, List<Annotation>> AnnoListDict{ get; set; }
		public List<string> SectionTitleArr{ get; set; }
		public List<AnnotationTag> getGuidList{ get; set;}
		public List<Guid> selectTagId{ get; set;}




		public PublicationAnnotationTableViewController (IntPtr handle) : base (handle)
		{

			AppDisplayUtil.Instance.publicationAnnotationVC = this;
			selectTagId = new List<Guid>();
			getGuidList = new List<AnnotationTag> ();
			SectionTitleArr = new List<string> ();
			AnnoListDict = new Dictionary<string, List<Annotation>> ();
 			AnnotationList = AnnotationUtil.Instance.GetAnnotationsByBookId(AppDataUtil.Instance.GetCurrentPublication ().BookId);//Get all the list

			foreach (var annotation in AnnotationList) {
				if (SectionTitleArr.IndexOf (annotation.GuideCardName) == -1) {
					SectionTitleArr.Add (annotation.GuideCardName);
				}
			}
 			for (int i = 0; i < SectionTitleArr.Count; i++) {
				List<Annotation> annoList = AnnotationList.FindAll (o => o.GuideCardName == SectionTitleArr[i]);
				AnnoListDict [SectionTitleArr [i]] = annoList;
			} 
		}



		public override nint NumberOfSections (UITableView tableView)
		{
			return SectionTitleArr.Count; 
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return AnnoListDict[SectionTitleArr[(int)section]].Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			PublicationAnnotationCell cell = tableView.DequeueReusableCell (PublicationAnnotationCell.Key) as PublicationAnnotationCell;
			if (cell == null)
				cell = PublicationAnnotationCell.Create();
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			var annotation = AnnoListDict [SectionTitleArr [indexPath.Section]] [indexPath.Row];
			UIColor TextColor = ColorUtil.ConvertFromHexColorCode ("#959595");

			DateTime date = annotation.UpdatedTime;
			cell.dateLabel.TextColor=TextColor;
			cell.dateLabel.Text = date.ToString("dd MMM yyyy");

			cell.TocLabel.Text = annotation.TOCTitle;
			cell.TocLabel.TextColor=ColorUtil.ConvertFromHexColorCode ("#000000");

			cell.conLabel.Text = annotation.HighlightText;
			cell.conLabel.TextColor=ColorUtil.ConvertFromHexColorCode ("#000000");

			cell.imageIma.Image = new UIImage ("Images/Setting/About.png");
			cell.NoteLabel.TextColor = ColorUtil.ConvertFromHexColorCode ("#cccccc");
			if (annotation.NoteText != "") {
				cell.NoteLabel.Text = annotation.NoteText;
			} else {
				cell.NoteLabel.Text = "Add a note...";
			}

			nfloat TagHeight = (tableView.Frame.Size.Width -60*4)/5;
			getGuidList = annotation.CategoryTags;
 			if (getGuidList.Count == 0) {
				UIView emptyBtn = new UIView (new CGRect(3,5,10, 10));
				emptyBtn.BackgroundColor = UIColor.White;
				emptyBtn.Layer.BorderColor = ColorUtil.ConvertFromHexColorCode ("#cccccc").CGColor;
				emptyBtn.Layer.BorderWidth = 1;
				emptyBtn.Layer.CornerRadius = 10/2;

				UILabel emptyLabel = new UILabel (new CGRect(25,0,120, 18));
				emptyLabel.Text = "Add a tag...";
				emptyLabel.TextColor = ColorUtil.ConvertFromHexColorCode ("#cccccc");
				emptyLabel.Font = UIFont.SystemFontOfSize (14);
				cell.bgView.AddSubview (emptyLabel);
				cell.bgView.AddSubview (emptyBtn);
			} else {

				for (int i = 0; i < getGuidList.Count; i++) {
					AnnotationTag tag = getGuidList [i];
					UIColor	defaultColor = ColorUtil.ConvertFromHexColorCode (tag.Color);
					TagsButton btn = new TagsButton (); 
					btn.Frame = new CGRect ((TagHeight + 60) * (i % 4)-15 + TagHeight, (i / 4) * 25, 60, 30); 
					btn.BackgroundColor = UIColor.White;
					btn.ColorView.BackgroundColor = defaultColor;
					btn.ColorLabel.Text = tag.Title;
 					cell.bgView.AddSubview (btn);
				}
 			}
			return cell;
		}


		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var annotation = AnnoListDict [SectionTitleArr [indexPath.Section]] [indexPath.Row];

			CGSize TocHeight = TextDisplayUtil.GetStringBoundRect (annotation.TOCTitle, UIFont.SystemFontOfSize (14), new CGSize (tableView.Frame.Size.Width - 30, 80));

			string text = annotation.HighlightText;
			CGSize ContentHeight = TextDisplayUtil.GetStringBoundRect (text, UIFont.SystemFontOfSize (14), new CGSize (tableView.Frame.Size.Width - 30, 80));

			CGSize NoteHeight = TextDisplayUtil.GetStringBoundRect (annotation.NoteText, UIFont.SystemFontOfSize (14), new CGSize (tableView.Frame.Size.Width - 30, 80));

			getGuidList = annotation.CategoryTags;
			if (getGuidList.Count > 4) {
				heightBgView = 60;
			} else if (getGuidList.Count == 0) {
				heightBgView = 30;
			} else if (getGuidList.Count < 4) {
				heightBgView = 40;
			}

			return TocHeight.Height+ContentHeight.Height+NoteHeight.Height+70+heightBgView;
		}  


		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return 30;
		}

		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			UIView bgView = new UIView ();
			bgView.BackgroundColor = ColorUtil.ConvertFromHexColorCode ("#CBCCCD");
			UILabel seactionText = new UILabel ();
			seactionText.Frame = new CGRect (20,5,280,20);
			seactionText.Font = UIFont.SystemFontOfSize (17);
			seactionText.TextColor = UIColor.Black;
			seactionText.Text = SectionTitleArr [(int)section];
			bgView.AddSubview (seactionText);
			return bgView;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TableView.TableFooterView = new UIView ();
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("filterAnnotationInPublication"), FilterAnnotation);
		}

		private void FilterAnnotation(NSNotification obj)
		{
			SectionTitleArr.Clear ();
			AnnoListDict.Clear ();

			List<Annotation> DissPlan = null;
 			if (AppDataUtil.Instance.AnnotationFilterSelectedIndex == 0) {//all
  				DissPlan = AnnotationUtil.Instance.GetAnnotations (AppDataUtil.Instance.GetCurrentPublication ().BookId,AnnotationType.All,selectTagId,false);
 			
			} else if (AppDataUtil.Instance.AnnotationFilterSelectedIndex == 1) {//highlight\
				DissPlan = AnnotationUtil.Instance.GetAnnotations (AppDataUtil.Instance.GetCurrentPublication ().BookId,AnnotationType.Highlight,selectTagId,false);
 			} else if (AppDataUtil.Instance.AnnotationFilterSelectedIndex == 2) {//Note
 				DissPlan = AnnotationUtil.Instance.GetAnnotations (AppDataUtil.Instance.GetCurrentPublication ().BookId,AnnotationType.StickyNote,selectTagId,false);
  			} 
 			List<Annotation> noTags=null;
			if (AppDisplayUtil.Instance.NoTagIndeifier == "SelectNoTag") {
				noTags = DissPlan.FindAll(o => o.CategoryTagIDs == null || o.CategoryTagIDs.Count == 0).ToList();//NoTag的筛选
 			}
  			
			if (noTags != null){
				DissPlan.Clear ();
				DissPlan.AddRange (noTags);
 			}
 			foreach (var annotation in DissPlan) {
				if (SectionTitleArr.IndexOf (annotation.GuideCardName) == -1) {
					SectionTitleArr.Add (annotation.GuideCardName);
				}
			}
			for (int i = 0; i < SectionTitleArr.Count; i++) {
				List<Annotation> annoList = DissPlan.FindAll (o => o.GuideCardName == SectionTitleArr[i]);
				AnnoListDict [SectionTitleArr [i]] = annoList;
			} 
			TableView.ReloadData ();
		}
	}
}

 