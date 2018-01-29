
using System;

using Foundation;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;


using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.iOS
{
	public class GotoTableViewControllerSource : UITableViewSource
	{
		public UILabel NoResultLabel{ get; set;}
 		public  List<PageSearchItem> pageList{ get; set;}
		public int PageNumber{ get; set;}
		public PageSearchItem  PageSearchItem{ get; set;}
		public bool IsInContentVC{ get; set; }

		UILabel titleLabel = new UILabel ();


		private UITableView tableView;

		public GotoTableViewControllerSource (UITableView tableview)
		{
			tableView =  tableview;
		
 			UIColor textColor = ColorUtil.ConvertFromHexColorCode ("#ABAEAB");
			NoResultLabel = new UILabel (new CGRect(55,110,210,80));
			NoResultLabel.Font = UIFont.SystemFontOfSize(14);
			NoResultLabel.Lines = 0;
			NoResultLabel.Text = "Use the number pad to go to \nthe page you're looking for.";
			NoResultLabel.TextAlignment = UITextAlignment.Center;
			NoResultLabel.TextColor = textColor;
			tableview.AddSubview(NoResultLabel);
			pageList = new List<PageSearchItem>();
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("inputPageNumber"), ChangePageNumber);


			titleLabel.Frame = new CoreGraphics.CGRect (0, 0, 320, 42);
			titleLabel.TextAlignment = UITextAlignment.Center;
			titleLabel.Font = UIFont.BoldSystemFontOfSize (17);
			titleLabel.Text = "Page Not Found"; 


		}

		private  async void ChangePageNumber(NSNotification obj)
		{
			if(obj.UserInfo != null ){
				string strNumber = obj.UserInfo.ObjectForKey(new NSString ("page")).ToString();
				PageNumber = int.Parse(strNumber);
			}
			var booId = AppDataUtil.Instance.GetCurrentPublication().BookId;
			pageList = await PageSearchUtil.Instance.SeachByPageNum (booId,PageNumber);
			if (PageNumber == 0) {
				NoResultLabel.Hidden = false;
				titleLabel.Text = "Page Not Found"; 

				NoResultLabel.Text = " The page you're looking for\n doesn't exist in this publication.\n Please try again.";
			} else if (pageList.Count == 0) {
				NoResultLabel.Hidden = false;
				titleLabel.Text = "Page Not Found"; 

				NoResultLabel.Text = " The page you're looking for\n doesn't exist in this publication.\n Please try again.";
 			} 
			else {
				titleLabel.Text = "Page Results"; 
 				NoResultLabel.Hidden = true;
 			}

			tableView.ReloadData();

		}


		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return pageList.Count;

		}
 
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (GotoTableViewControllerCell.Key) as GotoTableViewControllerCell;
			if (cell == null)
				cell = new GotoTableViewControllerCell ();
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			cell.contentLabel.Text = pageList[indexPath.Row].FileTitle; 
			cell.detailLabel.Text = pageList[indexPath.Row].GuideCardTitle;
 			return cell;
		}

 
		public  override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			PageSearchItem = pageList[indexPath.Row];
  			var tocId = PageSearchItem.TOCID;
  			AppDataUtil.Instance.SetOpendTOC (AppDataUtil.Instance.GetTOCNodeByID(tocId));
			AppDataUtil.Instance.SetHighlightedTOCNode (AppDataUtil.Instance.GetTOCNodeByID(tocId));
			AppDisplayUtil.Instance.DismissPopoverView ();

   
 		}



		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			UIView bgView =new UIView();
			bgView.Frame = new CoreGraphics.CGRect (0,0,320,44);
			bgView.BackgroundColor = UIColor.White;
 		   
			bgView.AddSubview (titleLabel);

 			UIView line = new UIView (new CGRect (0,43,320,1));
			line.BackgroundColor = ColorUtil.ConvertFromHexColorCode("#C3C4C2");
			bgView.AddSubview (line);
 			bgView.UserInteractionEnabled = false;

			return bgView;
 		}

		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return new nfloat(44);
		}


		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{   
			return new nfloat(60);
		}


	}
}

