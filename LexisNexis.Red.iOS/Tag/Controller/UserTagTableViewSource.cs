using System;

using Foundation;
using UIKit;
using LexisNexis.Red.Common.Business;
using System.Linq;
using LexisNexis.Red.Common.BusinessModel;
using System.Collections.Generic;

namespace LexisNexis.Red.iOS
{
	public class UserTagTableViewSource : UITableViewSource
	{
		private UIViewController curVC;
		public List<Tag> TagList;
		UITableView tagTableView;

		public UserTagTableViewSource (UIViewController  controller , UITableView tableView)
		{
			curVC = controller;
			tagTableView = tableView;
			TagList = AppDataUtil.Instance.TagList;

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("addAnnotationTag"), delegate(NSNotification obj) {
				TagList.Clear();
				List<AnnotationTag> annoTagList = AnnCategoryTagUtil.Instance.GetTags ();

				foreach (var annoTag in annoTagList) {
					TagList.Add(new Tag(annoTag, true));
				}
				tableView.ReloadData();

			});
		} 

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return TagList.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			CustomizeTagTableViewCell cell = tableView.DequeueReusableCell (CustomizeTagTableViewCell.Key) as CustomizeTagTableViewCell;
			if (cell == null)
				cell = CustomizeTagTableViewCell.Create ();

			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			Tag tag =  TagList[indexPath.Row];
			cell.annotationTagId = tag.AnnoTag;
			cell.ContentView.Add(new DoubleCircleView(5, 6, ColorUtil.ConvertFromHexColorCode(tag.AnnoTag.Color), UIColor.White, 15, 16));
			cell.TagNameLabel.Text = tag.AnnoTag.Title; 

			return cell;
		}


		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true;
		}

		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			List<AnnotationTag> tagIdList = new List<AnnotationTag> ();
			foreach (var annotationTag in TagList) {
				tagIdList.Add (annotationTag.AnnoTag);
			}
			CustomizeTagTableViewCell sourceCell = (CustomizeTagTableViewCell)tableView.CellAt (sourceIndexPath);
			CustomizeTagTableViewCell dexCell = (CustomizeTagTableViewCell)tableView.CellAt (destinationIndexPath);
			int sourceIndexAtTagIdList = tagIdList.IndexOf(sourceCell.annotationTagId); 
			int desIndexAtTagIdList = tagIdList.IndexOf (dexCell.annotationTagId);
			tagIdList.RemoveAt (sourceIndexAtTagIdList);
			tagIdList.Insert (desIndexAtTagIdList,sourceCell.annotationTagId);
			AnnCategoryTagUtil.Instance.Sort (tagIdList);
			tableView.ReloadData ();
			NSNotificationCenter.DefaultCenter.PostNotificationName("RefreshTagTableView", this);
		}


		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete) {
				string alertMsg = "You are about to delete a tag which will remove the tag from all instances where it has been used across LexisNexis Red. This cannot be undone. Do you want to delete this tag?";
				var deleteAlert = UIAlertController.Create ("Delete Tag", alertMsg, UIAlertControllerStyle.Alert);
				deleteAlert.AddAction (UIAlertAction.Create("Cancel",UIAlertActionStyle.Default,null)); 
				deleteAlert.AddAction(UIAlertAction.Create("OK",UIAlertActionStyle.Default,alert => deleteTag(indexPath)));
				curVC.PresentViewController(deleteAlert,true,null);
			}
		}

		public void  deleteTag(NSIndexPath indexPath){
			AnnCategoryTagUtil.Instance.DeleteTag(TagList[indexPath.Row].AnnoTag.TagId);
			TagList.Remove(TagList[indexPath.Row]);
			tagTableView.ReloadData ();
			NSNotificationCenter.DefaultCenter.PostNotificationName("RefreshTagTableView", this);
 		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			UIStoryboard storyboard = UIStoryboard.FromName ("TagEditstory", NSBundle.MainBundle);
			TagEditViewController tagEditVC = (TagEditViewController)storyboard.InstantiateViewController ("FromTable");
			UINavigationController navController = new UINavigationController (tagEditVC);
			navController.NavigationBar.TintColor = UIColor.Red;
			tagEditVC.Title = "Edit Tag";
			curVC.NavigationController.PushViewController (tagEditVC, true);
			tagEditVC.AnnoTag = TagList[indexPath.Row].AnnoTag;  

		}

	}
}
