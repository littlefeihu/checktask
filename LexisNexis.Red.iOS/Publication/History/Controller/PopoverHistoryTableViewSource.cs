using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.iOS
{
	public class PopoverHistoryTableViewSource : UITableViewSource
	{
		public List<RecentHistoryItem> HistoryList{ get; set;}

		/// <summary>
		/// A flag attribute which indentify whether this controller is poped from content view controller
		/// This property will determines the action when user touch rows
		/// </summary>
		/// <value><c>true</c> if this instance is in content V; otherwise, <c>false</c>.</value>
		public bool IsInContentVC{ get; set; }

		public PopoverHistoryTableViewSource (bool isInContentVC)
		{
			IsInContentVC = isInContentVC;
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return HistoryList != null ? HistoryList.Count : 0;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			CustomizedPopoverHisoryTableViewCell cell = tableView.DequeueReusableCell (CustomizedPopoverHisoryTableViewCell.Key) as CustomizedPopoverHisoryTableViewCell;
			if (cell == null)
				cell = CustomizedPopoverHisoryTableViewCell.Create();

			RecentHistoryItem item = HistoryList [indexPath.Row];

			cell.TitleLabel.Text = item.PublicationTitle;
			cell.GuideCardNameLabel.Text =  item.TOCTitle;     

			cell.TimeLabel.Text = ((DateTime)item.LastReadDate).ToString ("hh:mm t\\M, dd MMM yyyy"); 

			if (cell.TitleLabel.Constraints.Length > 0) {
				CGSize expectedTitleLabelSize = TextDisplayUtil.GetStringBoundRect(item.PublicationTitle, UIFont.BoldSystemFontOfSize(14), new CGSize (295, 600));
				for (int i = 0; i < cell.TitleLabel.Constraints.Length; i++) {
					if (cell.TitleLabel.Constraints [i].FirstAttribute == NSLayoutAttribute.Height && expectedTitleLabelSize.Height > 20) {
						cell.TitleLabel.Constraints [i].Constant = expectedTitleLabelSize.Height;
					}
				}

			}
  
			cell.CellColorHintView.BackgroundColor = ColorUtil.ConvertFromHexColorCode (item.ColorPrimary);

			return cell;
		}

		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return 0;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{

			RecentHistoryItem item = HistoryList [indexPath.Row];
			CGSize expectedTitleLabelSize = TextDisplayUtil.GetStringBoundRect(item.PublicationTitle, UIFont.BoldSystemFontOfSize(14), new CGSize (295, 600));

			CGSize expectedTOCTitleLabelSize = TextDisplayUtil.GetStringBoundRect(item.TOCTitle, UIFont.SystemFontOfSize(14), new CGSize (295, 600));

			float expectedTOCTitleHeight = (float)expectedTOCTitleLabelSize.Height;
			if (expectedTOCTitleHeight < 20) {
				expectedTOCTitleHeight = 20;
			} else if (expectedTOCTitleHeight > 50) {
				expectedTOCTitleHeight = 50;
			}

			return (expectedTitleLabelSize.Height > 20 ? expectedTitleLabelSize.Height : 20) + expectedTOCTitleHeight + 32;
		}

		public async override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			RecentHistoryItem item = HistoryList [indexPath.Row];
			List<Publication> cachedPublicationList = PublicationUtil.Instance.GetPublicationOffline ();

			var publication = cachedPublicationList.FirstOrDefault (o => o.BookId == item.BookId);


			if (await PageSearchUtil.Instance.IsPBO (publication.BookId)) {
				NSNotificationCenter.DefaultCenter.PostNotificationName ("JudgePBOBook", this,new NSDictionary ("PBOBook","YES"));
			} else {
				NSNotificationCenter.DefaultCenter.PostNotificationName ("JudgePBOBook", this,new NSDictionary ("PBOBook","NO"));

			}

			if (publication != null) {

				AppDataUtil.Instance.SetCurrentPublication (publication);
				var tocId =await PublicationContentUtil.Instance.GetTOCIDByDocId (publication.BookId, item.DOCID);
				AppDataUtil.Instance.SetOpendTOC (AppDataUtil.Instance.GetTOCNodeByID(tocId));
				AppDataUtil.Instance.SetHighlightedTOCNode (AppDataUtil.Instance.GetTOCNodeByID(tocId));

				AppDisplayUtil.Instance.DismissPopoverView ();
				tableView.CellAt (indexPath).Selected = false;

				AppDataUtil.Instance.AddBrowserRecord (new ContentBrowserRecord(AppDataUtil.Instance.GetCurrentPublication().BookId, tocId, 0));//add browser record

				if (IsInContentVC) {
					NSNotificationCenter.DefaultCenter.PostNotificationName("RemoveContentSearchResultViewInTOC", this);
					NSNotificationCenter.DefaultCenter.PostNotificationName("RemoveContentSearchResultViewInIndex", this); 
					NSNotificationCenter.DefaultCenter.PostNotificationName("RemoveContentSearchResultViewInAnnotation", this);

				} else {
					AppDisplayUtil.Instance.GotoPublicationDetailViewController ();
				}
			}

		}
	}
}

