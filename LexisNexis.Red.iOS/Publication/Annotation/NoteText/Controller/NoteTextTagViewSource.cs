
using System;

using Foundation;
using UIKit;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using System.Collections.Generic;
namespace LexisNexis.Red.iOS
{
	public class NoteTextTagViewSource : UITableViewSource
	{

		public	List<Tag> tagList = new List<Tag>();
		public	List<Guid> tagGuidList;
  	  	UITableView tableView;

		public NoteTextTagViewSource (UITableView TableView)
		{
			tagList = AppDataUtil.Instance.TagList;
			foreach (var tag in tagList) {
				tag.IsSelected = true;
			}
			tableView = TableView;
			tagGuidList = new List<Guid>();
 			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("addAnnotationTag"), delegate(NSNotification obj) {
				this.reloadTableView();
			});


			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("RefreshTagTableView"), delegate(NSNotification obj) {
				this.reloadTableView();
 			});
		}


		private void reloadTableView(){
			tagList.Clear();
			List<AnnotationTag> annoTagList = AnnCategoryTagUtil.Instance.GetTags ();
			foreach (var annoTag in annoTagList) {
				tagList.Add(new Tag(annoTag, true));
			}
			tableView.ReloadData();
 		}

 		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tagList.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (NoteTextTagViewCell.Key) as NoteTextTagViewCell;
			if (cell == null)
				cell = NoteTextTagViewCell.Create ();

			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			Tag tag = tagList[indexPath.Row];
			cell.ContentView.Add(new DoubleCircleView(5, 6, ColorUtil.ConvertFromHexColorCode(tag.AnnoTag.Color), UIColor.White, 15, 16));
			cell.TagNameLabel.Text = tag.AnnoTag.Title;
			cell.TintColor = UIColor.Red;
			cell.Accessory = tag.IsSelected?UITableViewCellAccessory.None:UITableViewCellAccessory.Checkmark;

			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return new nfloat (44);
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}


		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
 			NoteTextTagViewCell cell = (NoteTextTagViewCell)tableView.CellAt(indexPath);
			tagList[indexPath.Row].IsSelected = !tagList [indexPath.Row].IsSelected;
			Tag tag = tagList [indexPath.Row];

			if (cell.Accessory == UITableViewCellAccessory.None) {
 				tagGuidList.Add(tag.AnnoTag.TagId);
				AppDisplayUtil.Instance.noteVC.getGuidList = tagGuidList;
 			} else if(cell.Accessory == UITableViewCellAccessory.Checkmark) {
				if (tagGuidList.Count != 0) {
					tagGuidList.Remove (tag.AnnoTag.TagId);
					AppDisplayUtil.Instance.noteVC.getGuidList = tagGuidList;
  				}
			}
   			tableView.ReloadData ();
		}

 		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete) {
				AnnCategoryTagUtil.Instance.DeleteTag(tagList[indexPath.Row].AnnoTag.TagId);
				tagList.Remove(AppDataUtil.Instance.TagList[indexPath.Row]);
				tableView.ReloadData ();
			}
		}
	}
}


