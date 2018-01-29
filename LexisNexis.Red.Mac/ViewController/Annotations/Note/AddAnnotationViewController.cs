using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac
{
	public partial class AddAnnotationViewController : AppKit.NSViewController
	{
		NSPopover ParentPopover;
		EditTagsViewController EditTagsVC;
		public List<ColorTagState> TagsList { get; set; }

		#region Constructors

		// Called when created from unmanaged code
		public AddAnnotationViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public AddAnnotationViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public AddAnnotationViewController (NSPopover parentPopover) : base ("AddAnnotationView", NSBundle.MainBundle)
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
		public new AddAnnotationView View {
			get {
				return (AddAnnotationView)base.View;
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


			attributedTitle = Utility.AttributeTitle ("Edit", NSColor.Red, 12);
			EditButton.AttributedTitle = attributedTitle;
			alterTitle = Utility.AttributeTitle ("Edit", NSColor.Red, 12);
			EditButton.AttributedAlternateTitle = alterTitle;

			attributedTitle = Utility.AttributeTitle ("Note", NSColor.Brown, 12);
			NoteButton.AttributedTitle = attributedTitle;
			alterTitle = Utility.AttributeTitle ("Note", NSColor.DarkGray, 12);
			NoteButton.AttributedAlternateTitle = alterTitle;

			attributedTitle = Utility.AttributeTitle ("Delete Annotation", NSColor.Brown, 12);
			DelAnnotationBtn.AttributedTitle = attributedTitle;
			alterTitle = Utility.AttributeTitle ("Delete Annotation", NSColor.DarkGray, 12);
			DelAnnotationBtn.AttributedAlternateTitle = alterTitle;

			NoteBkgView.WantsLayer = true;
			NoteBkgView.Layer.BackgroundColor = Utility.ColorWithRGB (253, 251, 176, 1.0f).CGColor;

			HSeprator.WantsLayer = true;
			HSeprator.Layer.BackgroundColor = NSColor.Grid.CGColor;

			VSeprator.WantsLayer = true;
			VSeprator.Layer.BackgroundColor = NSColor.Grid.CGColor;

			NoteTextView.BackgroundColor = Utility.ColorWithRGB (253, 251, 176, 1.0f);

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
		}

		partial void NoteButtonClick (NSObject sender)
		{
		}

		partial void DelAnnotationBtnClick (NSObject sender)
		{
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
	}
}
