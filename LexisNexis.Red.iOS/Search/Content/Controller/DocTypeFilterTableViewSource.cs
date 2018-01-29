
using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.iOS
{
	public class DocTypeFilterTableViewSource : UITableViewSource
	{
		public List<string> DocTypeNameList{ get; set;}

		public Dictionary<string, bool> DocTypeSelectedStatus{ get; set;}

		public Dictionary<string, ContentCategory> DocTypeMapping{ get; set;}

		public DocTypeFilterTableViewSource ()
		{
			DocTypeNameList = new List<string> ();
			DocTypeSelectedStatus = new Dictionary<string, bool> ();
			DocTypeMapping = new Dictionary<string, ContentCategory> ();

			DocTypeNameList.AddRange (new []{ "All", "Legislation", "Commentary", "Forms & Precedents" });
			DocTypeMapping.Add ("All", ContentCategory.All);
			DocTypeMapping.Add ("Legislation", ContentCategory.LegislationType);
			DocTypeMapping.Add ("Commentary", ContentCategory.CommentaryType);
			DocTypeMapping.Add ("Forms & Precedents", ContentCategory.FormsPrecedentsType);


			foreach (var docTypeName in DocTypeNameList) {
				DocTypeSelectedStatus.Add (docTypeName, true);
			}
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// TODO: return the actual number of sections
			return 1;

		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			// TODO: return the actual number of items in the section
			return DocTypeNameList.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (DocTypeFilterTableViewCell.Key) as DocTypeFilterTableViewCell;
			if (cell == null)
				cell = new DocTypeFilterTableViewCell ();
			
			cell.TextLabel.Text = DocTypeNameList [indexPath.Row];
			cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
			cell.Accessory = DocTypeSelectedStatus[DocTypeNameList [indexPath.Row]] ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			cell.SelectionStyle = UITableViewCellSelectionStyle.None;
			
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Row == 0) {//all
				if (DocTypeSelectedStatus [DocTypeNameList [indexPath.Row]]) {// All is checked
					foreach (var docTypeName in DocTypeNameList) {
						DocTypeSelectedStatus[docTypeName] = false;
					}
				} else {
					foreach (var docTypeName in DocTypeNameList) {
						DocTypeSelectedStatus[docTypeName] = true;
					}
				}
			} else {
				DocTypeSelectedStatus [DocTypeNameList [indexPath.Row]] = !DocTypeSelectedStatus [DocTypeNameList [indexPath.Row]];
				DocTypeSelectedStatus [DocTypeNameList [0]] = true;
				for (int i = 1; i < DocTypeNameList.Count; i++) {
					if (!DocTypeSelectedStatus [DocTypeNameList [i]]) {
						DocTypeSelectedStatus [DocTypeNameList [0]] = false;
						break;
					}
				}
			}
			tableView.ReloadData ();

			List<string> selectedDocTypeNameList = new List<string> ();
			List<ContentCategory> selecteDocTypeList = new List<ContentCategory> ();
			foreach (var kvp in DocTypeSelectedStatus) {
				if (kvp.Value) {
					selectedDocTypeNameList.Add (kvp.Key);
					selecteDocTypeList.Add (DocTypeMapping [kvp.Key]);
				}
			}
		
			string selectDocTypeNameStr = "";
			if (selectedDocTypeNameList.Count == DocTypeNameList.Count) {//All doc type are selected
				selectDocTypeNameStr = "All";
			} else if (selectedDocTypeNameList.Count == 0) {//No content type are selected
				selectDocTypeNameStr = "All";//None
			} else {
				selectDocTypeNameStr = String.Join (" & ", selectedDocTypeNameList);
			}

			AppDataUtil.Instance.SelectedTypeInSearchFilter = selecteDocTypeList;

			NSNotificationCenter.DefaultCenter.PostNotificationName ("ApplyContentTypeFilterToSearchResult", this, new NSDictionary ());
			NSNotificationCenter.DefaultCenter.PostNotificationName ("ChangeSearchFilterButtonText", this, new NSDictionary ("title", selectDocTypeNameStr));//suppose to be received in ResultViewController




		}
	}
}

