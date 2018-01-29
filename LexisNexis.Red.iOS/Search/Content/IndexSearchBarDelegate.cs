using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.iOS
{
	public class IndexSearchBarDelegate : UISearchBarDelegate
	{
		public IndexSearchBarDelegate () : base ()
		{
		}

		public IndexSearchBarDelegate (IntPtr handle) : base (handle)
		{
			
		}

		public override void SearchButtonClicked (UISearchBar searchBar)
		{
			AppDataUtil.Instance.ContentSearchKeyword = searchBar.Text;
			NSNotificationCenter.DefaultCenter.PostNotificationName ("SearchContentInIndex", this, new NSDictionary()); 
			//searchBar.ResignFirstResponder ();
		}

		public override void OnEditingStarted (UISearchBar searchBar)
		{
			searchBar.ShowsCancelButton = true;


		}

		public override void OnEditingStopped (UISearchBar searchBar)
		{
			searchBar.ShowsCancelButton = false;
		}

		public override void CancelButtonClicked (UISearchBar searchBar)
		{
			searchBar.ResignFirstResponder ();
			searchBar.Text = null;

			NSNotificationCenter.DefaultCenter.PostNotificationName ("RemoveSearchResultView", this, new NSDictionary());//Suppose to be handled by ResultViewController
		}
	}

}

