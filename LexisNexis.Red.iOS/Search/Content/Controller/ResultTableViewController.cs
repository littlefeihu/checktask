
using System;
using System.Collections.Generic;


using Foundation;
using UIKit;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	public class ResultTableViewController : UITableViewController
	{

		public SearchResult  SearchRes{ get; set; }

		public ResultTableViewController (SearchResult res) : base (UITableViewStyle.Plain)
		{
			SearchRes = res;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Register the TableView's data source
			TableView.Source = new ResultTableViewSource (SearchRes);
			TableView.TableFooterView = new UIView ();

			NSNotificationCenter.DefaultCenter.AddObserver (new NSString ("ApplyContentTypeFilterToSearchResult"), delegate(NSNotification obj) {
				if(AppDataUtil.Instance.SelectedTypeInSearchFilter == null || AppDataUtil.Instance.SelectedTypeInSearchFilter.Count == 0){
				}else{
					((ResultTableViewSource)TableView.Source).InitialDataDisplayed(AppDataUtil.Instance.SelectedTypeInSearchFilter);
					TableView.ReloadData();
				}

			});
		}
	}
}