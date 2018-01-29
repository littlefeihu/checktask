using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	partial class IndexTableViewController : UITableViewController
	{
		public string[] SectionTitleArr{ get; set; }
		public Dictionary<string, List<Index>> IndexDict{ get; set; }

		public IndexTableViewController (IntPtr handle) : base (handle)
		{
			SectionTitleArr = new string[0];
		}

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Register the TableView's data source
			await LoadIndex();
			TableView.TableFooterView = new UIView ();

		}
		
		public async override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			//reload index when switch publlication
			if (AppDataUtil.Instance.GetOpendIndex () == null) {
				await LoadIndex ();
				TableView.ReloadData();
			}
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return SectionTitleArr.Length;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return IndexDict[SectionTitleArr[section]].Count;
		}

		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return SectionTitleArr [section];
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (IndexTableViewCell.Key) as IndexTableViewCell;
			if (cell == null)
				cell = new IndexTableViewCell ();

			//populate the cell with the appropriate data based on the indexPath
			cell.TextLabel.Text = IndexDict[SectionTitleArr[indexPath.Section]][indexPath.Row].Title;
			return cell;		
		}

		public override string[] SectionIndexTitles (UITableView tableView)
		{
			return SectionTitleArr;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			string sectionName = SectionTitleArr [indexPath.Section];
			Index selectedIndex = IndexDict [sectionName] [indexPath.Row];
			tableView.CellAt (indexPath).Selected = false;
			if (selectedIndex.Title.Substring (0, 1) == AppDataUtil.Instance.GetOpendIndex ().Title.Substring (0, 1)) {//if current index section has opened
				NSNotificationCenter.DefaultCenter.PostNotificationName("ExecuteJS", this, new NSDictionary("jsStr", "$(window).scrollTo($(\".main[data-filename='" + selectedIndex.FileName + "']\"), 300)"));
			} else {
				AppDataUtil.Instance.SetOpendIndex (IndexDict [sectionName] [indexPath.Row]);
			}
		}


		private async Task LoadIndex()
		{
			IndexDict = await PublicationContentUtil.Instance.GetIndexsByBookId (AppDataUtil.Instance.GetCurrentPublication().BookId);
			if (IndexDict != null) {
				SectionTitleArr = new string[IndexDict.Count];
				int i = 0;
				foreach (var key in IndexDict.Keys) {
					SectionTitleArr [i++] = key;
				}
				AppDataUtil.Instance.SetOpendIndex (IndexDict.First ().Value [0]);
				TableView.Hidden = false;

			} else {
				SectionTitleArr = new string[0];
				AppDataUtil.Instance.SetOpendIndex (null);
				TableView.Hidden = true;
			}

		}

	}
}
