
using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;


namespace LexisNexis.Red.iOS
{
	public class HighlightTableViewControllerSource : UITableViewSource
	{
		private UIButton leftEditButton;
		private	UIButton rightAddButton;
 		private	UIView line;
		UIViewController  highlightVC;
		UITableView highlightTable;
		public	List<Guid>tagGuidList = new List<Guid>();


 		public	List<Tag> tagList = new List<Tag>();


		public HighlightTableViewControllerSource (UIViewController controller,UITableView table)
		{
			this.initView();
			highlightVC = controller;


			highlightTable = table;
 			tagList = AppDataUtil.Instance.TagList;
			foreach (var tag in tagList) {
				tag.IsSelected = true;
			}

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
			highlightTable.ReloadData();
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			return tagList.Count;
		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (HighlightTableViewControllerCell.Key) as HighlightTableViewControllerCell;
			if (cell == null)
				cell = HighlightTableViewControllerCell.Create ();

			cell.SelectionStyle = UITableViewCellSelectionStyle.None;

			Tag tag = AppDataUtil.Instance.TagList [indexPath.Row];
			cell.ContentView.Add(new DoubleCircleView(5, 6, ColorUtil.ConvertFromHexColorCode(tag.AnnoTag.Color), UIColor.White, 15, 16));
			cell.tagNameLabel.Text = tag.AnnoTag.Title;
			cell.TintColor = UIColor.Red;
			cell.Accessory = tag.IsSelected?UITableViewCellAccessory.None:UITableViewCellAccessory.Checkmark;

			return cell;
		}


		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			HighlightTableViewControllerCell cell = (HighlightTableViewControllerCell)tableView.CellAt(indexPath);

			tagList[indexPath.Row].IsSelected = !tagList [indexPath.Row].IsSelected;
 			Tag tag = tagList [indexPath.Row];

			if (cell.Accessory == UITableViewCellAccessory.None) {
				Guid tagId = AnnCategoryTagUtil.Instance.AddTag(tag.AnnoTag.Title, tag.AnnoTag.Color);
				tagGuidList.Add(tagId);
				AppDisplayUtil.Instance.highlightVC.getGuidList = tagGuidList;
			} else if(cell.Accessory == UITableViewCellAccessory.Checkmark) {
				if (tagGuidList.Count != 0) {
					tagGuidList.RemoveAt(indexPath.Row);
					AppDisplayUtil.Instance.highlightVC.getGuidList = tagGuidList;
				}
			}
			tableView.ReloadData ();
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return new nfloat (44);
		}
		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			UIView bgView =new UIView();
			bgView.Frame = new CoreGraphics.CGRect (0,0,320,44);
			bgView.BackgroundColor = UIColor.White;
			bgView.AddSubview (leftEditButton);
 			bgView.AddSubview (rightAddButton);

			bgView.AddSubview (line);

			return bgView;
		}
		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return 44;
		}

		private  void initView(){

			leftEditButton = new UIButton (new CoreGraphics.CGRect (16,0,80,44));
			leftEditButton.TitleLabel.Font = UIFont.SystemFontOfSize (17);
			leftEditButton.SetTitle ("Assign to:",UIControlState.Normal);
			leftEditButton.SetTitleColor (UIColor.LightGray,UIControlState.Normal); 
 
 
			rightAddButton = new UIButton (new CoreGraphics.CGRect (260,0,60,44));
			rightAddButton.TitleLabel.Font = UIFont.SystemFontOfSize (17);
			rightAddButton.SetTitle ("Edit",UIControlState.Normal);
			rightAddButton.SetTitleColor (UIColor.Red,UIControlState.Normal); 
			rightAddButton.TouchUpInside += RightAddButton_TouchUpInside;

			line = new UIView ();
			line.Frame = new CoreGraphics.CGRect (0,43,320,1);
			line.BackgroundColor = ColorUtil.ConvertFromHexColorCode ("#C3C4C2");
		}

		void RightAddButton_TouchUpInside (object sender, EventArgs e)
		{
 			UIStoryboard storyboard = UIStoryboard.FromName ("PopoverTagManage", NSBundle.MainBundle);
			UserTagTableViewController userTagVC = (UserTagTableViewController)storyboard.InstantiateViewController ("NavigationController");
			userTagVC.typeString = "NoteTag";
 			UINavigationController navController = new UINavigationController (userTagVC);
			navController.NavigationBar.TintColor = UIColor.Red;
			userTagVC.Title = "Edit Tag";
			highlightVC.NavigationController.PushViewController (userTagVC, true);
  		}

	}
}

