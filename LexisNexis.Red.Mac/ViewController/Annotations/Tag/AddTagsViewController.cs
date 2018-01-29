using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac
{
	public partial class AddTagsViewController : AppKit.NSViewController
	{
		string []colorValueArrays = { 
			"#FF00FF", "#800080", "#4B0082", "#0000FF",  
			"#FFA500", "#00FF00", "#008080", "#00FFFF",
			"#FF0000", "#BEBEBE", "#A9A9A9", "#000000"
		};

		string []colorNameArrays = { 
			"Magenta", "Purple", "Indigo", "Blue", 
			"Orange", "Green", "Teal", "Cyan",
			"Red", "Grey", "Dark Grey", "Black"
		};

		private ColorTagButton [] buttonArray { get; set; }

		private NSPopover ParentPopover;
		private object ViewController;
		public string TagColorValue { get; set; }
		public string TagColorName { get; set; }
		public Guid TagGuid { get; set; }
		public bool IsTagEdit{ get; set; }
		#region Constructors

		// Called when created from unmanaged code
		public AddTagsViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public AddTagsViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public AddTagsViewController (NSPopover parentPopOver, object viewController, string colorValue, string colorName, Guid tagGuid) : base ("AddTagsView", NSBundle.MainBundle)
		{
			ParentPopover = parentPopOver;
			ViewController = viewController;
			TagColorValue = colorValue;
			TagColorName = colorName;
			TagGuid = tagGuid;
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new AddTagsView View {
			get {
				return (AddTagsView)base.View;
			}
		}

		public override void ViewDidLoad ()
		{
			SetTitle ();

			TagNameLableTF.StringValue = "TAG NAME";
			TagNameTF.PlaceholderString = "Add new tag";
			TagNameTF.StringValue = this.TagColorName;
			TagColorLableTF.StringValue = "TAG COLOUR";

			var attributedTitle = Utility.AttributeTitle ("Done", NSColor.Red, 13);
			DoneButton.AttributedTitle = attributedTitle;
			var alterTitle = Utility.AttributeTitle ("Done", NSColor.DarkGray, 13);
			DoneButton.AttributedAlternateTitle = alterTitle;

			SetBackButtonState ();

			int curIndex = 0;
			for (int i=0; i<colorValueArrays.Length; i++) {
				if (colorValueArrays[i].Equals (TagColorValue)) {
					curIndex = i;
					break;
				}
			}

			var buttons = GetAllbuttonArray();
			for (int i = 0; i < buttons.Length; i++) {
				var frame = buttons [i].Frame;
				buttons [i].Hidden = true;
				bool isSelected = (i == curIndex) ? true : false;
				var button = new ColorTagButton(12,10,colorValueArrays[i],colorValueArrays[i],frame,isSelected);
				button.Target = this;
				button.Action = new ObjCRuntime.Selector ("ColorTagClick:");
				button.Tag = i;
				((NSView)TagColorBox.ContentView).AddSubview (button);
			}

			NSView[] lineViews = { HorLine1, HorLine2, VerLine1, VerLine2, VerLine3 };
			for (int i = 0; i < lineViews.Length; i++) {
				lineViews [i].WantsLayer = true;
				lineViews [i].Layer.BackgroundColor = NSColor.Grid.CGColor;
			}

			DeleteButton.Image = Utility.ImageWithFilePath ("/Images/Annotation/Delete.png");
		}
			
		public void UpdateTagNameAndColor(string colorName, string colorValue, Guid tagGuid)
		{
			this.TagColorValue = colorValue;
			this.TagColorName = colorName;
			this.TagGuid = tagGuid;
			this.IsTagEdit = !string.IsNullOrEmpty (colorValue);

			SetTitle ();
			SetBackButtonState ();

			var views = TagColorBox.Subviews;
			var contentView = (NSView)TagColorBox.ContentView;
			var subviews = contentView.Subviews;
			foreach (var view in subviews) {
				if (view.Class.Name.Equals("LexisNexis_Red_Mac_ColorTagButton")) {
					var currButton = (ColorTagButton)view;
					bool isSelected = (currButton.InnerCircleColor == colorValue)?true:false;
					currButton.IsSelected = isSelected;
					currButton.SetNeedsDisplay ();
				}
			}

			TagNameTF.StringValue = colorName;
		}



		#region mark action
		[Action("ColorTagClick:")]
		private void ColorTagClick(NSObject sender)
		{
			var button = (ColorTagButton)sender;
			var colorValue = colorValueArrays [button.Tag];

			this.TagColorValue = colorValue;
			if (string.IsNullOrEmpty (TagNameTF.StringValue)) {
				TagNameTF.StringValue = colorNameArrays [button.Tag];
			}

			RemoveColorSelectState ();

			button.IsSelected = true;
			button.SetNeedsDisplay ();
		}

		partial void RemoveColorName (NSObject sender)
		{
			TagNameTF.StringValue = "";
		}

		partial void BackClick (NSObject sender)
		{
			SwitchToEditTagView();
		}

		partial void DoneClick (NSObject sender)
		{
			if (string.IsNullOrEmpty(TagNameTF.StringValue)) {
				return;
			}

			this.TagColorName = TagNameTF.StringValue;

			if (ViewController is EditTagsViewController) {
				var viewController = (EditTagsViewController)ViewController;
				viewController.AddTagToTableView(this.TagColorValue, this.TagColorName);

				viewController.SwitchTagView (1, string.Empty, string.Empty, Guid.Empty);
			}else {
				AddTagToDB();
				ParentPopover.Close ();
			}
		}
		#endregion
			
		#region private methods
		private void SwitchToEditTagView()
		{
			if (ViewController is EditTagsViewController) {
				var viewController = (EditTagsViewController)ViewController;
				viewController.SwitchTagView (1, string.Empty, string.Empty, Guid.Empty);
			} else {
				ParentPopover.Close ();
			}
		}

		private void AddTagToDB()
		{
			if (this.IsTagEdit) {
				AnnCategoryTagUtil.Instance.UpdateTag (this.TagGuid, this.TagColorName, this.TagColorValue);
			} else {
				this.TagGuid = AnnCategoryTagUtil.Instance.AddTag (this.TagColorName, this.TagColorValue);
			}
		}

		private void RemoveColorSelectState()
		{
			var subviews = ((NSView)TagColorBox.ContentView).Subviews;
			foreach (var view in subviews) {
				if (view is ColorTagButton) {
					var currButton = (ColorTagButton)view;
					if (currButton.IsSelected) {
						currButton.IsSelected = false;
						currButton.SetNeedsDisplay ();
					}
				}
			}
		}

		private void SetTitle ()
		{
			if (this.TagGuid.Equals(Guid.Empty)) {
				TitleLabelTF.StringValue = "New tag";
				this.IsTagEdit = false;
			} else {
				TitleLabelTF.StringValue = "Edit tag";
				this.IsTagEdit = true;
			}
		}

		private void SetBackButtonState ()
		{
			NSAttributedString attributedTitle = Utility.AttributeTitle ("Back", NSColor.Red, 13);
			BackButton.AttributedTitle = attributedTitle;
			NSAttributedString alterTitle = Utility.AttributeTitle ("Back", NSColor.DarkGray, 13);
			BackButton.AttributedAlternateTitle = alterTitle;
			BackButton.Cell.ImageScale = NSImageScale.None;
			BackButton.Image = Utility.ImageWithFilePath ("/Images/Annotation/Back.png");
		}

		private NSButton [] GetAllbuttonArray ()
		{
			NSButton[] buttons = { ColorButton1, ColorButton2, ColorButton3, ColorButton4,
				ColorButton5, ColorButton6, ColorButton7, ColorButton8,
				ColorButton9, ColorButton10, ColorButton11, ColorButton12};
			return buttons;
		}

		#endregion
	}
}
