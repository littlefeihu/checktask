using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.iOS
{
	public class HistoryTableViewSource : UITableViewSource
	{

		public List<RecentHistoryItem> HistoryList{ get; set;}
		private UITableView tableViewT;

		public HistoryTableViewSource (UITableView tableView)
		{ 
			tableViewT = tableView;
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("GainBooksCount"), delegate(NSNotification obj) {
				HistoryList = PublicationContentUtil.Instance.GetRecentHistory ();
				tableViewT.ReloadData();
			});
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

			RecentHistoryItem item = HistoryList [indexPath.Row];

			CustomizedHistoryTableViewCell cell = tableView.DequeueReusableCell (CustomizedHistoryTableViewCell.Key) as CustomizedHistoryTableViewCell;
			if (cell == null)
				cell = CustomizedHistoryTableViewCell.Create();

			cell.TitleLabel.Text = item.PublicationTitle;
			cell.TitleLabel.LineBreakMode = UILineBreakMode.MiddleTruncation;
			cell.GuideCardNameLabel.Text =  item.TOCTitle;
			cell.TimeLabel.Text = ((DateTime)item.LastReadDate).ToString ("hh:mm t\\M, dd MMM yyyy");
			cell.CellColorHintView.BackgroundColor = ColorUtil.ConvertFromHexColorCode (item.ColorPrimary);

			return cell;

		}

		public override nfloat GetHeightForHeader (UITableView tableView, nint section)
		{
			return 0;
		}

		public  async override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			RecentHistoryItem item = HistoryList [indexPath.Row];
			List<Publication> cachedPublicationList = PublicationUtil.Instance.GetPublicationOffline ();

			var publication = cachedPublicationList.FirstOrDefault (o => o.BookId == item.BookId);
			if (publication != null) {
				AppDataUtil.Instance.SetCurrentPublication (publication);
				var tocId =await PublicationContentUtil.Instance.GetTOCIDByDocId (publication.BookId, item.DOCID);
				AppDataUtil.Instance.SetOpendTOC (AppDataUtil.Instance.GetTOCNodeByID(tocId));
				AppDataUtil.Instance.SetHighlightedTOCNode (AppDataUtil.Instance.GetTOCNodeByID(tocId));

				AppDisplayUtil.Instance.GotoPublicationDetailViewController ();
				AppDataUtil.Instance.AddBrowserRecord (new ContentBrowserRecord(AppDataUtil.Instance.GetCurrentPublication().BookId, tocId, 0));//add browser record

				HistoryList = PublicationContentUtil.Instance.GetRecentHistory ();
				tableView.ReloadData ();
			}
		}
	}
}

