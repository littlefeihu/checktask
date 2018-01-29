using System;
using System.Collections.Generic;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS
{
	public class PopoverHistoryTableViewController : UITableViewController
	{

		/// <summary>
		/// A flag attribute which indentify whether this controller is poped from content view controller
		/// This property will determines the action when user touch rows
		/// </summary>
		/// <value><c>true</c> if this instance is in content V; otherwise, <c>false</c>.</value>
		public bool IsInContentVC{ get; set; }

		public PopoverHistoryTableViewController (bool isInContentVC = false) : base (UITableViewStyle.Plain)
		{
			Title = "Recent History";
			IsInContentVC = isInContentVC;
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
			TableView.TableFooterView = new UIView ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			List<RecentHistoryItem> historyList = PublicationContentUtil.Instance.GetRecentHistory ();
			PopoverHistoryTableViewSource TableViewSource = new PopoverHistoryTableViewSource (IsInContentVC);
			TableViewSource.HistoryList = historyList;
			TableView.Source = TableViewSource;
		}
	}
}

