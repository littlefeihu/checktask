
using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	public class PublicationSortingSource : UITableViewSource
	{
		private List<Publication> cachedPublications;

		private PublicationSortingController curViewController;
		public HistoryTableViewController historyTVC{ get; set; }
		List<RecentHistoryItem> historyList{ get; set;}

  
		public PublicationSortingSource (UIViewController controller)
		{
			cachedPublications = PublicationUtil.Instance.GetPublicationOffline ();
			curViewController = (PublicationSortingController)controller;
		}

		#region override
		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			return cachedPublications.Count;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return "";
		}

		public override string TitleForFooter (UITableView tableView, nint section)
		{
			return "";
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (PublicationSortingCell.Key) as PublicationSortingCell;
			if (cell == null)
				cell = new PublicationSortingCell ();
			
			// TODO: populate the cell with the appropriate data based on the indexPath
			int publicationIndex = (int)indexPath.LongRow;
			Publication curPublication = cachedPublications[publicationIndex];
			cell.TextLabel.Text = curPublication.Name;
			cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
			cell.PublicationId = curPublication.BookId;
			cell.TextLabel.Lines = 0;
			CGSize expectedLableSize = TextDisplayUtil.GetStringBoundRect(curPublication.Name, UIFont.SystemFontOfSize(14), new CGSize (200, 600));

			if (expectedLableSize.Height > 21) {
				cell.TextLabel.Frame = new CGRect (cell.TextLabel.Frame.X, cell.TextLabel.Frame.Y, 200, expectedLableSize.Height);
			}

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

		public async override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			List<int> publicationIds = new List<int> ();
 			foreach (var publication in cachedPublications) {
				publicationIds.Add (publication.BookId);
			}
			PublicationSortingCell sourceCell = (PublicationSortingCell)tableView.CellAt (sourceIndexPath);
			PublicationSortingCell desCell = (PublicationSortingCell)tableView.CellAt (destinationIndexPath);
			int sourceIndexAtPublicationIdList = publicationIds.IndexOf(sourceCell.PublicationId);

			int desIndexAtPublicationIdList = publicationIds.IndexOf (desCell.PublicationId);

			publicationIds.RemoveAt (sourceIndexAtPublicationIdList);
			publicationIds.Insert (desIndexAtPublicationIdList, sourceCell.PublicationId);
 
			await PublicationUtil.Instance.OrganiseDlsOrder (publicationIds);
 			cachedPublications = PublicationUtil.Instance.GetPublicationOffline ();
 			curViewController.ReloadPublicationList ();
 
 		}

	

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
		{
			if (editingStyle == UITableViewCellEditingStyle.Delete) {
				var deletePublicationAlert = UIAlertController.Create ("Delete publication", "You will have to call our customer support if you want to re-install it", UIAlertControllerStyle.Alert);
				deletePublicationAlert.AddAction (UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, null));
				deletePublicationAlert.AddAction (UIAlertAction.Create ("Delete", UIAlertActionStyle.Default, action=> ConfirmDeletePublictaion(tableView, indexPath)));
				curViewController.PresentViewController (deletePublicationAlert, true, null);
			} 
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			int publicationIndex = (int)indexPath.LongRow;
			Publication curPublication = cachedPublications[publicationIndex];
			CGSize expectedLableSize = TextDisplayUtil.GetStringBoundRect(curPublication.Name, UIFont.SystemFontOfSize(14), new CGSize (200, 600));

			return ((expectedLableSize.Height + 24) > 44) ? (expectedLableSize.Height + 24) : 44;
		}
		#endregion
		

		public async void ConfirmDeletePublictaion(UITableView tableView, NSIndexPath indexPath)
		{
			if (cachedPublications [(int)indexPath.LongRow] != null) {
				await PublicationUtil.Instance.DeletePublicationByUser (cachedPublications[(int)indexPath.LongRow].BookId);
				cachedPublications.RemoveAt ((int)indexPath.LongRow);
				tableView.DeleteRows (new NSIndexPath[]{indexPath}, UITableViewRowAnimation.Automatic);
				curViewController.ReloadPublicationList ();
 
			NSNotificationCenter.DefaultCenter.PostNotificationName("GainBooksCount", this);

   			}

		}

	}
}

