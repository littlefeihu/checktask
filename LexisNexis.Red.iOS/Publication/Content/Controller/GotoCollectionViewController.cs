
using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using System.Text;

using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.iOS
{
	[Register ("GotoCollectionViewController")]
	partial class GotoCollectionViewController :UICollectionViewController 
	{
		List<string> numbersList;
		UILabel inputNumberLabel = new UILabel ();
		private UIColor lineColor;
		private UIColor bgColor;
		const int  INPUT_TEXT_LIMIT = 7 ;
		const int INDEX_PATH_ROW = 11;
		const int CURRENT_TEXT_LENTH = 1;

		public GotoCollectionViewController (IntPtr handle) : base (handle)
		{
			numbersList = new List<string>{ 
				"1", "2", "3", "4",
				"5", "6", "7", "8",
				"9", "", "0", ""
			};
		}

		public GotoCollectionViewController (UICollectionViewLayout layout) : base (layout)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			lineColor = ColorUtil.ConvertFromHexColorCode ("#CAD2D3");
			bgColor  = ColorUtil.ConvertFromHexColorCode ("#F1F1F4");

			CollectionView.RegisterClassForCell (typeof(GotoCollectionViewCell), GotoCollectionViewCell.Key);
			CollectionView.BackgroundColor = UIColor.White;
			CollectionView.RegisterClassForSupplementaryView (typeof(GotoCollectionReusableView), UICollectionElementKindSection.Header, "HeaderView");

		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return 12;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell (GotoCollectionViewCell.Key, indexPath) as GotoCollectionViewCell;


			if (indexPath.Row == 9 ) {
				cell.ContentView.BackgroundColor = lineColor;
				cell.titleLabe.Frame =new CoreGraphics.CGRect (0, 0, 106, 44);
				cell.titleLabe.BackgroundColor = lineColor;
			}
			if (indexPath.Row == 11) {
				cell.ContentView.BackgroundColor = lineColor;
				UIImageView clearImage = new UIImageView (new UIImage ("Images/Content/delete.png"));
				clearImage.Frame = new CoreGraphics.CGRect (43,12,20,20);
				cell.titleLabe.AddSubview (clearImage);
			}
			cell.titleLabe.Text = numbersList[indexPath.Row];
			return cell;
		}



		public override  UICollectionReusableView GetViewForSupplementaryElement (UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
		{
			var headerView =(GotoCollectionReusableView)collectionView.DequeueReusableSupplementaryView (elementKind, "HeaderView", indexPath);
			headerView.BackgroundColor = bgColor;
			inputNumberLabel.Frame = new CoreGraphics.CGRect (0,25,318,30);
			inputNumberLabel.TextAlignment = UITextAlignment.Center;
			inputNumberLabel.Font = UIFont.SystemFontOfSize (40);
			headerView.AddSubview (inputNumberLabel);
			return headerView;
		}

		public  override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var currenttext = inputNumberLabel.Text;
			var value = numbersList[indexPath.Row];
			var text = currenttext+value;

			if (indexPath.Row == INDEX_PATH_ROW && !string.IsNullOrEmpty (currenttext)) {
				if (currenttext.Length == CURRENT_TEXT_LENTH) {
					NSNotificationCenter.DefaultCenter.PostNotificationName ("inputPageNumber", this, new NSDictionary ("page", "0"));
					inputNumberLabel.Text = "";
				} else {
					inputNumberLabel.Text = currenttext.Remove (currenttext.Length - 1);
					NSNotificationCenter.DefaultCenter.PostNotificationName ("inputPageNumber", this, new NSDictionary ("page", inputNumberLabel.Text));
				}
				return;
			}
			if (text != "") {
				if (text.Length <= INPUT_TEXT_LIMIT) {
					inputNumberLabel.Text = text;
					NSNotificationCenter.DefaultCenter.PostNotificationName ("inputPageNumber", this, new NSDictionary ("page", inputNumberLabel.Text));
				}  
			}
		}
	}

}

