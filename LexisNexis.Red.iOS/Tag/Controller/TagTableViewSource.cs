
using System;
using System.Collections;
using System.Collections.Generic;


using Foundation;
using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	public class TagTableViewSource : UITableViewSource
	{

		private bool isNoTagSelected = true;
		public List<Tag> tagList ;
		UITableView tagTableView;
		List<Guid> seleTagId = new List<Guid>();


		public TagTableViewSource (UITableView Tagtable)
		{
			tagTableView = Tagtable;
			tagList = AppDataUtil.Instance.TagList;
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("RefreshTagTableView"), delegate(NSNotification obj) {
				tagList.Clear();
				List<AnnotationTag> annoTagList = AnnCategoryTagUtil.Instance.GetTags ();
				foreach (var annoTag in annoTagList) {
					tagList.Add(new Tag(annoTag, true));
				}
				tagTableView.ReloadData();
			});

		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tagList.Count + 2;
		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			CustomizeTagTableViewCell cell = tableView.DequeueReusableCell (CustomizeTagTableViewCell.Key) as CustomizeTagTableViewCell;
			if (cell == null)
				cell = CustomizeTagTableViewCell.Create ();

			cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			if (indexPath.Row == 0) {
				cell.TagNameLabel.Text = "All Tags";
				cell.ContentView.AddSubview(new AllTagView (12, 10, 16));

				bool isAllSelected = isNoTagSelected;
				if (isAllSelected) {
					foreach (var tag in tagList) {
						if (!tag.IsSelected) {
							isAllSelected = false;
							break;
						}
					}
				}
				cell.Accessory = isAllSelected ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			} else if(indexPath.Row == 1) {
				cell.ContentView.Add(new DoubleCircleView(4, 5, UIColor.White, ColorUtil.ConvertFromHexColorCode( "#808080" ), 16, 17));
				cell.TagNameLabel.Text = "No tag"; 
				cell.Accessory = isNoTagSelected ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;

			} else {
				Tag tag = tagList [indexPath.Row - 2];
				cell.ContentView.Add(new DoubleCircleView(5, 6, ColorUtil.ConvertFromHexColorCode(tag.AnnoTag.Color), UIColor.White, 15, 16));
				cell.TagNameLabel.Text = tag.AnnoTag.Title; 
 				cell.Accessory = tag.IsSelected? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			}

			return cell;
		}


		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
  			if (indexPath.Row == 0) {//check all or uncheck
				bool isSelectedAll = tableView.CellAt (indexPath).Accessory == UITableViewCellAccessory.None;
 				isNoTagSelected = isSelectedAll;
 
				for (int i = 0; i < tagList.Count; i++) {
					tagList [i].IsSelected = isSelectedAll;
				}
 			}else if(indexPath.Row == 1){
  				isNoTagSelected = !isNoTagSelected;
				AppDisplayUtil.Instance.NoTagIndeifier = "SelectNoTag";
 			}else{
				tagList [indexPath.Row - 2].IsSelected = !tagList [indexPath.Row - 2].IsSelected;
 				Tag tag = tagList [indexPath.Row - 2];
				seleTagId.Add (tag.AnnoTag.TagId);
				AppDisplayUtil.Instance.publicationAnnotationVC.selectTagId = seleTagId;
  			}
			tableView.ReloadData ();
  
			NSNotificationCenter.DefaultCenter.PostNotificationName("filterAnnotationInPublication", this); //,new NSDictionary ("Guid",guidId )
		} 
	}
}

