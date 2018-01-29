using System;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.iOS
{
	public class ResultTableViewSource : UITableViewSource
	{

		public SearchResult SearchRes{ get; set;}

		public Dictionary<string, List<SearchDisplayResult>> ResListDict{ get; set;}

		public List<string> SectionTitleList{ get; set;}

		public ResultTableViewSource (SearchResult res)
		{
			SearchRes = res;
			ResListDict = new Dictionary<string, List<SearchDisplayResult>> ();
			SectionTitleList = new List<string> ();
			InitialDataDisplayed ();
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return SectionTitleList.Count;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return ResListDict[SectionTitleList[(int)section]].Count;
		}


		public override string TitleForHeader (UITableView tableView, nint section)
		{
			return SectionTitleList[(int)section];
		}


		public override UIView GetViewForHeader (UITableView tableView, nint section)
		{
			UIView headerView = new UIView (new CGRect(0, 0, 320, 20));
			headerView.BackgroundColor = UIColor.GroupTableViewBackgroundColor;

			UILabel headerLabel = new UILabel (new CGRect (20, 5, 300, 20));
			headerLabel.Font = UIFont.BoldSystemFontOfSize (14);
			headerLabel.Text = SectionTitleList [(int)section];

			headerView.AddSubview (headerLabel);
			 
			return headerView;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (ResultTableViewCell.Key) as ResultTableViewCell;
			if (cell == null)
				cell = new ResultTableViewCell ();
			
			SearchDisplayResult searchDR = ResListDict[SectionTitleList[indexPath.Section]] [indexPath.Row];

			cell.TextLabel.Font = UIFont.BoldSystemFontOfSize (14);
			cell.DetailTextLabel.Font = UIFont.SystemFontOfSize (14);
		
			if (searchDR.isDocument) {
				cell.TextLabel.AttributedText = TextDisplayUtil.GetHighlightedString (searchDR.Head, SearchRes.FoundWordList);
				cell.DetailTextLabel.AttributedText = TextDisplayUtil.GetHighlightedString (searchDR.SnippetContent, SearchRes.FoundWordList);
			} else {
				cell.TextLabel.AttributedText = TextDisplayUtil.GetHighlightedString (searchDR.TocTitle, SearchRes.FoundWordList);
				cell.DetailTextLabel.AttributedText = TextDisplayUtil.GetHighlightedString ( searchDR.GuideCardTitle, SearchRes.FoundWordList);
			}

			return cell;
		}
			
		 
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			SearchDisplayResult searchDR = ResListDict[SectionTitleList[indexPath.Section]] [indexPath.Row];


			if (searchDR.isDocument && searchDR.TocId == AppDataUtil.Instance.GetOpendTOC ().ID && AppDataUtil.Instance.GetOpendContentType() == PublicationContentTypeEnum.TOC) {
				NSNotificationCenter.DefaultCenter.PostNotificationName ("scrollToHeading", this, new NSDictionary ("tocId", searchDR.TocId, "headingType", searchDR.HeadType.ToString(),"headingNum", searchDR.HeadSequence));//post notification which supposed to be handled by LNWebview instance
			} else {
            	TOCNode touchedNode = PublicationContentUtil.Instance.GetTOCByTOCId (searchDR.TocId, AppDataUtil.Instance.GetCurPublicationTocRootNode());
            	AppDataUtil.Instance.SetOpendTOC (touchedNode);
            	AppDataUtil.Instance.HighlightSearchKeyword = string.Join (" ", SearchRes.FoundWordList);
            	AppDataUtil.Instance.ScrollToHtmlTagId = searchDR.Head;
				AppDataUtil.Instance.AddBrowserRecord (new SearchBrowserRecord(AppDataUtil.Instance.GetCurrentPublication().BookId,
					touchedNode.ID,
					0,0,string.Join(" ", SearchRes.KeyWordList),searchDR.HeadType.ToString(),
					searchDR.HeadSequence
					));//add browser search record
			}

			tableView.CellAt (indexPath).Selected = false;
		}


		public void InitialDataDisplayed (List<ContentCategory> selectedType = null)
		{
			List<SearchDisplayResult> resList;

			if (selectedType != null && selectedType.Count > 0) {
				resList = SearchRes.SearchDisplayResultList.FindAll (o => selectedType.Contains (o.ContentType));
			} else {
				resList = SearchRes.SearchDisplayResultList;
			}


			SectionTitleList.Clear ();
			ResListDict.Clear ();

			var inDocList = resList.FindAll (o => o.isDocument == true);
			if (inDocList.Count  > 0) {
				SectionTitleList.Add ("Document");
				ResListDict.Add ("Document", inDocList);
			}

			var inPublicationList = resList.FindAll (o => o.isDocument == false);
			if (inPublicationList.Count > 0) {
				SectionTitleList.Add ("Publication");
				ResListDict.Add ("Publication", inPublicationList);
			}
		}

	}
}

