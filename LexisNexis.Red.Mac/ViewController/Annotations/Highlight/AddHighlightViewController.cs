using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.Mac
{
	public class ColorTagState
	{
		public AnnotationTag Tag { get; set; }
		public NSCellStateValue CheckState { get; set; }
	}

	public partial class AddHighlightViewController : AppKit.NSViewController
	{
		#region propertyies
		NSPopover ParentPopover;
		public List<ColorTagState> TagsList { get; set; }
		AddAnnotationViewController AddAnnotationVC;
		EditTagsViewController EditTagsVC;
		NSMutableIndexSet CheckIndexSet { get; set; }
		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public AddHighlightViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public AddHighlightViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public AddHighlightViewController (NSPopover parentPopover) : base ("AddHighlightView", NSBundle.MainBundle)
		{
			ParentPopover = parentPopover;
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new AddHighlightView View {
			get {
				return (AddHighlightView)base.View;
			}
		}

		public override void ViewDidLoad ()
		{
			ReloadTagsData ();

			TagsTableView.UsesAlternatingRowBackgroundColors = false;
			TagsTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;
			TagsTableView.GridStyleMask = NSTableViewGridStyle.None;
			TagsTableView.EnclosingScrollView.BorderType = NSBorderType.BezelBorder;

			TagsTableView.DataSource = new AHLTagsTableDataSource (TagsList);
			TagsTableView.Delegate = new AHLTagsTableDelegate (TagsList, this);
			TagsTableView.ReloadData ();

			var attributedTitle = Utility.AttributeTitle ("Delete", NSColor.Red, 12);
			DelNoteButton.AttributedTitle = attributedTitle;
			var alterTitle = Utility.AttributeTitle ("Delete", NSColor.DarkGray, 12);
			DelNoteButton.AttributedAlternateTitle = alterTitle;

			attributedTitle = Utility.AttributeTitle ("Add Note", NSColor.Red, 12);
			AddNoteButton.AttributedTitle = attributedTitle;
			alterTitle = Utility.AttributeTitle ("Add Note", NSColor.DarkGray, 12);
			AddNoteButton.AttributedAlternateTitle = alterTitle;

			attributedTitle = Utility.AttributeTitle ("Edit", NSColor.Red, 12);
			EditButton.AttributedTitle = attributedTitle;
			alterTitle = Utility.AttributeTitle ("Edit", NSColor.Red, 12);
			EditButton.AttributedAlternateTitle = alterTitle;
		}

		partial void AddButtonClick (NSObject sender)
		{
			if (AddAnnotationVC == null) {
				AddAnnotationVC = new AddAnnotationViewController(ParentPopover);
			}else {
			}

			ParentPopover.ContentViewController = AddAnnotationVC;
		}

		partial void DelButtonClick (NSObject sender)
		{
		}

		partial void EditButtonClick (NSObject sender)
		{
			if (EditTagsVC == null) {
				EditTagsVC = new EditTagsViewController(ParentPopover, this);
			}

			ParentPopover.ContentViewController = EditTagsVC;
			EditButton.Cell.State = NSCellStateValue.On;
		}

		public void switchContentViewer (int viewType)
		{
			switch (viewType) {
			case 0:
				ParentPopover.ContentViewController = EditTagsVC;
				break;
			case 1:
				ParentPopover.ContentViewController = this;
				ReloadTagsData ();
				break;
			case 2:
				ParentPopover.ContentViewController = AddAnnotationVC;
				break;
			default:
			break;
			}
		}

		public void ReloadTagsData ()
		{
			List<ColorTagState> backupTags = null;
			if (TagsList == null) {
				TagsList = new List<ColorTagState> ();
			} else {
				backupTags = new List<ColorTagState> ();
				backupTags.AddRange (TagsList);
				TagsList.Clear ();
			}

			if (AnnCategoryTagUtil.Instance == null) {
				return;
			}

			var tagList = AnnCategoryTagUtil.Instance.GetTags ();
			var tempTagList = new List<AnnotationTag> ();
			tempTagList.AddRange (tagList);

			if (tagList != null) {
				foreach(var tag in tempTagList) {
					var colorTag = new ColorTagState ();
					colorTag.CheckState = NSCellStateValue.Off;
					if (backupTags != null) {
						for (int i = 0; i < backupTags.Count; i++) {
							if (tag.TagId == backupTags [i].Tag.TagId) {
								colorTag.CheckState = backupTags [i].CheckState;
								break;
							}
						}
					}

					colorTag.Tag = tag;
					TagsList.Add (colorTag);
				}
			}

			if (backupTags != null) {
				backupTags.Clear ();
				backupTags = null;
			}

			if (TagsTableView != null) {
				TagsTableView.ReloadData ();
			}
		}

		public void SetCheckStateAtIndex(int index, NSCellStateValue state)
		{
			TagsList [index].CheckState = state;
		}

		public void SetCheckIndexSet (nint row, bool isSelected)
		{
			if (CheckIndexSet == null) {
				CheckIndexSet = new NSMutableIndexSet ();
			}

			nuint index = (nuint)row;
			if (isSelected) {
				CheckIndexSet.Add (index);
			} else {
				CheckIndexSet.Remove (index);
			}

			Console.WriteLine ("{0}", CheckIndexSet.Count);
		}
	}
}
