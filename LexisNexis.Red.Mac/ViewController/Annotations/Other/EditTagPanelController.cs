using System;

using Foundation;
using AppKit;
using System.Collections.Generic;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class EditTagPanelController : NSWindowController
	{
		#region Constructors
		public EditTagPanelController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public EditTagPanelController (NSCoder coder) : base (coder)
		{
		}

		public EditTagPanelController () : base ("EditTagPanel")
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
			Window.MakeFirstResponder (null);
			Window.BackgroundColor = NSColor.White;

		}
		#endregion

		#region Properties
		public new EditTagPanel Window {
			get { return (EditTagPanel)base.Window; }
		}
		public List<AnnotationTag> TagsList { get; set; }
		#endregion

		#region Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

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
			TagTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;
			TagTableView.GridStyleMask = NSTableViewGridStyle.SolidHorizontalLine;
			TagTableView.EnclosingScrollView.BorderType = NSBorderType.BezelBorder;

			TagTableView.DataSource = new EditTagTableDataSource (TagsList, this);
			TagTableView.Delegate = new EditTagTableDelegate (TagsList, this);
			TagTableView.ReloadData ();

			var attributedTitle = Utility.AttributeTitle ("Done", NSColor.Red, 13);
			DoneButton.AttributedTitle = attributedTitle;
			var alterTitle = Utility.AttributeTitle ("Done", NSColor.DarkGray, 13);
			DoneButton.AttributedAlternateTitle = alterTitle;


//			TagTableView.SelectionShouldChange += (t) => true;
//			TagTableView.SelectionDidChange += ( sender,  e) => 
//				HandleSelectionDidChange ((NSNotification)sender);

			string[] typeArray = {"NSStringPboardType"};
			TagTableView.RegisterForDraggedTypes (typeArray);
		}

		void HandleSelectionDidChange(NSNotification sender)
		{
			var aTableView = (NSTableView)sender.Object;
			//selectedIndex = aTableView.SelectedRow;
		}
		#endregion

		#region action
		partial void AddButtonClick (NSObject sender)
		{
			PopoverAddTagPanel(sender, string.Empty, string.Empty);
		}

		partial void DoneButtonClick (NSObject sender)
		{
			foreach (AnnotationTag tag in TagsList) {
                
				AnnCategoryTagUtil.Instance.AddTag(tag.Title, tag.Color);
			}
				
			var NSApp = NSApplication.SharedApplication;
			Window.Close();
			Window.OrderOut(null);
			NSApp.EndSheet(Window);
		}
		#endregion

		#region api with EditTagTableDataSource
		public void RemoveTagAtRow (int row)
		{
			AnnCategoryTagUtil.Instance.DeleteTag(TagsList[row].TagId);
			TagsList.RemoveAt (row);
			TagTableView.ReloadData ();
		}
			
		public void EditTagAtRow (NSView rowView, int row)
		{
			var colorValue = TagsList[row].Color;
			var colorName = TagsList [row].Title;

			PopoverAddTagPanel (rowView, colorValue, colorName);
		}
		#endregion

		#region mark api with AddTagsViewController
		public void AddTagToTableView (string color, string title)
		{
			var newTag = new AnnotationTag ();
			newTag.Color = color;
			newTag.Title = title;
			TagsList.Add (newTag);
			TagTableView.ReloadData ();
		}
		#endregion	

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
