
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class TagState
	{
		public string Color { get; set; }
		public string Title { get; set; }
		public Guid TagID { get; set; }
		public NSCellStateValue CheckState { get; set; }
	}

	public partial class AnnocationsViewController : NSViewController
	{
		
		public List<TagState> TagList { get; set; }
		private NSMenu TagMenu { get; set; }
		private List<Annotation> AnnotationList { get; set; }
		#region Constructors

		// Called when created from unmanaged code
		public AnnocationsViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public AnnocationsViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public AnnocationsViewController () : base ("AnnocationsView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new AnnocationsView View {
			get {
				return (AnnocationsView)base.View;
			}
		}

		public override void ViewDidLoad ()
		{	
			AllButtonClick (AllButton);

			InitTagFilterBtn ();

			InitProperties ();
		}

		public void ReloadAnnotationDataWithBookID (int bookID)
		{
			GetTagsFromDB ();
			UpdateTableViewDataByBookID (bookID);
		}
			
		public void UpdateTableViewDataByBookID (int bookID)
		{
			if (AnnotationList == null) {
				AnnotationList = new List<Annotation> ();
			} else {
				AnnotationList.Clear ();
			}
				
			var annotationList = AnnotationUtil.Instance.GetAnnotations ();
			if (annotationList != null) {
				var sameTitleList = annotationList.FindAll ((item)=>item.BookId == bookID);
				AnnotationList.AddRange (sameTitleList);
			}

			if (AnnotationList == null || AnnotationList.Count == 0) {
				InfoView.Hidden = false;
				AnnotationTableView.EnclosingScrollView.Hidden = true;
			} else {
				InfoView.Hidden = true;
				AnnotationTableView.EnclosingScrollView.Hidden = false;

				AnnotationTableView.ReloadData ();
			}
		}

		private void AddAnnotation ()
		{
			int offset = 0;
			string fileName = string.Empty;
			string levelId  = string.Empty;
			string xpath = string.Empty;
			string spanId  = string.Empty;
			
			AnnotationDataItem item = new AnnotationDataItem (offset, fileName, levelId, xpath, spanId);
		}

		public void RefreshTagListState()
		{
			for (int i=0; i<this.TagList.Count; i++) {
				this.TagList[i].CheckState = NSCellStateValue.Off;
			}
			this.TagList[0].CheckState = NSCellStateValue.On;
		}

		private void InitProperties ()
		{
			AnnotationTableView.UsesAlternatingRowBackgroundColors = false;
			AnnotationTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.Regular;
			AnnotationTableView.GridStyleMask = NSTableViewGridStyle.None;
			AnnotationTableView.EnclosingScrollView.BorderType = NSBorderType.BezelBorder;
			AnnotationTableView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Light;
			AnnotationTableView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;

			//AnnotationTableView.DataSource = new AnnotationsTableDataSource (AnnotationList);
			//AnnotationTableView.Delegate = new AnnotationsTableDelegate (AnnotationList, this);
			AnnotationTableView.ReloadData ();

			InfoLabelTF.StringValue = "No annotations match your search criteria.";
		}

		#region mark action
		partial void AllButtonClick (NSObject sender)
		{
			SetButtonAttributedTitle(AllButton,"All",true);
			SetButtonAttributedTitle(NotesButton,"Notes",false);
			SetButtonAttributedTitle(HighlightsButton,"Highlights",false);
		}

		partial void NotesButtonClick (NSObject sender)
		{
			SetButtonAttributedTitle(AllButton,"All",false);
			SetButtonAttributedTitle(NotesButton,"Notes",true);
			SetButtonAttributedTitle(HighlightsButton,"Highlights",false);
		}

		partial void HighlightButtonClick (NSObject sender)
		{
			SetButtonAttributedTitle(AllButton,"All",false);
			SetButtonAttributedTitle(NotesButton,"Notes",false);
			SetButtonAttributedTitle(HighlightsButton,"Highlights",true);
		}

		partial void TagFilterBtnClick (NSObject sender)
		{
			PopupMenuAtLocation(sender);
		}

		partial void TagFilterPopBtnClick (NSObject sender)
		{
			var button = (NSPopUpButton)sender;
			int index = Convert.ToInt32(button.IndexOfSelectedItem);

			this.TagList[index].CheckState = this.TagList[index].CheckState==NSCellStateValue.On?NSCellStateValue.Off:NSCellStateValue.On;

			var menuItems = button.Items();

			for (int i=0; i<this.TagList.Count; i++) {
				menuItems[i].State = this.TagList[i].CheckState;
			}
		}

		[Action("TagFilterMenuItemClick:")]
		void TagFilterMenuItemClick (NSObject sender)
		{
			var menuItem = (NSMenuItem)sender;
			int index = Convert.ToInt32(menuItem.Tag);

			if (index == 0 || index == 1) {
				for (int i=0; i<this.TagList.Count; i++) {
					this.TagList[i].CheckState = NSCellStateValue.Off;
				}
				this.TagList [index].CheckState = NSCellStateValue.On;
			} else if (index==2){   //separator
			}else {
				this.TagList[0].CheckState = NSCellStateValue.Off;
				this.TagList[1].CheckState = NSCellStateValue.Off;
				this.TagList [index].CheckState = this.TagList [index].CheckState == NSCellStateValue.On ? NSCellStateValue.Off : NSCellStateValue.On;
				menuItem.State = this.TagList [index].CheckState;
			}
		}

		#endregion

		#region mark private methods
		public void GetTagsFromDB ()
		{
			if (TagList == null) {
				TagList = new List<TagState> (0);
			} else {
				TagList.Clear ();
			}

			var allTag = new TagState ();
			allTag.Title = "All Tags";
			allTag.Color = string.Empty;
			allTag.CheckState = NSCellStateValue.On;
			TagList.Add (allTag);

			var noTag = new TagState ();
			noTag.Title = "No Tag";
			noTag.Color = string.Empty;
			noTag.CheckState = NSCellStateValue.Off;
			TagList.Add (noTag);

			var separatorTag = new TagState ();
			separatorTag.Title = "Seperator";
			separatorTag.Color = string.Empty;
			separatorTag.CheckState = NSCellStateValue.Off;
			TagList.Add (separatorTag);

			if (AnnCategoryTagUtil.Instance == null) {
				return;
			}

			var tagList = AnnCategoryTagUtil.Instance.GetTags ();
			foreach (var item in tagList) {
				var tagState = new TagState ();
				tagState.Title = item.Title;
				tagState.Color = item.Color;
				tagState.TagID = item.TagId;
				tagState.CheckState = NSCellStateValue.Off;
				TagList.Add (tagState);
			}
		}
			
		private void SetButtonAttributedTitle(NSButton button, string title, bool isStateOn)
		{
			float fontSize = 12.5f;
			if (isStateOn) {
				var attributedTitle = Utility.AttributeTitle (title, NSColor.Red, fontSize);
				button.AttributedTitle = attributedTitle;
				var alterTitle = Utility.AttributeTitle (title, NSColor.Black, fontSize);
				button.AttributedAlternateTitle = alterTitle;
				button.WantsLayer = true;
				button.Layer.BackgroundColor = Utility.ColorWithRGB (251, 212, 213, 1.0f).CGColor;
				button.Layer.CornerRadius = 5;
			} else {
				var attributedTitle = Utility.AttributeTitle (title, NSColor.Black, fontSize);
				button.AttributedTitle = attributedTitle;
				var alterTitle = Utility.AttributeTitle (title, NSColor.Red, fontSize);
				button.AttributedAlternateTitle = alterTitle;
				button.WantsLayer = true;
				button.Layer.BackgroundColor = Utility.ColorWithRGB (255, 255, 255, 1.0f).CGColor;
				button.Layer.CornerRadius = 5;
			}
		}

		//for tagfilter button
		private void InitTagFilterBtn ()
		{
			TagFilterPopBtn.Hidden = true;
			TagFilterButton.Image = Utility.ImageWithFilePath ("/Images/Annotation/arrow_down.png");
		}

		private void PopupMenuAtLocation(NSObject sender)
		{
			var button = (NSButton)sender;
			CGPoint location = new CGPoint ();
			location.X = button.Frame.GetMinX ();
			location.Y = button.Frame.GetMidY ();

			location = button.Superview.ConvertPointToView(location, null);

			NSWindow window = Utility.GetMainWindowConroller ().Window;
			nint windowNumber = window.WindowNumber;

			if (TagMenu == null) {
				TagMenu = new NSMenu ();
			} else {
				TagMenu.RemoveAllItems ();
			}

			NSMenuItem menuItem = new NSMenuItem ("All Tags");
			menuItem.Image = Utility.ImageWithFilePath ("/Images/Annotation/All_Icon@1x.png");
			menuItem.Action = new ObjCRuntime.Selector ("TagFilterMenuItemClick:");
			menuItem.Target = this;
			menuItem.Tag = 0;
			TagMenu.InsertItem (menuItem, 0);

			menuItem = new NSMenuItem ("No Tag");
			menuItem.Image = CreateImageWithColor (string.Empty);
			menuItem.Action = new ObjCRuntime.Selector ("TagFilterMenuItemClick:");
			menuItem.Target = this;
			menuItem.Tag = 1;
			TagMenu.InsertItem (menuItem, 1);

			TagMenu.AddItem (NSMenuItem.SeparatorItem);

			for (int i = 3; i < this.TagList.Count; i++) {
				menuItem = new NSMenuItem (this.TagList[i].Title);
				menuItem.Image = CreateImageWithColor (this.TagList[i].Color);
				menuItem.Action = new ObjCRuntime.Selector ("TagFilterMenuItemClick:");
				menuItem.Target = this;
				menuItem.Tag = i;
				menuItem.State = NSCellStateValue.Off;
				TagMenu.InsertItem (menuItem, i);
			}

			var menuItems = TagMenu.ItemArray();

			for (int i=0; i<this.TagList.Count; i++) {
				menuItems[i].State = this.TagList[i].CheckState;
			}

			NSEvent fakeMouseEvent = 
				NSEvent.MouseEvent (NSEventType.LeftMouseUp,
					location,
					(NSEventModifierMask)NSEventMask.LeftMouseUp,
					0,
					windowNumber,
					window.GraphicsContext, 0, 1, 1);
			NSMenu.PopUpContextMenu(TagMenu,fakeMouseEvent,(NSView)sender);
		}

		private NSImage CreateImageWithColor(string colorValue)
		{
			NSGraphicsContext.GlobalSaveGraphicsState ();
			CGSize size = new CGSize(12, 12);
			NSImage tintImage = new NSImage (size);
			tintImage.LockFocus ();

			float cornerRadius = 5f;
			CGRect rect = new CGRect (0,0,10,10);
			NSBezierPath path = NSBezierPath.FromRoundedRect (rect,cornerRadius,cornerRadius); 
			if (string.IsNullOrEmpty (colorValue)) {
				NSColor.Grid.Set ();
				path.Stroke ();
			} else {
				Utility.ColorWithHexColorValue (colorValue, 1.0f).SetFill ();
				path.Fill ();
			}

			tintImage.UnlockFocus ();
			CGContext context = NSGraphicsContext.CurrentContext.CGContext;

			return tintImage;
		}

		//for popup button
		private void CreateTagFilterMenu ()
		{
			TagFilterButton.Hidden = true;

			NSMenu tagMenu = new NSMenu();

			NSMenuItem menuItem = new NSMenuItem ("All Tags");
			menuItem.Image = Utility.ImageWithFilePath ("/Images/Annotation/All_Icon@1x.png");

			tagMenu.InsertItem (menuItem, 0);

			menuItem = new NSMenuItem ("No Tag");
			menuItem.Image = CreateImageWithColor (string.Empty);
			tagMenu.InsertItem (menuItem, 1);

			for (int i = 2; i < this.TagList.Count; i++) {
				menuItem = new NSMenuItem (this.TagList[i].Title);
				menuItem.Image = CreateImageWithColor (this.TagList[i].Color);
				menuItem.ToolTip = this.TagList [i].Title;
				menuItem.State = NSCellStateValue.Off;
				tagMenu.InsertItem (menuItem, i);
			}

			TagFilterPopBtn.Menu = tagMenu;
		}
			
		#endregion
	}
}

