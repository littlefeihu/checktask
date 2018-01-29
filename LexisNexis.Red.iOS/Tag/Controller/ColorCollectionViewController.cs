using System;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom.Compiler;

using Foundation;
using UIKit;

using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;



namespace LexisNexis.Red.iOS
{
	partial class ColorCollectionViewController : UICollectionViewController
	{

		List<TagColor> optionTagColorList;

		public string SelectedColorCode{ get; set; } 

		public	AnnotationTag ColorTag { get; set;}
		UIColor firstDefaultColor ;
		string firstDefaultSelectColor;


		UIColor defaultColor = UIColor.Black;
		public ColorCollectionViewController (IntPtr handle) : base (handle)
		{
			optionTagColorList = AnnCategoryTagUtil.Instance.GetTagColors ();
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			CollectionView.RegisterClassForCell (typeof(TagColorCollectionViewCell), "TagColorCollectionViewCell");
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (ColorTag != null) {
				SelectedColorCode = ColorTag.Color;
			} else {
				firstDefaultSelectColor = "#FF00FF";
			}
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

			var cell = collectionView.DequeueReusableCell (TagColorCollectionViewCell.Key, indexPath) as TagColorCollectionViewCell;
			defaultColor = ColorUtil.ConvertFromHexColorCode (optionTagColorList [indexPath.Row].ColorValue);
			if (ColorTag != null) {
				UIColor selectedColor = ColorUtil.ConvertFromHexColorCode (ColorTag.Color);
				nfloat selectedRed, selectedGreen, selectedBlue, selectedAlpha;
				selectedColor.GetRGBA (out selectedRed, out selectedGreen, out selectedBlue, out selectedAlpha);

				nfloat curRed, curGreen, curBlue, curAlpha;
				defaultColor.GetRGBA (out curRed, out curGreen, out curBlue, out curAlpha);

				if (selectedRed == curRed && selectedGreen == curGreen && selectedBlue == curBlue && selectedAlpha == curAlpha) {
					cell.SetOptionColorView (new TripleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
				} else {

					cell.SetOptionColorView (new DoubleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
				}
			} else if (firstDefaultSelectColor != null) {

				if (indexPath.Row == 0 && firstDefaultSelectColor == "#FF00FF") {
					cell.SetOptionColorView (new TripleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
				} else if (firstDefaultSelectColor != "#FF00FF") 
				{
 					UIColor selectedColor = ColorUtil.ConvertFromHexColorCode (firstDefaultSelectColor);
					nfloat selectedRed, selectedGreen, selectedBlue, selectedAlpha;
					selectedColor.GetRGBA (out selectedRed, out selectedGreen, out selectedBlue, out selectedAlpha);

					nfloat curRed, curGreen, curBlue, curAlpha;
					defaultColor.GetRGBA (out curRed, out curGreen, out curBlue, out curAlpha);

					if (selectedRed == curRed && selectedGreen == curGreen && selectedBlue == curBlue && selectedAlpha == curAlpha) {
						cell.SetOptionColorView (new TripleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
					} else {

						cell.SetOptionColorView (new DoubleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
					}
  				} else {
					cell.SetOptionColorView (new DoubleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
				}
 			}
 
			else {
				cell.SetOptionColorView (new DoubleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
			}
			return cell;
		}

		public override bool ShouldSelectItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;//Selected
		}

		public override bool ShouldDeselectItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		public override bool ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell =(TagColorCollectionViewCell)collectionView.CellForItem(indexPath);
			if (ColorTag != null) {
				defaultColor = ColorUtil.ConvertFromHexColorCode (optionTagColorList [indexPath.Row].ColorValue);
				cell.SetOptionColorView (new DoubleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
				SelectedColorCode = optionTagColorList [indexPath.Row].ColorValue;
				ColorTag.Color = SelectedColorCode;
				collectionView.ReloadData ();
			} else if (firstDefaultSelectColor == "#FF00FF") { 
				defaultColor = ColorUtil.ConvertFromHexColorCode (optionTagColorList [indexPath.Row].ColorValue);
				cell.SetOptionColorView (new DoubleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
				SelectedColorCode = optionTagColorList [indexPath.Row].ColorValue;
				firstDefaultSelectColor = SelectedColorCode;
				collectionView.ReloadData ();
			} else if (firstDefaultSelectColor != "#FF00FF") {
				defaultColor = ColorUtil.ConvertFromHexColorCode (optionTagColorList [indexPath.Row].ColorValue);
				cell.SetOptionColorView (new DoubleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
				SelectedColorCode = optionTagColorList [indexPath.Row].ColorValue;
				firstDefaultSelectColor= SelectedColorCode;
				collectionView.ReloadData ();
			}
			else{
				defaultColor = ColorUtil.ConvertFromHexColorCode (optionTagColorList [indexPath.Row].ColorValue);
				SelectedColorCode = optionTagColorList [indexPath.Row].ColorValue;
				cell.SetOptionColorView (new TripleCircleView (ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));
			}


		}

		public override void ItemDeselected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell =(TagColorCollectionViewCell) collectionView.CellForItem(indexPath);//

			defaultColor = ColorUtil.ConvertFromHexColorCode (optionTagColorList [indexPath.Row].ColorValue);
			cell.SetOptionColorView (new DoubleCircleView(ViewConstant.TAG_INNER_CIRCLE_RADIUS, ViewConstant.TAG_OUTER_CIRCLE_RADIUS, defaultColor, UIColor.White, 0, 0));

		}
	}

}
