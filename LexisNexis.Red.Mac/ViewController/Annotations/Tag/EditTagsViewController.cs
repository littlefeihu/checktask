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
	public partial class EditTagsViewController : AppKit.NSViewController
	{
		#region Properties
		public List<AnnotationTag> TagsList { get; set; }
		private NSPopover ParentPopover;
		private object ParentConroller { get; set; }

		private const int ViewType_AddTag = 0;
		private const int ViewType_EditTag = 1;
		private AddTagsViewController AddTagsVC;
		private bool IsTagEdit{ get; set; }
		private int CurrentTagIndex{ get; set; }
		private bool IsDraggedTag{ get; set; }

		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public EditTagsViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public EditTagsViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public EditTagsViewController (NSPopover popoverView, object viewController) : base ("EditTagsView", NSBundle.MainBundle)
		{
			this.ParentConroller = viewController;
			ParentPopover = popoverView;
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new EditTagsView View {
			get {
				return (EditTagsView)base.View;
			}
		}

		#region Methods
		public override void ViewDidLoad ()
		{
			if (TagsList == null) {
				TagsList = new List<AnnotationTag> (0);
			} else {
				TagsList.Clear ();
			}

			var tagList = AnnCategoryTagUtil.Instance.GetTags ();
			if (tagList != null) {
				TagsList.AddRange (tagList);
			}

			TagTableView.UsesAlternatingRowBackgroundColors = false;
			TagTableView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
			TagTableView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;
			TagTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;
			TagTableView.EnclosingScrollView.BackgroundColor = NSColor.White;
			TagTableView.EnclosingScrollView.BorderType = NSBorderType.BezelBorder;
			TagTableView.GridStyleMask = NSTableViewGridStyle.None;

			TagTableView.DataSource = new EditTagTableDataSource (TagsList, this);
			TagTableView.Delegate = new EditTagTableDelegate (TagsList, this);
			TagTableView.ReloadData ();

			var attributedTitle = Utility.AttributeTitle ("Done", NSColor.Red, 13);
			DoneButton.AttributedTitle = attributedTitle;
			var alterTitle = Utility.AttributeTitle ("Done", NSColor.Red, 13);
			DoneButton.AttributedAlternateTitle = alterTitle;

			string[] typeArray = {"NSStringPboardType"};
			TagTableView.RegisterForDraggedTypes (typeArray);

			this.CurrentTagIndex = -1;
		}
		#endregion

		#region action
		partial void AddButtonClick (NSObject sender)
		{
			this.IsTagEdit = false;
			SwitchTagView(ViewType_AddTag, "#FF00FF", "Magenta", Guid.Empty);
		}

		partial void DoneButtonClick (NSObject sender)
		{
			var winController = Utility.GetMainWindowConroller();
			winController.SetEditTagState(false);

			if (IsDraggedTag) {
				var tagArray = new List<AnnotationTag>();
				tagArray.AddRange(TagsList);
				AnnCategoryTagUtil.Instance.Sort(tagArray);
			}

			if (this.ParentConroller is AddHighlightViewController) {
				var controller = (AddHighlightViewController)this.ParentConroller;
				controller.switchContentViewer(1);
			} else if (this.ParentConroller is AddAnnotationViewController) {
				var controller = (AddAnnotationViewController)this.ParentConroller;
				controller.switchContentViewer(1);
			}
			else {
				this.ParentPopover.Close();
			}
		}
		#endregion

		#region api with EditTagTableDataSource and EditTagTableDelegate
		public void EditTagAtRow (NSView rowView, int row)
		{
			var colorValue = TagsList[row].Color;
			var colorName = TagsList [row].Title;
			var guid = TagsList [row].TagId;
			this.IsTagEdit = true;
			this.CurrentTagIndex = row;
			SwitchTagView(ViewType_AddTag,colorValue,colorName,guid);
		}

		public void RemoveTagAtRow (int row)
		{
			AnnCategoryTagUtil.Instance.DeleteTag(TagsList[row].TagId);
			TagsList.RemoveAt (row);
			TagTableView.ReloadData ();
		}

		public void DragItemFromIndexToIndex (NSTableView tableView, int dragRow, int toRow)
		{
			IsDraggedTag = true;

			//Console.WriteLine("from:{0} to:{1}", dragRow, toRow);
			AnnotationTag dragItem = TagsList [dragRow];

			if (dragRow<toRow) {
				TagsList.Insert(toRow,dragItem);
				TagsList.RemoveAt(dragRow);
			} else {
				TagsList.RemoveAt(dragRow);
				TagsList.Insert(toRow,dragItem);
			}
		}

		#endregion

		#region mark api with AddTagsViewController
		public void AddTagToTableView (string color, string title)
		{
			if (this.IsTagEdit) {
				var curTag = TagsList [this.CurrentTagIndex];
				curTag.Color = color;
				curTag.Title = title;
				AnnCategoryTagUtil.Instance.UpdateTag (curTag.TagId,title,color);
			} else {
				var newTag = new AnnotationTag ();
				newTag.Color = color;
				newTag.Title = title;
				Guid id = AnnCategoryTagUtil.Instance.AddTag (title, color);
				newTag.TagId = id;

				TagsList.Add (newTag);
			}

			TagTableView.ReloadData ();
		}
		#endregion	

		public void SwitchTagView(int ViewType, string colorValue, string colorName, Guid tagGuid)
		{
			switch (ViewType) {
			case ViewType_AddTag:
				if (AddTagsVC == null) {
					AddTagsVC = new AddTagsViewController (ParentPopover, this, colorValue, colorName, tagGuid);
				} else {
					AddTagsVC.UpdateTagNameAndColor (colorName, colorValue, tagGuid);
				}

				ParentPopover.ContentViewController = AddTagsVC;
				break;
			case ViewType_EditTag:
				ParentPopover.ContentViewController = this;
				break;
			default:
				break;
			}
		}

		private void PopoverAddTagPanel(NSObject sender, string colorValue, string colorName)
		{
			this.Invoke (() => {
				var popover = new NSPopover ();
				popover.Behavior = NSPopoverBehavior.Transient;
				popover.ContentViewController = new AddTagsViewController(popover, this, colorValue, colorName, Guid.Empty);
				popover.Show (new CGRect(0,0,0,0), (NSView)sender, NSRectEdge.MaxYEdge);
			}, 0.1f);
		}
	}
}
